using UnityEngine;

public class WeaponPaintballRandom : WeaponBase
{
    public Sprite BulletSprite;

    void OnCollision(ProjectileManager.Basic projectile, ActorBase other, float damage, Vector3 dir)
    {
        float slowTime = PlayerUpgrades.Data.PaintballBaseDuration * PlayerUpgrades.Data.PaintballDurationMul;
        if (other.OnPaintballHit(projectile.Color, slowTime))
        {
            if (--projectile.CustomCounter == 0)
                projectile.DieOnCollision = true;
        }
    }

    public override void FireFromPoint(Vector3 point, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.0f;
        Color color = PoisonDaggers.Instance.Color;

        GameManager.Instance.MakeFlash(point);
        AudioManager.Instance.PlayClip(FireAudio, volumeScale: 0.1f);

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = PlayerUpgrades.Data.PaintballBaseSpeed;
        basic.MaxDistance = PlayerUpgrades.Data.PaintballBaseRange * PlayerUpgrades.Data.PaintballRangeMul;
        basic.Radius = 0.3f;
        basic.DieOnCollision = false;
        basic.CustomCounter = 1;
        basic.CustomCollisionResponse = OnCollision;
        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 1.0f;
        scale.y = 1.0f;
        scale.z = 1.0f;

        basic.Position = point;
        basic.SpriteInfo.Renderer.sprite = BulletSprite;
        basic.SpriteInfo.Renderer.sortingLayerID = sortingLayer;
        basic.Direction = direction;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;

        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        ProjectileManager.Instance.Fire(basic);
    }
}
