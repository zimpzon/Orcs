using UnityEngine;

public enum AutoPickUpType { Money, Xp, }

public class AutoPickUpScript : MonoBehaviour
{
    public AutoPickUpType Type;
    public int Value = 1;
    public float AttractDistance = 5.0f;
    public float PickupDistance = 0.3f;
    public float AttractPower = 20.0f;
    public float ThrowForce = 5.0f;
    public float Drag = 5.0f;
    
    float forceScale_ = 1.0f;
    float throwEndTime_;
    Transform transform_;
    SpriteRenderer spriteRenderer_;
    float sqrPickupDistance_;
    Vector3 force_;
    float throwStartTime_;
    float sqrAttractDistance_;

    private void Awake()
    {
        transform_ = transform;
        spriteRenderer_ = GetComponent<SpriteRenderer>();
        sqrPickupDistance_ = PickupDistance * PickupDistance;
    }

    public void Throw(Vector3 direction, float forceScale)
    {
        forceScale_ = forceScale;
        direction.Normalize();
        sqrAttractDistance_ = AttractDistance * AttractDistance;
        
        float time = G.D.GameTime;
        float randomValue = forceScale * 0.1f;
        force_ = direction * (Random.value * randomValue + ThrowForce) * forceScale_;

        throwEndTime_ = time + 0.5f;
        throwStartTime_ = GameManager.Instance.GameTime + 0.05f;
    }

    public void Die()
    {
        PickUpManagerScript.Instance.ReturnPickUpToCache(Type, gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.GameTime < throwStartTime_)
            return;

        float dt = G.D.GameDeltaTime;
        float time = G.D.GameTime;
        var myPos = transform_.position;

        var playerPos = G.D.PlayerPos + Vector3.up * 0.3f;
        var diff = playerPos - myPos;

        if (diff.sqrMagnitude < sqrAttractDistance_ && time > throwEndTime_)
        {
            var direction = diff.normalized;
            force_ = direction * AttractPower;
        }

        float forceMagnitude = force_.magnitude;
        if (forceMagnitude > 0.1f)
        {
            var newPos = myPos + force_ * dt;
            newPos = GameManager.Instance.ClampToBounds(newPos, spriteRenderer_.sprite);

            transform_.position = newPos;
            force_ *= 1.0f - dt * Drag;
        }

        if (diff.sqrMagnitude < sqrPickupDistance_ && time > throwEndTime_)
        {
            if (Type == AutoPickUpType.Money)
            {
                SaveGame.Members.Money += Value;
                SaveGame.RoundGold += Value;
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.MoneyPickup);
            }
            else if (Type == AutoPickUpType.Xp)
            {
                float t = 1 - ((GameManager.Instance.xpToLevel - GameManager.Instance.currentXp) / GameManager.Instance.xpToLevel);
                const float pitchMin = 1.0f;
                const float pitchMax = 1.0f;
                float pitch = (pitchMax - pitchMin) * t + pitchMin;
                GameManager.Instance.AddXp(Value);
                float volume = 0.8f + t * 0.0f;
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.MoneyPickup, volume, pitch);
            }

            Die();
        }
    }
}