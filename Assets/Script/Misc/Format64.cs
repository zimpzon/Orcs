namespace Assets.Script.Misc
{
    public static class Format64
    {
        public static string Format(long number, bool abbreviate = true)
        {
            if (number < 1000)
                return number.ToString(); // No formatting for numbers under 1000

            if (number < 1_000_000)
                return FormatThousands(number, abbreviate);

            if (number >= 1_000_000_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000_000_000D, abbreviate, "Qn", " quintillion");
            if (number >= 1_000_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000_000D, abbreviate, "Q", " quadrillion");
            if (number >= 1_000_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000_000D, abbreviate, "T", " trillion");
            if (number >= 1_000_000_000)
                return FormatLargeNumber(number, 1_000_000_000D, abbreviate, "B", " billion");

            return FormatLargeNumber(number, 1_000_000D, abbreviate, "M", " million");
        }

        private static string FormatThousands(long number, bool abbreviate)
        {
            double value = number / 1000.0;

            // Remove trailing zeros from decimal representation
            string formatted = value.ToString("0.###");

            return formatted + (abbreviate ? "K" : " thousand");
        }

        private static string FormatLargeNumber(long number, double divisor, bool abbreviate, string shortSuffix, string longSuffix)
        {
            double value = number / divisor;

            // Remove trailing zeros from decimal representation
            string formatted = value.ToString("0.###");

            return formatted + (abbreviate ? shortSuffix : longSuffix);
        }
    }
}
