using System;
using UnityEngine;

namespace Assets.Script.Misc
{
    public static class MathUtil
    {
        public static string FormatLongNumber(long number)
        {
            if (number >= 1_000_000_000_000_000_000) return (number / 1_000_000_000_000_000_000D).ToString("0.00") + "Qn"; // Quintillion
            if (number >= 1_000_000_000_000_000) return (number / 1_000_000_000_000_000D).ToString("0.00") + "Q";  // Quadrillion
            if (number >= 1_000_000_000_000) return (number / 1_000_000_000_000D).ToString("0.00") + "T";  // Trillion
            if (number >= 1_000_000_000) return (number / 1_000_000_000D).ToString("0.00") + "B";  // Billion
            if (number >= 1_000_000) return (number / 1_000_000D).ToString("0.00") + "M";  // Million
            if (number >= 1_000) return (number / 1_000D).ToString("0.00") + "K";  // Thousand
            return number.ToString();
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
    }
}
