using Assets.Script;
using System.Collections;
using UnityEngine;

public class MeleeThrow : MonoBehaviour, IKillableObject
{
    public float forceTest = 1.5f;

    Transform trans_;
    Vector3 startPos_;
    float damage_;

    private void Awake()
    {
        trans_ = transform;
    }

    float degrees_;

    public float rotationspeed = 200.0f;

    IEnumerator Run()
    {
        float speed = 0;

        float now = GameManager.Instance.GameTime;
        float suckingEndtime = now + 5.0f;

        const int MaxSuck = 15;
        void SuckEnemies()
        {
            int count = BlackboardScript.GetEnemies(trans_.position, 2.0f);
            count = Mathf.Min(count, MaxSuck);
            for (int i = 0; i < count; ++i)
            {
                var enemy = BlackboardScript.EnemyOverlap[i];
                enemy.AddForce((trans_.position - enemy.transform.position).normalized * GameManager.Instance.GameDeltaTime * 3.0f);
            }
        }

        while (now < suckingEndtime)
        {
            now = GameManager.Instance.GameTime;

            float x = Mathf.Sin(now * 1.5f) * 2 + startPos_.x;
            float y = Mathf.Cos(now * 1.5f) * 2 + startPos_.y;
            float swayX = Mathf.Sin(now * 0.17f) * 0.25f;
            float swayY = Mathf.Cos(now * 0.27f) * 0.25f;
            trans_.position = new Vector2(x + swayX, y + swayY);

            trans_.rotation = Quaternion.Euler(0.0f, 0.0f, degrees_);
            degrees_ += rotationspeed * GameManager.Instance.GameDeltaTime;

            var dirPlayer = (GameManager.Instance.PlayerScript.transform.position - trans_.position).normalized;
            trans_.position += dirPlayer * speed * GameManager.Instance.GameDeltaTime;

            SuckEnemies();

            yield return null;
        }

        // charge
        var p = trans_.position;
        float startCharge = now;
        const float chargeTime = 2.0f;
        float endCharge = now + chargeTime;

        while (now < endCharge)
        {
            float t = (now - startCharge) / chargeTime;

            now = GameManager.Instance.GameTime;
            float x = Random.value * 0.2f;
            float y = Random.value * 0.2f;
            trans_.position = p + new Vector3(x, y) * t;

            trans_.rotation = Quaternion.Euler(0.0f, 0.0f, degrees_);
            degrees_ += rotationspeed * GameManager.Instance.GameDeltaTime;

            SuckEnemies();

            yield return null;
        }

        // explode
        int countExplode = BlackboardScript.GetEnemies(trans_.position, 0.75f);
        for (int i = 0; i < countExplode; ++i)
        {
            var enemy = BlackboardScript.EnemyOverlap[i];
            var dir = RndUtil.RandomInsideUnitCircle().normalized;
            enemy.AddForce(dir * 1.0f);
            enemy.ApplyDamage(damage_, dir, 1.5f);
        }

        //float dist = Vector3.Distance(GameManager.Instance.PlayerScript.transform.position, trans_.position);
        GameManager.Instance.MakePoof(trans_.position, 5, 1.0f);
        GameManager.Instance.MakeFlash(trans_.position, 5.0f);
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.LivingBombExplode, 0.75f);
        Kill();
    }

    public void Kill()
    {
        CacheManager.Instance.MeleeThrowCache.ReturnInstance(gameObject);
    }

    public void Throw(float damage, Vector3 scale)
    {
        trans_.localScale = scale;
        startPos_ = trans_.position;
        damage_ = damage;

        StartCoroutine(Run());
    }
}
