namespace Assets.Script.Misc
{
    public static class Format64
    {
        public static string Format(long number, bool abbreviate = true)
        {
            if (number < 1_000_000)
                return number.ToString("N0"); // e.g., 123,456

            if (number >= 1_000_000_000_000_000_000)
                return Format(number, 1_000_000_000_000_000_000D, abbreviate, "Qn", " quadrillion");

            if (number >= 1_000_000_000_000_000)
                return Format(number, 1_000_000_000_000_000D, abbreviate, "Q", " quadrillion");

            if (number >= 1_000_000_000_000)
                return Format(number, 1_000_000_000_000D, abbreviate, "T", " trillion");

            if (number >= 1_000_000_000)
                return Format(number, 1_000_000_000D, abbreviate, "B", " billion");

            return Format(number, 1_000_000D, abbreviate, "M", " million"); // 1,000,000 <= number < 1,000,000,000
        }

        private static string Format(long number, double divisor, bool abbreviate, string shortSuffix, string longSuffix)
        {
            double value = number / divisor;
            return value.ToString("0.000") + (abbreviate ? shortSuffix : longSuffix);
        }
    }
}
