using TMPro;
using UnityEngine;

public class FloatingTextScript : MonoBehaviour, IKillableObject
{
    Transform transform_;
    TextMeshPro text_;
    GameObjectPool textPool_;
    Vector3 position_;
    float dieTime_;
    float speed_;
    float _fadeTime;
    Color _baseColor;

    private void Awake()
    {
        text_ = GetComponent<TextMeshPro>();
        transform_ = transform;
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

        // movement
        position_.y += G.D.GameDeltaTime * speed_;
        transform_.position = position_;

        if (G.D.GameTime >= dieTime_)
            Die();
    }

    public void Kill()
    {
        Die();
    }
}