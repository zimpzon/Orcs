using System.Collections;
using TMPro;
using UnityEngine;

public class PoisonDaggers : MonoBehaviour, IPlayerToggleEfffect
{
    public static PoisonDaggers Instance;

    public GameObject ChildRoot;
    public ParticleSystem Projectiles;
    public Color Color;
    float nextFire_;

    private void Awake()
    {
        Instance = this;
        Disable();
    }

    public void Disable()
    {
        StopAllCoroutines();

        var em = Projectiles.emission;
        em.enabled = false;
        ChildRoot.SetActive(false);
    }

    public void TryEnable()
    {
        if (PlayerUpgrades.Data.PaintballBought)
        {
            StartCoroutine(Think());
        }
    }

    IEnumerator Think()
    {
        while (true)
        {
            while (!PlayerUpgrades.Data.PaintballActiveInRound || G.D.GameTime < nextFire_)
                yield return null;

            ChildRoot.SetActive(true);

            Vector3 from = Random.insideUnitCircle.normalized * 3;
            Vector3 dir = (Vector3.zero - from).normalized;
            Vector3 to = from + dir * 8;

            var pos = from;
            var moveDir = (to - from).normalized;
            transform.position = pos;
            float speed = 10.0f;

            float textEnd = G.D.GameTime + 3.0f;
            ChildRoot.SetActive(true);

            while (G.D.GameTime < textEnd)
                yield return null;

            Projectiles.Clear();
            var em = Projectiles.emission;
            em.enabled = true;
            em.rateOverTime = PlayerUpgrades.Data.PaintballPerSec;

            while (true)
            {
                pos += moveDir * speed * G.D.GameDeltaTime;
                transform.position = pos;

                if (GameManager.Instance.IsOutsideBounds(transform.position))
                    break;
                
                yield return null;
            }
            
            yield return null;
        }
    }
}
