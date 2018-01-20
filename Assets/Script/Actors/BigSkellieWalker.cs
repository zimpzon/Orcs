using Assets.Script;
using UnityEngine;

public class BigSkellieWalker : ActorBase
{
    float Speed;

    Vector3 moveVec_;
    Vector3 scale_;
    Vector3 target_;

    protected override void PreEnable()
    {
        Speed = 2.0f * GameMode.MoveSpeedModifier;
        Hp = 600 * GameMode.HitpointModifier;
        mass_ = 3.0f * GameMode.MassModifier;
        ActorType = ActorTypeEnum.LargeWalker;
    }

    protected override void PostEnable()
    {
        scale_ = baseScale_  * (GameMode.HasExtraLargeWalkers ? 1.5f : 1.0f);
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
        GameManager.Instance.MakePoof(position_, 4, 2.0f);
        if (Random.value < 0.5f)
            GameProgressScript.Instance.EnemyExplosion(ActorTypeEnum.SmallWalker, transform_.position, 3, 3.0f);
        else
            GameProgressScript.Instance.EnemyExplosion(ActorTypeEnum.Caster, transform_.position, 1, 3.0f);
    }

    protected override void PreUpdate()
    {
        bool dead = Hp <= 0.0f;
        if (dead)
            return;

        if (Time.time > flashEndTime_)
        {
            material_.SetFloat(flashParamId_, 0.0f);

            bool closeToPlayer = BlackboardScript.DistanceToPlayer(position_) < 4.0f;
            if (closeToPlayer || isPainted_ || isLivingBomb_)
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
