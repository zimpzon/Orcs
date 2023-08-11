using UnityEngine;

public class CirclingAxe : MonoBehaviour, IKillableObject
{
    Transform trans_;

    private void Awake()
    {
        trans_ = transform;
    }

    float angleLocal = 1500f;
    Vector3 lastPos_;
    Vector3 dir_;
    float speed_;
    float damage_;
    float dieTime_;

    public void Throw(Vector3 dir, float damage, float speed)
    {
        damage_ = damage;
        speed_ = speed;
        dir_ = dir.normalized;
        dieTime_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.CirclingAxeBaseLifetime * PlayerUpgrades.Data.CirclingAxeLifetimeMul;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        int layer = col.gameObject.layer;
        if (layer != GameManager.Instance.LayerEnemy)
            return;

        var actor = col.gameObject.GetComponent<ActorBase>();
        float forceMul = 0.0f;
        actor.OnPaintballHit(Color.green, 2.0f);
        GameManager.Instance.DamageEnemy(actor, damage_, (trans_.position - lastPos_).normalized , forceMul);
    }

    void Update()
    {
        if (GameManager.Instance.GameTime > dieTime_)
        {
            Kill();
            return;
        }

        lastPos_ = trans_.position;
        var newPos = lastPos_ + dir_ * speed_ * GameManager.Instance.GameDeltaTime;
        if (newPos.x <= GameManager.ArenaBounds.xMin || newPos.x >= GameManager.ArenaBounds.xMax)
            dir_.x *= -1;

        if (newPos.y <= GameManager.ArenaBounds.yMin || newPos.y >= GameManager.ArenaBounds.yMax)
            dir_.y *= -1;

        trans_.position = newPos;
        trans_.Rotate(0, 0, -angleLocal * GameManager.Instance.GameDeltaTime, Space.Self);
    }

    public void Kill()
    {
        CacheManager.Instance.CirclingAxeCache.ReturnInstance(gameObject);
    }
}
