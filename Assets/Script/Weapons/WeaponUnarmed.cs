using Assets.Script;
using UnityEngine;

public class WeaponUnarmed : WeaponBase
{
    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.0f;
        GameManager.Instance.PlayerScript.HolyTalk();
        var pos = GameManager.Instance.PlayerTrans.position;
        const float Radius = 5.0f;
        GameManager.Instance.MakeFlash(pos, Radius * 5f);
        GameManager.Instance.MakePoof(pos, 1, Radius * 1f);
        GameManager.Instance.ShakeCamera(1.0f);
        AudioManager.Instance.PlayClip(AudioManager.Instance.PlayerAudioSource, FireAudio, volumeScale: 1.0f);

        int aliveCount = BlackboardScript.GetEnemies(pos, Radius);
        for (int i = 0; i < aliveCount; ++i)
        {
            int idx = BlackboardScript.Matches[i].Idx;
            ActorBase enemy = BlackboardScript.Enemies[idx];
            var dir = enemy.transform.position - pos;
            float distance = dir.magnitude + 0.0001f;
            dir /= distance;
            dir.Normalize();
            var force = (Radius * distance) * dir * 0.045f;
            enemy.AddForce(force);
            enemy.SetSlowmotion();
        }
    }
}
