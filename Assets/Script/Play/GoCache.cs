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

    private List<GameObject> objects = new List<GameObject>();

    public void PreCreate(int count)
    {
        while (objects.Count < count)
        {
            var newInstance = (GameObject)Instantiate(Prefab);
            newInstance.SetActive(false);
            objects.Add(newInstance);
        }
    }

    void Awake()
    {
        PreCreate(PreCreateCount);
    }

    public GameObject GetInstance()
    {
        if (objects.Count > 0)
        {
            int last = objects.Count - 1;
            var result = objects[last];
            objects.RemoveAt(last);
            InFlight++;
            return result;
        }

        var newInstance = (GameObject)Instantiate(Prefab);
        InFlight++;
        return newInstance;
    }

    public void ReturnInstance(GameObject instance)
    {
        InFlight--;
        instance.SetActive(false);
        objects.Add(instance);
    }

    private void LateUpdate()
    {
        InCache = objects.Count;
        Total = InCache + InFlight;
    }
}
