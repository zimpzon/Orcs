using UnityEngine;

public class PickUpManagerScript : MonoBehaviour
{
    public static PickUpManagerScript Instance;

    public GoCache CacheXp;
    public GoCache CacheMoney;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetPickUpFromCache(AutoPickUpType type)
    {
        return GetCacheFromType(type).GetInstance();
    }

    public void ReturnPickUpToCache(AutoPickUpType type, GameObject go)
    {
        GetCacheFromType(type).ReturnInstance(go);
    }

    private GoCache GetCacheFromType(AutoPickUpType type)
    {
        GoCache result = null;
        switch (type)
        {
            case AutoPickUpType.Xp: result = CacheXp; break;
            case AutoPickUpType.Money: result = CacheMoney; break;
            default: Debug.LogError("PickupManager: Unknown type: " + type.ToString()); break;
        }
        return result;
    }
}
