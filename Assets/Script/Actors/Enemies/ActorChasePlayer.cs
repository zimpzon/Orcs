using System;
using System.Collections;
using UnityEngine;

public class ActorChasePlayer : MonoBehaviour
{
    [NonSerialized] public bool DoPositionNudging;
    [NonSerialized] public Vector2 NudgeTowards;

    enum TargetMode { Chase, Flee }

    Vector3 moveVec_;
    Vector3 target_;
    ActorBase actorBase_;

    TargetMode currentMode_;
    float modeEndTime_;

    void Awake()
    {
        actorBase_ = GetComponent<ActorBase>();
    }

    void OnEnable()
    {
        PickNewMode();
        target_ = GetNewTarget();
        StartCoroutine(Think());
    }

    float _nextTargetTime;

    protected virtual IEnumerator Think()
    {
        while (actorBase_.IsSpawning)
            yield return null;

        while (true)
        {
            if (G.D.GameTime > modeEndTime_)
            {
                PickNewMode();
                target_ = GetNewTarget();
            }

            if (G.D.GameTime > _nextTargetTime)
            {
                target_ = GetNewTarget();
                _nextTargetTime = G.D.GameTime + 0.25f; // Less frequent than before
            }

            yield return null;
        }
    }

    void PickNewMode()
    {
        float roll = UnityEngine.Random.value;
        currentMode_ = (roll < 0.3f) ? TargetMode.Chase : TargetMode.Flee;

        // Stay in this mode for 1.5–3 seconds
        float duration = UnityEngine.Random.Range(1.5f, 3.0f);
        modeEndTime_ = G.D.GameTime + duration;
    }

    protected virtual Vector3 GetNewTarget()
    {
        if (currentMode_ == TargetMode.Chase)
        {
            return G.D.PlayerPos;
        }
        else
        {
            return PositionUtility.GetPointInsideArena();
        }
    }

    void Update()
    {
        if (actorBase_.Hp <= 0.0f || actorBase_.IsSpawning || GameManager.Instance.GameState != GameManager.State.Idle_Fighting)
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

        actorBase_.UpdatePosition(moveVec_, actorBase_.Speed);
    }
}
