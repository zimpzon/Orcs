using UnityEngine;

public class UpgradeData
{
    public float CooldownReductionMul = 1.0f;
    public float DamageAddPct = 0.0f;
    public float MoveSpeedAddPct = 0.0f;

    // orc pickups
    public bool OrcPickupForceWaveEnabled = true;
    public float OrcPickupForceWaveAmount = 1.0f;
    public float OrcPickupForceWaveRadius = 5.0f;

    public bool OrcPickupSawbladeEnabled = true;
    public int OrcPickupSawbladeCount = 1;

    // paintball
    public bool PaintballEnabled = true;
    public float PaintballCd = 4;
    public int PaintballCount = 10;
    public float PaintballTimer;

    // on kill
    public bool OnKillDropBombEnabled = true;
    public int OnKillDropBombKillCount = 4;
    public int OnKillDropBombCurrentKillCount = 0;
    public int OnKillDropBombChance = 4;
}

public class PlayerUpgrades : MonoBehaviour
{
    public static PlayerUpgrades Instance;

    public static UpgradeData Data = new UpgradeData();

    public static void ResetData()
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

        Data.PaintballTimer += Time.deltaTime;
        if (Data.PaintballTimer > Data.PaintballCd)
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
            Data.PaintballTimer = 0;
        }
    }

    public void UpdateUpgrades()
    {
        UpdatePaintball();
    }
}
