using Assets.Script;
using UnityEngine;

public class WeaponSword : WeaponBase
{
    public static int Swing(Vector3 where, float damage, float radius, AudioClip clipHit, AudioClip clipMiss, out float recoil)
    {
        recoil = -0.2f;

        ProjectileManager.Instance.DeflectSource = where;
        ProjectileManager.Instance.DoDeflect = true;
        ProjectileManager.Instance.DeflectRadius = radius;

        int aliveCount = BlackboardScript.GetEnemies(where, radius);
        var clip = aliveCount == 0 ? clipMiss : clipHit;
        AudioManager.Instance.PlayClipWithRandomPitch(clip, volumeScale: 0.5f);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            enemy.SetSlowmotion();
            float force = 30.0f * PlayerUpgrades.Data.OrcJediKnockBackForceMul;
            enemy.ApplyDamage(damage, enemy.transform.position - where, force);
        }
        return aliveCount;
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        recoil = 0.0f;
    }
}
