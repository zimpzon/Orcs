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
        bool useQueue = false)
    {
        var go = textPool_.GetFromPool();
        var script = go.GetComponent<FloatingTextScript>();
        script.Init(textPool_, position, text, color, speed, timeToLive, fadeTime, fontStyle, fontAsset);
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