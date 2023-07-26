using Assets.Script;
using System.Collections;
using UnityEngine;

public class SkellieCaster : ActorBase
{
    float Speed;

    Vector3 moveVec_;
    Vector3 scale_;
    float Cd = 3.0f;
    float cd = 3.0f;
    Vector3 target_;

    Vector3 GetNewTarget()
    {
        Vector3 result = PositionUtility.GetPointInsideArena(1.0f, 1.0f);
        return result;
    }

    protected override void PreEnable()
    {
        Speed = 3.0f * GameMode.MoveSpeedModifier;
        Hp = 150 * GameMode.HitpointModifier;
        Mass = 1.2f * GameMode.MassModifier;
        ActorType = ActorTypeEnum.Caster;
    }

    protected override void PostEnable()
    {
        scale_ = transform_.localScale;
        position_ = transform_.position;
        Animations = SpriteData.Instance.SkellieCasterWalkSprites;
        GetNewTarget();

        StartCoroutine(Think());
    }

    void Shoot()
    {
        if (Hp <= 0.0f)
            return;

        Vector2 direction = BlackboardScript.PlayerTrans.position - position_;
        direction.Normalize();
        direction = RndUtil.RandomSpread(direction, 3);

        Vector3 muzzlePoint = direction * 0.6f;
        AddForce(-direction * 0.05f);

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsPlayer;

        basic.Speed = 4.0f;
        basic.Damage = 100.0f;
        basic.MaxDistance = 15.0f;
        basic.Radius = 0.3f;
        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 3.0f;
        scale.y = 3.0f;
        scale.z = 1.0f;

        basic.Position = transform_.position + muzzlePoint;
        basic.SpriteInfo.Renderer.sprite = SpriteData.Instance.Bullet;
        basic.SpriteInfo.Renderer.sortingLayerID = GameManager.Instance.SortLayerTopEffects;
        basic.Direction = direction;
        basic.Color = new Color(1.0f, 0.4f, 0.1f);
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        ProjectileManager.Instance.Fire(basic);
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.EnemyShoot, 0.5f);
    }

    protected override void OnDeath()
    {
        GameManager.Instance.MakePoof(position_, 3, 1.0f);
    }

    IEnumerator Think()
    {
        while (isSpawning_)
            yield return null;

        const float DelayBeforeFirstShoot = 1.0f;
        float nextShoot = Time.time + DelayBeforeFirstShoot;

        while (true)
        {
            float distanceToPlayer = BlackboardScript.DistanceToPlayer(position_);
            const float MinDistToShoot = 4.0f;
            if (IsFullyReady && Time.time > nextShoot && distanceToPlayer > MinDistToShoot)
            {
                int projectileCount = 5 + SaveGame.RoundScore / 15;
                projectileCount = Mathf.RoundToInt(projectileCount * GameMode.FireCountModifier);
                if (projectileCount > 12)
                    projectileCount = 12;
                if (projectileCount < 1)
                    projectileCount = 1;

                for (int i = 0; i < projectileCount; ++i)
                {
                    Shoot();
                    GameManager.Instance.MakeFlash(position_, 1.0f);
                    yield return new WaitForSeconds(0.25f * GameMode.FireRateModifier);
                }
                nextShoot = Time.time + cd;
                cd = (Cd + Random.value) * GameMode.FireRateModifier;
            }

            if (distanceToPlayer < 8.0f)
            {
                target_ = GameManager.Instance.PlayerTrans.position;
                target_.x = -target_.x;
                target_.y = -target_.y;
                target_ = GameManager.Instance.ClampToBounds(target_, renderer_.sprite);
            }
            else
            {
                if (Vector3.Distance(position_, target_) < 0.25f)
                    target_ = GetNewTarget();
            }

            float deltaX = target_.x - transform_.position.x;
            float deltaY = target_.y - transform_.position.y;

            moveVec_.x = deltaX;
            moveVec_.y = deltaY;
            moveVec_.z = 0;
            moveVec_.Normalize();

            UpdatePosition(moveVec_, Speed);
            yield return null;
        }
    }

    protected override void PreUpdate()
    {
        bool dead = Hp <= 0.0f;
        if (dead)
            return;

        if (Time.time > flashEndTime_)
            material_.SetFloat(flashParamId_, 0.0f);

        Vector3 scale = scale_;
        scale.x = moveVec_.x < 0 ? -scale_.x : scale.x;
        transform_.localScale = scale;
    }
}
