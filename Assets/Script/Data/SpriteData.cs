using UnityEngine;

public class SpriteData : MonoBehaviour
{
    public static SpriteData Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Sprite ShamanProjectile;
    public Sprite PlayerDagger;
    public Sprite RoundBullet;
    public Sprite Sawblade;
}
