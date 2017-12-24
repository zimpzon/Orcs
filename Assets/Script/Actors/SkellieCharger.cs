using Assets.Script;
using System.Collections;
using UnityEngine;

public class SkellieCharger : ActorBase
{
    public Color ChargeColor;
    float Speed;
    float chargeSpeed_ = 0.0f;
    Vector3 moveVec_;
    Vector3 scale_;
    Vector3 target_;

    protected override void PreAwake()
    {
        Speed = 1.5f * GameMode.MoveSpeedModifier;
        Hp = 50 * GameMode.HitpointModifier;
        mass_ = 1.0f * GameMode.MassModifier;
        ActorType = ActorTypeEnum.SmallCharger;
    }

    protected override void PostStart()
    {
        scale_ = transform_.localScale;
        position_ = this.Transform.position;
        currentAnimations_ = SpriteData.Instance.Skellie2WalkSprites;
        target_ = GetNewTarget();
        StartCoroutine(Think());
    }

    IEnumerator Think()
    {
        while (isSpawning_)
            yield return null;

        const float DelayBeforeFirstCharge = 1.0f;
        float nextCharge = Time.time + DelayBeforeFirstCharge;

        while (true)
        {
            float distanceToPlayer = BlackboardScript.DistanceToPlayer(position_);
            const float MinDistToCharge = 3.0f;
            if (isFullyReady_ && Time.time > nextCharge && distanceToPlayer > MinDistToCharge)
            {
                target_ = GameManager.Instance.PlayerTrans.position + (Vector3)(Random.insideUnitCircle * 2);
                chargeSpeed_ = Speed * 5;
                if (chargeSpeed_ > 8.0f)
                    chargeSpeed_ = 8.0f;

                material_.color = ChargeColor;

                while ((transform_.position - target_).magnitude > 0.2f)
                {
                    if (Random.value < 0.1f)
                        GameManager.Instance.MakePoof(transform_.position + Vector3.down * 0.2f, 1, 0.3f);

                    yield return null;
                }

                material_.color = Color.white;
                chargeSpeed_ = 0.0f;
                float cd = (1.0f  + Random.value * 3) * GameMode.ChargeCdModifier;
                nextCharge = Time.time + cd;
            }

            yield return null;
        }
    }

    Vector3 GetNewTarget()
    {
        Vector3 result = PositionUtility.GetPointInsideArena(1.0f, 1.0f);
        return result;
    }

    protected override void OnDeath()
    {
        GameManager.Instance.MakePoof(position_, 3, 1.0f);
        if (GameMode.SmallEnemiesExplode)
        {
            ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
            ep.startColor = new Color32(255, 255, 255, 255);
            ep.position = position_;
            ep.startSize = 0.75f;
            ep.startLifetime = 2.0f;
            for (int i = 0; i < Mathf.RoundToInt(5 * GameMode.ExplosionModifier); ++i)
            {
                Vector2 rndDir = Random.insideUnitCircle.normalized;
                ep.velocity = rndDir * 1.5f;
                GameManager.Instance.NpcFlameParticles.Emit(ep, 1);
            }
        }
    }

    protected override void PreUpdate()
    {
        bool dead = Hp <= 0.0f;
        if (dead)
            return;

        if (Time.time > flashEndTime_)
        {
            material_.SetFloat(flashParamId_, 0.0f);
        }

        if (chargeSpeed_ == 0.0f)
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

        UpdatePosition(moveVec_, Speed + chargeSpeed_);

        Vector3 scale = scale_;
        scale.x = moveVec_.x < 0 ? -scale_.x : scale.x;
        transform_.localScale = scale;
    }
}
