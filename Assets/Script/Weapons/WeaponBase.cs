using UnityEngine;

public enum WeaponType {
    None, Unarmed, Shotgun, Grenade, Machinegun, SuperShotgun, SawedShotgun, ShotgunSlug, Sniper, Horn, Staff, Staff2, Paintball, PaintBallRandom,
    Rambo, Sword1, Mine, Yoda, Sawblade, Last };

public class WeaponNone : WeaponBase
{
    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil) { recoil = 0.0f; }
}

public class WeaponBase
{
    public static string WeaponDisplayName(WeaponType wepType)
    {
        switch (wepType)
        {
            case WeaponType.None: return "None";
            case WeaponType.Unarmed: return "Unarmed";
            case WeaponType.Horn: return "Orc Whistle";
            case WeaponType.Staff: return "Plague";
            case WeaponType.Staff2: return "Orcs Revenge";
            case WeaponType.Paintball: return "Paintball";
            case WeaponType.Rambo: return "Rambo";
            case WeaponType.Shotgun: return "Puny Shotgun";
            case WeaponType.SuperShotgun: return "Super Shotgun";
            case WeaponType.SawedShotgun: return "Short-barelled";
            case WeaponType.ShotgunSlug: return "Slug";
            case WeaponType.Sniper: return "Sniper";
            case WeaponType.Machinegun: return "Machinegun";
            case WeaponType.Grenade: return "Grenade";
            case WeaponType.Sword1: return "Knightsaber";
            case WeaponType.Mine: return "Mines";
            case WeaponType.Yoda: return "Yoda";
            case WeaponType.Sawblade: return "Nightmare Sawblade";
            default: return wepType.ToString();
        }
    }

    static WeaponNone None = new WeaponNone();
    static WeaponHorn Horn;
    static WeaponStaff Staff;
    static WeaponStaff2 Staff2;
    static WeaponPaintball Paintball;
    static WeaponPaintballRandom PaintballRandom;
    static WeaponRambo Rambo;
    static WeaponShutgunSlug ShotgunSlug;
    static WeaponMachinegun Machinegun;
    static WeaponGrenade Grenade;
    static WeaponUnarmed Unarmed;
    static WeaponSword Sword;
    static WeaponYoda Yoda;
    static WeaponSawblade Sawblade;

    public WeaponType Type;
    public Vector3 Scale;
    public Vector3 Muzzle;
    public float Cd;
    public float MoveSpeedModifier;
    public Sprite Sprite;
    public AudioClip FireAudio;

    protected float lastFire_;
    public bool IsPrimary = false;

    public float GetCdLeft()
    {
        return Mathf.Max(0.0f, (lastFire_ - Time.time) + PlayerUpgrades.Data.MagicMissileBaseBulletCd * PlayerUpgrades.Data.MagicMissileCdMul);
    }

    public Vector3 GetMuzzlePoint(Transform weaponTrans)
    {
        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        return worldMuzzle;
    }

    public virtual void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil) { recoil = 0; }
    public virtual void FireFromPoint(Vector3 point, Vector3 direction, float damage, float scale, int sortingLayer, out float recoil) { recoil = 0; }
    public virtual void Eject(Vector3 pos, Vector3 direction, Color color, float weaponScale) { }

    public virtual void StopFire() { }
    public virtual void OnAcquired() { }

    public static WeaponBase GetWeapon(WeaponType type)
    {
        // To find a muzzle point:
        //  Add this in fire: Debug.DrawLine(worldMuzzle, worldMuzzle + direction * 5.0f, Color.red, 0.1f);
        //  Adjust for scale and maybe pivot?
        //  Select weapon, ctrl+shift+p to pause
        switch (type)
        {
            case WeaponType.None:
                return None;

            case WeaponType.ShotgunSlug:
                if (ShotgunSlug == null)
                {
                    ShotgunSlug = new WeaponShutgunSlug();
                    ShotgunSlug.Type = WeaponType.ShotgunSlug;
                    ShotgunSlug.Cd = 0.3f;
                    ShotgunSlug.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    ShotgunSlug.BulletSprite = SpriteData.Instance.RoundBullet;
                    ShotgunSlug.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    ShotgunSlug.FireAudio = AudioManager.Instance.AudioData.PlayerShotgunFire;
                    ShotgunSlug.MoveSpeedModifier = 0.5f;
                }
                return ShotgunSlug;

            case WeaponType.Staff:
                if (Staff == null)
                {
                    Staff = new WeaponStaff();
                    Staff.Type = WeaponType.Staff;
                    Staff.Cd = 0.3f;
                    Staff.Scale = new Vector3(0.5f, 0.8f, 1.0f);
                    Staff.BulletSprite = SpriteData.Instance.ShamanProjectile;
                    Staff.Muzzle = new Vector3(1.5f, 0.013f, 0.0f);
                    Staff.FireAudio = AudioManager.Instance.AudioData.PlayerStaffFire;
                    Staff.MoveSpeedModifier = 0.8f;
                }
                return Staff;

            case WeaponType.Machinegun:
                if (Machinegun == null)
                {
                    Machinegun = new ();
                    Machinegun.Type = WeaponType.Machinegun;
                    Machinegun.Scale = new Vector3(0.2f, 0.08f, 1.0f);
                    Machinegun.BulletSprite = SpriteData.Instance.PlayerDagger;
                    Machinegun.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Machinegun.FireAudio = AudioManager.Instance.AudioData.EnemyShoot;
                    Machinegun.MoveSpeedModifier = 1.0f;
                }
                return Machinegun;

            case WeaponType.Paintball:
                if (Paintball == null)
                {
                    Paintball = new WeaponPaintball();
                    Paintball.Type = WeaponType.Paintball;
                    Paintball.Cd = 0.2f;
                    Paintball.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    Paintball.BulletSprite = SpriteData.Instance.RoundBullet;
                    Paintball.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Paintball.FireAudio = AudioManager.Instance.AudioData.PlayerPaintballFire;
                    Paintball.MoveSpeedModifier = 1.0f;
                }
                return Paintball;

            case WeaponType.PaintBallRandom:
                if (PaintballRandom == null)
                {
                    PaintballRandom = new WeaponPaintballRandom();
                    PaintballRandom.Type = WeaponType.PaintBallRandom;
                    PaintballRandom.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    PaintballRandom.BulletSprite = SpriteData.Instance.PlayerDagger;
                    PaintballRandom.Muzzle = new Vector3(0.0f, 0.0f, 0.0f);
                    PaintballRandom.FireAudio = AudioManager.Instance.AudioData.EnemyShoot;
                    PaintballRandom.MoveSpeedModifier = 1.0f;
                }
                return PaintballRandom;

            case WeaponType.Rambo:
                if (Rambo == null)
                {
                    Rambo = new WeaponRambo();
                    Rambo.Type = WeaponType.Rambo;
                    Rambo.Cd = 0.07f;
                    Rambo.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    Rambo.BulletSprite = SpriteData.Instance.RoundBullet;
                    Rambo.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Rambo.FireAudio = AudioManager.Instance.AudioData.PlayerMachinegunFire;
                    Rambo.MoveSpeedModifier = 0.0f;
                }
                return Rambo;

            case WeaponType.Grenade:
                if (Grenade == null)
                {
                    Grenade = new WeaponGrenade();
                    Grenade.Type = WeaponType.Grenade;
                    Grenade.Cd = GrenadeScript.Cd;
                    Grenade.FireAudio = AudioManager.Instance.AudioData.PlayerThrowBomb;
                }
                return Grenade;

            case WeaponType.Unarmed:
                if (Unarmed == null)
                {
                    Unarmed = new WeaponUnarmed();
                    Unarmed.MoveSpeedModifier = 1.0f;
                    Unarmed.Type = WeaponType.Unarmed;
                    Unarmed.FireAudio = AudioManager.Instance.AudioData.UnarmedBlast;
                    Unarmed.Cd = 0.5f;
                }
                return Unarmed;

            case WeaponType.Horn:
                if (Horn == null)
                {
                    Horn = new WeaponHorn();
                    Horn.Type = WeaponType.Horn;
                    Horn.Cd = 2.0f;
                    Horn.FireAudio = AudioManager.Instance.AudioData.OrcHorn;
                }
                return Horn;

            case WeaponType.Sword1:
                if (Sword == null)
                {
                    Sword = new WeaponSword();
                    Sword.Type = WeaponType.Sword1;
                    Sword.Cd = 0.3f;
                    Sword.FireAudio = AudioManager.Instance.AudioData.SaberSwing;
                    Sword.MoveSpeedModifier = 0.0f;
                }
                return Sword;

            case WeaponType.Yoda:
                if (Yoda == null)
                {
                    Yoda = new WeaponYoda();
                    Yoda.Type = WeaponType.Yoda;
                }
                return Yoda;

            case WeaponType.Sawblade:
                if (Sawblade == null)
                {
                    Sawblade = new WeaponSawblade();
                    Sawblade.Type = WeaponType.Sawblade;
                    Sawblade.Cd = 1.0f;
                    Sawblade.BulletSprite = SpriteData.Instance.Sawblade;
                    Sawblade.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Sawblade.FireAudio = AudioManager.Instance.AudioData.PlayerThrowBomb;
                    Sawblade.MoveSpeedModifier = 0.2f;
                }
                return Sawblade;

            default:
                Debug.LogError("Unknown weapon type: " + type.ToString());
                return Unarmed;
        }
    }
}

// Debug.DrawLine(worldMuzzle, worldMuzzle + direction* 5.0f, Color.red, 0.1f);
