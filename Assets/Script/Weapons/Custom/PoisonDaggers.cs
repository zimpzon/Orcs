using System.Collections;
using TMPro;
using UnityEngine;

public class PoisonDaggers : MonoBehaviour, IPlayerToggleEfffect
{
    public static PoisonDaggers Instance;

    public GameObject ChildRoot;
    public TextMeshPro Text;
    public Sprite Projectile;
    public Color Color;
    float nextFire_;
    Vector2 offset_;
    float offsetStrength_;

    private void Awake()
    {
        Instance = this;
        Disable();
    }

    public void Disable()
    {
        Text.text = "";
        StopAllCoroutines();
        ChildRoot.SetActive(false);
    }

    public void TryEnable()
    {
        if (PlayerUpgrades.Data.PaintballBought)
        {
            StartCoroutine(Think());
        }
    }

    void SetNextFire()
    {
        nextFire_ = G.D.GameTime + PlayerUpgrades.Data.PaintballCd * PlayerUpgrades.Data.PaintballCdMul;
        ChildRoot.SetActive(false);
    }

    IEnumerator Think()
    {
        while (true)
        {
            offset_ = Vector2.zero;
            offsetStrength_ = 0;

            while (!PlayerUpgrades.Data.PaintballActiveInRound || G.D.GameTime < nextFire_)
                yield return null;

            ChildRoot.SetActive(true);

            var paintball = WeaponBase.GetWeapon(WeaponType.PaintBallRandom);
            paintball.Sprite = Projectile;
            Vector3 from = PositionUtility.GetPointInsideArena(0.8f, 0.8f);
            Vector3 dir = (Vector3.zero - from).normalized;
            Vector3 to = from + dir * 8;

            float angleStep = 360 / PlayerUpgrades.Data.PaintballCount;
            float angle = 0;
            var pos = from;
            var moveDir = (to - from).normalized;
            transform.position = pos;
            float speed = 2.0f;
            float nextSpread = 0;

            float textEnd = G.D.GameTime + 3.0f;
            Text.text = "Chill Tornado!";
            ChildRoot.SetActive(true);

            while (G.D.GameTime < textEnd)
                yield return null;

            Text.text = "";

            while (true)
            {
                var fireDir = Quaternion.Euler(0, 0, angle) * Vector2.up;
                if (G.D.GameTime > nextSpread)
                {
                    paintball.FireFromPoint(pos + (Vector3)offset_ * offsetStrength_, fireDir, damage: 0.0f, scale: 0.5f, GameManager.Instance.SortLayerTopEffects, out _);
                    angle += angleStep * 0.5f;

                    nextSpread = G.D.GameTime + 0.08f;
                }

                offset_ += (Vector2)(fireDir * G.D.GameDeltaTime * 3.0f);

                pos += moveDir * speed * G.D.GameDeltaTime;
                transform.position = pos + (Vector3)offset_ * offsetStrength_;

                offsetStrength_ += G.D.GameDeltaTime;
                if (offsetStrength_ > 1)
                    offsetStrength_ = 1;

                float dist = Vector2.Distance(pos, to);

                if (dist < 0.1f)
                    break;

                yield return null;
            }

            SetNextFire();

            yield return null;
        }
    }
}
