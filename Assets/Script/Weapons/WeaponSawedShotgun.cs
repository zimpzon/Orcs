using UnityEngine;

public class WeaponSawedShutgun : WeaponBase
{
    public Sprite BulletSprite;

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.2f;
        Color color = new Color(0.8f, 0.6f, 0.1f);
        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        worldMuzzle += -direction * 0.2f; // Start a little behind muzzle because its very unfun missing an enemy that is too close

        GameManager.Instance.MakePoof(worldMuzzle, 4);
        GameManager.Instance.MakeFlash(worldMuzzle);
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio);

        for (int i = 0; i < 6; ++i)
        {
            float spreadFactor = 1.5f; // Increase this to limit spread (unit circle is moved further away)
            Vector3 dir = direction * spreadFactor;
            Vector2 spread = RndUtil.RandomInsideUnitCircle();
            dir.x += spread.x;
            dir.y += spread.y;
            dir.Normalize();

            ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
            basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
            basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

            basic.Speed = 10.0f + Random.value * 2;
            basic.Damage = 80.0f;
            basic.DamageFalloffDistance = 0.0f;
            basic.DamageFalloffPerMeter = 0.0f;
            basic.MaxDistance = 8.0f;
            basic.Radius = 0.8f;

            Vector3 scale = basic.SpriteInfo.Transform.localScale;
            scale.x = 4.0f;
            scale.y = 4.0f;
            scale.z = 1.0f;

            basic.Position = worldMuzzle;
            basic.SpriteInfo.Renderer.sprite = BulletSprite;
            basic.SpriteInfo.Renderer.sortingLayerID = sortingLayer;
            basic.Direction = dir;
            basic.Color = color;
            basic.DieTime = 0.0f;
            basic.SpriteInfo.Transform.localScale = scale;

            ProjectileManager.Instance.Fire(basic);
        }
    }
}
