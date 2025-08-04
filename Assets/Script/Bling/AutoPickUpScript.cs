using UnityEngine;

public enum AutoPickUpType { Money, Xp, }

public class AutoPickUpScript : MonoBehaviour, IKillOnSaveWipe
{
    public AutoPickUpType Type;
    public long Value = 1;
    float PickupDistance = 0.2f;
    float AttractPower = 22.0f;
    float ThrowForce = 5.0f;
    float Drag = 5.0f;
    
    float forceScale_ = 1.0f;
    float throwEndTime_;
    Vector2 _baseScale;
    SpriteRenderer spriteRenderer_;
    float sqrPickupDistance_;
    Vector3 force_;
    float throwStartTime_;
    float sqrAttractDistance_;
    bool _initComplete;
    bool _isLarge;

    private void Awake()
    {
        // Total hack because localScale became zero immediately, even if wasn't to begin with, wtf?
        sqrPickupDistance_ = PickupDistance * PickupDistance;
    }

    private void Init()
    {
        _baseScale = transform.localScale;
        spriteRenderer_ = GetComponent<SpriteRenderer>();
        _initComplete = true;
    }

    public void Throw(Vector3 direction, float forceScale, bool isLargeCoin = false)
    {
        if (!_initComplete)
            Init();

        _isLarge = isLargeCoin;
        forceScale *= GameManager.Instance.ArenaScale;
        forceScale_ = forceScale;

        direction.Normalize();
        sqrAttractDistance_ = PlayerUpgrades.Data.GoldXpAttractRange * PlayerUpgrades.Data.GoldXpAttractRange;
        
        float time = G.D.GameTime;
        float randomValue = forceScale * 0.1f;
        force_ = (Random.value * randomValue + ThrowForce) * forceScale_ * direction;

        throwEndTime_ = time + 1.0f;
        throwStartTime_ = GameManager.Instance.GameTime + 0.05f;

        transform.localScale = _baseScale * (isLargeCoin ? 1.2f : 0.6f);
    }

    public void Die()
    {
        transform.localScale = _baseScale;
        PickUpManagerScript.Instance.ReturnPickUpToCache(Type, gameObject);
    }

    public void Kill()
    {
        Die();
    }

    private void Update()
    {
        if (GameManager.Instance.GameTime < throwStartTime_)
            return;

        float dt = G.D.GameDeltaTime;
        float time = G.D.GameTime;
        var myPos = transform.position;

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

            transform.position = newPos;
            force_ *= 1.0f - dt * Drag;
        }

        if (diff.sqrMagnitude < sqrPickupDistance_ && time > throwEndTime_)
        {
            if (Type == AutoPickUpType.Money)
            {
                G.D.PlayerScript.OnGoldPickedUp(_isLarge, Value);
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