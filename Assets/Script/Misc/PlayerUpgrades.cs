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
    public float CritValueMul = 2.0f;

    // gold
    public float MoneyDoubleChance = 0.05f;
    public float DropMoneyOnKillChance = 0.01f;
    public int DropMoneyOnKillMin = 1;
    public int DropMoneyOnKillMax = 3;

    // xp
    public float XpDoubleChance = 0.05f;

    // player
    public int BaseHealth = 100;
    public float HealthMul = 1.0f;
    public float BaseHealthRegenSec = 0.5f;
    public float HealthRegenSecMul = 1.0f;
    public float OnDamageTimeImmune = 0.2f;
    public float HealthDefenseMul = 1.0f;
    public float MoveSpeedMul = 1.0f;

    // weapons
    public float MagicMissileBaseDamage = 15.0f;
    public float MagicMissileBaseBulletCd = 0.1f;
    public float MagicMissileBaseCd = 1.0f;
    public float MagicMissileCdMul = 1.0f;
    public float MagicMissileBaseRange = 6.0f;
    public float MagicMissileBaseSpeed = 8.0f;
    public float MagicMissileSpeedMul = 1.0f;
    public float MagicMissileRangeMul = 1.0f;
    public float MagicMissileBaseBullets = 2;
    public float MagicMissileBulletsMul = 1.0f;

    public bool MeleeThrowEnabled = false;
    public float MeleeThrowBasePower = 8.0f;
    public float MeleeThrowPowerMul = 1.0f;
    public float MeleeThrowBaseDamage = 20.0f;
    public float MeleeThrowBaseCd = 2.0f;
    public float MeleeThrowCdMul = 1.0f;

    // orc
    public float OrcReviveTime = 8.0f;
    public float OrcReviveTimeMul = 1.0f;

    public bool OrcJedisEnabled = false;
    public float OrcJediKnockBackForceMul = 1.0f;

    public bool OrcPickupSawbladeEnabled = false;
    public bool OrcPickupSmallSawbladeEnabled = false;
    public float OrcPickupSawbladeDurabilityMul = 1.0f;
    public bool OrcPickupSawbladePickNewTarget = false;

    // paintball
    public bool PaintballEnabled = false;
    public float PaintballCd = 5.0f;
    public float PaintballCdMul = 1.0f;
    public int PaintballCount = 10;

    public CounterClass Counters = new CounterClass();
}

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    public static UpgradeData Data = new UpgradeData();

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
        if (!Data.PaintballEnabled)
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
