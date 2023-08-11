using Assets.Script.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PositionUtility;

namespace Assets.Script.Actors.Spawning
{
    public static class SpawnUtil
    {
        public static void FleeAllActors()
        {
            var allEnemies = BlackboardScript.GetAllEnemies();
            for (int i = 0; i < allEnemies.Length; ++i)
            {
                var actor = allEnemies[i];
                var actorPos = actor.transform.position;

                var target = actorPos.y < 0 ? Vector3.down : Vector3.up;
                actor.SetForcedTarget(actorPos + target, despawnAtDestination: true, breakAtDamage: false, ActorForcedTargetType.Direction);
            }
        }

        public static IEnumerator ActionAtTime(TimeSpan time, Action action)
        {
            while (GameManager.Instance.GameTime < time.TotalSeconds)
                yield return null;

            action();
        }

        public static IEnumerator SpawnAndMaintain(
            ActorTypeEnum actorType,
            TimeSpan startTime,
            TimeSpan endTime,
            int startingCount,
            int endCount,
            int maxSpawnCountPerTick,
            float timeBetweenTicks,
            bool outsideScreen,
            SpawnDirection dir)
        {
            List<ActorBase> alive = new();

            var wait = new WaitForSeconds(timeBetweenTicks);

            float startTimeSec = (float)startTime.TotalSeconds;
            float endTimeSec = (float)endTime.TotalSeconds;

            while (GameManager.Instance.GameTime < startTimeSec)
                yield return null;

            void RemoveDead()
            {
                while (true)
                {
                    bool removed = false;
                    for (int i = 0; i < alive.Count; ++i)
                    {
                        if (alive[i].IsDead)
                        {
                            alive.RemoveAt(i);
                            removed = true;
                            break;
                        }
                    }

                    if (!removed)
                        break;
                }
            }

            float runTimeSec = (float)(endTime - startTime).TotalSeconds;
            float countToBeAdded = endCount - startingCount;

            while (GameManager.Instance.GameTime < endTimeSec)
            {
                float t = (GameManager.Instance.GameTime - startTimeSec) / runTimeSec;
                float currentMaintainCount = startingCount + t * countToBeAdded;

                if (alive.Count < currentMaintainCount)
                {
                    for (int i = 0; i < maxSpawnCountPerTick && alive.Count < currentMaintainCount; i++)
                    {
                        var spawn = ActorCache.Instance.GetActor(actorType);
                        float offset = 1.0f;
                        Vector3 pos;
                        if (outsideScreen)
                            pos = GetPointOutsideScreen(dir, offset);
                        else
                            pos = GetPointInsideArena(1.0f, 1.0f);

                        spawn.transform.position = pos;
                        spawn.SetActive(true);
                        alive.Add(spawn.GetComponent<ActorBase>());
                    }

                    RemoveDead();

                    if (timeBetweenTicks != 0.0)
                        yield return wait;
               }

                RemoveDead();
                GameManager.SetDebugOutput(actorType.ToString(), alive.Count);
                yield return null;
            }
        }

        public static IEnumerator Swarm(
            ActorTypeEnum actorType,
            TimeSpan startTime,
            TimeSpan endTime,
            int spawnCountPerTick,
            float timeBetweenTicks,
            bool outsideScreen,
            SpawnDirection dir)
        {
            var wait = new WaitForSeconds(timeBetweenTicks);

            float startTimeSec = (float)startTime.TotalSeconds;
            float endTimeSec = (float)endTime.TotalSeconds;

            while (GameManager.Instance.GameTime < startTimeSec)
            {
                yield return null;
            }

            while (GameManager.Instance.GameTime < endTimeSec)
            {
                for (int i = 0; i < spawnCountPerTick; i++)
                {
                    var spawn = ActorCache.Instance.GetActor(actorType);
                    float offset = 1.0f;
                    Vector3 pos;
                    if (outsideScreen)
                        pos = GetPointOutsideScreen(dir, offset);
                    else
                        pos = GetPointInsideArena(1.0f, 1.0f);

                    spawn.transform.position = pos;
                    spawn.SetActive(true);
                }

                yield return wait;
            }

            yield return null;
        }

        public static IEnumerator Single(
            ActorTypeEnum actorType,
            TimeSpan time,
            bool outsideScreen,
            SpawnDirection dir)
        {
            float startTimeSec = (float)time.TotalSeconds;

            while (GameManager.Instance.GameTime < startTimeSec)
                yield return null;

            var spawn = ActorCache.Instance.GetActor(actorType);
            float offset = 1.0f;
            Vector3 pos;
            if (outsideScreen)
                pos = GetPointOutsideScreen(dir, offset);
            else
                pos = GetPointInsideArena(1.0f, 1.0f);

            spawn.transform.position = pos;
            spawn.SetActive(true);
        }

        static readonly List<Vector2> posList = new ();

        public static IEnumerator SpawnFormation(
            ActorTypeEnum actorType,
            bool despawnAtDestination,
            bool breakFreeAtDamage,
            TimeSpan time,
            Vector2 fromPos,
            Vector2 target,
            ActorForcedTargetType targetType,
            int w,
            int h,
            float stepX = 1,
            float stepY = 1,
            float pivotX = 0.5f,
            float pivotY = 0.5f,
            float skewX = 0,
            float skewY = 0)
        {
            float startTimeSec = (float)time.TotalSeconds;

            while (GameManager.Instance.GameTime < startTimeSec)
                yield return null;

            FormationUtil.GetFormation(w, h, stepX, stepY, pivotX, pivotY, skewX, skewY, posList);

            for (int i = 0; i < posList.Count; ++i)
            {
                var spawn = ActorCache.Instance.GetActor(actorType);
                Vector2 startPos = fromPos + posList[i];
                Vector2 endPos = target + posList[i];
                spawn.transform.position = startPos;
                spawn.SetActive(true);
                spawn.GetComponent<ActorBase>().SetForcedTarget(endPos, despawnAtDestination, breakFreeAtDamage, targetType);
            }
        }
    }
}
