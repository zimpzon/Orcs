using Assets.Script;
using UnityEngine;

public class PoisonDaggers : MonoBehaviour, IPlayerToggleEfffect
{
    public static PoisonDaggers Instance;

    public Color Color;
    ParticleSystem particles_;

    private void Awake()
    {
        Instance = this;
        particles_ = GetComponent<ParticleSystem>();
    }

    public void Disable()
    {
        enabled = false;
    }

    public void TryEnable()
    {
        if (PlayerUpgrades.Data.PaintballBought)
        {
            enabled = true;
            SetNextUpdate();
        }
    }

    void SetNextUpdate()
    {
        PlayerUpgrades.Data.Counters.PaintballTimer = 0;
    }

    void UpdatePaintball()
    {
        if (!PlayerUpgrades.Data.PaintballActiveInRound || !enabled)
            return;

        PlayerUpgrades.Data.Counters.PaintballTimer += GameManager.Instance.GameDeltaTime;

        if (PlayerUpgrades.Data.Counters.PaintballTimer > PlayerUpgrades.Data.PaintballCd * PlayerUpgrades.Data.PaintballCdMul)
        {
            var paintball = WeaponBase.GetWeapon(WeaponType.PaintBallRandom);

            Vector3 location = PositionUtility.GetPointInsideArena(0.9f, 0.9f);
            float angleStep = 360 / PlayerUpgrades.Data.PaintballCount;
            float angle = 0;
            for (int i = 0; i < PlayerUpgrades.Data.PaintballCount; ++i)
            {
                var dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
                paintball.FireFromPoint(location, dir, damage: 0.0f, scale: 1.0f, GameManager.Instance.SortLayerTopEffects, out _);
                angle += angleStep;
            }

            transform.position = location;
            particles_.Emit(1);

            float initialSlowRadius = PlayerUpgrades.Data.PaintballBaseRange * 0.25f;
            float slowTime = PlayerUpgrades.Data.PaintballBaseDuration * PlayerUpgrades.Data.PaintballDurationMul;

            int aliveCount = BlackboardScript.GetEnemies(location, initialSlowRadius);
            for (int i = 0; i < aliveCount; ++i)
            {
                ActorBase enemy = BlackboardScript.EnemyOverlap[i];
                enemy.OnPaintballHit(Color, slowTime);
            }

            GameManager.Instance.MakeFlash(transform.position, 3);
            GameManager.Instance.MakePoof(transform.position, 4, 1.0f);

            SetNextUpdate();
        }
    }

    void Update()
    {
        UpdatePaintball();
    }
}
