using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorDefaultWalker : MonoBehaviour
{
    Vector3 moveVec_;
    Vector3 target_;
    ActorBase actorBase_;

    void Start()
    {
        actorBase_ = GetComponent<ActorBase>();
        target_ = GetNewTarget();
        StartCoroutine(Think());
    }

    protected virtual IEnumerator Think()
    {
        while (actorBase_.IsSpawning)
            yield return null;

        while (true)
        {
            float distanceToPlayer = BlackboardScript.DistanceToPlayer(actorBase_.transform.position);
            target_ = GameManager.Instance.PlayerTrans.position;
            yield return null;
        }
    }

    protected virtual Vector3 GetNewTarget()
    {
        Vector3 result = PositionUtility.GetPointInsideArena(1.0f, 1.0f);
        return result;
    }

    void Update()
    {
        bool dead = actorBase_.Hp <= 0.0f;
        if (dead)
            return;

        float deltaX = target_.x - actorBase_.transform.position.x;
        float deltaY = target_.y - actorBase_.transform.position.y;

        moveVec_.x = deltaX;
        moveVec_.y = deltaY;
        moveVec_.z = 0;
        moveVec_.Normalize();

        actorBase_.UpdatePosition(moveVec_, actorBase_.Speed);
    }
}
