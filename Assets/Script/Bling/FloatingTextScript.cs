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

    private void Awake()
    {
        text_ = GetComponent<TextMeshPro>();
        transform_ = transform;
    }

    public void Init(GameObjectPool textPool, Vector3 position, string text, Color color, float speed = 1.0f, float timeToLive = 2.0f, FontStyles fontStyle = FontStyles.Bold)
    {
        textPool_ = textPool;

        text_.SetText(text);
        text_.color = color;
        text_.fontStyle = fontStyle;

        transform_.position = position;
        position_ = position;
        speed_ = speed;
        dieTime_ = G.D.GameTime + timeToLive;
    }

    public void Die()
    {
        textPool_.ReturnToPool(this.gameObject);
    }

    void Update()
    {
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