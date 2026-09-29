using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Assets.Script.Enemies
{
    public class ActorCache : MonoBehaviour
    {
        class CacheEntry
        {
            public int TotalCount = 0;
            public Stack<GameObject> Free = new();
            public HashSet<GameObject> InCache = new();
        }
        public GameObject ObjectsParent;
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
                var prefab = EnemyPrefabs.Enemies.Where(e => e.GetComponent<ActorBase>()?.ActorType == actorType)?.FirstOrDefault();
                if (prefab == null)
                    Debug.LogError($"ActorType {actorType} not found in scriptable object Enemies");
                Prefabs[actorType] = prefab;
                Caches[actorType] = new CacheEntry();
                ExpandCache(StartCapacity, actorType);
            }
            var cacheEntry = Caches[actorType];
            if (cacheEntry.Free.Count == 0)
                ExpandCache(Mathf.Max(1, cacheEntry.TotalCount / 2), actorType);

            var actor = cacheEntry.Free.Pop();
            cacheEntry.InCache.Remove(actor);
            return actor;
        }
        public void ReturnObject(GameObject actor)
        {
            var actorType = actor.GetComponent<ActorBase>().ActorType;
            actor.SetActive(false);

            // Double returns used to corrupt the cache and hand out the same actor twice in one round.
            if (!Caches.TryGetValue(actorType, out var cacheEntry) || !cacheEntry.InCache.Add(actor))
                return;

            cacheEntry.Free.Push(actor);
        }
        void ExpandCache(int count, ActorTypeEnum actorType)
        {
            var prefab = Prefabs[actorType];
            var cacheEntry = Caches[actorType];
            for (int i = 0; i < count; ++i)
            {
                var newObject = Instantiate(prefab);
                newObject.SetActive(false);
                var originalScale = newObject.transform.localScale;
                newObject.transform.SetParent(ObjectsParent.transform, worldPositionStays: true);
                newObject.transform.localScale = originalScale;
                cacheEntry.Free.Push(newObject);
                cacheEntry.InCache.Add(newObject);
                cacheEntry.TotalCount++;
            }
        }
    }
}
