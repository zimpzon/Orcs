using System.Collections.Generic;
using UnityEngine;

public class GoCache : MonoBehaviour
{
    public GameObject Prefab;
    public string Name;

    public int PreCreateCount = 1;

    public int InCache;
    public int InFlight;
    public int Total;

    private List<GameObject> cachedObjects = new List<GameObject>();

    public void PreCreate(int count)
    {
        while (cachedObjects.Count < count)
        {
            AddInstance();
        }
    }

    void Start()
    {
        PreCreate(PreCreateCount);
    }

    private void AddInstance()
    {
        var newInstance = (GameObject)Instantiate(Prefab);
        newInstance.transform.position = Vector3.left * (10000 + Random.value * 10000); // Whoops, why are they still in the physics system when active = false? Have to hide them.
        newInstance.SetActive(false);
        cachedObjects.Add(newInstance);
    }

    public GameObject GetInstance()
    {
        if (cachedObjects.Count == 0)
        {
            for (int i = 0; i < 10; ++i)
                AddInstance();
        }

        int last = cachedObjects.Count - 1;
        var result = cachedObjects[last];
        cachedObjects.RemoveAt(last);
        InFlight++;
        return result;
    }

    public void ReturnInstance(GameObject instance)
    {
        InFlight--;
        // Whoops, why are they still in the physics system when active = false? Have to hide them.
        instance.transform.position = Vector3.left * 20000;
        instance.SetActive(false);
        cachedObjects.Add(instance);
    }

    private void LateUpdate()
    {
        InCache = cachedObjects.Count;
        Total = InCache + InFlight;
    }
}
