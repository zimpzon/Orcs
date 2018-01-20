using UnityEngine;

public class WeaponSawblade : WeaponBase
{
    public Sprite BulletSprite;

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.2f;
        Color color = Color.white;
        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);

        GameManager.Instance.MakeFlash(worldMuzzle);
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.PlayerAudioSource, FireAudio);
        Vector3 dir = direction.normalized;

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        basic.Speed = 7.0f;
        basic.Damage = 2.0f;
        basic.DamageFalloffDistance = 0.0f;
        basic.DamageFalloffPerMeter = 0.0f;
        basic.Force = 1.0f;
        basic.MaxDistance = 30.0f;
        basic.Radius = 0.5f;
        basic.DieOnCollision = false;
        basic.ReflectOnEdges = true;
        basic.CollisionSound = AudioManager.Instance.AudioData.Chainsaw;
        basic.StickySoundRepeater = AudioManager.Instance.RepeatingSawblade;
        basic.RotationSpeed = 360.0f * 1;
        basic.RotationSpeedWhenStuck = 360.0f * 6;
        basic.StickToTarget = true;

        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 0.5f;
        scale.y = 0.5f;
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
