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
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio, volumeScale: 0.5f);

        float spreadFactor = 30f; // Increase this to limit spread (unit circle is moved further away)
        Vector3 dir = direction * spreadFactor;
        Vector2 spread = RndUtil.RandomInsideUnitCircle();
        dir.x += spread.x;
        dir.y += spread.y;
        dir.Normalize();

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 10.0f + Random.value * 2;
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
        basic.Force = 0.75f;

        ProjectileManager.Instance.Fire(basic);
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        recoil = 0;
    }
}
