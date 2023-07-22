using UnityEngine;

public class UpgradeData
{
    public class CounterClass
    {
        public float PaintballTimer;
        public int OnKillDropBombCurrentKillCount;
        public int OrcJediCounter;
    }

    // global
    public float DamageMul = 1.0f;
    public float SawbladeDurabilityMul = 1.0f;

    public float PrimaryCdMul = 1.0f;
    public float PrimaryCdBetweenBulletsMul = 1.0f;
    public float PrimaryDamageMul = 1.0f;
    public int PrimaryBulletsAdd = 0;
    public float PrimaryRangeMul = 1.0f;

    // orc
    public bool OrcJedisEnabled = false;
    public float OrcJediKnockBackForceMul = 1.0f;

    public bool OrcPickupSawbladeEnabled = false;
    public bool OrcPickupSmallSawbladeEnabled = false;

    // paintball
    public bool PaintballEnabled = true;
    public float PaintballCd = 4;
    public int PaintballCount = 50;

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

    // Start is called before the first frame update
    void Start()
    {
    }

    void UpdatePaintball()
    {
        if (!Data.PaintballEnabled)
            return;

        Data.Counters.PaintballTimer += Time.deltaTime;
        if (Data.Counters.PaintballTimer > Data.PaintballCd)
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
