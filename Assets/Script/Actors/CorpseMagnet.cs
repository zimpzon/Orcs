using Assets.Script;
using System.Collections.Generic;
using UnityEngine;

class CorpseMagnet : MonoBehaviour
{
    public float Radius = 5;

    Transform trans_;
    float nextScanTime;
    List<ActorBase> corpses_ = new List<ActorBase>();
    int count_ = 0;

    void Start()
    {
        trans_ = this.transform;
    }

    private void Update()
    {
        if (Time.time >= nextScanTime)
        {
            DoScan();
            nextScanTime = Time.time + 0.1f;
        }

        for (int i = 0; i < count_; ++i)
        {
            if (corpses_[i].Hp > 0.0f)
                continue;

            Vector3 dir = trans_.position - corpses_[i].transform.position;
            float dist2 = dir.sqrMagnitude;
            if (dist2 > 0.1f)
            {
                Vector3 move = dir;
                corpses_[i].transform.position += move * Time.deltaTime;
            }
        }
    }

    private void DoScan()
    {
        count_ = BlackboardScript.GetDeadEnemies(trans_.position, Radius, 10);
        corpses_.Clear();
        for (int i = 0; i < count_; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            corpses_.Add(BlackboardScript.DeadEnemies[idx]);
        }
    }
}
