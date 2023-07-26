using Assets.Script;
using System.Collections;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public static float Cd = 1.5f;
    public static float FuseTime = 3.0f;
    public const float DefaultRadius = 3.5f;
    public const float DefaultDamage = 280.0f;

    SpriteRenderer bombRenderer_;
    SpriteRenderer shadowRenderer_;
    Transform trans_;
    ParticleSystem fuseParticles_;
    AudioSource audioSource_;
    Material bombMaterial_;

    int flashParamId_;

    public void Hide()
    {
        StopAllCoroutines();
        trans_.position = Vector3.right * 10000;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, Radius);
    //}

    void Awake()
    {
        trans_ = this.transform;
        bombRenderer_ = GetComponent<SpriteRenderer>();
        bombMaterial_ = bombRenderer_.material;
        fuseParticles_ = trans_.Find("FuseParticles").GetComponent<ParticleSystem>();
        shadowRenderer_ = trans_.Find("BlobShadow").GetComponent<SpriteRenderer>();
        audioSource_ = GetComponent<AudioSource>();
        flashParamId_ = Shader.PropertyToID("_FlashAmount");
        Hide();
    }

    public void Throw(Vector3 from, Vector3 to, float radius = DefaultRadius, float damage = DefaultDamage)
    {
        StopAllCoroutines();
        StartCoroutine(ThrowCo(from, to, radius, damage));
    }

    IEnumerator ThrowCo(Vector3 from, Vector3 to, float radius, float damage)
    {
        shadowRenderer_.enabled = false;
        var fuseEmission = fuseParticles_.emission;
        fuseEmission.enabled = true;

        float length = (to - from).magnitude;
        float force = Mathf.Min(50.0f, length);
        float velocityY = 0.5f * force;
        float downForce = -15.0f;
        float speed = 4.0f * force;

        float fuseT0 = Time.time;
        float fuseT1 = fuseT0 + FuseTime;
        Vector3 dir = (to - from).normalized;
        Vector3 pos = from;
        float offsetY = 0;

        audioSource_.clip = AudioManager.Instance.AudioData.BombFuseBurn;
        audioSource_.volume = 0.5f * AudioManager.Instance.MasterVolume;
        audioSource_.Play();

        while (Time.time < fuseT1)
        {
            float delta = Time.deltaTime;

            Vector3 showPos = pos;
            showPos.y += offsetY;
            trans_.position = showPos;
            bombRenderer_.sortingOrder = (Mathf.RoundToInt(trans_.position.y * 100f));
            bombMaterial_.SetFloat(flashParamId_, (Mathf.Sin((Time.time * 15) + 1.0f) * 0.5f) * 0.75f);

            offsetY += velocityY * delta;
            velocityY += downForce * delta;

            if (offsetY <= 0)
            {
                if (velocityY <= 0.1f)
                    shadowRenderer_.enabled = true;

                velocityY = -velocityY * 0.5f;
            }

            pos += dir * speed * Time.deltaTime;
            speed *= 1.0f - (5.0f * delta);

            yield return null;
        }

        fuseEmission.enabled = false;

        audioSource_.clip = AudioManager.Instance.AudioData.BombExplode;
        audioSource_.volume = 0.25f * AudioManager.Instance.MasterVolume;
        audioSource_.Play();
        GameManager.Instance.MakeFlash(pos, radius * 2.0f);
        GameManager.Instance.MakePoof(pos, 2, radius * 2.05f);
        GameManager.Instance.ShakeCamera(1.0f);

        int deadCount = BlackboardScript.GetDeadEnemies(pos, radius);
        for (int i = 0; i < deadCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.DeadEnemies[idx];
            enemy.AddForce((enemy.transform.position - pos) * 0.25f);
        }

        int aliveCount = BlackboardScript.GetEnemies(pos, radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            enemy.ApplyDamage(damage, enemy.transform.position - pos, 1.0f);
        }

        for (int i = 0; i < 3; ++i)
        {
            Vector2 rnd = RndUtil.RandomInsideUnitCircle() * radius * 0.2f;
            Vector3 flamePos = pos;
            flamePos.x += rnd.x;
            flamePos.y += rnd.y;
            GameManager.Instance.EmitFlame(flamePos, Random.value + 0.5f);
            yield return null;
        }

        Hide();
    }
}
