using System.Numerics;

namespace Assets.Script.Misc
{
    public static class FormatScientific
    {
        public static string Format(Decimal512 number)
        {
            BigInteger rawValue = number.RawValue;
            BigInteger scaleFactor = BigInteger.Pow(10, 4);

            // Handle zero and negative cases with simple ToString()
            if (rawValue <= 0)
            {
                return number.ToString();
            }

            // For numbers < 10, use ToString()
            if (rawValue < 10 * scaleFactor)
            {
                return number.ToString();
            }

            // Everything else gets scientific notation
            return FormatAsScientific(rawValue, scaleFactor);
        }

        private static string FormatAsScientific(BigInteger rawValue, BigInteger scaleFactor)
        {
            // Get the whole number part and fractional part
            BigInteger wholePart = rawValue / scaleFactor;
            BigInteger fractionalPart = rawValue % scaleFactor;

            // Convert to strings
            string wholeStr = wholePart.ToString();
            string fractionalStr = fractionalPart.ToString().PadLeft(4, '0');

            // Combine into one string of significant digits
            string allDigits = wholeStr + fractionalStr;
            allDigits = allDigits.TrimEnd('0'); // Remove trailing zeros

            if (string.IsNullOrEmpty(allDigits))
            {
                allDigits = "0";
            }

            // Calculate exponent: original decimal position - 1 (scientific notation puts decimal after first digit)
            int exponent = wholeStr.Length - 1;

            // Create mantissa - always 3 decimal places
            string mantissa;
            if (allDigits.Length == 1)
            {
                mantissa = $"{allDigits}.000";
            }
            else
            {
                string decimalPart = allDigits.Substring(1);

                // Always pad or truncate to exactly 3 decimal places
                if (decimalPart.Length > 3)
                {
                    decimalPart = decimalPart.Substring(0, 3);
                }
                else
                {
                    decimalPart = decimalPart.PadRight(3, '0');
                }

                mantissa = $"{allDigits[0]}.{decimalPart}";
            }

            return $"{mantissa}e+{exponent}";
        }
    }
}
