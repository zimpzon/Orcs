using UnityEngine;

public class WeaponPaintball : WeaponBase
{
    const float SlowTime = 3.0f;
    public Sprite BulletSprite;

    void OnCollision(ProjectileManager.Basic projectile, ActorBase other, float damage, Vector3 dir)
    {
        if (other.OnPaintballHit(projectile.Color, SlowTime))
        {
            if (--projectile.CustomCounter == 0)
                projectile.DieOnCollision = true;
        }
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = G.D.GameTime;
        recoil = 0.0f;
        Color color = Color.HSVToRGB(UnityEngine.Random.value, 1.0f, 1.0f);

        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        worldMuzzle += -direction * 0.2f; // Start a little behind muzzle because its very unfun missing an enemy that is too close

        GameManager.Instance.MakePoof(worldMuzzle, 1);
        GameManager.Instance.MakeFlash(worldMuzzle);
        AudioManager.Instance.PlayClip(FireAudio, volumeScale: 0.8f);

        float spreadFactor = 15f; // Increase this to limit spread (unit circle is moved further away)
        Vector3 dir = direction * spreadFactor;
        Vector2 spread = RndUtil.RandomInsideUnitCircle();
        dir.x += spread.x;
        dir.y += spread.y;
        dir.Normalize();

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 10.0f + Random.value * 2;
        basic.Damage = 25.0f;
        basic.MaxDistance = 12.0f;
        basic.Radius = 0.3f;
        basic.DieOnCollision = false;
        basic.CustomCounter = 1;
        basic.CustomCollisionResponse = OnCollision;
        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 3.0f;
        scale.y = 3.0f;
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
