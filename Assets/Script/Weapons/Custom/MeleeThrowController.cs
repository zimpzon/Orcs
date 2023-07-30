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

    void Throw(Vector3 dir, float damage, Vector3 scale)
    {
        var melee = CacheManager.Instance.MeleeThrowCache.GetInstance().GetComponent<MeleeThrow>();
        melee.transform.position = transform.position;
        melee.gameObject.SetActive(true);
        melee.Throw(dir, damage, scale);
    }

    void Update()
    {
        if (!enabled_ || !PlayerUpgrades.Data.MeleeThrowEnabledInRound)
            return;

        if (GameManager.Instance.GameTime > nextThrow_)
        {
            float damage = PlayerUpgrades.Data.MeleeThrowBaseDamage * PlayerUpgrades.Data.MeleeThrowPowerMul;
            Vector3 scale = Vector3.one * 2.0f;
            if (PlayerUpgrades.Data.MeleeThrowLeft)
                Throw(Vector3.left, damage, scale);

            if (PlayerUpgrades.Data.MeleeThrowRight)
                Throw(Vector3.right, damage, scale);

            if (PlayerUpgrades.Data.MeleeThrowUp)
                Throw(Vector3.up, damage * 0.5f, scale * 0.75f);

            if (PlayerUpgrades.Data.MeleeThrowDown)
                Throw(Vector3.down, damage * 0.5f, scale * 0.75f);

            if (PlayerUpgrades.Data.MeleeThrowDownLeft)
                Throw(Vector3.down + Vector3.left, damage * 0.25f, scale * 0.5f);

            if (PlayerUpgrades.Data.MeleeThrowDownRight)
                Throw(Vector3.down + Vector3.right, damage * 0.25f, scale * 0.5f);

            if (PlayerUpgrades.Data.MeleeThrowUpLeft)
                Throw(Vector3.up + Vector3.left, damage * 0.25f, scale * 0.5f);

            if (PlayerUpgrades.Data.MeleeThrowUpRight)
                Throw(Vector3.up + Vector3.right, damage * 0.25f, scale * 0.5f);

                SetNextThrow();
        }
    }
}
