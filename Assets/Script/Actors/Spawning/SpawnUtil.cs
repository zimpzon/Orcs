using Assets.Script.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PositionUtility;

namespace Assets.Script.Actors.Spawning
{
    public static class SpawnUtil
    {
        public static IEnumerator Message(TimeSpan showTime, string text, Color? color = null, int? fontSize = null, float duration = 3)
        {
            while (G.D.GameTime < showTime.TotalSeconds)
                yield return null;

            if (G.D.GameTime > showTime.TotalSeconds + 5)
                yield break;

            var info = new GameInfo
            {
                Text = text,
                Duration = duration,
                FadeInDuration = 0.25f,
                FadeOutDuration = 2.0f,
                FontSize = fontSize,
                Color = color,
            };

            GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);
        }

        public static IEnumerator ActionAtTime(TimeSpan time, Action action)
        {
            while (G.D.GameTime < time.TotalSeconds)
                yield return null;

            action();
        }

        public static IEnumerable<ActorBase> Random(ActorTypeEnum actorType, int count)
        {
            var points = Enumerable.Range(0, count).Select(i => GetPointInsideArena(minX: 0.2f)).ToList();
            points = points.OrderBy(p => p.x).ToList();
            for (int i = 0; i < points.Count; i++)
            {
                var spawn = SpawnActor(actorType);
                Vector3 pos = points[i];
                spawn.transform.position = pos;
                yield return spawn.GetComponent<ActorBase>();
            }
        }

        public static IEnumerable<ActorBase> Single(ActorTypeEnum actorType, Vector2 pos)
        {
            var spawn = SpawnActor(actorType);
            spawn.transform.position = pos;
            yield return spawn.GetComponent<ActorBase>();
        }

        public static IEnumerable<ActorBase> Square(ActorTypeEnum actorType, Vector2 center, int size, float spacing)
        {
            // Calculate grid points around center
            var points = new List<Vector3>();

            int half = size / 2;

            for (int x = -half; x <= half; x++)
            {
                for (int y = -half; y <= half; y++)
                {
                    Vector3 point = new Vector3(center.x + x * spacing, center.y + y * spacing, 0f);
                    points.Add(point);
                }
            }

            points = points.OrderBy(p => p.x).ToList();

            foreach (var point in points)
            {
                var spawn = SpawnActor(actorType);
                spawn.transform.position = point;
                yield return spawn.GetComponent<ActorBase>();
            }
        }

        public static IEnumerable<ActorBase> Circle(ActorTypeEnum actorType, Vector2 center, float radius, int count)
        {
            // Calculate points in a circle around center
            var points = new List<Vector3>();

            for (int i = 0; i < count; i++)
            {
                float angle = i * (360f / count) * Mathf.Deg2Rad;
                Vector3 point = new Vector3(
                    center.x + radius * Mathf.Cos(angle),
                    center.y + radius * Mathf.Sin(angle),
                    0f
                );
                points.Add(point);
            }

            points = points.OrderBy(p => p.x).ToList();

            foreach (var point in points)
            {
                var spawn = SpawnActor(actorType);
                spawn.transform.position = point;
                yield return spawn.GetComponent<ActorBase>();
            }
        }

        static readonly List<Vector2> posList = new ();

        public static IEnumerable<ActorBase> SpawnFormation(
            ActorTypeEnum actorType,
            bool despawnAtDestination,
            bool breakFreeAtDamage,
            TimeSpan? time,
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
            if (time.HasValue)
            {
                float startTimeSec = (float)time.Value.TotalSeconds;

                while (GameManager.Instance.GameTime < startTimeSec)
                    yield return null;
            }

            FormationUtil.GetFormation(w, h, stepX, stepY, pivotX, pivotY, skewX, skewY, posList);

            for (int i = 0; i < posList.Count; ++i)
            {
                Vector2 startPos = fromPos + posList[i];
                Vector2 endPos = target + posList[i];
                var spawn = SpawnActor(actorType);
                spawn.transform.position = startPos;
                spawn.SetActive(true);
                spawn.GetComponent<ActorBase>().SetForcedTarget(endPos, despawnAtDestination, breakFreeAtDamage, targetType);
            }
        }

        private static GameObject SpawnActor(ActorTypeEnum actorType)
        {
            var spawn = ActorCache.Instance.GetActor(actorType);
            return spawn;
        }
    }
}
