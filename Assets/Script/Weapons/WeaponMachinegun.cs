using UnityEngine;

public class WeaponMachinegun : WeaponBase
{
    public Sprite BulletSprite;

    public override void FireFromPoint(Vector3 point, Vector3 direction, float damage, float scale, int sortingLayer, out float recoil)
    {
        lastFire_ = G.D.GameTime;
        recoil = 0.01f;
        IsPrimary = true;
        Color color = new (256 / 255.0f, 248 / 255.0f, 220 / 255.0f, 1.0f);

        if (scale == 1.0f)
        {
            // lol hack for multiple daggers
            GameManager.Instance.MakeFlash(point);
            AudioManager.Instance.PlayClip(FireAudio, volumeScale: 1.5f, pitch: 1.2f);
        }

        float spreadFactor = 40f; // Increase this to limit spread (unit circle is moved further away)
        Vector3 dir = direction * spreadFactor;
        Vector2 spread = RndUtil.RandomInsideUnitCircle();
        dir.x += spread.x;
        dir.y += spread.y;
        dir.Normalize();

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = PlayerUpgrades.Data.MagicMissileBaseSpeed * PlayerUpgrades.Data.MagicMissileSpeedMul + Random.value;
        basic.Damage = damage;
        basic.MaxDistance = PlayerUpgrades.Data.MagicMissileBaseRange * PlayerUpgrades.Data.MagicMissileRangeMul;

        if (PlayerUpgrades.Data.IsRambo)
        {
            basic.MaxDistance *= 5;
            basic.SwayFactor = 0.2f;
        }

        basic.Radius = 0.3f;

        basic.Position = point;
        basic.SpriteInfo.Renderer.sprite = BulletSprite;
        basic.SpriteInfo.Renderer.sortingLayerID = sortingLayer;
        basic.Direction = dir;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = new Vector2(scale, scale);
        basic.Force = 0.5f;
        basic.JumpToNearbyTarget = PlayerUpgrades.Data.MagicMissileJumpDamageMul > 0;
        basic.JumpDamageMul = PlayerUpgrades.Data.MagicMissileJumpDamageMul;
        basic.DieOnCollision = !basic.JumpToNearbyTarget;

        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        ProjectileManager.Instance.Fire(basic);
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        recoil = 0;
    }
}
