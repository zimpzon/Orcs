using System;
using System.Globalization;
using UnityEngine;

namespace Assets.Script.Misc
{
    public static class FormatTime
    {
        public const string DateTimeSerializedFormat = "yyyy-MM-dd HH:mm:ss";

        public static string DateTimeToString(DateTime dateTime)
            => dateTime.ToString(DateTimeSerializedFormat);

        public static DateTime FromString(string s)
        {
            bool couldBeParsed = DateTime.TryParseExact(
                s,
                DateTimeSerializedFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsed);
            
            if (couldBeParsed)
            {
                return parsed;
            }
            else
            {
                Debug.Log($"could not parse DateTime: {s}");
                return DateTime.UtcNow;
            }
        }

        public static string Format(int days, int hours, int minutes, int? seconds = null, bool useShorthand = false)
        {
            if (useShorthand)
            {
                return seconds.HasValue ?
                    $"{days}D, {hours}H, {minutes}M, {seconds}S" :
                    $"{days}D, {hours}H, {minutes}M";
            }
            else
            {
                string dayText = days == 1 ? "day" : "days";
                string hourText = hours == 1 ? "hour" : "hours";
                string minuteText = minutes == 1 ? "minute" : "minutes";

                return seconds.HasValue ?
                    $"{days} {dayText}, {hours} {hourText}, {minutes} {minuteText}, {seconds} seconds" :
                    $"{days} {dayText}, {hours} {hourText}, {minutes} {minuteText}";
            }
        }

        // Example usage:
        // string sss1 = Format.FormatTimeSpan(200, 14, 55, useShorthand: false);
        // string sss2 = Format.FormatTimeSpan(200, 14, 55, useShorthand: true);
        // 
        // Output:
        // sss1: "200 days, 14 hours, 55 minutes"
        // sss2: "200D, 14H, 55M"
    }
}
