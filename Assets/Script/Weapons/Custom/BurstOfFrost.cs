using Assets.Script;
using UnityEngine;

public class BurstOfFrost : MonoBehaviour, IPlayerToggleEfffect
{
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
        renderer_.transform.localScale = Vector3.one * 0.1f; // another hack :p why doesn't it go away when disabled?
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
        nextBurst_ = G.D.GameTime + PlayerUpgrades.Data.BurstOfFrostBaseCd * PlayerUpgrades.Data.BurstOfFrostCdMul;
    }

    void Update()
    {
        if (G.D.GameTime > nextBurst_ && !isBursting_)
        {
            if (!PlayerUpgrades.Data.BurstOfFrostEnabledInRound)
                return;

            isBursting_ = true;
            burstStartTime_ = G.D.GameTime;
            float scale = PlayerUpgrades.Data.BurstOfFrostBaseRange * PlayerUpgrades.Data.BurstOfFrostRangeMul;
            renderer_.transform.localScale = Vector2.one * scale * 2; // range is radius, scale is diameter, so x2
            GameManager.Instance.MakeFlash(transform.position, 3.0f);
            snowflakes_.Emit(15);
        }

        if (isBursting_)
        {
            const float BurstTime = 0.1f;
            float t = Mathf.Clamp01(1.0f - (burstStartTime_ + BurstTime - G.D.GameTime) / BurstTime);
            baseColor_.a = baseAlpha_ + t * 0.15f;
            renderer_.color = baseColor_;

            if (t >= 1.0f)
            {
                isBursting_ = false;
                SetNextBurst();
                baseColor_.a = baseAlpha_;
                renderer_.color = baseColor_;

                Burst(transform.position, PlayerUpgrades.Data.BurstOfFrostBaseRange * PlayerUpgrades.Data.BurstOfFrostRangeMul, Force, damage: 0);
            }
        }
    }

    public void Burst(Vector3 pos, float radius, float force, float damage = 0)
    {
        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            ActorBase enemy = BlackboardScript.EnemyOverlap[i];
            if (enemy.IsBoss)
                continue;

            if (Random.value < PlayerUpgrades.Data.BurstOfFrostBaseFreezeChance * PlayerUpgrades.Data.BurstOfFrostFreezeChanceMul)
                enemy.OnFreeze(G.D.BurstOfFrostColor, PlayerUpgrades.Data.BurstOfFrostBaseFreezeTime * PlayerUpgrades.Data.BurstOfFrostFreezeTimeMul);

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
