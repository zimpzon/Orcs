using TMPro;
using UnityEngine;

public class FloatingTextScript : MonoBehaviour, IKillableObject
{
    Transform transform_;
    TextMeshPro text_;
    GameObjectPool textPool_;
    Vector3 position_;
    Vector3 baseScale_;
    float dieTime_;
    float speed_;
    float _fadeTime;
    Color _baseColor;

    // Bumped on every Init so FloatingTextSpawner can tell a pooled, reused text from the one it handed out.
    public int Generation { get; private set; }

    // Damage number mode: pop-in, arcing flight, merging of follow-up hits (see FloatingTextSpawner.SpawnDamage).
    bool isDamage_;
    Vector2 velocity_;
    float popStartTime_;
    float popStrength_;
    float sizeMul_;
    long damageTotal_;
    bool damageIsCrit_;
    float lastMergeTime_;
    Color critColor_;

    const float DamageGravity = -7.0f;
    const float DamageLifeTime = 0.9f;
    const float DamageFadeTime = 0.35f;
    const float PopDuration = 0.18f;
    const float MaxMergeAge = 0.17f;
    float damageSpawnTime_;

    private void Awake()
    {
        text_ = GetComponent<TextMeshPro>();
        transform_ = transform;
        baseScale_ = transform_.localScale;
    }

    public void Init(
        GameObjectPool textPool,
        Vector3 position,
        string text,
        Color color,
        float speed = 1.0f,
        float timeToLive = 2.0f,
        float fadeTime = 0.0f,
        FontStyles fontStyle = FontStyles.Bold,
        TMP_FontAsset fontAsset = null)
    {
        Generation++;
        isDamage_ = false;
        transform_.localScale = baseScale_;

        _baseColor = color;
        textPool_ = textPool;

        text_.SetText(text);
        text_.color = color;
        text_.fontStyle = fontStyle;
        text_.font = fontAsset is null ? GameManager.Instance.FontDefaultFloatingText : fontAsset;
        transform_.position = position;
        position_ = position;
        speed_ = speed;
        _fadeTime = fadeTime;
        dieTime_ = G.D.GameTime + timeToLive;
    }

    public void InitDamage(GameObjectPool textPool, Vector3 position, long amount, bool isCrit, Color color, Color critColor, float sizeMul)
    {
        Init(textPool, position, string.Empty, color, timeToLive: DamageLifeTime, fadeTime: DamageFadeTime);
        isDamage_ = true;
        critColor_ = critColor;
        damageTotal_ = 0;
        damageIsCrit_ = false;
        sizeMul_ = 0;
        damageSpawnTime_ = G.D.GameTime;

        // Small random sideways kick and a strong upward start; gravity turns it into a short arc.
        velocity_ = new Vector2(Random.Range(-1.2f, 1.2f), Random.Range(3.0f, 4.0f));
        AddDamage(amount, isCrit, sizeMul);
    }

    // True if this text is still young enough to absorb another hit instead of spawning a new number.
    // Capped by total age too: an enemy hit non-stop would otherwise keep one number alive forever while
    // gravity carries it off-screen, and no new numbers would ever appear.
    public bool CanMerge(float mergeWindow)
        => isDamage_ && gameObject.activeSelf
           && G.D.GameTime - lastMergeTime_ < mergeWindow
           && G.D.GameTime - damageSpawnTime_ < MaxMergeAge;

    public void AddDamage(long amount, bool isCrit, float sizeMul)
    {
        damageTotal_ += amount;
        damageIsCrit_ |= isCrit;
        sizeMul_ = Mathf.Max(sizeMul_, sizeMul);
        lastMergeTime_ = G.D.GameTime;

        // Every merge re-pops the text and keeps it alive a little longer.
        popStartTime_ = G.D.GameTime;
        popStrength_ = isCrit ? 0.7f : 0.4f;
        dieTime_ = Mathf.Max(dieTime_, G.D.GameTime + DamageLifeTime * 0.6f);

        string number = $"-{Assets.Script.Misc.Format64.Format(damageTotal_)}";
        if (damageIsCrit_)
        {
            _baseColor = critColor_;
            text_.SetText($"<size=55%>CRIT!</size>\n{number}");
        }
        else
        {
            text_.SetText(number);
        }
    }

    public void Die()
    {
        textPool_.ReturnToPool(this.gameObject);
    }

    void Update()
    {
        float timeLeft = dieTime_ - G.D.GameTime;

        float alpha = 1f;
        if (_fadeTime > 0.0f && timeLeft <= _fadeTime)
        {
            alpha = Mathf.Clamp01(timeLeft / _fadeTime);
        }

        text_.color = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);

        if (isDamage_)
            UpdateDamageMotion(alpha);
        else
            position_.y += G.D.GameDeltaTime * speed_;

        transform_.position = position_;

        if (G.D.GameTime >= dieTime_)
            Die();
    }

    void UpdateDamageMotion(float alpha)
    {
        float dt = G.D.GameDeltaTime;
        velocity_.y += DamageGravity * dt;
        velocity_.x *= 1.0f - 3.0f * dt;
        position_ += (Vector3)(velocity_ * dt);

        // Pop: overshoot then settle (decaying bounce), stronger for crits.
        float popT = Mathf.Clamp01((G.D.GameTime - popStartTime_) / PopDuration);
        float pop = 1.0f + popStrength_ * Mathf.Sin(popT * Mathf.PI) * (1.0f - popT * 0.5f);

        // Crits are bigger, and everything shrinks a bit while fading out.
        float crit = damageIsCrit_ ? 1.3f : 1.0f;
        float shrink = Mathf.Lerp(0.6f, 1.0f, alpha);
        transform_.localScale = baseScale_ * (sizeMul_ * crit * pop * shrink);
    }

    public void Kill()
    {
        Die();
    }
}
