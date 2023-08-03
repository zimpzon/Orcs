using UnityEngine;

public class MeleeThrowController : MonoBehaviour, IPlayerToggleEfffect
{
    bool enabled_;
    float nextThrow_;

    public void Disable()
    {
        enabled_ = false;
    }

    public void TryEnable()
    {
        if (PlayerUpgrades.Data.MeleeThrowBought)
        {
            enabled_ = true;
            SetNextThrow();
        }
    }

    void SetNextThrow()
    {
        nextThrow_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.MeleeThrowBaseCd * PlayerUpgrades.Data.MeleeThrowCdMul;
    }

    void Throw(Vector3 location, Vector3 dir, float damage, Vector3 scale)
    {
        var melee = CacheManager.Instance.MeleeThrowCache.GetInstance().GetComponent<MeleeThrow>();

        melee.transform.position = location;
        melee.gameObject.SetActive(true);
        melee.Throw(damage, scale);
    }

    void Update()
    {
        if (!enabled_ || !PlayerUpgrades.Data.MeleeThrowEnabledInRound)
            return;

        if (GameManager.Instance.GameTime > nextThrow_)
        {
            Vector3 location = PositionUtility.GetPointInsideArena(0.9f, 0.9f);
            float damage = PlayerUpgrades.Data.MeleeThrowBaseDamage * PlayerUpgrades.Data.MeleeThrowPowerMul;
            Vector3 scale = Vector3.one * 2.0f;
            if (PlayerUpgrades.Data.MeleeThrowLeft)
                Throw(location, Vector3.left, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowRight)
            //    Throw(location, Vector3.right, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowUp)
            //    Throw(location, Vector3.up, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowDown)
            //    Throw(location, Vector3.down, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowDownLeft)
            //    Throw(location, Vector3.down + Vector3.left, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowDownRight)
            //    Throw(location, Vector3.down + Vector3.right, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowUpLeft)
            //    Throw(location, Vector3.up + Vector3.left, damage, scale);

            //if (PlayerUpgrades.Data.MeleeThrowUpRight)
            //    Throw(location, Vector3.up + Vector3.right, damage, scale);

                SetNextThrow();
        }
    }
}
