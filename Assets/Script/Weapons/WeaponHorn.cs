using UnityEngine;

public class WeaponHorn : WeaponBase
{
    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.0f;
        GameManager.Instance.Orc.SetChasePlayer(true);
        AudioManager.Instance.PlayClipWithRandomPitch(FireAudio);
    }
}
