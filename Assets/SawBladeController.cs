using UnityEngine;

public class SawBladeController : MonoBehaviour, IPlayerToggleEfffect
{
    bool enabled_;
    float nextThrow_;

    public void Enable(bool enable)
    {
        enabled_ = enable;
    }

    void Update()
    {
        if (!enabled_ || !PlayerUpgrades.Data.SawBladeEnabled)
            return;

        if (GameManager.Instance.GameTime > nextThrow_)
        {
            var sawblades = WeaponBase.GetWeapon(WeaponType.Sawblade);
            sawblades.Eject(transform.position, RndUtil.RandomInsideUnitCircleDiagonals(), weaponScale: 0.75f);

            nextThrow_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.SawBladeBaseCd * PlayerUpgrades.Data.SawBladeCdMul;
        }
    }
}
