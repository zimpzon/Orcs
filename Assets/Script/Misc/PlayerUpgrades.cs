using System;
using UnityEngine;

public class UpgradeData
{
    // global
    public float DamageMul = 1.0f;
    public float BaseCritChance = 0.05f;
    public float CritChanceMul = 1.0f;
    public float CritValueMul = 1.5f;
    public float TimeScale = 1.0f;
    public bool SpawnChestUnlocked = false;
    public TimeSpan SpawnChestUnlockCriteria_GameTime = new(0, 5, 0);
    public TimeSpan GameStartTime = TimeSpan.Zero;

    // click
    public long ClickPower = 10;

    // gold
    public float MoneyDoubleChance = 0.05f;
    public float DropMoneyOnKillChance = 0.1f;
    public int DropMoneyOnKillMin = 1;
    public int DropMoneyOnKillMax = 3;

    // xp
    public float XpDoubleChance = 0.05f;
    public float XpValueMul = 1.0f;

    // gold & xp
    public bool GoldXpMultiplierBought = false;
    public int GoldXpMultiplyValue = 1;
    public float GoldXpAttractRange = 3f * GameManager.Instance.ArenaScale;

    // player
    public int BaseHealth = 100;
    public float HealthMul = 1.0f;
    public float BaseHealthRegenSec = 0.2f;
    public float HealthRegenSecAdd = 0.0f;
    public float OnDamageTimeImmune = 0.2f;
    public float HealthDefenseMul = 1.0f;
    public float BaseMoveSpeed = 3.0f * GameManager.Instance.ArenaScale;
    public float MoveSpeedMul = 1.0f;

    // weapons
    public float MagicMissileBaseDamage = 10.0f;
    public float MagicMissileDamageMul = 1.0f;
    public float MagicMissileEffectiveDamage => MagicMissileBaseDamage * MagicMissileDamageMul;

    public float MagicMissileBaseCd = 1.0f;
    public float MagicMissileCdMul = 1.0f;
    public float MagicMissileEffectiveCd => MagicMissileBaseCd * MagicMissileCdMul;

    public float MagicMissileBaseRange = 6.0f * GameManager.Instance.ArenaScale;
    public float MagicMissileRangeMul = 1.0f;
    public float MagicMissileEffectiveRange => MagicMissileBaseRange * MagicMissileRangeMul;

    public float MagicMissileBaseSpeed = 8.0f;
    public float MagicMissileSpeedMul = 1.0f;

    public float MagicMissileJumpDamageMul = 0.0f;
    public int MagicMissileMultiShots = 0;

    public bool MeleeThrowBought = false;
    public bool MeleeThrowEnabledInRound = false;
    public float MeleeThrowBasePower = 8.0f;
    public float MeleeThrowDrag = 8.0f;
    public float MeleeThrowPowerMul = 1.0f;
    public float MeleeThrowBaseDamage = 80.0f;
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
    public float SawBladeMaxDistance = 35 * GameManager.Instance.ArenaScale;
    public float SawBladeBaseCd = 6.0f;
    public float SawBladeCdMul = 1.0f;

    public bool BurstOfFrostBought = false;
    public bool BurstOfFrostEnabledInRound = false;
    public float BurstOfFrostBaseCd = 1.4f;
    public float BurstOfFrostCdMul = 1.0f;
    public float BurstOfFrostBaseRange = 2.0f;
    public float BurstOfFrostRangeMul = 1.0f;
    public float BurstOfFrostBaseFreezeChance = 0.65f;
    public float BurstOfFrostFreezeChanceMul = 1.0f;
    public float BurstOfFrostBaseFreezeTime = 2.0f;
    public float BurstOfFrostFreezeTimeMul = 1.0f;

    public bool PaintballBought = false;
    public bool PaintballActiveInRound = false;
    public float PaintballRangeMul = 1.0f;
    public float PaintballBaseSpeed = 5.0f;
    public float PaintballBaseSlowMul = 0.9f; // slow amount at mass = 1
    public float PaintballBaseDuration = 5.0f;
    public float PaintballDurationMul = 1.0f;
    public float PaintballCd = 3.0f;
    public float PaintballCdMul = 1.0f;
    public float PaintballPerSec = 12;
    public float PaintballBaseDamagePerSec = 30.0f;
    public float PaintballDamagePerSecMul = 1.0f;

    // circling axe
    public bool CirclingAxeEnabled = false;
    public float CirclingAxeBaseCd = 8.0f;
    public float CirclingAxeCdMul = 1.0f;
    public float CirclingAxeBaseDamage = 55.0f;
    public float CirclingAxeDamageMul = 1.0f;
    public float CirclingAxeBaseSpeed = 6.0f * GameManager.Instance.ArenaScale;
    public float CirclingAxeSpeedMul = 1.0f;
    public float CirclingAxeBaseLifetime = 4.5f;
    public float CirclingAxeLifetimeMul = 1.0f;
}

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    //public static UpgradeData Data = new();
    public static UpgradeData Data;

    public static void ResetAll()
    {
        Data = new UpgradeData();
    }

    void Awake()
    {
        Instance = this;
        Data = new();
}
}
