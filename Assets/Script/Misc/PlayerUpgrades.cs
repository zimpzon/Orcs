using UnityEngine;

public class UpgradeData
{
    public class CounterClass
    {
        public float PaintballTimer;
    }

    // global
    public float DamageMul = 1.0f;
    public float BaseCritChance = 0.05f;
    public float CritChanceMul = 1.0f;
    public float CritValueMul = 1.5f;
    public float TimeScale = 1.0f;

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
    public float BaseHealthRegenSec = 0.2f;
    public float HealthRegenSecAdd = 0.0f;
    public float OnDamageTimeImmune = 0.2f;
    public float HealthDefenseMul = 1.0f;
    public float BaseMoveSpeed = 4.0f;
    public float MoveSpeedMul = 1.0f;
    public bool IsRambo = false;
    public float RamboEndTime = 0;

    public int RescueDuckHp = 5;

    // weapons
    public float MagicMissileBaseDamage = 40.0f;
    public float MagicMissileDamageMul = 1.0f;
    public float MagicMissileBaseBulletCd = 0.15f;
    public float MagicMissileBaseCd = 2.0f;
    public float MagicMissileCdMul = 1.0f;
    public float MagicMissileBaseRange = 2.2f;
    public float MagicMissileRangeMul = 1.0f;
    public float MagicMissileBaseSpeed = 8.0f;
    public float MagicMissileSpeedMul = 1.0f;
    public float MagicMissileBaseBullets = 4;
    public float MagicMissileBulletsAdd = 0.0f;
    public int MagicMissileMultiShots = 0;

    public bool MeleeThrowBought = false;
    public bool MeleeThrowEnabledInRound = false;
    public float MeleeThrowBasePower = 8.0f;
    public float MeleeThrowDrag = 8.0f;
    public float MeleeThrowPowerMul = 1.0f;
    public float MeleeThrowBaseDamage = 20.0f;
    public float MeleeThrowBaseCd = 15.0f;
    public float MeleeThrowCdMul = 1.0f;
    public bool MeleeThrowLeft = true;
    public bool MeleeThrowRight = true;
    public bool MeleeThrowUp = true;
    public bool MeleeThrowDown = true;
    public bool MeleeThrowUpLeft = true;
    public bool MeleeThrowUpRight = true;
    public bool MeleeThrowDownLeft = true;
    public bool MeleeThrowDownRight = true;

    public bool SawBladeBought = false;
    public bool SawBladeEnabledInRound = false;
    public float SawBladeMaxDamage = 250;
    public float SawBladeDurabilityMul = 1.0f;
    public float SawBladeMaxDistance = 35;
    public float SawBladeBaseCd = 6.0f;
    public float SawBladeCdMul = 1.0f;

    public bool BurstOfFrostBought = false;
    public bool BurstOfFrostEnabledInRound = false;
    public float BurstOfFrostBaseCd = 1.5f;
    public float BurstOfFrostCdMul = 1.0f;
    public float BurstOfFrostBaseRange = 2.0f;
    public float BurstOfFrostRangeMul = 1.0f;
    public float BurstOfFrostBaseFreezeChance = 0.7f;
    public float BurstOfFrostFreezeChanceMul = 1.0f;
    public float BurstOfFrostBaseFreezeTime = 2.0f;
    public float BurstOfFrostFreezeTimeMul = 1.0f;

    public bool PaintballBought = false;
    public bool PaintballActiveInRound = false;
    public float PaintballBaseRange = 3.0f;
    public float PaintballRangeMul = 1.0f;
    public float PaintballBaseSpeed = 5.0f;
    public float PaintballBaseSlowMul = 0.9f; // slow amount at mass = 1
    public float PaintballBaseDuration = 5.0f;
    public float PaintballDurationMul = 1.0f;
    public float PaintballCd = 3.0f;
    public float PaintballCdMul = 1.0f;
    public int PaintballCount = 12;
    public float PaintballBaseDamagePerSec = 30.0f;
    public float PaintballDamagePerSecMul = 1.0f;

    // circling axe
    public bool CirclingAxeEnabled = false;
    public float CirclingAxeBaseCd = 8.0f;
    public float CirclingAxeCdMul = 1.0f;
    public float CirclingAxeBaseDamage = 45.0f;
    public float CirclingAxeDamageMul = 1.0f;
    public float CirclingAxeBaseSpeed = 6.0f;
    public float CirclingAxeSpeedMul = 1.0f;
    public float CirclingAxeBaseLifetime = 4.0f;
    public float CirclingAxeLifetimeMul = 1.0f;

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
}
