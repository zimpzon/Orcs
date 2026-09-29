using System.Globalization;
using UnityEngine;

namespace Assets.Script.Misc
{
    // Number format for displaying numbers in the UI: invariant formatting, but with the player's own
    // decimal and thousands separators (e.g. 1.234,5 for Danish/German, 1,234.5 for English).
    // Only affects display - the thread culture stays en-US (GameManager.Start) so saves/parsing are unaffected.
    public static class DisplayNumberFormat
    {
        public static NumberFormatInfo Info { get; private set; } = CultureInfo.InvariantCulture.NumberFormat;
        public static string DecimalSeparator => Info.NumberDecimalSeparator;

        // Runs before any Awake/Start, i.e. before GameManager forces the thread culture to en-US.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void CaptureOsCulture()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            NumberFormatInfo os = CultureInfo.CurrentCulture.NumberFormat;
            var info = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            info.NumberDecimalSeparator = os.NumberDecimalSeparator;
            // Some cultures group with (narrow) no-break spaces which the game fonts may not have.
            info.NumberGroupSeparator = os.NumberGroupSeparator.Replace(' ', ' ').Replace(' ', ' ');
            Info = info;
#endif
        }

        // "12.500" -> "12.5", "10.000" -> "10" (using the display decimal separator).
        public static string TrimTrailingDecimals(string formatted)
        {
            string sep = DecimalSeparator;
            if (!formatted.Contains(sep))
                return formatted;

            formatted = formatted.TrimEnd('0');
            if (formatted.EndsWith(sep))
                formatted = formatted.Substring(0, formatted.Length - sep.Length);

            return formatted;
        }
    }
}
