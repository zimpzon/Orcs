using UnityEngine;

public enum WeaponType { None, Unarmed, Shotgun, Grenade, Machinegun, SuperShotgun, SawedShotgun, ShotgunSlug, Sniper, Horn, Staff, Staff2, Paintball, Rambo, Sword1, Mine, Last };

public class WeaponNone : WeaponBase
{
    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil) { recoil = 0.0f; }
}

public abstract class WeaponBase
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
            default: return wepType.ToString();
        }
    }

    static WeaponNone None = new WeaponNone();
    static WeaponHorn Horn;
    static WeaponStaff Staff;
    static WeaponStaff2 Staff2;
    static WeaponPaintball Paintball;
    static WeaponRambo Rambo;
    static WeaponShutgun Shotgun;
    static WeaponSuperShutgun SuperShotgun;
    static WeaponSawedShutgun SawedShotgun;
    static WeaponShutgunSlug ShotgunSlug;
    static WeaponSniper Sniper;
    static WeaponMachinegun Machinegun;
    static WeaponGrenade Grenade;
    static WeaponUnarmed Unarmed;
    static WeaponSword Sword;
    static WeaponMine Mine;

    public WeaponType Type;
    public Vector3 Scale;
    public Vector3 Muzzle;
    public float Cd;
    public float MoveSpeedModifier;
    public Sprite Sprite;
    public AudioClip FireAudio;

    protected float lastFire_;

    public float GetCdLeft()
    {
        return Mathf.Max(0.0f, (lastFire_ + Cd) - Time.time);
    }

    public Vector3 GetMuzzlePoint(Transform weaponTrans)
    {
        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        return worldMuzzle;
    }

    public abstract void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil);
    public virtual void StopFire() { }

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

            case WeaponType.Shotgun:
                if (Shotgun == null)
                {
                    Shotgun = new WeaponShutgun();
                    Shotgun.Type = WeaponType.Shotgun;
                    Shotgun.Cd = 0.5f;
                    Shotgun.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    Shotgun.Sprite = SpriteData.Instance.Shotgun;
                    Shotgun.BulletSprite = SpriteData.Instance.RoundBullet;
                    Shotgun.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Shotgun.FireAudio = AudioManager.Instance.AudioData.PlayerShotgunFire;
                    Shotgun.MoveSpeedModifier = 0.25f;
                }
                return Shotgun;

            case WeaponType.SuperShotgun:
                if (SuperShotgun == null)
                {
                    SuperShotgun = new WeaponSuperShutgun();
                    SuperShotgun.Type = WeaponType.SuperShotgun;
                    SuperShotgun.Cd = 0.4f;
                    SuperShotgun.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    SuperShotgun.Sprite = SpriteData.Instance.Shotgun;
                    SuperShotgun.BulletSprite = SpriteData.Instance.RoundBullet;
                    SuperShotgun.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    SuperShotgun.FireAudio = AudioManager.Instance.AudioData.PlayerShotgunFire;
                    SuperShotgun.MoveSpeedModifier = 0.25f;
                }
                return SuperShotgun;

            case WeaponType.SawedShotgun:
                if (SawedShotgun == null)
                {
                    SawedShotgun = new WeaponSawedShutgun();
                    SawedShotgun.Type = WeaponType.SawedShotgun;
                    SawedShotgun.Cd = 0.4f;
                    SawedShotgun.Scale = new Vector3(1.0f, 4.0f, 1.0f);
                    SawedShotgun.Sprite = SpriteData.Instance.Shotgun;
                    SawedShotgun.BulletSprite = SpriteData.Instance.RoundBullet;
                    SawedShotgun.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    SawedShotgun.FireAudio = AudioManager.Instance.AudioData.PlayerShotgunFire;
                    SawedShotgun.MoveSpeedModifier = 0.25f;
                }
                return SawedShotgun;

            case WeaponType.ShotgunSlug:
                if (ShotgunSlug == null)
                {
                    ShotgunSlug = new WeaponShutgunSlug();
                    ShotgunSlug.Type = WeaponType.ShotgunSlug;
                    ShotgunSlug.Cd = 0.3f;
                    ShotgunSlug.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    ShotgunSlug.Sprite = SpriteData.Instance.Shotgun;
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
                    Staff.Sprite = SpriteData.Instance.Staff;
                    Staff.BulletSprite = SpriteData.Instance.Bullet;
                    Staff.Muzzle = new Vector3(1.5f, 0.013f, 0.0f);
                    Staff.FireAudio = AudioManager.Instance.AudioData.PlayerStaffFire;
                    Staff.MoveSpeedModifier = 0.8f;
                }
                return Staff;

            case WeaponType.Staff2:
                if (Staff2 == null)
                {
                    Staff2 = new WeaponStaff2();
                    Staff2.Type = WeaponType.Staff2;
                    Staff2.Cd = 0.5f;
                    Staff2.Scale = new Vector3(0.5f, 0.8f, 1.0f);
                    Staff2.Sprite = SpriteData.Instance.Staff;
                    Staff2.BulletSprite = SpriteData.Instance.Orc;
                    Staff2.Muzzle = new Vector3(1.5f, 0.013f, 0.0f);
                    Staff2.FireAudio = AudioManager.Instance.AudioData.PlayerStaffFire;
                    Staff2.MoveSpeedModifier = 0.8f;
                }
                return Staff2;

            case WeaponType.Sniper:
                if (Sniper == null)
                {
                    Sniper = new WeaponSniper();
                    Sniper.Type = WeaponType.Sniper;
                    Sniper.Cd = 0.4f;
                    Sniper.Scale = new Vector3(1.5f, 2.0f, 1.0f);
                    Sniper.Sprite = SpriteData.Instance.Sniper;
                    Sniper.BulletSprite = SpriteData.Instance.Bullet;
                    Sniper.Muzzle = new Vector3(0.6f, 0.02f, 0.0f);
                    Sniper.FireAudio = AudioManager.Instance.AudioData.PlayerSniperFire;
                    Sniper.MoveSpeedModifier = 0.25f;
                }
                return Sniper;

            case WeaponType.Machinegun:
                if (Machinegun == null)
                {
                    Machinegun = new WeaponMachinegun();
                    Machinegun.Type = WeaponType.Machinegun;
                    Machinegun.Cd = 0.08f;
                    Machinegun.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    Machinegun.Sprite = SpriteData.Instance.MachineGun;
                    Machinegun.BulletSprite = SpriteData.Instance.RoundBullet;
                    Machinegun.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Machinegun.FireAudio = AudioManager.Instance.AudioData.PlayerMachinegunFire;
                    Machinegun.MoveSpeedModifier = 0.5f;
                }
                return Machinegun;

            case WeaponType.Paintball:
                if (Paintball == null)
                {
                    Paintball = new WeaponPaintball();
                    Paintball.Type = WeaponType.Paintball;
                    Paintball.Cd = 0.2f;
                    Paintball.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    Paintball.Sprite = SpriteData.Instance.MachineGun;
                    Paintball.BulletSprite = SpriteData.Instance.RoundBullet;
                    Paintball.Muzzle = new Vector3(0.5f, 0.013f, 0.0f);
                    Paintball.FireAudio = AudioManager.Instance.AudioData.PlayerPaintballFire;
                    Paintball.MoveSpeedModifier = 1.0f;
                }
                return Paintball;

            case WeaponType.Rambo:
                if (Rambo == null)
                {
                    Rambo = new WeaponRambo();
                    Rambo.Type = WeaponType.Rambo;
                    Rambo.Cd = 0.07f;
                    Rambo.Scale = new Vector3(1.5f, 4.0f, 1.0f);
                    Rambo.Sprite = SpriteData.Instance.MachineGun;
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

            case WeaponType.Mine:
                if (Mine == null)
                {
                    Mine = new WeaponMine();
                    Mine.Type = WeaponType.Mine;
                    Mine.Cd = 0.3f;
                    Mine.FireAudio = AudioManager.Instance.AudioData.PlayerThrowBomb;
                }
                return Mine;

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

            //case WeaponType.Flamethrower:
            //    if (Flamethrower == null)
            //    {
            //        Flamethrower = new WeaponFlamethrower();
            //        Flamethrower.Type = WeaponType.Flamethrower;
            //        Flamethrower.Cd = 0.05f;
            //        Flamethrower.Scale = new Vector3(1.5f, 2.0f, 1.0f);
            //        Flamethrower.Sprite = SpriteData.Instance.Flamethrower;
            //        Flamethrower.BulletSprite = SpriteData.Instance.RoundBullet;
            //        Flamethrower.Muzzle = new Vector3(0.6f, 0.02f, 0.0f);
            //        Flamethrower.FireAudio = AudioManager.Instance.AudioData.PlayerFlamethrowerFire;
            //        Flamethrower.MoveSpeedModifier = 0.5f;
            //    }
            //return Flamethrower;

            default:
                Debug.LogError("Unknown weapon type: " + type.ToString());
                return Shotgun;
        }
    }
}

// Debug.DrawLine(worldMuzzle, worldMuzzle + direction* 5.0f, Color.red, 0.1f);
