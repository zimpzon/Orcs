using UnityEngine;

public class SawBladeController : MonoBehaviour, IPlayerToggleEfffect
{
    public static SawBladeController I;

    public Color Color;

    bool enabled_;
    float nextThrow_;

    void Awake()
    {
        I = this;
    }

    public void Disable()
    {
        enabled_ = false;
    }

    public void TryEnable()
    {
        if (PlayerUpgrades.Data.SawBladeBought)
        {
            enabled_ = true;
            SetNextThrow();
        }
    }

    void SetNextThrow()
    {
        nextThrow_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.SawBladeBaseCd * PlayerUpgrades.Data.SawBladeCdMul;
    }

    public void Throw()
    {
        var sawblades = WeaponBase.GetWeapon(WeaponType.Sawblade);
        sawblades.Eject(transform.position, RndUtil.RandomInsideUnitCircle(), Color, weaponScale: 1.0f);
    }

    void Update()
    {
        if (!enabled_ || !PlayerUpgrades.Data.SawBladeEnabledInRound)
            return;

        if (GameManager.Instance.GameTime > nextThrow_)
        {
            Throw();
            SetNextThrow();
        }
    }
}
