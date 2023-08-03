using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorOgreShamanShoot : MonoBehaviour
{
    ActorBase actorBase_;
    float cd;

    void Awake()
    {
        actorBase_ = GetComponent<ActorBase>();
    }

    private void OnEnable()
    {
        //StartCoroutine(Think());
    }

    void Shoot()
    {
        if (actorBase_.Hp <= 0.0f)
            return;

        Vector2 direction = BlackboardScript.PlayerTrans.position - actorBase_.transform.position;
        direction.Normalize();
        direction = RndUtil.RandomSpread(direction, 3);

        Vector3 muzzlePoint = direction * 0.6f;
        actorBase_.AddForce(-direction * 0.1f);

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

        basic.Position = actorBase_.transform.position + muzzlePoint;
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

    IEnumerator Think()
    {
        while (!actorBase_.IsFullyReady)
            yield return null;

        const float DelayBeforeFirstShoot = 1.0f;
        float nextShoot = Time.time + DelayBeforeFirstShoot;

        while (true)
        {
            float distanceToPlayer = BlackboardScript.DistanceToPlayer(actorBase_.transform.position);
            const float MinDistToShoot = 4.0f;
            if (actorBase_.IsFullyReady && GameManager.Instance.GameTime > nextShoot && distanceToPlayer > MinDistToShoot)
            {
                int projectileCount = 5 + SaveGame.RoundScore / 15;
                projectileCount = 2;

                for (int i = 0; i < projectileCount; ++i)
                {
                    Shoot();
                    GameManager.Instance.MakeFlash(actorBase_.transform.position, 1.0f);
                    yield return new WaitForSeconds(0.25f);
                }

                nextShoot = Time.time + cd;
                cd = 1 + Random.value;
            }

            //if (distanceToPlayer < 8.0f)
            //{
            //    target_ = GameManager.Instance.PlayerTrans.position;
            //    target_.x = -target_.x;
            //    target_.y = -target_.y;
            //    target_ = GameManager.Instance.ClampToBounds(target_, renderer_.sprite);
            //}
            //else
            //{
            //    if (Vector3.Distance(position_, target_) < 0.25f)
            //        target_ = GetNewTarget();
            //}

            yield return null;
        }
    }

    void Update()
    {
        
    }
}
