using UnityEngine;

public class WeaponPaintballRandom : WeaponBase
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
        lastFire_ = Time.time;
        recoil = 0.0f;
        Color color = new Color(0.4f, 0.8f, 0.3f);

        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);

        GameManager.Instance.MakeFlash(worldMuzzle);
        AudioManager.Instance.PlayClip(FireAudio, volumeScale: 0.3f);

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 5.0f;
        basic.MaxDistance = 8.0f;
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
        basic.Direction = direction;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;

        ProjectileManager.Instance.Fire(basic);
    }
}
