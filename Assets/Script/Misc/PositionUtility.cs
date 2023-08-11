using System.Collections.Generic;
using UnityEngine;

class CoResult<T>
{
    public T Value = default;
}

public static class PositionUtility
{
    public static Vector2 TopMidOut = new(0, GameManager.ArenaBounds.yMax + 2);
    public static Vector2 BottomMidOut = new(0, GameManager.ArenaBounds.yMin - 2);
    public static Vector2 LeftMidOut = new(GameManager.ArenaBounds.xMin - 2, 0);
    public static Vector2 RightMidOut = new(GameManager.ArenaBounds.xMax + 2, 0);

    public static Vector2 TopLeftOut = new(GameManager.ArenaBounds.xMin - 2, GameManager.ArenaBounds.yMin + 2);
    public static Vector2 TopRightOut = new(GameManager.ArenaBounds.xMax + 2, GameManager.ArenaBounds.yMin + 2);
    public static Vector2 BottomLeftOut = new(GameManager.ArenaBounds.xMin - 2, GameManager.ArenaBounds.yMin - 2);
    public static Vector2 BottomRightOut = new(GameManager.ArenaBounds.xMax + 2, GameManager.ArenaBounds.yMin - 2);

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

    public static Vector2 GetPointInsideRect(Rect r)
    {
        float x = Random.Range(r.xMin + 0.5f, r.xMax - 0.5f);
        float y = Random.Range(r.yMax - 0.5f, r.yMin + 0.5f);
        return new Vector2(x, y);
    }

    public static Vector3 GetPointInsideArena(float maxOffsetX = 1.0f, float maxOffsetY = 1.0f)
    {
        Vector3 point = Vector3.zero;
        for (int i = 0; i < 5; ++i)
        {
            float x = Random.Range(-0.4f, 0.4f) * maxOffsetX;
            float y = Random.Range(-0.4f, 0.4f) * maxOffsetY;
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
            float x = Random.Range(-0.5f, 0.5f) * maxOffsetX;
            float y = Random.Range(-0.5f, 0.5f) * maxOffsetY;
            Rect scr = AspectUtility.screenRelativeRect;
            point = new Vector3(scr.width * x, scr.height * y, 0.0f);
            bool notOnTopOfPlayer = Vector3.Distance(point, GameManager.Instance.PlayerTrans.position) > 2.0f;
            if (notOnTopOfPlayer)
                break;
        }
        return point;
    }

    public static Vector3 GetClosePointOutsideScreen(Vector3 fromPos)
    {
        SpawnDirection dir;

        bool xLeft = fromPos.x < 0.5f;
        bool yTop = fromPos.y < 0.5f;
        if (xLeft && yTop)
        {
            dir = fromPos.x < fromPos.y ? SpawnDirection.Left : SpawnDirection.Top;
        }
        else if (!xLeft && yTop)
        {
            dir = fromPos.x > fromPos.y ? SpawnDirection.Right : SpawnDirection.Top;
        }
        else if (xLeft)
        {
            dir = fromPos.x < fromPos.y ? SpawnDirection.Left : SpawnDirection.Bottom;
        }
        else
        {
            dir = fromPos.x > fromPos.y ? SpawnDirection.Right : SpawnDirection.Bottom;
        }

        return GetPointOutsideScreen(dir, offset: 1.0f, maxDistFromCenter: 3.0f);
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
