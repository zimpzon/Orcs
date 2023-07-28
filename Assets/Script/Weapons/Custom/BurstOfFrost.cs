using Assets.Script;
using UnityEngine;

public class BurstOfFrost : MonoBehaviour, IPlayerToggleEfffect
{
    public Color FrozenColor = Color.cyan;

    const float FreezeTime = 3.0f;
    const float BurstTime = 0.05f;
    const float MaxScale = 5.0f;
    const float Radius = 3.0f;
    const float Force = 1.0f;
    const float FreezeChance = 0.25f;

    public static BurstOfFrost Instance;

    Color baseColor_;
    float baseAlpha_;
    SpriteRenderer renderer_;
    float nextBurst_;
    bool isBursting_;
    float burstStartTime_;

    private void Awake()
    {
        Instance = this;
        renderer_ = GetComponent<SpriteRenderer>();
        baseColor_ = renderer_.color;
        baseAlpha_ = baseColor_.a;
    }

    void Update()
    {
        if (GameManager.Instance.GameTime > nextBurst_ && !isBursting_)
        {
            isBursting_ = true;
            burstStartTime_ = GameManager.Instance.GameTime;
        }

        if (isBursting_)
        {
            float t = Mathf.Clamp01(1.0f - (burstStartTime_ + BurstTime - GameManager.Instance.GameTime) / BurstTime);
            float scale = t * MaxScale;
            //renderer_.transform.localScale = Vector3.one * Mathf.PingPong(scale, 0.95f + 0.1f);
            baseColor_.a = 8.0f / 255.0f;
            renderer_.color = baseColor_;

            if (t >= 1.0f)
            {
                //renderer_.transform.localScale = Vector3.one;
                isBursting_ = false;
                nextBurst_ = GameManager.Instance.GameTime + 1.0f;
                baseColor_.a = baseAlpha_;
                renderer_.color = baseColor_;

                Burst(transform.position, Radius, Force, damage: 10);
            }
        }
    }

    public void Burst(Vector3 pos, float radius, float force, float damage = 0)
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.SaberHit, volumeScale: 0.1f, pitch: 0.5f);

        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];

            if (Random.value < FreezeChance)
                enemy.OnFreeze(FrozenColor, FreezeTime);

            var dir = enemy.transform.position - pos;
            float distance = dir.magnitude + 0.0001f;
            dir /= distance;
            dir.Normalize();

            force = Mathf.Clamp(((radius - distance) / radius) * force, min: force * 0.5f, max: force);
            var push = dir * force;
            enemy.AddForce(push);
            enemy.ApplyDamage(damage, push.normalized, forceModifier: 0.01f);
        }
    }

    public void Enable(bool enable)
    {
        gameObject.SetActive(enable);
        isBursting_ = false;
        if (enable)
        {
            nextBurst_ = GameManager.Instance.GameTime + 0.5f;
        }
        else
        {
            nextBurst_ = float.MaxValue;
        }
    }
}
