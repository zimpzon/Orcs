using UnityEngine;

public class WeaponMachinegun : WeaponBase
{
    public Sprite BulletSprite;

    public override void FireFromPoint(Vector3 point, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.01f;
        IsPrimary = true;
        Color color = new Color(0.2f, 0.5f, 1.0f);

        GameManager.Instance.MakeFlash(point);
        AudioManager.Instance.PlayClip(FireAudio, volumeScale: 0.7f, pitch: 0.2f);
        //AudioManager.Instance.PlayClipWithRandomPitch(FireAudio, volumeScale: 0.5f);

        float spreadFactor = 40f; // Increase this to limit spread (unit circle is moved further away)
        Vector3 dir = direction * spreadFactor;
        Vector2 spread = RndUtil.RandomInsideUnitCircle();
        dir.x += spread.x;
        dir.y += spread.y;
        dir.Normalize();

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 8.0f + Random.value * 0.1f;
        basic.Damage = 75.0f;
        basic.MaxDistance = PlayerUpgrades.Data.MagicMissileBaseRange * PlayerUpgrades.Data.MagicMissileRangeMul;
        basic.Radius = 0.3f;

        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 3.0f;
        scale.y = 3.0f;
        scale.z = 1.0f;

        basic.Position = point;
        basic.SpriteInfo.Renderer.sprite = BulletSprite;
        basic.SpriteInfo.Renderer.sortingLayerID = sortingLayer;
        basic.Direction = dir;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;
        basic.Force = 0.5f;
        //float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        ProjectileManager.Instance.Fire(basic);
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        recoil = 0;
    }
}
