using Assets.Script;
using UnityEngine;

public class BurstOfFrost : MonoBehaviour, IPlayerToggleEfffect
{
    public Color FrozenColor = Color.cyan;

    const float Force = 1.0f;

    public static BurstOfFrost Instance;

    Color baseColor_;
    float baseAlpha_;
    SpriteRenderer renderer_;
    float nextBurst_;
    bool isBursting_;
    float burstStartTime_;
    ParticleSystem snowflakes_;

    private void Awake()
    {
        Instance = this;
        renderer_ = GetComponent<SpriteRenderer>();
        snowflakes_ = GetComponent<ParticleSystem>();
        baseColor_ = renderer_.color;
        baseAlpha_ = baseColor_.a;
    }

    public void Disable()
    {
        renderer_.enabled = false;
        isBursting_ = false;
        nextBurst_ = float.MaxValue;
    }

    public void TryEnable()
    {
        if (PlayerUpgrades.Data.BurstOfFrostBought)
        {
            isBursting_ = false;
            renderer_.enabled = true;
            SetNextBurst();
        }
    }

    void SetNextBurst()
    {
        nextBurst_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.BurstOfFrostBaseCd * PlayerUpgrades.Data.BurstOfFrostCdMul;
    }

    void Update()
    {
        if (GameManager.Instance.GameTime > nextBurst_ && !isBursting_)
        {
            if (!PlayerUpgrades.Data.BurstOfFrostEnabledInRound)
                return;

            isBursting_ = true;
            burstStartTime_ = GameManager.Instance.GameTime;
            float scale = PlayerUpgrades.Data.BurstOfFrostBaseRange * PlayerUpgrades.Data.BurstOfFrostRangeMul;
            renderer_.transform.localScale = Vector2.one * scale * 1.5f;
            snowflakes_.Emit(5);
        }

        if (isBursting_)
        {
            const float BurstTime = 0.1f;
            float t = Mathf.Clamp01(1.0f - (burstStartTime_ + BurstTime - GameManager.Instance.GameTime) / BurstTime);
            baseColor_.a = baseAlpha_ + t * 0.15f;
            renderer_.color = baseColor_;

            if (t >= 1.0f)
            {
                isBursting_ = false;
                nextBurst_ = GameManager.Instance.GameTime + 1.0f;
                baseColor_.a = baseAlpha_;
                renderer_.color = baseColor_;

                Burst(transform.position, PlayerUpgrades.Data.BurstOfFrostBaseRange * PlayerUpgrades.Data.BurstOfFrostRangeMul, Force, damage: 0);
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
            if (enemy.IsBoss)
                continue;

            if (Random.value < PlayerUpgrades.Data.BurstOfFrostBaseFreezeChance * PlayerUpgrades.Data.BurstOfFrostFreezeChanceMul)
                enemy.OnFreeze(FrozenColor, PlayerUpgrades.Data.BurstOfFrostBaseFreezeTime * PlayerUpgrades.Data.BurstOfFrostFreezeTimeMul);

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
}
