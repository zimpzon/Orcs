using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorDefaultWalker : ActorBase
{
    Vector3 moveVec_;
    Vector3 target_;

    protected override void PostEnable()
    {
        position_ = transform_.position;
        target_ = GetNewTarget();
        StartCoroutine(Think());
    }
    IEnumerator Think()
    {
        while (isSpawning_)
            yield return null;

        while (true)
        {
            float distanceToPlayer = BlackboardScript.DistanceToPlayer(position_);
            target_ = GameManager.Instance.PlayerTrans.position;
            yield return null;
        }
    }

    Vector3 GetNewTarget()
    {
        Vector3 result = PositionUtility.GetPointInsideArena(1.0f, 1.0f);
        return result;
    }

    protected override void PreUpdate()
    {
        bool dead = Hp <= 0.0f;
        if (dead)
            return;

        float deltaX = target_.x - transform_.position.x;
        float deltaY = target_.y - transform_.position.y;

        moveVec_.x = deltaX;
        moveVec_.y = deltaY;
        moveVec_.z = 0;
        moveVec_.Normalize();

        UpdatePosition(moveVec_, Speed);
    }
}
