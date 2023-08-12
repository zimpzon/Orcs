using UnityEngine;

public class WeaponGrenade : WeaponBase
{
    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = G.D.GameTime;
        recoil = 0.1f;
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio);
    }
}
