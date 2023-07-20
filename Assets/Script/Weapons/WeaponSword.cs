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
        AudioManager.Instance.PlayClipWithRandomPitch(clip);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            enemy.SetSlowmotion();
            enemy.ApplyDamage(damage, enemy.transform.position - where, 1.0f, true);
        }
        return aliveCount;
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;

        const float Damage = 500;
        const float Radius = 3.0f;
        var pos = GameManager.Instance.PlayerTrans.position;
        Swing(pos, Damage, Radius, AudioManager.Instance.AudioData.SaberHit, AudioManager.Instance.AudioData.SaberSwing, out recoil);
        GameManager.Instance.PlayerScript.SwingMelee();
    }
}
