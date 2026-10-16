using System;
using System.Collections.Generic;
using System.Globalization;

namespace Assets.Script.Misc
{
    public static class Format64
    {
        public static string Format(long number, bool abbreviate = true, bool alwaysThreeDecimalsForLargeNumbers = false)
        {
            if (number < 0)
            {
                // long.MinValue can't be negated; it's far out of any real range here anyway.
                return number == long.MinValue ? number.ToString() : "-" + Format(-number, abbreviate, alwaysThreeDecimalsForLargeNumbers);
            }

            if (number < 1000)
                return number.ToString(); // No formatting for numbers under 1000

            if (number < 1_000_000)
                return FormatThousands(number, abbreviate, alwaysThreeDecimalsForLargeNumbers);

            if (number >= 1_000_000_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000_000_000D, abbreviate, "Qi", " quintillion", alwaysThreeDecimalsForLargeNumbers); // same suffixes as Format512
            if (number >= 1_000_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000_000D, abbreviate, "Qd", " quadrillion", alwaysThreeDecimalsForLargeNumbers);
            if (number >= 1_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000D, abbreviate, "T", " trillion", alwaysThreeDecimalsForLargeNumbers);
            if (number >= 1_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000D, abbreviate, "B", " billion", alwaysThreeDecimalsForLargeNumbers);

            return FormatLargeNumber(number, 1_000_000D, abbreviate, "M", " million", alwaysThreeDecimalsForLargeNumbers);
        }

        // Below 10,000 plain (6095), then full with the player's thousands separator (10.000 / 10,000) below a million,
        // abbreviated (1,5M) above.
        public static string FormatGrouped(long number)
        {
            if (number > -10_000 && number < 10_000)
                return number.ToString();
            return number > -1_000_000 && number < 1_000_000 ? number.ToString("#,0", DisplayNumberFormat.Info) : Format(number);
        }

        private static string FormatThousands(long number, bool abbreviate, bool alwaysThreeDecimalsForLargeNumbers)
        {
            double value = Truncate3(number / 1000.0);

            // Remove trailing zeros from decimal representation
            string formatted = alwaysThreeDecimalsForLargeNumbers ? value.ToString("N3", DisplayNumberFormat.Info) : value.ToString("0.###", DisplayNumberFormat.Info);

            return formatted + (abbreviate ? "K" : " thousand");
        }

        // Cut to 3 decimals instead of letting the format string round: rounding turned 999,999,999 into "1000M" (same
        // at every suffix boundary from M up). The epsilon keeps e.g. 1.23 (1229.9999... x 1000) at 1.23.
        private static double Truncate3(double value) => Math.Floor(value * 1000.0 + 1e-6) / 1000.0;

        private static string FormatLargeNumber(long number, double divisor, bool abbreviate, string shortSuffix, string longSuffix, bool alwaysThreeDecimalsForLargeNumbers)
        {
            double value = Truncate3(number / divisor);

            // Remove trailing zeros from decimal representation
            string formatted = alwaysThreeDecimalsForLargeNumbers ? value.ToString("N3", DisplayNumberFormat.Info) : value.ToString("0.###", DisplayNumberFormat.Info);

            return formatted + (abbreviate ? shortSuffix : longSuffix);
        }
    }

    public class Format
    {
        public static string FormatTimeShort(TimeSpan ts)
        {
            // Handle very long times
            if (ts.TotalDays > 9999)
                return "forget it";

            // Format based on magnitude
            if (ts.TotalDays >= 1)
            {
                if (ts.TotalDays >= 365)
                    return $"{(int)(ts.TotalDays / 365)}y {ts.Days % 365}d";
                return $"{ts.Days}d {ts.Hours:D2}h {ts.Minutes:D2}m";
            }
            else if (ts.TotalHours >= 1)
            {
                return $"{ts.Hours}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
            }
            else if (ts.TotalMinutes >= 1)
            {
                return $"{ts.Minutes}m {ts.Seconds:D2}s";
            }
            else
            {
                return $"{ts.Seconds}s";
            }
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            var parts = new List<string>();

            if (timeSpan.Days > 0)
            {
                parts.Add($"{timeSpan.Days} {(timeSpan.Days == 1 ? "day" : "days")}");
                if (timeSpan.Hours > 0)
                    parts.Add($"{timeSpan.Hours} {(timeSpan.Hours == 1 ? "hour" : "hours")}");
            }
            else if (timeSpan.Hours > 0)
            {
                parts.Add($"{timeSpan.Hours} {(timeSpan.Hours == 1 ? "hour" : "hours")}");
                if (timeSpan.Minutes > 0)
                    parts.Add($"{timeSpan.Minutes} {(timeSpan.Minutes == 1 ? "minute" : "minutes")}");
            }
            else if (timeSpan.Minutes > 0)
            {
                parts.Add($"{timeSpan.Minutes} {(timeSpan.Minutes == 1 ? "minute" : "minutes")}");
                if (timeSpan.Seconds > 0)
                    parts.Add($"{timeSpan.Seconds} {(timeSpan.Seconds == 1 ? "second" : "seconds")}");
            }
            else
            {
                parts.Add($"{timeSpan.Seconds} {(timeSpan.Seconds == 1 ? "second" : "seconds")}");
            }

            return string.Join(", ", parts);
        }
    }
}
