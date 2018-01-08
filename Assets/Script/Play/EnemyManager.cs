using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    public GoCache CacheSmallWalker;
    public GoCache CacheLargeWalker;
    public GoCache CacheCharger;
    public GoCache CacheCaster;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetEnemyFromCache(ActorTypeEnum type)
    {
        return GetCacheFromType(type).GetInstance();
    }

    public void ReturnEnemyToCache(ActorTypeEnum type, GameObject go)
    {
        GetCacheFromType(type).ReturnInstance(go);
    }

    public GoCache GetCacheFromType(ActorTypeEnum type)
    {
        GoCache result = null;
        switch (type)
        {
            case ActorTypeEnum.SmallWalker: result = EnemyManager.Instance.CacheSmallWalker; break;
            case ActorTypeEnum.LargeWalker: result = EnemyManager.Instance.CacheLargeWalker; break;
            case ActorTypeEnum.SmallCharger: result = EnemyManager.Instance.CacheCharger; break;
            case ActorTypeEnum.Caster: result = EnemyManager.Instance.CacheCaster; break;
            default: Debug.LogError("EnemyManager: Unknown enemy type: " + type.ToString()); break;
        }
        return result;
    }
}
