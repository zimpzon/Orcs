using System;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public struct Decimal512 : IComparable<Decimal512>, IEquatable<Decimal512>, ISerializationCallbackReceiver
{
    private static readonly BigInteger MaxValue = (BigInteger.One << 512) - 1;
    private static readonly double ScaleFactorDouble = Math.Pow(10, 4); // 4 decimal places as double
    private static readonly BigInteger ScaleFactor = new BigInteger(ScaleFactorDouble); // BigInteger version for calculations

    [System.NonSerialized]
    private BigInteger rawValue;

    // For JsonUtility serialization - this will be included in JSON
    [SerializeField]
    private string serializedValue;

    public Decimal512(double value)
    {
        // Scale the double value directly
        rawValue = new BigInteger(value * ScaleFactorDouble);
        ValidateRange(rawValue);
        serializedValue = rawValue.ToString();
    }

    private Decimal512(BigInteger raw)
    {
        ValidateRange(raw);
        rawValue = raw;
        serializedValue = raw.ToString();
    }

    // Static method to create from serialized string (used by parent class)
    public static Decimal512 FromSerializedString(string serialized)
    {
        if (string.IsNullOrEmpty(serialized))
            return new Decimal512(0.0);

        var parsed = BigInteger.Parse(serialized);
        return new Decimal512(parsed);
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

    public void Add(Decimal512 other)
    {
        var result = rawValue + other.rawValue;
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Subtract(Decimal512 other)
    {
        var result = rawValue - other.rawValue;
        if (result < 0)
            throw new OverflowException("Result below 0.");
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Multiply(Decimal512 other)
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
        // Use double for the multiplication to avoid overflow
        double scaledFactor = (double)factor * ScaleFactorDouble;
        var result = rawValue * new BigInteger(scaledFactor) / ScaleFactor;
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public static Decimal512 operator +(Decimal512 left, Decimal512 right)
    {
        var result = left.rawValue + right.rawValue;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator -(Decimal512 left, Decimal512 right)
    {
        var result = left.rawValue - right.rawValue;
        if (result < 0)
            throw new OverflowException("Result below 0.");
        return new Decimal512(result);
    }

    public static Decimal512 operator *(Decimal512 left, Decimal512 right)
    {
        var result = left.rawValue * right.rawValue;
        // Since both values are scaled, we need to divide by the scale factor once
        result = result / ScaleFactor;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator *(Decimal512 left, float right)
    {
        // Use double for the multiplication to avoid overflow
        double scaledRight = (double)right * ScaleFactorDouble;
        var result = left.rawValue * new BigInteger(scaledRight) / ScaleFactor;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator *(float left, Decimal512 right)
    {
        return right * left;
    }

    public static Decimal512 operator *(Decimal512 left, int right)
    {
        var result = left.rawValue * right;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator *(int left, Decimal512 right)
    {
        return right * left;
    }

    public static Decimal512 operator *(Decimal512 left, long right)
    {
        var result = left.rawValue * right;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator *(long left, Decimal512 right)
    {
        return right * left;
    }

    public static Decimal512 operator *(Decimal512 left, double right)
    {
        // Use double arithmetic throughout to avoid overflow
        double scaledRight = right * ScaleFactorDouble;
        var result = left.rawValue * new BigInteger(scaledRight) / ScaleFactor;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator *(double left, Decimal512 right)
    {
        return right * left;
    }

    // Add these methods to your Decimal512 struct

    public void Divide(Decimal512 other)
    {
        if (other.rawValue == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        // Scale up the dividend to maintain precision
        var result = (rawValue * ScaleFactor) / other.rawValue;
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Divide(float divisor)
    {
        if (divisor == 0.0f)
            throw new DivideByZeroException("Cannot divide by zero.");

        // Convert float to scaled BigInteger
        double scaledDivisor = (double)divisor * ScaleFactorDouble;
        var result = (rawValue * ScaleFactor) / new BigInteger(scaledDivisor);
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    public void Divide(double divisor)
    {
        if (divisor == 0.0)
            throw new DivideByZeroException("Cannot divide by zero.");

        // Convert double to scaled BigInteger
        double scaledDivisor = divisor * ScaleFactorDouble;
        var result = (rawValue * ScaleFactor) / new BigInteger(scaledDivisor);
        ValidateRange(result);
        rawValue = result;
        serializedValue = result.ToString();
    }

    // Division operators
    public static Decimal512 operator /(Decimal512 left, Decimal512 right)
    {
        if (right.rawValue == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        // Scale up the dividend to maintain precision
        var result = (left.rawValue * ScaleFactor) / right.rawValue;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator /(Decimal512 left, float right)
    {
        if (right == 0.0f)
            throw new DivideByZeroException("Cannot divide by zero.");

        // Convert float to scaled BigInteger
        double scaledRight = (double)right * ScaleFactorDouble;
        var result = (left.rawValue * ScaleFactor) / new BigInteger(scaledRight);
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator /(Decimal512 left, double right)
    {
        if (right == 0.0)
            throw new DivideByZeroException("Cannot divide by zero.");

        // Convert double to scaled BigInteger
        double scaledRight = right * ScaleFactorDouble;
        var result = (left.rawValue * ScaleFactor) / new BigInteger(scaledRight);
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator /(Decimal512 left, int right)
    {
        if (right == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        var result = left.rawValue / right;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public static Decimal512 operator /(Decimal512 left, long right)
    {
        if (right == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        var result = left.rawValue / right;
        ValidateRange(result);
        return new Decimal512(result);
    }

    public BigInteger RawValue => rawValue;

    // Property to access the serialized value (useful for debugging)
    public string SerializedValue => serializedValue;

    public double ToDouble()
    {
        return (double)rawValue / ScaleFactorDouble;
    }

    public override string ToString()
    {
        BigInteger whole = rawValue / ScaleFactor;
        BigInteger fraction = rawValue % ScaleFactor;

        // Remove trailing zeros from fraction part
        string fractionStr = fraction.ToString().PadLeft(4, '0').TrimEnd('0');
        if (string.IsNullOrEmpty(fractionStr))
            return whole.ToString();

        return $"{whole}.{fractionStr}";
    }

    public string ToString(int decimalPlaces)
    {
        if (decimalPlaces < 0 || decimalPlaces > 4)
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Decimal places must be between 0 and 4");

        BigInteger whole = rawValue / ScaleFactor;
        BigInteger fraction = rawValue % ScaleFactor;

        if (decimalPlaces == 0)
            return whole.ToString();

        BigInteger divisor = BigInteger.Pow(10, 4 - decimalPlaces);
        BigInteger roundedFraction = fraction / divisor;

        string fractionStr = roundedFraction.ToString().PadLeft(decimalPlaces, '0');
        return $"{whole}.{fractionStr}";
    }

    public static Decimal512 Parse(string input)
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

        // Pad or truncate fraction part to 4 digits
        if (fractionPart.Length > 4)
            fractionPart = fractionPart.Substring(0, 4);
        else
            fractionPart = fractionPart.PadRight(4, '0');

        if (!BigInteger.TryParse(fractionPart, out BigInteger fraction))
            throw new FormatException("Invalid fraction part.");

        BigInteger scaled = whole * ScaleFactor + fraction;
        return new Decimal512(scaled);
    }

    private static void ValidateRange(BigInteger value)
    {
        if (value < 0 || value > MaxValue)
            throw new OverflowException("Value is outside 512-bit unsigned range.");
    }

    // --- Comparison operators ---
    public int CompareTo(Decimal512 other) => rawValue.CompareTo(other.rawValue);
    public bool Equals(Decimal512 other) => rawValue.Equals(other.rawValue);
    public override bool Equals(object obj) => obj is Decimal512 other && Equals(other);
    public override int GetHashCode() => rawValue.GetHashCode();

    public static bool operator ==(Decimal512 left, Decimal512 right) => left.rawValue == right.rawValue;
    public static bool operator !=(Decimal512 left, Decimal512 right) => left.rawValue != right.rawValue;
    public static bool operator <(Decimal512 left, Decimal512 right) => left.rawValue < right.rawValue;
    public static bool operator >(Decimal512 left, Decimal512 right) => left.rawValue > right.rawValue;
    public static bool operator <=(Decimal512 left, Decimal512 right) => left.rawValue <= right.rawValue;
    public static bool operator >=(Decimal512 left, Decimal512 right) => left.rawValue >= right.rawValue;

    // Implicit conversions
    public static implicit operator Decimal512(double d) => new Decimal512(d);
    public static implicit operator Decimal512(int i) => new Decimal512((double)i);
    public static implicit operator Decimal512(long l) => new Decimal512((double)l);
}