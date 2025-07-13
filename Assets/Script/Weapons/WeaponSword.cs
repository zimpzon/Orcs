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
        AudioManager.Instance.PlayClipWithRandomPitch(clip, volumeScale: 0.2f);
        for (int i = 0; i < aliveCount; ++i)
        {
            ActorBase enemy = BlackboardScript.EnemyOverlap[i];
            enemy.SetSlowmotion();
        }
        return aliveCount;
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        recoil = 0.0f;
    }
}
