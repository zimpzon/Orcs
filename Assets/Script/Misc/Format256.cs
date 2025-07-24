using System;
using System.Numerics;
using System.Globalization;

public static class Format256
{
    private static readonly BigInteger ScaleFactor = BigInteger.Pow(10, 18); // Match Decimal256's scale factor

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

    public static string Format(Decimal256 number, bool abbreviate = true)
    {
        BigInteger raw = number.RawValue; // 18 decimal precision
        BigInteger whole = raw / ScaleFactor;
        BigInteger frac = raw % ScaleFactor;

        foreach (var (threshold, shortSuffix, longSuffix) in Suffixes)
        {
            if (whole >= threshold)
            {
                // Calculate the scaled value for display
                decimal value = (decimal)raw / (decimal)(threshold * ScaleFactor);

                if (value < 100)
                    return value.ToString("0.0", CultureInfo.InvariantCulture) + (abbreviate ? shortSuffix : longSuffix);
                else
                    return Math.Round(value).ToString("N0", CultureInfo.InvariantCulture) + (abbreviate ? shortSuffix : longSuffix);
            }
        }

        if (whole < 1000)
        {
            // Show decimal places only if there are meaningful fractional digits
            string fracStr = frac.ToString().PadLeft(18, '0').TrimEnd('0');
            if (string.IsNullOrEmpty(fracStr))
                return whole.ToString();

            // Show only the first few decimal places for readability
            if (fracStr.Length > 2)
                fracStr = fracStr.Substring(0, 1);

            return $"{whole}.{fracStr}";
        }
        else
        {
            return whole.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
}