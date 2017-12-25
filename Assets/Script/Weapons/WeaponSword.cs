using Assets.Script;
using UnityEngine;

public class WeaponSword : WeaponBase
{
    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = -0.2f;
        GameManager.Instance.PlayerScript.SwingMelee();

        const float Damage = 500;
        const float Radius = 3.0f;
        var pos = GameManager.Instance.PlayerTrans.position;
        ProjectileManager.Instance.DeflectSource = pos;
        ProjectileManager.Instance.DoDeflect = true;
        ProjectileManager.Instance.DeflectRadius = Radius;

        int aliveCount = BlackboardScript.GetEnemies(pos, Radius);
        var clip = aliveCount == 0 ? FireAudio : AudioManager.Instance.AudioData.SaberHit;
        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.PlayerAudioSource, clip);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            enemy.SetSlowmotion();
            enemy.ApplyDamage(Damage, enemy.transform.position - pos, 1.0f, true);
        }
    }
}
