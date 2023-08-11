using UnityEngine;

static class DebugUtil
{
    public static void DrawRect(Rect r, Color col)
    {
        Debug.DrawLine(new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax), col);
        Debug.DrawLine(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin), col);
        Debug.DrawLine(new Vector2(r.xMin, r.yMax), new Vector2(r.xMin, r.yMin), col);
        Debug.DrawLine(new Vector2(r.xMax, r.yMax), new Vector2(r.xMax, r.yMin), col);
    }
}
