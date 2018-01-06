using Assets.Script;
using System.Collections;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    public static float Cd = 1.5f;
    public static float FuseTime = 1.0f;
    public static float Radius = 3.5f;
    public static float Damage = 280.0f;

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    void Start()
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

    public void Throw(Vector3 from, Vector3 to)
    {
        StopAllCoroutines();
        StartCoroutine(ThrowCo(from, to));
    }

    IEnumerator ThrowCo(Vector3 from, Vector3 to)
    {
        shadowRenderer_.enabled = false;
        var fuseEmission = fuseParticles_.emission;
        fuseEmission.enabled = true;

        float length = (to - from).magnitude;
        float force = Mathf.Min(5.0f, length);
        float velocityY = 1.0f * force;
        float downForce = -25.0f;
        float speed = 5.0f * force;

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
                shadowRenderer_.enabled = true;
                velocityY = -velocityY * 0.5f;
            }

            pos += dir * speed * Time.deltaTime;
            speed *= 1.0f - (5.0f * delta);

            yield return null;
        }

        fuseEmission.enabled = false;

        audioSource_.clip = AudioManager.Instance.AudioData.BombExplode;
        audioSource_.volume = 1.0f * AudioManager.Instance.MasterVolume; ;
        audioSource_.Play();
        GameManager.Instance.MakeFlash(pos, Radius * 1.5f);
        GameManager.Instance.MakePoof(pos, 6, Radius * 1.5f);
        GameManager.Instance.ShakeCamera(4.0f);

        int deadCount = BlackboardScript.GetDeadEnemies(pos, Radius);
        for (int i = 0; i < deadCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.DeadEnemies[idx];
            enemy.AddForce((enemy.transform.position - pos) * 0.25f);
//            enemy.Explode(2.0f + Random.value * 2);
        }

        int aliveCount = BlackboardScript.GetEnemies(pos, Radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            enemy.ApplyDamage(Damage, enemy.transform.position - pos, 1.0f, true);
        }

        for (int i = 0; i < 10; ++i)
        {
            Vector2 rnd = Random.insideUnitCircle * Radius * 0.5f;
            Vector3 flamePos = pos;
            flamePos.x += rnd.x;
            flamePos.y += rnd.y;
            GameManager.Instance.EmitFlame(flamePos, Random.value + 0.5f);
            yield return null;
        }

        // Kill player if very close, else just push
        float playerDist = BlackboardScript.DistanceToPlayer(pos);
        if (playerDist < 3.0f)
        {
            GameManager.Instance.PlayerScript.AddForce((GameManager.Instance.PlayerTrans.position - pos) * 0.1f);
        }

        Hide();
    }
}
