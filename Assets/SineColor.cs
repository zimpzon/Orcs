using UnityEngine;

public class SineColor : MonoBehaviour
{
    Color baseColor_;
    SpriteRenderer spriteRenderer_;

    void Awake()
    {
        spriteRenderer_ = GetComponent<SpriteRenderer>();
        baseColor_ = spriteRenderer_.color;
    }

    void Update()
    {
        float a = (Mathf.Sin(G.D.GameTime * 15) + 1) * 0.25f + 0.1f;
        baseColor_.a = a;
        spriteRenderer_.color = baseColor_;
    }
}
