using Assets.Script;
using System.Collections;
using TMPro;
using UnityEngine;

public enum OrcMood { Default, Nervous };

public class OrcController : MonoBehaviour
{
    public OrcMood Mood;
    public TextMeshPro Text;

    Vector3 scale_;
    Transform trans_;
    SpriteRenderer renderer_;
    ParticleSystem hearts_;
    Transform arrow_;
    bool pickedUp_;
    bool chasePlayer_ = false;
    Vector3 target_;

    const float ChaseSpeed = 7.0f;
    const float NervousMinDistance = 4.0f;

    void Start()
    {
        trans_ = this.transform;
        renderer_ = GetComponent<SpriteRenderer>();
        scale_ = trans_.localScale;
        hearts_ = trans_.Find("Hearts").GetComponent<ParticleSystem>();
        arrow_ = trans_.Find("Arrow").GetComponent<Transform>();
        Text.text = "";

        StartCoroutine(Think());
    }

    public void SetChasePlayer(bool chase)
    {
        chasePlayer_ = chase;
    }

    public void Hide()
    {
        SetPosition(Vector3.right * 10000);
    }

    public void SetPosition(Vector3 pos)
    {
        trans_.position = pos;
        pickedUp_ = false;
        chasePlayer_ = false;
        target_ = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (pickedUp_)
            return;

        if (Mood != OrcMood.Nervous)
            hearts_.Emit(1);

        pickedUp_ = true;
        GameManager.Instance.OnOrcPickup(trans_.position);
    }

    IEnumerator Think()
    {
        float nextMove = Time.time + 3.0f;
        while (true)
        {
            if (Mood == OrcMood.Nervous && Time.time > nextMove && !chasePlayer_)
            {
                // Look confused for a little bit
                Text.text = "?";
                yield return new WaitForSeconds(2.0f);
                if (Mood != OrcMood.Nervous || chasePlayer_) continue;

                // Get alarmed for a very short time
                Text.text = "!";
                yield return new WaitForSeconds(0.5f);
                if (Mood != OrcMood.Nervous || chasePlayer_) continue;
                Text.text = "";

                // Run randomly
                target_ = PositionUtility.GetPointInsideArena(1.0f, 1.0f);
                nextMove = Time.time + 5 + Random.value * 5;
            }

            if (chasePlayer_ && Text.text != "")
                Text.text = "";

            yield return null;
        }
    }

    void Update()
    {
        Text.enabled = Mood == OrcMood.Nervous;
        renderer_.sortingOrder = Mathf.RoundToInt(trans_.position.y * 100f) * -1;

        float distanceToPlayer = BlackboardScript.DistanceToPlayer(trans_.position);
        bool isNervous = Mood == OrcMood.Nervous;
        bool playerIsClose = distanceToPlayer < 3.0f;

        var em = hearts_.emission;
        em.enabled = (playerIsClose || chasePlayer_) && Mood != OrcMood.Nervous;

        renderer_.flipX = trans_.position.x > GameManager.Instance.PlayerTrans.position.x;

        Vector3 arrowPos = arrow_.localPosition;
        arrowPos.y = 1.0f + Mathf.Sin(Time.time * 8) * 0.25f;
        arrow_.localPosition = arrowPos;

        var playerPos = GameManager.Instance.PlayerTrans.position;
        if (chasePlayer_)
            target_ = playerPos;

        var targetVec = target_ - trans_.position;
        float distanceToTarget = targetVec.magnitude;
        var targetDir = targetVec.normalized;

        if (isNervous && distanceToPlayer <= NervousMinDistance || distanceToTarget < 0.2f)
        {
            target_ = trans_.position; // Stop
        }
        else
        {
            trans_.position += targetDir * ChaseSpeed * Time.deltaTime;
        }

        //if (chasePlayer_)
        //{
        //    if (!(Mood == OrcMood.Nervous && dist < NervousMinDistance))
        //    {
        //        trans_.position += dir * ChaseSpeed * Time.deltaTime;

        //        const float PushRadius = 0.1f;
        //        int aliveCount = BlackboardScript.GetEnemies(trans_.position, PushRadius);
        //        for (int i = 0; i < aliveCount; ++i)
        //        {
        //            int idx = BlackboardScript.Matches[i].Idx;
        //            ActorBase enemy = BlackboardScript.Enemies[idx];

        //            Vector3 pushDir = (enemy.transform.position - trans_.position).normalized;
        //            enemy.AddForce(pushDir);
        //        }
        //    }
    }
}
