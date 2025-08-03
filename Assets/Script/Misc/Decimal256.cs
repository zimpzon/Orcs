using System;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public struct Decimal256 : IComparable<Decimal256>, IEquatable<Decimal256>, ISerializationCallbackReceiver
{
    private static readonly BigInteger MaxValue = (BigInteger.One << 256) - 1;
    private static readonly BigInteger ScaleFactor = BigInteger.Pow(10, 4); // 4 decimal places

    [System.NonSerialized]
    private BigInteger rawValue;

    // For JsonUtility serialization - this will be included in JSON
    [SerializeField]
    private string serializedValue;

    public Decimal256(decimal value)
    {
        // Multiply first to preserve decimal places, then convert to BigInteger
        rawValue = new BigInteger(value * (decimal)ScaleFactor);
        ValidateRange(rawValue);
        serializedValue = rawValue.ToString();
    }

    public Decimal256(float value)
    {
        // Convert to decimal first, multiply to preserve precision, then convert to BigInteger
        decimal decimalValue = (decimal)value;
        rawValue = new BigInteger(decimalValue * (decimal)ScaleFactor);
        ValidateRange(rawValue);
        serializedValue = rawValue.ToString();
    }

    public Decimal256(double value)
    {
        // Convert to decimal first, multiply to preserve precision, then convert to BigInteger
        decimal decimalValue = (decimal)value;
        rawValue = new BigInteger(decimalValue * (decimal)ScaleFactor);
        ValidateRange(rawValue);
        serializedValue = rawValue.ToString();
    }

    private Decimal256(BigInteger raw)
    {
        ValidateRange(raw);
        rawValue = raw;
        serializedValue = raw.ToString();
    }

    // Static method to create from serialized string (used by parent class)
    public static Decimal256 FromSerializedString(string serialized)
    {
        if (string.IsNullOrEmpty(serialized))
            return new Decimal256(0m);

        var parsed = BigInteger.Parse(serialized);
        return new Decimal256(parsed);
    }

    // Unity serialization callbacks
    public void OnBeforeSerialize()
    {
        // Ensure serializedValue is up to date before serialization
        if (rawValue != default(BigInteger))
        {
            serializedValue = rawValue.ToString();
        }
    }

    public void OnAfterDeserialize()
    {
        // Restore rawValue from serialized string after deserialization
        if (!string.IsNullOrEmpty(serializedValue))
        {
            rawValue = BigInteger.Parse(serializedValue);
        }
        else
        {
            rawValue = BigInteger.Zero;
        }
    }

    public void Add(Decimal256 other)
    {
        var result = rawValue + other.rawValue;
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Subtract(Decimal256 other)
    {
        var result = rawValue - other.rawValue;
        if (result < 0)
            throw new OverflowException("Result below 0.");
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Multiply(Decimal256 other)
    {
        var result = rawValue * other.rawValue;
        // Since both values are scaled, we need to divide by the scale factor once
        result = result / ScaleFactor;
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Multiply(float factor)
    {
        // Convert float to decimal, multiply by scale factor to preserve precision, then multiply
        decimal scaledFactor = (decimal)factor * (decimal)ScaleFactor;
        var result = rawValue * new BigInteger(scaledFactor) / ScaleFactor;
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public static Decimal256 operator +(Decimal256 left, Decimal256 right)
    {
        var result = left.rawValue + right.rawValue;
        ValidateRange(result);
        return new Decimal256(result);
    }

    public static Decimal256 operator -(Decimal256 left, Decimal256 right)
    {
        var result = left.rawValue - right.rawValue;
        if (result < 0)
            throw new OverflowException("Result below 0.");
        return new Decimal256(result);
    }

    public static Decimal256 operator *(Decimal256 left, Decimal256 right)
    {
        var result = left.rawValue * right.rawValue;
        // Since both values are scaled, we need to divide by the scale factor once
        result = result / ScaleFactor;
        ValidateRange(result);
        return new Decimal256(result);
    }

    public static Decimal256 operator *(Decimal256 left, float right)
    {
        // Convert float to decimal, multiply by scale factor to preserve precision, then multiply
        decimal scaledRight = (decimal)right * (decimal)ScaleFactor;
        var result = left.rawValue * new BigInteger(scaledRight) / ScaleFactor;
        ValidateRange(result);
        return new Decimal256(result);
    }

    public static Decimal256 operator *(float left, Decimal256 right)
    {
        return right * left;
    }

    public static Decimal256 operator *(Decimal256 left, int right)
    {
        var result = left.rawValue * right;
        ValidateRange(result);
        return new Decimal256(result);
    }

    public static Decimal256 operator *(int left, Decimal256 right)
    {
        return right * left;
    }

    public static Decimal256 operator *(Decimal256 left, long right)
    {
        var result = left.rawValue * right;
        ValidateRange(result);
        return new Decimal256(result);
    }

    public static Decimal256 operator *(long left, Decimal256 right)
    {
        return right * left;
    }

    public static Decimal256 operator *(Decimal256 left, double right)
    {
        // Convert double to decimal, multiply by scale factor to preserve precision, then multiply
        decimal scaledRight = (decimal)right * (decimal)ScaleFactor;
        var result = left.rawValue * new BigInteger(scaledRight) / ScaleFactor;
        ValidateRange(result);
        return new Decimal256(result);
    }

    public static Decimal256 operator *(double left, Decimal256 right)
    {
        return right * left;
    }

    public BigInteger RawValue => rawValue;

    // Property to access the serialized value (useful for debugging)
    public string SerializedValue => serializedValue;

    public double ToDouble()
    {
        return (double)rawValue / (double)ScaleFactor;
    }

    public override string ToString()
    {
        BigInteger whole = rawValue / ScaleFactor;
        BigInteger fraction = rawValue % ScaleFactor;

        // Remove trailing zeros from fraction part
        string fractionStr = fraction.ToString().PadLeft(18, '0').TrimEnd('0');
        if (string.IsNullOrEmpty(fractionStr))
            return whole.ToString();

        return $"{whole}.{fractionStr}";
    }

    public string ToString(int decimalPlaces)
    {
        if (decimalPlaces < 0 || decimalPlaces > 18)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be between 0 and 18");

        BigInteger whole = rawValue / ScaleFactor;
        BigInteger fraction = rawValue % ScaleFactor;

        if (decimalPlaces == 0)
            return whole.ToString();

        BigInteger divisor = BigInteger.Pow(10, 18 - decimalPlaces);
        BigInteger roundedFraction = fraction / divisor;

        string fractionStr = roundedFraction.ToString().PadLeft(decimalPlaces, '0');
        return $"{whole}.{fractionStr}";
    }

    public static Decimal256 Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Input is null or empty.");

        input = input.Trim();
        string[] parts = input.Split('.');

        if (parts.Length > 2)
            throw new FormatException("Invalid decimal format.");

        string integerPart = parts[0];
        string fractionPart = parts.Length == 2 ? parts[1] : "0";

        if (!BigInteger.TryParse(integerPart, out BigInteger whole))
            throw new FormatException("Invalid integer part.");

        // Pad or truncate fraction part to 18 digits
        if (fractionPart.Length > 18)
            fractionPart = fractionPart.Substring(0, 18);
        else
            fractionPart = fractionPart.PadRight(18, '0');

        if (!BigInteger.TryParse(fractionPart, out BigInteger fraction))
            throw new FormatException("Invalid fraction part.");

        BigInteger scaled = whole * ScaleFactor + fraction;
        return new Decimal256(scaled);
    }

    private static void ValidateRange(BigInteger value)
    {
        if (value < 0 || value > MaxValue)
            throw new OverflowException("Value is outside 256-bit unsigned range.");
    }

    // --- Comparison operators ---
    public int CompareTo(Decimal256 other) => rawValue.CompareTo(other.rawValue);
    public bool Equals(Decimal256 other) => rawValue.Equals(other.rawValue);
    public override bool Equals(object obj) => obj is Decimal256 other && Equals(other);
    public override int GetHashCode() => rawValue.GetHashCode();

    public static bool operator ==(Decimal256 left, Decimal256 right) => left.rawValue == right.rawValue;
    public static bool operator !=(Decimal256 left, Decimal256 right) => left.rawValue != right.rawValue;
    public static bool operator <(Decimal256 left, Decimal256 right) => left.rawValue < right.rawValue;
    public static bool operator >(Decimal256 left, Decimal256 right) => left.rawValue > right.rawValue;
    public static bool operator <=(Decimal256 left, Decimal256 right) => left.rawValue <= right.rawValue;
    public static bool operator >=(Decimal256 left, Decimal256 right) => left.rawValue >= right.rawValue;

    // Implicit conversions
    public static implicit operator Decimal256(decimal d) => new Decimal256(d);
    public static implicit operator Decimal256(float f) => new Decimal256(f);
    public static implicit operator Decimal256(double d) => new Decimal256(d);
    public static implicit operator Decimal256(int i) => new Decimal256((decimal)i);
    public static implicit operator Decimal256(long l) => new Decimal256((decimal)l);
}