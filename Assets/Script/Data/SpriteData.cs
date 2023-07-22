using UnityEngine;

[CreateAssetMenu()]
public class SpriteData : ScriptableObject
{
    public static SpriteData Instance;

    private void OnEnable()
    {
        Instance = this;
    }

    public Sprite[] PlayerRunSprites;
    public Sprite[] PlayerIdleSprites;
    public Sprite[] PlayerMeleeSprites;

    public Sprite[] SkellieWalkSprites;
    public Sprite[] Skellie2WalkSprites;
    public Sprite[] SkellieCasterWalkSprites;
    public Sprite[] SkellieGunWalkSprites;
    public Sprite[] Pirate1WalkSprites;

    public Sprite MeleeTrail;

    public Sprite Orc;
    public Sprite Bullet;
    public Sprite RoundBullet;
    public Sprite Sawblade;

    public Sprite Shotgun;
    public Sprite Sniper;
    public Sprite Flamethrower;
    public Sprite MachineGun;
    public Sprite Staff;
    public Sprite Coin1;
    public Sprite Coin2;
    public Sprite Coin3;
    public Sprite Coin4;
    public Sprite Xp;
}
