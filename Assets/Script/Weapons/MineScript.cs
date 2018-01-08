using Assets.Script;
using System.Collections;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    public static float Radius = 3.5f;
    public static float Damage = 100.0f;

    public Sprite Unarmed;
    public Sprite Armed;

    SpriteRenderer mineRenderer_;
    public SpriteRenderer ShadowRenderer;
    Transform trans_;
    AudioSource audioSource_;
    bool isArmed_;
    int triggerCount_ = -1;
    bool isExploding_;
    static Collider2D[] MineOverlap = new Collider2D[10];

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    public void Trigger()
    {
        triggerCount_ = 5;
    }

    private void Awake()
    {
        trans_ = this.transform;
        mineRenderer_ = GetComponent<SpriteRenderer>();
        audioSource_ = GetComponent<AudioSource>();

        mineRenderer_.sprite = Unarmed;
        isArmed_ = false;
    }

    private void Update()
    {
        if (triggerCount_ > 0)
            triggerCount_--;

        if (triggerCount_ == 0 && !isExploding_)
            StartCoroutine(ExplodeCo());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isArmed_ || isExploding_)
            return;

        StartCoroutine(ExplodeCo());
    }

    public void Throw(Vector3 from, Vector3 to)
    {
        StartCoroutine(ThrowCo(from, to));
    }

    // TODO PE: NOT EXPLOSION! SOMETHING COOL!
    IEnumerator ExplodeCo()
    {
        isExploding_ = true;

        while (true)
        {
            var pos = trans_.position;

            GameManager.Instance.MakeFlash(pos, new Color(0.4f, 0.4f, 0.2f), Radius * 1.5f);

            float force = 0.1f;

            int aliveCount = BlackboardScript.GetEnemies(pos, Radius);
            for (int i = 0; i < aliveCount; ++i)
            {
                int idx = BlackboardScript.Matches[i].Idx;
                ActorBase enemy = BlackboardScript.Enemies[idx];
                enemy.SetSlowmotion();
//                enemy.AddForce((enemy.transform.position - pos) * force);
            }

            // Kill player if very close, else just push
            float playerDist = BlackboardScript.DistanceToPlayer(pos);
            if (playerDist < Radius * 0.2f)
            {
                GameManager.Instance.PlayerScript.KillPlayer();
            }
            else if (playerDist < 1.0f)
            {
                GameManager.Instance.PlayerScript.AddForce((GameManager.Instance.PlayerTrans.position - pos) * 0.25f);
            }

            yield return new WaitForSeconds(0.4f);
        }
    }

    //IEnumerator ExplodeCo()
    //{
    //    isExploding_ = true;

    //    var pos = trans_.position;
    //    audioSource_.clip = AudioManager.Instance.AudioData.BombExplode;
    //    audioSource_.volume = 0.25f;
    //    audioSource_.Play();
    //    GameManager.Instance.MakeFlash(pos, Radius * 1.5f);
    //    GameManager.Instance.MakePoof(pos, 3, Radius * 1.5f);
    //    GameManager.Instance.ShakeCamera(2.0f);

    //    int deadCount = BlackboardScript.GetDeadEnemies(pos, Radius);
    //    for (int i = 0; i < deadCount; ++i)
    //    {
    //        int idx = BlackboardScript.Matches[i].Idx;
    //        ActorBase enemy = BlackboardScript.DeadEnemies[idx];
    //        enemy.AddForce((enemy.transform.position - pos) * 0.25f);
    //    }

    //    int aliveCount = BlackboardScript.GetEnemies(pos, Radius);
    //    for (int i = 0; i < aliveCount; ++i)
    //    {
    //        int idx = BlackboardScript.Matches[i].Idx;
    //        ActorBase enemy = BlackboardScript.Enemies[idx];
    //        enemy.ApplyDamage(Damage, enemy.transform.position - pos, 1.0f, true);
    //    }

    //    for (int i = 0; i < 3; ++i)
    //    {
    //        Vector2 rnd = Random.insideUnitCircle * Radius * 0.5f;
    //        Vector3 flamePos = pos;
    //        flamePos.x += rnd.x;
    //        flamePos.y += rnd.y;
    //        GameManager.Instance.EmitFlame(flamePos, Random.value + 0.5f);
    //        yield return null;
    //    }

    //    // Kill player if very close, else just push
    //    float playerDist = BlackboardScript.DistanceToPlayer(pos);
    //    if (playerDist < Radius * 0.2f)
    //    {
    //        GameManager.Instance.PlayerScript.KillPlayer();
    //    }
    //    else if (playerDist < 1.0f)
    //    {
    //        GameManager.Instance.PlayerScript.AddForce((GameManager.Instance.PlayerTrans.position - pos) * 0.25f);
    //    }

    //    int minesCloseBy = Physics2D.OverlapCircleNonAlloc(pos, Radius * 1.2f, MineOverlap, 1 << GameManager.Instance.LayerPlayerProjectile);
    //    for (int i = 0; i < minesCloseBy; ++i)
    //    {
    //        var mine = MineOverlap[i].GetComponent<MineScript>();
    //        if (mine != null)
    //            mine.Trigger();
    //    }

    //    mineRenderer_.enabled = false;
    //    ShadowRenderer.enabled = false;

    //    Destroy(this.gameObject, 0.7f);
    //}

    IEnumerator ThrowCo(Vector3 from, Vector3 to)
    {
        ShadowRenderer.enabled = false;

        float length = (to - from).magnitude;
        float force = Mathf.Min(10.0f, length);
        float velocityY = 1.0f * force * 0.5f;
        float downForce = -25.0f;
        float speed = 5.0f * force;

        Vector3 dir = (to - from).normalized;
        Vector3 pos = from;
        float offsetY = 0;

        while (true)
        {
            float delta = Time.deltaTime;

            Vector3 showPos = pos;
            showPos.y += offsetY;
            trans_.position = showPos;

            offsetY += velocityY * delta;
            velocityY += downForce * delta;
            if (offsetY <= 0)
            {
                ShadowRenderer.enabled = true;
                velocityY = -velocityY * 0.5f;
            }

            pos += dir * speed * Time.deltaTime;
            speed *= 1.0f - (5.0f * delta);
            if (speed < 0.05f)
                break;

            yield return null;
        }

        isArmed_ = true;
        mineRenderer_.sprite = Armed;
    }
}
