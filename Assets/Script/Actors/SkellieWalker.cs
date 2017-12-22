using Assets.Script;
using UnityEngine;

public class SkellieWalker : ActorBase
{
    float Speed;

    Vector3 moveVec_;
    Vector3 scale_;
    Vector3 target_;

    protected override void PreAwake()
    {
        Speed = 1.5f * GameMode.MoveSpeedModifier;
        Hp = 30 * GameMode.HitpointModifier;
        mass_ = 1.0f * GameMode.MassModifier;
        ActorType = ActorTypeEnum.SmallWalker;
    }

    protected override void PostStart()
    {
        scale_ = transform_.localScale;
        position_ = this.Transform.position;
        currentAnimations_ = SpriteData.Instance.SkellieWalkSprites;
        target_ = GetNewTarget();
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

            bool closeToPlayer = BlackboardScript.DistanceToPlayer(position_) < 2.0f;
            if (closeToPlayer || isPainted_)
            {
                target_ = GameManager.Instance.PlayerTrans.position;
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
        }

        Vector3 scale = scale_;
        scale.x = moveVec_.x < 0 ? -scale_.x : scale.x;
        transform_.localScale = scale;
    }
}
