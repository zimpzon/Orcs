using UnityEngine;

class WeaponFlamethrower : WeaponBase
{
    public override void OnAcquired()
    {
        AudioManager.Instance.StopClip(AudioManager.Instance.PlayerAudioSource);
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.001f;

        Vector3 worldMuzzle = weaponTrans.TransformPoint(Muzzle);
        worldMuzzle += -direction * 0.5f; // Start a little behind muzzle because its very unfun missing an enemy that is too close (even further for flamethrower)

        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.PlayerAudioSource, FireAudio);
    }
}
