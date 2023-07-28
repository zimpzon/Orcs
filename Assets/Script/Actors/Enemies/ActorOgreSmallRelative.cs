using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorOgreSmallRelative : ActorDefaultWalker
{
    Vector3 moveVec_;
    Vector3 target_;

    protected override IEnumerator Think()
    {
        while (isSpawning_)
            yield return null;

        while (true)
        {
            float distanceToPlayer = BlackboardScript.DistanceToPlayer(position_);
            target_ = Vector3.zero;// GameManager.Instance.PlayerTrans.position;
            yield return null;
        }
    }

}
