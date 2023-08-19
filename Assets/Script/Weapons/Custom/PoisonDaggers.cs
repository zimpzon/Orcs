using System.Collections;
using TMPro;
using UnityEngine;

public class PoisonDaggers : MonoBehaviour, IPlayerToggleEfffect
{
    public static PoisonDaggers Instance;

    public GameObject ChildRoot;
    public TextMeshPro Text;
    public SpriteRenderer Sprite;
    public Color Color;
    float nextFire_;

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
            ChildRoot.SetActive(true);
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
            if (!PlayerUpgrades.Data.PaintballActiveInRound || G.D.GameTime < nextFire_)
                yield return null;

            var paintball = WeaponBase.GetWeapon(WeaponType.PaintBallRandom);

            Vector3 from = PositionUtility.GetPointInsideArena(0.8f, 0.8f);
            Vector3 to = from + (Vector3)Random.insideUnitCircle.normalized * 6;

            float angleStep = 360 / PlayerUpgrades.Data.PaintballCount;
            float angle = 0;
            var pos = from;
            var moveDir = (to - from).normalized;
            bool flip = moveDir.x > 0;
            Sprite.flipX = flip;
            transform.position = pos;
            float speed = 2.0f;
            float nextSpread = 0;

            float textEnd = G.D.GameTime + 3.0f;
            Text.text = "Venom Vortex!";
            ChildRoot.SetActive(true);

            while (G.D.GameTime < textEnd)
                yield return null;

            Text.text = "";

            while (true)
            {
                if (G.D.GameTime > nextSpread)
                {
                    var dir = Quaternion.Euler(0, 0, angle) * Vector2.up;
                    paintball.FireFromPoint(pos, dir, damage: 0.0f, scale: 1.0f, GameManager.Instance.SortLayerTopEffects, out _);
                    angle += angleStep * 0.5f;
                    nextSpread = G.D.GameTime + 0.1f;
                }

                pos += moveDir * speed * G.D.GameDeltaTime;
                transform.position = pos;

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
