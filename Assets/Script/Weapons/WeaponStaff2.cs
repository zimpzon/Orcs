using UnityEngine;

public class WeaponStaff2 : WeaponBase
{
    public Sprite BulletSprite;

    void OnCollision(ProjectileManager.Basic projectile, ActorBase other, float damage, Vector3 dir)
    {
        other.ApplyDamage(damage, dir, 1.0f);
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.0f;
        Color color = Color.white;
        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        worldMuzzle += -direction * 0.2f; // Start a little behind muzzle because its very unfun missing an enemy that is too close

        GameManager.Instance.MakePoof(worldMuzzle, 4);
        GameManager.Instance.MakeFlash(worldMuzzle);
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio);

        Vector3 dir = direction.normalized;

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 6.0f;
        basic.SwayFactor = 0.1f;
        basic.Damage = 20.0f;
        basic.DamageFalloffDistance = 0.0f;
        basic.DamageFalloffPerMeter = 0.0f;
        basic.MaxDistance = 15.0f;
        basic.Radius = 1.0f;
        basic.DieOnCollision = false;
        basic.CustomCollisionResponse = OnCollision;

        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 1.0f * (direction.x < 0 ? -1 : 1);
        scale.y = 1.0f;
        scale.z = 1.0f;

        basic.Position = worldMuzzle;
        basic.SpriteInfo.Renderer.sprite = BulletSprite;
        basic.SpriteInfo.Renderer.sortingLayerID = sortingLayer;
        basic.Direction = dir;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;
        basic.SpriteInfo.Transform.rotation = Quaternion.identity;

        ProjectileManager.Instance.Fire(basic);
    }
}
