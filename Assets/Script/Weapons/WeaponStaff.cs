using UnityEngine;

public class WeaponStaff : WeaponBase
{
    public Sprite BulletSprite;

    void OnCollision(ProjectileManager.Basic projectile, ActorBase other, float damage, Vector3 dir)
    {
        other.OnLivingBombHit(500.0f);
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.0f;
        Color color = new Color(0.8f, 0.3f, 1.0f);
        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        worldMuzzle += -direction * 0.2f; // Start a little behind muzzle because its very unfun missing an enemy that is too close

        GameManager.Instance.MakePoof(worldMuzzle, 4);
        GameManager.Instance.MakeFlash(worldMuzzle);
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.PlayerAudioSource, FireAudio);

        Vector3 dir = direction.normalized;

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 8.0f;
        basic.Damage = 0.0f;
        basic.DamageFalloffDistance = 0.0f;
        basic.DamageFalloffPerMeter = 0.0f;
        basic.MaxDistance = 15.0f;
        basic.Radius = 1.0f;
        basic.CustomCollisionResponse = OnCollision;

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
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        ProjectileManager.Instance.Fire(basic);
    }
}
