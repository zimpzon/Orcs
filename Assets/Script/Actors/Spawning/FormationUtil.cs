using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Actors.Spawning
{
    class FormationUtil
    {
        public static void GetFormation(
            int w,
            int h,
            float stepX,
            float stepY,
            float pivotX,
            float pivotY,
            float skewX,
            float skewY,
            List<Vector2> list)
        {
            list.Clear();

            float right = w * stepX + skewX * h;
            float bottom = h * stepY + skewY * w;
            float centerX = right * pivotX;
            float centerY = bottom * pivotY;

            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w; ++x)
                {
                    list.Add(new Vector2(x * stepX - centerX + (skewX * y), y * stepY - centerY + (skewY * x)));
                }
            }
        }
    }
}
