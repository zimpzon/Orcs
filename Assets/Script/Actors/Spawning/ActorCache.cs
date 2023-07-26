using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Enemies
{
    public class ActorCache : MonoBehaviour
    {
        class CacheEntry
        {
            public int Idx = 0;
            public List<GameObject> Objects = new();
        }

        public static ActorCache Instance;

        public EnemyPrefabs EnemyPrefabs;

        private const int StartCapacity = 50;
        readonly Dictionary<ActorTypeEnum, CacheEntry> Caches = new();
        readonly Dictionary<ActorTypeEnum, GameObject> Prefabs = new();

        private void Awake()
        {
            Instance = this;

            foreach (var prefab in EnemyPrefabs.Enemies)
                prefab.SetActive(false);
        }

        public GameObject GetActor(ActorTypeEnum actorType)
        {
            if (!Caches.TryGetValue(actorType, out var cache))
            {
                var prefab = EnemyPrefabs.Enemies.Where(e => e.GetComponent<ActorBase>().ActorType == actorType).First();
                Prefabs[actorType] = prefab;
                Caches[actorType] = new CacheEntry();
                ExpandCache(StartCapacity, actorType);
            }

            var cacheEntry = Caches[actorType];

            if (cacheEntry.Idx >= cacheEntry.Objects.Count)
                ExpandCache(cacheEntry.Objects.Count / 2, actorType);

            return cacheEntry.Objects[cacheEntry.Idx++];
        }

        public void ReturnObject(GameObject actor)
        {
            var actorType = actor.GetComponent<ActorBase>().ActorType;
            actor.SetActive(false);

            // still in the physics system when active = false, I think.
            actor.transform.position = Vector3.left * (10000 + Random.value * 10000);

            var cacheEntry = Caches[actorType];
            cacheEntry.Objects[--cacheEntry.Idx] = actor;
        }

        void ExpandCache(int count, ActorTypeEnum actorType)
        {
            var prefab = Prefabs[actorType];
            var cacheEntry = Caches[actorType];

            for (int i = 0; i < count; ++i)
            {
                var newObject = Instantiate(prefab);
                newObject.SetActive(false);
                newObject.transform.position = Vector3.left * (10000 + Random.value * 10000);
                cacheEntry.Objects.Add(newObject);
            }
        }
    }
}
