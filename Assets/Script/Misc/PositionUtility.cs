using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CoResult<T>
{
    public T Value = default(T);
}

static class PositionUtility
{
    public enum SpawnDirection { Top, Bottom, Left, Right, TopOrBottom, LeftOrRight, Any, Last };
    public enum GroupFormation { Circle, Line };

    public static Vector3 GetPointInsideArena(float maxOffsetX, float maxOffsetY)
    {
        float x = Random.Range(-0.45f, 0.45f) * maxOffsetX;
        float y = Random.Range(-0.45f, 0.45f) * maxOffsetY;
        Rect scr = AspectUtility.screenRelativeRect;
        Vector3 point = new Vector3(scr.width * x, scr.height * y, 0.0f);
        return point;
    }

    public static Vector3 GetPointInsideScreen(float maxOffsetX, float maxOffsetY)
    {
        float x = Random.Range(-0.5f, 0.5f) * maxOffsetX;
        float y = Random.Range(-0.5f, 0.5f) * maxOffsetY;
        Rect scr = AspectUtility.screenRelativeRect;
        Vector3 point = new Vector3(scr.width * x, scr.height * y, 0.0f);
        return point;
    }

    public static Vector3 GetPointOutsideScreen(SpawnDirection dir, float offset, float maxDistFromCenter)
    {
        if (dir == SpawnDirection.Any)
        {
            dir = (SpawnDirection)Random.Range(0, 6); // All but any
        }

        if (dir == SpawnDirection.LeftOrRight)
        {
            dir = Random.value < 0.5f ? SpawnDirection.Left : SpawnDirection.Right;
        }
        else if (dir == SpawnDirection.TopOrBottom)
        {
            dir = Random.value < 0.5f ? SpawnDirection.Top : SpawnDirection.Bottom;
        }

        Rect scr = AspectUtility.screenRelativeRect;
        float x = 0.0f; float y = 0.0f; float offsetX = 0.0f; float offsetY = 0.0f;
        float rnd = Random.Range(-1.0f, 1.0f) * maxDistFromCenter;
        switch (dir)
        {
            case SpawnDirection.Top:     x = 0.5f + rnd; y = 0.0f; offsetY = -offset; break;
            case SpawnDirection.Bottom:  x = 0.5f + rnd; y = 1.0f; offsetY =  offset; break;
            case SpawnDirection.Left:    x = 0.0f; y = 0.5f + rnd; offsetX = -offset; break;
            case SpawnDirection.Right:   x = 1.0f; y = 0.5f + rnd; offsetX =  offset; break;
        }

        Vector3 point = new Vector3(scr.xMin + scr.width * x + offsetX, scr.yMin + scr.height * y + offsetY, 0.0f);
        return point;
    }

    public const float PI2 = 3.14159265f * 2;

    public static IEnumerator SpawnGroup(
        GameObject proto,
        int count,
        float timeBetweenEntities,
        bool outsideScreen,
        SpawnDirection dir)
    {
        for (int entity = 0; entity < count; ++entity)
        {
            var spawn = GameObject.Instantiate<GameObject>(proto);
            float offset = 2.0f;
            Vector3 pos;
            if (outsideScreen)
                pos = GetPointOutsideScreen(dir, offset, Random.value * 0.5f);
            else
                pos = GetPointInsideArena(1.0f, 1.0f);

            spawn.transform.position = pos;
            if (timeBetweenEntities != 0.0)
                yield return new WaitForSeconds(timeBetweenEntities);
        }
    }

    public static IEnumerator SpawnList(GameObject proto, List<Vector3> list, float delayBetween)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            var spawn = GameObject.Instantiate<GameObject>(proto);
            spawn.transform.position = list[i];
            if (delayBetween != 0.0)
                yield return new WaitForSeconds(delayBetween);
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
            Vector2 rnd = Random.insideUnitCircle * radius;
            p.x += rnd.x;
            p.y += rnd.y;
            list.Add(p);
        }
    }
}
