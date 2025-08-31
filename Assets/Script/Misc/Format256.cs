using System;
using System.Numerics;
using System.Globalization;

public static class Format512
{
    private static readonly (BigInteger Threshold, string Short, string Long)[] Suffixes =
    {
        (BigInteger.Pow(10, 63), "Vg", " vigintillion"),
        (BigInteger.Pow(10, 60), "Nv", " novemdecillion"),
        (BigInteger.Pow(10, 57), "Oc", " octodecillion"),
        (BigInteger.Pow(10, 54), "Sp", " septendecillion"),
        (BigInteger.Pow(10, 51), "Sx", " sexdecillion"),
        (BigInteger.Pow(10, 48), "Qi", " quindecillion"),
        (BigInteger.Pow(10, 45), "Qt", " quattuordecillion"),
        (BigInteger.Pow(10, 42), "Tr", " tredecillion"),
        (BigInteger.Pow(10, 39), "Dd", " duodecillion"),
        (BigInteger.Pow(10, 36), "Ud", " undecillion"),
        (BigInteger.Pow(10, 33), "Dc", " decillion"),
        (BigInteger.Pow(10, 30), "No", " nonillion"),
        (BigInteger.Pow(10, 27), "Oc", " octillion"),
        (BigInteger.Pow(10, 24), "Sp", " septillion"),
        (BigInteger.Pow(10, 21), "Sx", " sextillion"),
        (BigInteger.Pow(10, 18), "Qi", " quintillion"),
        (BigInteger.Pow(10, 15), "Qd", " quadrillion"),
        (BigInteger.Pow(10, 12), "T",  " trillion"),
        (BigInteger.Pow(10, 9),  "B",  " billion"),
        (BigInteger.Pow(10, 6),  "M",  " million")
    };

    public static string FormatWithDecimals(Decimal512 number, bool abbreviate = true, bool alwaysThreeDecimalsForLargeNumbers = false)
    {
        BigInteger rawValue = number.RawValue;
        BigInteger scaleFactor = BigInteger.Pow(10, 4);

        // For numbers < 1000, show with up to 1 decimal place (but not if decimal is 0)
        if (rawValue < 1000 * scaleFactor)
        {
            decimal decimalValue = (decimal)rawValue / (decimal)scaleFactor;

            if (decimalValue == Math.Floor(decimalValue))
            {
                return ((long)decimalValue).ToString();
            }
            else
            {
                return decimalValue.ToString("0.0", CultureInfo.InvariantCulture);
            }
        }

        // For numbers >= 1000, show with appropriate precision but no fractional decimals
        foreach (var (threshold, shortSuffix, longSuffix) in Suffixes)
        {
            BigInteger scaledThreshold = threshold * scaleFactor;
            if (rawValue >= scaledThreshold)
            {
                // Calculate the scaled down value
                decimal scaledValue = (decimal)(rawValue / (scaledThreshold / 1000)) / 1000m;

                // Format with precision but remove any fractional decimals
                string numberPart;
                if (scaledValue >= 100)
                {
                    numberPart = scaledValue.ToString("N3", CultureInfo.InvariantCulture);
                }
                else if (scaledValue >= 10)
                {
                    numberPart = scaledValue.ToString("N3", CultureInfo.InvariantCulture);
                    if (!alwaysThreeDecimalsForLargeNumbers)
                        numberPart = numberPart.TrimEnd('0').TrimEnd('.');
                }
                else
                {
                    numberPart = scaledValue.ToString("N3", CultureInfo.InvariantCulture);
                    if (!alwaysThreeDecimalsForLargeNumbers)
                        numberPart = numberPart.TrimEnd('0').TrimEnd('.');
                }

                return numberPart + (abbreviate ? shortSuffix : longSuffix);
            }
        }

        // For numbers >= 1000 but < 1M, format as whole number with thousands separators
        BigInteger wholeValue = rawValue / scaleFactor;
        return wholeValue.ToString("N0", CultureInfo.InvariantCulture);
    }

    public static string Format(Decimal512 number, bool abbreviate = true)
    {
        BigInteger rawValue = number.RawValue;
        BigInteger scaleFactor = BigInteger.Pow(10, 4);

        // For numbers < 1000, always show as whole number
        if (rawValue < 1000 * scaleFactor)
        {
            decimal decimalValue = (decimal)rawValue / (decimal)scaleFactor;
            return ((long)Math.Round(decimalValue)).ToString();
        }

        // For numbers >= 1M, show with thousands separator but no fractional decimals
        foreach (var (threshold, shortSuffix, longSuffix) in Suffixes)
        {
            BigInteger scaledThreshold = threshold * scaleFactor;
            if (rawValue >= scaledThreshold)
            {
                // Calculate the scaled down value
                decimal scaledValue = (decimal)(rawValue / (scaledThreshold / 1000)) / 1000m;

                // Format with thousands separator but remove any fractional decimals
                string numberPart;
                if (scaledValue >= 100)
                {
                    numberPart = scaledValue.ToString("N3", CultureInfo.InvariantCulture);
                }
                else if (scaledValue >= 10)
                {
                    numberPart = scaledValue.ToString("N3", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
                }
                else
                {
                    numberPart = scaledValue.ToString("N3", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
                }

                return numberPart + (abbreviate ? shortSuffix : longSuffix);
            }
        }

        // For numbers >= 1000 but < 1M, format as whole number with thousands separators
        BigInteger wholeValue = rawValue / scaleFactor;
        return wholeValue.ToString("N0", CultureInfo.InvariantCulture);
    }
}