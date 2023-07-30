using UnityEngine;

public class MeleeThrow : MonoBehaviour, IKillableObject
{
    Transform trans_;
    Vector3 startPos_;
    Vector3 startDir_;
    float speed_;
    float startSign_;

    private void Awake()
    {
        trans_ = transform;
    }

    float drag_ = 9.0f;
    float degrees_;
    float damage_;

    public float rotationspeed = 700.0f;

    void Update()
    {
        trans_.rotation = Quaternion.Euler(0.0f, 0.0f, degrees_);
        degrees_ += rotationspeed * Time.deltaTime;

        speed_ -= drag_ * Time.deltaTime;

        trans_.position += speed_ * Time.deltaTime * startDir_;

        float dist = Vector3.Distance(startPos_, trans_.position);
        bool isBack = Mathf.Sign(speed_) != startSign_ && dist < 0.3f;
        if (isBack)
        {
            GameManager.Instance.MakePoof(trans_.position, 3, 0.25f);
            Kill();
        }
    }

    public void Kill()
    {
        CacheManager.Instance.MeleeThrowCache.ReturnInstance(gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int layer = col.gameObject.layer;
        if (layer != GameManager.Instance.LayerEnemy)
            return;

        var actor = col.gameObject.GetComponent<ActorBase>();
        float forceMul = Mathf.Sign(speed_) == startSign_ ? 0.4f : 0.01f;
        GameManager.Instance.DamageEnemy(actor, damage_, speed_ * startDir_, forceMul);
    }

    public void Throw(Vector2 dir, float damage, Vector3 scale)
    {
        trans_.localScale = scale;
        startPos_ = trans_.position;
        speed_ = PlayerUpgrades.Data.MeleeThrowBasePower * PlayerUpgrades.Data.MeleeThrowPowerMul;
        damage_ = damage;
        startSign_ = Mathf.Sign(speed_);
        startDir_ = dir.normalized;
    }
}
