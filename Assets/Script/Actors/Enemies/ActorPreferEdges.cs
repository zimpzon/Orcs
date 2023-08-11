using Assets.Script;
using System.Collections;
using UnityEngine;

public class ActorPreferEdges : MonoBehaviour
{
    Vector3 moveVec_;
    Vector3 target_;
    ActorBase actorBase_;
    Rect? myRect_;
    Transform trans_;

    void Awake()
    {
        actorBase_ = GetComponent<ActorBase>();
        trans_ = transform;
    }

    void OnEnable()
    {
        myRect_ = null;
        target_ = BlackboardScript.PlayerTrans.position;

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
            var myPos = trans_.position;
            if (myRect_ == null)
            {
                if (GameManager.TopRect.Contains(myPos))
                    myRect_ = GameManager.TopRect;
                else if (GameManager.BottomRect.Contains(myPos))
                    myRect_ = GameManager.BottomRect;
                else if (GameManager.LeftRect.Contains(myPos))
                    myRect_ = GameManager.LeftRect;
                else if (GameManager.RightRect.Contains(myPos))
                    myRect_ = GameManager.RightRect;

                if (myRect_ != null)
                    target_ = PositionUtility.GetPointInsideRect(myRect_.Value);
            }

            if (myRect_ == null)
                target_ = BlackboardScript.PlayerTrans.position;

            yield return null;
        }
    }

    void Update()
    {
        //DebugUtil.DrawRect(GameManager.TopRect, Color.yellow);
        //DebugUtil.DrawRect(GameManager.BottomRect, Color.red);
        //DebugUtil.DrawRect(GameManager.LeftRect, Color.cyan);
        //DebugUtil.DrawRect(GameManager.RightRect, Color.blue);

        bool dead = actorBase_.Hp <= 0.0f;
        if (dead)
            return;

        float deltaX = target_.x - actorBase_.transform.position.x;
        float deltaY = target_.y - actorBase_.transform.position.y;

        if (myRect_.HasValue && Mathf.Abs(deltaX) < 0.5f && Mathf.Abs(deltaY) < 0.5f)
            target_ = PositionUtility.GetPointInsideRect(myRect_.Value);

        moveVec_.x = deltaX;
        moveVec_.y = deltaY;
        moveVec_.z = 0;
        moveVec_.Normalize();

        actorBase_.UpdatePosition(moveVec_, actorBase_.Speed);
    }
}
