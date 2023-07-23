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
    public float SawbladeDurabilityMul = 1.0f;
    public bool SawbladePickNewTarget = false;

    // gold
    public float MoneyDoubleChance = 0.0f;

    // weapons
    public float WeaponsCdMul = 1.0f;
    public float WeaponsDamageMul = 1.0f;
    public float WeaponsRangeMul = 1.0f;
    public int MachinegunBulletsAdd = 0;

    // orc
    public bool OrcJedisEnabled = false;
    public float OrcJediKnockBackForceMul = 1.0f;

    public bool OrcPickupSawbladeEnabled = false;
    public bool OrcPickupSmallSawbladeEnabled = false;

    // paintball
    public bool PaintballEnabled = false;
    public float PaintballCd = 5.0f;
    public float PaintballCdMul = 1.0f;
    public int PaintballCount = 10;

    // on kill
    public bool OnKillDropBombEnabled = false;
    public int OnKillDropBombKillCount = 20;

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
