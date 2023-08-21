using Assets.Script;
using System.Collections.Generic;
using UnityEngine;

public class ActorFlock : MonoBehaviour
{
    ActorBase actorBase_;
    ActorDefaultWalker defaultWalker_;
    float cd;
    Transform trans_;
    List<ActorBase> skipSelf_;

    void Awake()
    {
        actorBase_ = GetComponent<ActorBase>();
        //actorBase_.AlwaysLookAtPlayer = true;
        skipSelf_ = new List<ActorBase> { actorBase_ };
        defaultWalker_ = GetComponent<ActorDefaultWalker>();
        defaultWalker_.DoPositionNudging = true;
        trans_ = transform;
    }

    const float Cd = 0.1f;

    void OnEnable()
    {
    }

    void SetNextCheck()
    {
        cd = G.D.GameTime + Cd + Random.value * 0.1f;
    }

    private void Update()
    {
        CheckForCrowded();
    }

    void CheckForCrowded()
    {
        if (actorBase_.HasForcedDestination || GameManager.Instance.GameTime < cd)
            return;

        var closest = BlackboardScript.GetClosestEnemy(trans_.position, 2.0f, skipSelf_, actorBase_.ActorType);
        if (closest != null)
        {
            var dir = (closest.transform.position - trans_.position).normalized;
            var desiredPos = (closest.transform.position - dir) * 3;
            desiredPos = GameManager.Instance.ClampToBounds(desiredPos, 2.5f);
            Debug.DrawLine(trans_.position, desiredPos);
            defaultWalker_.NudgeTowards = desiredPos;
        }

        SetNextCheck();
    }
}
