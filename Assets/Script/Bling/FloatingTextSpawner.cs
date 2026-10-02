using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance;

    GameObjectPool textPool_;
    Queue<GameObject> queue_ = new Queue<GameObject>();
    float nextQueueItem_;

    void Awake()
    {
        Instance = this;
        textPool_ = GetComponentInChildren<GameObjectPool>();
    }

    public void Spawn(
        Vector3 position,
        string text,
        Color color,
        float speed = 1.0f,
        float timeToLive = 2.0f,
        float fadeTime = 0.0f,
        FontStyles fontStyle = FontStyles.Bold,
        TMP_FontAsset fontAsset = null,
        bool useQueue = false,
        bool soft = false)
    {
        var go = textPool_.GetFromPool();
        var script = go.GetComponent<FloatingTextScript>();
        script.Init(textPool_, position, text, color, speed, timeToLive, fadeTime, fontStyle, fontAsset, soft);
        if (useQueue)
        {
            go.SetActive(false);
            queue_.Enqueue(go);
        }
        else
        {
            go.SetActive(true);
        }
    }

    // Damage numbers: hits on the same enemy within MergeWindow are summed into one number (which pops on each
    // merge) instead of spamming a new text per hit.
    const float MergeWindow = 0.12f;
    // Overall size of damage numbers (1 = the floating text prefab's normal size).
    const float DamageTextScale = 0.85f;
    readonly Dictionary<int, (FloatingTextScript script, int generation)> damageTexts_ = new();
    static readonly Color CritColor = new Color(1.0f, 0.82f, 0.2f);

    public void SpawnDamage(ActorBase enemy, long amount, bool isCrit, Color color)
    {
        // Bigger hits (relative to the enemy's max HP) get bigger text.
        float hitFraction = enemy.BaseHp > 0 ? Mathf.Clamp01(amount / (float)enemy.BaseHp) : 0f;
        float sizeMul = DamageTextScale * (0.85f + 0.5f * Mathf.Sqrt(hitFraction));

        int key = enemy.UniqueId;
        if (damageTexts_.TryGetValue(key, out var entry)
            && entry.script.Generation == entry.generation
            && entry.script.CanMerge(MergeWindow))
        {
            entry.script.AddDamage(amount, isCrit, sizeMul);
            return;
        }

        if (damageTexts_.Count > 200)
            PruneStaleDamageTexts();

        var go = textPool_.GetFromPool();
        var script = go.GetComponent<FloatingTextScript>();
        Vector3 position = enemy.transform.position + Vector3.up * 0.8f + Vector3.right * Random.Range(-0.3f, 0.3f);
        script.InitDamage(textPool_, position, amount, isCrit, color, CritColor, sizeMul);
        go.SetActive(true);
        damageTexts_[key] = (script, script.Generation);
    }

    // Gold pickup numbers: collapse like damage numbers, but split each burst of coins into a random 4-5 numbers.
    // When a burst starts we pick the group count; each new number then takes its share of the coins still in flight
    // (AutoPickUpScript.ActiveMoneyCount). Each merge pops the number.
    const int GoldGroupsMin = 4;
    const int GoldGroupsMax = 5;
    const float GoldBurstGap = 0.5f;       // no gold pickup for this long = next pickup starts a new burst
    const float GoldMergeWindow = 0.4f;    // never merge pickups further apart than this
    // Overall size of gold numbers (1 = the floating text prefab's normal size).
    const float GoldTextScale = 1.0f;
    FloatingTextScript goldText_;
    int goldTextGeneration_;
    int goldMergesLeft_;
    int goldGroupsLeft_;
    int goldBurstIndex_;
    float lastGoldPickupTime_ = -100f;

    public void SpawnGold(Vector3 position, Decimal512 amount, Color color)
    {
        float now = GameManager.Instance.GameTime;
        bool newBurst = now - lastGoldPickupTime_ > GoldBurstGap;
        lastGoldPickupTime_ = now;

        if (!newBurst && goldMergesLeft_ > 0 && goldText_ != null && goldText_.Generation == goldTextGeneration_
            && goldText_.CanMerge(GoldMergeWindow, maxMergeAge: float.MaxValue))
        {
            goldText_.AddGold(amount);
            goldMergesLeft_--;
            return;
        }

        if (newBurst)
        {
            goldGroupsLeft_ = Random.Range(GoldGroupsMin, GoldGroupsMax + 1);
            goldBurstIndex_ = 0;
        }

        // Fan the numbers of a burst out: alternate left/right of the player, a bit further out each time,
        // and drift outward to that side so they don't overlap.
        float side = goldBurstIndex_ % 2 == 0 ? -1f : 1f;
        position += new Vector3(side * (0.35f + 0.35f * (goldBurstIndex_ / 2)), 0.12f * goldBurstIndex_, 0);
        goldBurstIndex_++;

        // Coins still in flight include the one being picked up right now.
        int coinsLeft = Mathf.Max(1, AutoPickUpScript.ActiveMoneyCount);
        int groups = Mathf.Max(1, goldGroupsLeft_);
        goldMergesLeft_ = Mathf.CeilToInt(coinsLeft / (float)groups) - 1;
        goldGroupsLeft_ = Mathf.Max(1, goldGroupsLeft_ - 1);

        var go = textPool_.GetFromPool();
        goldText_ = go.GetComponent<FloatingTextScript>();
        goldText_.InitGold(textPool_, position, amount, color, GoldTextScale, side);
        go.SetActive(true);
        goldTextGeneration_ = goldText_.Generation;
    }

    readonly List<int> staleKeys_ = new();

    void PruneStaleDamageTexts()
    {
        staleKeys_.Clear();
        foreach (var (key, entry) in damageTexts_)
        {
            if (entry.script.Generation != entry.generation || !entry.script.CanMerge(MergeWindow))
                staleKeys_.Add(key);
        }

        foreach (var key in staleKeys_)
            damageTexts_.Remove(key);
    }

    public void Update()
    {
        if (queue_.Count > 0 && GameManager.Instance.GameTime > nextQueueItem_)
        {
            var go = queue_.Dequeue();
            go.SetActive(true);

            nextQueueItem_ = GameManager.Instance.GameTime + 0.15f;
        }
    }
}