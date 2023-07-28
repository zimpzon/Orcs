using UnityEngine;

public class CacheManager : MonoBehaviour
{
    public static CacheManager Instance;

    public GoCache MeleeThrowCache;

    private void OnEnable()
    {
        Instance = this;
    }
}
