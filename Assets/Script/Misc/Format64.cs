using System.Globalization;

namespace Assets.Script.Misc
{
    public static class Format64
    {
        public static string Format(long number, bool abbreviate = true, bool alwaysThreeDecimalsForLargeNumbers = false)
        {
            if (number < 1000)
                return number.ToString(); // No formatting for numbers under 1000

            if (number < 1_000_000)
                return FormatThousands(number, abbreviate, alwaysThreeDecimalsForLargeNumbers);

            if (number >= 1_000_000_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000_000_000D, abbreviate, "Qn", " quintillion", alwaysThreeDecimalsForLargeNumbers);
            if (number >= 1_000_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000_000D, abbreviate, "Q", " quadrillion", alwaysThreeDecimalsForLargeNumbers);
            if (number >= 1_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000D, abbreviate, "T", " trillion", alwaysThreeDecimalsForLargeNumbers);
            if (number >= 1_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000D, abbreviate, "B", " billion", alwaysThreeDecimalsForLargeNumbers);

            return FormatLargeNumber(number, 1_000_000D, abbreviate, "M", " million", alwaysThreeDecimalsForLargeNumbers);
        }

        private static string FormatThousands(long number, bool abbreviate, bool alwaysThreeDecimalsForLargeNumbers)
        {
            double value = number / 1000.0;

            // Remove trailing zeros from decimal representation
            string formatted = alwaysThreeDecimalsForLargeNumbers ? value.ToString("N3", CultureInfo.InvariantCulture) : value.ToString("0.###", CultureInfo.InvariantCulture);

            return formatted + (abbreviate ? "K" : " thousand");
        }

        private static string FormatLargeNumber(long number, double divisor, bool abbreviate, string shortSuffix, string longSuffix, bool alwaysThreeDecimalsForLargeNumbers)
        {
            double value = number / divisor;

            // Remove trailing zeros from decimal representation
            string formatted = alwaysThreeDecimalsForLargeNumbers ? value.ToString("N3", CultureInfo.InvariantCulture) : value.ToString("0.###", CultureInfo.InvariantCulture);

            return formatted + (abbreviate ? shortSuffix : longSuffix);
        }
    }
}
