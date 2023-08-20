using UnityEngine;

public class CirclingAxeController : MonoBehaviour, IPlayerToggleEfffect
{
    public Color Color;

    bool enabled_;
    float nextThrow_;

    public void Disable()
    {
        enabled_ = false;
        nextThrow_ = 0;
    }

    public void TryEnable()
    {
        enabled_ = true;
        nextThrow_ = 0;
    }

    void SetNextThrow()
    {
        nextThrow_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.CirclingAxeBaseCd * PlayerUpgrades.Data.CirclingAxeCdMul;
    }

    void Throw()
    {
        var axe = CacheManager.Instance.CirclingAxeCache.GetInstance().GetComponent<CirclingAxe>();

        float damage = PlayerUpgrades.Data.CirclingAxeBaseDamage * PlayerUpgrades.Data.CirclingAxeDamageMul;
        float speed = PlayerUpgrades.Data.CirclingAxeBaseSpeed * PlayerUpgrades.Data.CirclingAxeSpeedMul;
        axe.transform.position = G.D.PlayerPos;
        axe.gameObject.SetActive(true);

        var dir = RndUtil.RandomInsideUnitCircle().normalized;
        axe.Throw(dir, damage, speed);
    }

    void Update()
    {
        if (!enabled_ || !PlayerUpgrades.Data.CirclingAxeEnabled)
            return;

        if (GameManager.Instance.GameTime > nextThrow_)
        {
            Throw();
            SetNextThrow();
        }
    }
}
