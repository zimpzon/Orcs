using UnityEngine;

public enum AutoPickUpType { Money, Xp, }

public class AutoPickUpScript : MonoBehaviour
{
    public AutoPickUpType Type;
    public int Value = 1;
    public float AttractDistanceMul = 1.0f;
    public float PickupDistance = 0.8f;
    public float AttractPower = 20.0f;
    public AudioClip PickUpSound;
    public GameObjectPool ObjectPool;

    float throwEndTime_;
    Transform transform_;
    float sqrPickupDistance_;
    Vector3 force_;

    private void Awake()
    {
        transform_ = transform;
        sqrPickupDistance_ = PickupDistance * PickupDistance;
    }

    public void Throw(Vector3 force)
    {
        float time = Time.time;
        force_ = force;
        throwEndTime_ = time + 0.5f;
    }

    void Die()
    {
        if (ObjectPool != null)
        {
            ObjectPool.ReturnToPool(gameObject);
            ObjectPool = null;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float time = Time.time;
        var myPos = transform_.position;

        var playerPos = GameManager.Instance.PlayerTrans.position + Vector3.up * 0.5f;
        var diff = playerPos - myPos;
        float attractDistance = 5.0f;
        float sqrDistance = attractDistance * attractDistance;
        if (diff.sqrMagnitude < sqrDistance && time > throwEndTime_)
        {
            var direction = diff.normalized;
            force_ = direction * AttractPower;
        }

        float forceMagnitude = force_.magnitude;
        if (forceMagnitude > 0.1f)
        {
            var newPos = myPos + force_ * dt;
            transform_.position = newPos;

            forceMagnitude -= dt * 2;
            force_ = Vector3.ClampMagnitude(force_, forceMagnitude);
        }

        if (diff.sqrMagnitude < sqrPickupDistance_ && time > throwEndTime_)
        {
            AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.MoneyPickup);
            Die();
        }
    }
}