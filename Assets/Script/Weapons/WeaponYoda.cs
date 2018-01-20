using UnityEngine;

public class WeaponYoda : WeaponBase
{
    public override void OnAcquired()
    {
        if (GameManager.Instance.Orc.State == OrcState.Yoda)
            return;

        GameManager.Instance.Orc.SetYoda();
    }

    public override void Fire(Transform weaponTrans, Vector3 direction, int sortingLayer, out float recoil)
    {
        lastFire_ = Time.time;
        recoil = 0.0f;
    }
}
