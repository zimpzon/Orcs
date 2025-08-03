using UnityEngine;

public class WeaponSawblade : WeaponBase
{
    public Sprite BulletSprite;

    public override void Eject(Vector3 pos, Vector3 direction, Color color, float weaponScale = 1.0f)
    {
        GameManager.Instance.MakeFlash(pos);
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio);
        Vector3 dir = direction.normalized;

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();
        basic.Type = ProjectileManager.ProjectileType.HarmsEnemies;

        float volume = 0.0f;
        if (G.D.GameTime > NextNoise)
        {
            volume = 0.1f;
            NextNoise = G.D.GameTime + 15;
        }
        weaponScale = 0.55f;

        // We have PlayerUpgrades.Data.NecromancerEffectiveDamage. How often do we apply that?
        // I have chosen every 0.5 sec = 2 times per sec.
        const float ApplyDamgeCd = 0.5f;

        basic.DamageSource = ActorDamageSource.Necromancer;
        basic.Speed = 8.0f;
        basic.Damage = PlayerUpgrades.Data.NecromancerEffectiveDamage;
        basic.StickyDamageCd = ApplyDamgeCd;
        basic.StickyMaxTotalDamage = int.MaxValue;
        basic.StickyDamageSimulateCd = 0.2f;
        basic.Force = 0.5f;
        basic.MaxDistance = 9999;
        basic.RotationSpeed = 360.0f * 2;
        basic.RotationSpeedWhenStuck = 360.0f * 6;
        basic.ReflectOnEdges = true;


        basic.DamageFalloffDistance = 0.0f;
        basic.DamageFalloffPerMeter = 0.0f;
        basic.Radius = 0.4f * weaponScale;
        basic.DieOnCollision = false;
        basic.CollisionSound = AudioManager.Instance.AudioData.Chainsaw;
        basic.StickySoundRepeater = AudioManager.Instance.RepeatingSawblade;
        basic.Volume = volume;
        basic.StickToTarget = true;
        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 0.7f * weaponScale;
        scale.y = 0.7f * weaponScale;
        scale.z = 1.0f;

        basic.Position = pos;
        basic.SpriteInfo.Renderer.sprite = BulletSprite;
        basic.SpriteInfo.Renderer.sortingLayerID = GameManager.Instance.SortLayerTopEffects;
        basic.Direction = dir;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.SpriteInfo.Transform.localScale = scale;

        ProjectileManager.Instance.Fire(basic);
    }

    static float NextNoise;
}
