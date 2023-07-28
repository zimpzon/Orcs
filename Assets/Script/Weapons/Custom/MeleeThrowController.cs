using UnityEngine;

public class MeleeThrowController : MonoBehaviour, IPlayerToggleEfffect
{
    bool enabled_;
    float nextThrow_;

    public void Enable(bool enable)
    {
        enabled_ = enable;
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
        if (!enabled_ || !PlayerUpgrades.Data.MeleeThrowEnabled)
            return;

        if (GameManager.Instance.GameTime > nextThrow_)
        {
            float damage = PlayerUpgrades.Data.MeleeThrowBaseDamage * PlayerUpgrades.Data.DamageMul * PlayerUpgrades.Data.MeleeThrowPowerMul;
            Vector3 scale = Vector3.one;
            Throw(Vector3.right, damage, scale);
            Throw(Vector3.left, damage, scale);
            Throw(Vector3.up, damage, scale);
            Throw(Vector3.down, damage, scale);
            Throw(Vector3.down + Vector3.left, damage * 0.25f, Vector3.one * 0.5f);
            Throw(Vector3.down + Vector3.right, damage * 0.25f, Vector3.one * 0.5f);
            Throw(Vector3.up + Vector3.left, damage * 0.25f, Vector3.one * 0.5f);
            Throw(Vector3.up + Vector3.right, damage * 0.25f, Vector3.one * 0.5f);

            nextThrow_ = GameManager.Instance.GameTime + PlayerUpgrades.Data.MeleeThrowBaseCd * PlayerUpgrades.Data.MeleeThrowCdMul;
        }
    }
}
