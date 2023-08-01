﻿using Assets.Script.Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CoResult<T>
{
    public T Value = default;
}

public static class PositionUtility
{
    public enum SpawnDirection { Top, Bottom, Left, Right, TopOrBottom, LeftOrRight, Inside, Any };
    public enum GroupFormation { Circle, Line };

    public static SpawnDirection GetRandomDirOutside()
    {
        return (SpawnDirection)UnityEngine.Random.Range(0, (int)SpawnDirection.Inside); // TODO PEE: Not pretty. Pick one BEFORE .Inside.
    }

    public static SpawnDirection GetRandomDirAny()
    {
        return (SpawnDirection)UnityEngine.Random.Range(0, (int)SpawnDirection.Any); // TODO PEE: Not pretty. Pick one BEFORE .Any.
    }

    public static Vector3 GetPointInsideArena(float maxOffsetX = 1.0f, float maxOffsetY = 1.0f)
    {
        Vector3 point = Vector3.zero;
        for (int i = 0; i < 5; ++i)
        {
            float x = UnityEngine.Random.Range(-0.4f, 0.4f) * maxOffsetX;
            float y = UnityEngine.Random.Range(-0.4f, 0.4f) * maxOffsetY;
            Rect scr = AspectUtility.screenRelativeRect;
            point = new Vector3(scr.width * x, scr.height * y, 0.0f);
            bool notOnTopOfPlayer = Vector3.Distance(point, GameManager.Instance.PlayerTrans.position) > 2.0f;
            if (notOnTopOfPlayer)
                break;
        }
        return point;
    }

    public static Vector3 GetPointInsideScreen(float maxOffsetX, float maxOffsetY)
    {
        Vector3 point = Vector3.zero;
        for (int i = 0; i < 5; ++i)
        {
            float x = UnityEngine.Random.Range(-0.5f, 0.5f) * maxOffsetX;
            float y = UnityEngine.Random.Range(-0.5f, 0.5f) * maxOffsetY;
            Rect scr = AspectUtility.screenRelativeRect;
            point = new Vector3(scr.width * x, scr.height * y, 0.0f);
            bool notOnTopOfPlayer = Vector3.Distance(point, GameManager.Instance.PlayerTrans.position) > 2.0f;
            if (notOnTopOfPlayer)
                break;
        }
        return point;
    }

    public static Vector3 GetPointOutsideScreen(SpawnDirection dir, float offset, float maxDistFromCenter = 1.0f)
    {
        if (dir == SpawnDirection.Any)
        {
            dir = GetRandomDirAny();
        }

        if (dir == SpawnDirection.LeftOrRight)
        {
            dir = UnityEngine.Random.value < 0.5f ? SpawnDirection.Left : SpawnDirection.Right;
        }
        else if (dir == SpawnDirection.TopOrBottom)
        {
            dir = UnityEngine.Random.value < 0.5f ? SpawnDirection.Top : SpawnDirection.Bottom;
        }

        Rect scr = AspectUtility.screenRelativeRect;
        float x = 0.0f; float y = 0.0f; float offsetX = 0.0f; float offsetY = 0.0f;
        float rnd = UnityEngine.Random.Range(-0.55f, 0.55f) * maxDistFromCenter;
        switch (dir)
        {
            case SpawnDirection.Top:     x = 0.5f + rnd; y = 1.0f; offsetY =  offset; break;
            case SpawnDirection.Bottom:  x = 0.5f + rnd; y = 0.0f; offsetY = -offset; break;
            case SpawnDirection.Left:    x = 0.0f; y = 0.5f + rnd; offsetX = -offset; break;
            case SpawnDirection.Right:   x = 1.0f; y = 0.5f + rnd; offsetX =  offset; break;
        }

        Vector3 point = new Vector3(scr.xMin + scr.width * x + offsetX, scr.yMin + scr.height * y + offsetY, 0.0f);
        return point;
    }

    public const float PI2 = 3.14159265f * 2;

    public static IEnumerator SpawnAndMaintain(
        ActorTypeEnum actorType,
        TimeSpan startTime,
        TimeSpan endTime,
        int maintainCount,
        float maintainCountIncreasePerSec,
        int spawnCountPerTick,
        float timeBetweenTicks,
        bool outsideScreen,
        SpawnDirection dir)
    {
        List<ActorBase> alive = new ();

        var wait = new WaitForSeconds(timeBetweenTicks);

        float startTimeSec = (float)startTime.TotalSeconds;
        float endTimeSec = (float)endTime.TotalSeconds;

        while (GameManager.Instance.GameTime < startTimeSec)
        {
            yield return null;
        }

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

        float extraSpawns = 0;

        while (GameManager.Instance.GameTime < endTimeSec)
        {
            extraSpawns += maintainCountIncreasePerSec * Time.deltaTime;

            if (alive.Count < maintainCount + extraSpawns)
            {
                for (int i = 0; i < spawnCountPerTick && alive.Count < maintainCount + extraSpawns; i++)
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
            yield return null;
        }
    }

    public static IEnumerator SpawnGroup(
        ActorTypeEnum actorType,
        int count,
        float timeBetweenEntities,
        bool outsideScreen,
        SpawnDirection dir)
    {
        for (int entity = 0; entity < count; ++entity)
        {
            var spawn = ActorCache.Instance.GetActor(actorType);
            float offset = 1.0f;
            Vector3 pos;
            if (outsideScreen)
                pos = GetPointOutsideScreen(dir, offset, UnityEngine.Random.value * 0.5f);
            else
                pos = GetPointInsideArena(1.0f, 1.0f);

            spawn.transform.position = pos;
            spawn.SetActive(true);
            if (timeBetweenEntities != 0.0)
                yield return new WaitForSeconds(timeBetweenEntities);
        }
    }

    public static void GetCircleEdge(int count, float radius, Vector3 center, List<Vector3> list)
    {
        list.Clear();
        float step = PI2 / count;
        for (int i = 0; i < count; ++i)
        {
            Vector3 p = center;
            p.x += Mathf.Sin(i * step) * radius;
            p.y += Mathf.Cos(i * step) * radius;
            list.Add(p);
        }
    }

    public static void GetRandomInsideCircle(int count, float radius, Vector3 center, List<Vector3> list)
    {
        list.Clear();
        for (int i = 0; i < count; ++i)
        {
            Vector3 p = center;
            Vector2 rnd = RndUtil.RandomInsideUnitCircle() * radius;
            p.x += rnd.x;
            p.y += rnd.y;
            list.Add(p);
        }
    }
}
