using System;
using UnityEngine;

[CreateAssetMenu()]
public class SpriteData : ScriptableObject
{
    public static SpriteData Instance;

    [Serializable]
    public class Settingvalues
    {
        public ActorTypeEnum ActorType = ActorTypeEnum.None;
        public float Scale = 1.0f;
        public float Hp = 100;
        public float MoveSpeed = 1.0f;
        public float MaxHp = 666;
    };

    [Serializable]
    public class ActorSettings
    {
        public Sprite[] WalkSprites;
        public Settingvalues Settings;
    };

    private void OnEnable()
    {
        Instance = this;
    }

    public ActorSettings OgreLarge;
    public ActorSettings Ogre;
    public ActorSettings OgreMini;

    public ActorSettings OgreCasterMini;
    public ActorSettings OgreCaster;
    public ActorSettings OgreCasterLarge;

    public ActorSettings OgreHeadband;
    public ActorSettings OgreHeadbandWeapon;

    public Sprite[] PlayerRunSprites;
    public Sprite[] PlayerIdleSprites;
    public Sprite[] PlayerMeleeSprites;

    public Sprite[] OgreWalkSprites;
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
