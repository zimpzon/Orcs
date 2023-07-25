using Assets.Script;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponSniper : WeaponBase
{
    public Sprite BulletSprite;

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.1f;

        Color color = new Color(0.8f, 0.6f, 0.1f);

        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        worldMuzzle += -direction * 0.2f; // Start a little behind muzzle because its very unfun missing an enemy that is too close

        GameManager.Instance.MakeFlash(worldMuzzle);
        GameManager.Instance.MakePoof(worldMuzzle, 2);
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio);

        ProjectileManager.Basic basic = ProjectileManager.Instance.GetProjectile();
        basic.SpriteInfo = ProjectileCache.Instance.GetSprite();

        basic.Speed = 100.0f;
        basic.Damage = 450.0f;
        basic.MaxDistance = 80.0f;

        basic.Radius = 0.1f;
        Vector3 scale = basic.SpriteInfo.Transform.localScale;
        scale.x = 1.0f;
        scale.y = 1.0f;
        scale.z = 1.0f;
        basic.SpriteInfo.Transform.localScale = scale;

        basic.Position = worldMuzzle;
        basic.SpriteInfo.Renderer.sprite = BulletSprite;
        basic.SpriteInfo.Renderer.sortingLayerID = sortingLayer;
        basic.Direction = direction;
        basic.Color = color;
        basic.DieTime = 0.0f;
        basic.Type = ProjectileManager.ProjectileType.HarmsNothing;
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        basic.SpriteInfo.Transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

        ProjectileManager.Instance.Fire(basic);

        // TODO PE: Spherecast will return sorted list
        // Sniper immediately hits the full line. Since we sort by distance we have to allow a large number of hits or we might not include closest in match list.
        float damage = basic.Damage;
        int count = BlackboardScript.GetEnemies(worldMuzzle, worldMuzzle + direction * 1000, 50);
        // Copy matches to local array. Sort it by distance.
        List<BlackboardScript.HitMatch> matches = new List<BlackboardScript.HitMatch>(BlackboardScript.Matches.Take(count));
        var ordered = matches.OrderBy(hit => hit.Distance).Take(5).ToList();

        for (int i = 0; i < ordered.Count; ++i)
        {
            BlackboardScript.HitMatch match = ordered[i];
            ActorBase enemy = BlackboardScript.Enemies[match.Idx];
            float enemyDamage = Mathf.Min(enemy.Hp, damage);
            GameManager.Instance.DamageEnemy(enemy, enemyDamage, direction, 1.0f);
            damage -= enemyDamage;
            if (damage <= 0)
                break;
        }
    }
}
