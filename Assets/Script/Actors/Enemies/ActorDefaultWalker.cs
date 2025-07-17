using System;
using System.Collections;
using UnityEngine;

public class ActorDefaultWalker : MonoBehaviour
{
    [NonSerialized] public bool DoPositionNudging;
    [NonSerialized] public Vector2 NudgeTowards;

    Vector3 moveVec_;
    Vector3 target_;
    ActorBase actorBase_;

    void Awake()
    {
        actorBase_ = GetComponent<ActorBase>();
    }

    void OnEnable()
    {
        target_ = GetNewTarget();
        StartCoroutine(Think());
    }

    protected virtual IEnumerator Think()
    {
        while (actorBase_.IsSpawning)
        {
            yield return null;
        }

        while (true)
        {
            float distanceToTarget = (actorBase_.transform.position - target_).sqrMagnitude;
            if (distanceToTarget < 1)
            {
                target_ = GetNewTarget();
            }

            yield return null;
        }
    }

    protected virtual Vector3 GetNewTarget()
    {
        //FloatingTextSpawner.Instance.Spawn(actorBase_.transform.position, "new target", Color.yellow);
        Vector3 result = PositionUtility.GetPointInsideArena();
        return result;
    }

    void Update()
    {
        bool dead = actorBase_.Hp <= 0.0f;
        if (dead || actorBase_.IsSpawning || GameManager.Instance.GameState != GameManager.State.Idle_Fighting)
            return;

        float deltaX = target_.x - actorBase_.transform.position.x;
        float deltaY = target_.y - actorBase_.transform.position.y;

        moveVec_.x = deltaX;
        moveVec_.y = deltaY;
        moveVec_.z = 0;

        moveVec_.Normalize();

        if (DoPositionNudging)
        {
            var dir = ((Vector3)NudgeTowards - actorBase_.transform.position).normalized;
            moveVec_ += dir * 0.25f;
        }

        //Debug.DrawLine(transform.position, target_, Color.green);

        actorBase_.UpdatePosition(moveVec_, actorBase_.Speed);
    }
}
