using UnityEngine;

public class CacheManager : MonoBehaviour
{
    public static CacheManager Instance;

    public GoCache MeleeThrowCache;
    public GoCache CirclingAxeCache;

    private void OnEnable()
    {
        Instance = this;
    }
}
