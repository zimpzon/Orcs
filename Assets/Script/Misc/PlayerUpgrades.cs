using UnityEngine;

public class UpgradeData
{
    public class CounterClass
    {
        public float PaintballTimer;
        public int OnKillDropBombCurrentKillCount;
    }

    // global
    public float DamageMul = 1.0f;
    public float BaseCritChance = 0.01f;
    public float CritChanceMul = 1.0f;
    public float CritValueMul = 1.5f;

    // gold
    public float MoneyDoubleChance = 0.05f;
    public float DropMoneyOnKillChance = 0.01f;
    public int DropMoneyOnKillMin = 1;
    public int DropMoneyOnKillMax = 3;

    // xp
    public float XpDoubleChance = 0.05f;
    public float XpValueMul = 1.0f;

    // player
    public int BaseHealth = 100;
    public float HealthMul = 1.0f;
    public float BaseHealthRegenSec = 0.1f;
    public float HealthRegenSecAdd = 0.0f;
    public float OnDamageTimeImmune = 0.2f;
    public float HealthDefenseMul = 1.0f;
    public float MoveSpeedMul = 1.0f;

    // weapons
    public float MagicMissileBaseDamage = 15.0f;
    public float MagicMissileDamageMul = 1.0f;
    public float MagicMissileBaseBulletCd = 0.2f;
    public float MagicMissileBaseCd = 1.0f;
    public float MagicMissileCdMul = 1.0f;
    public float MagicMissileBaseRange = 4.0f;
    public float MagicMissileRangeMul = 1.0f;
    public float MagicMissileBaseSpeed = 8.0f;
    public float MagicMissileSpeedMul = 1.0f;
    public float MagicMissileBaseBullets = 3;
    public float MagicMissileBulletsMul = 1.0f;

    public bool MeleeThrowBought = true;
    public bool MeleeThrowEnabledInRound = false;
    public float MeleeThrowBasePower = 8.0f;
    public float MeleeThrowPowerMul = 1.0f;
    public float MeleeThrowBaseDamage = 20.0f;
    public float MeleeThrowBaseCd = 2.0f;
    public float MeleeThrowCdMul = 1.0f;
    public bool MeleeThrowLeft = false;
    public bool MeleeThrowRight = false;
    public bool MeleeThrowUp = false;
    public bool MeleeThrowDown = false;
    public bool MeleeThrowUpLeft = false;
    public bool MeleeThrowUpRight = false;
    public bool MeleeThrowDownLeft = false;
    public bool MeleeThrowDownRight = false;

    public bool SawBladeBought = true;
    public bool SawBladeEnabledInRound = false;
    public float SawBladeMaxDamage = 200;
    public float SawBladeDurabilityMul = 1.0f;
    public float SawBladeMaxDistance = 25;
    public float SawBladeBaseCd = 2.0f;
    public float SawBladeCdMul = 1.0f;

    public bool BurstOfFrostBought = true;
    public bool BurstOfFrostEnabledInRound = false;
    public float BurstOfFrostBaseCd = 1.0f;
    public float BurstOfFrostCdMul = 1.0f;
    public float BurstOfFrostBaseRange = 2.0f;
    public float BurstOfFrostRangeMul = 1.0f;
    public float BurstOfFrostBaseFreezeChance = 0.25f;
    public float BurstOfFrostFreezeChanceMul = 1.0f;
    public float BurstOfFrostBaseFreezeTime = 2.0f;
    public float BurstOfFrostFreezeTimeMul = 1.0f;

    public bool PaintballBought = true;
    public bool PaintballActiveInRound = false;
    public float PaintballBaseRange = 3.0f;
    public float PaintballRangeMul = 1.0f;
    public float PaintballBaseSlowMul = 0.9f;
    public float PaintballBaseDuration = 4.0f;
    public float PaintballDurationMul = 1.0f;
    public float PaintballCd = 2.0f;
    public float PaintballCdMul = 1.0f;
    public int PaintballCount = 5;
    public float PaintballBaseDamagePerSec = 30.0f;
    public float PaintballDamagePerSecMul = 30.0f;

    // orc
    public float OrcReviveTime = 8.0f;
    public float OrcReviveTimeMul = 1.0f;

    public bool OrcJedisEnabled = false;
    public float OrcJediKnockBackForceMul = 1.0f;

    public CounterClass Counters = new ();
}

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    public static UpgradeData Data = new ();

    public static void ResetAll()
    {
        Data = new UpgradeData();
    }

    void Awake()
    {
        Instance = this;
    }

    void UpdatePaintball()
    {
        if (!Data.PaintballActiveInRound)
            return;

        Data.Counters.PaintballTimer += Time.deltaTime;
        if (Data.Counters.PaintballTimer > Data.PaintballCd * PlayerUpgrades.Data.PaintballCdMul)
        {
            var paintball = WeaponBase.GetWeapon(WeaponType.PaintBallRandom);

            float angleStep = 360 / Data.PaintballCount;
            float angle = 0;
            for (int i = 0; i < Data.PaintballCount; ++i)
            {
                var dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
                paintball.Fire(this.transform, dir, GameManager.Instance.SortLayerTopEffects, out _);
                angle += angleStep;
            }
            Data.Counters.PaintballTimer = 0;
        }
    }

    public void UpdateUpgrades()
    {
        UpdatePaintball();
    }
}
