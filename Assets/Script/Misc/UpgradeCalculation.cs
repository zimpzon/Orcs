using System;
using System.Collections.Generic;

namespace Assets.Script.Misc
{
    public static class UpgradeApplier
    {
        public static void ApplyAll()
        {
            PlayerUpgrades.Data.ClickDamage = SaveGame.Members.ClickDamageProgress.GetValue();
        }
    }

    public class HardcodedValue
    {
        public class Item
        {
            public double Value = 1;
            public long RequiredLevel = 1;
        }
        public List<Item> Values = new();
    }

    public class CalculatedValue
    {
        public long MinimumIncreasePerCount = 1; // Value must increase at least this per count

        public double BaseValue; // Starting value
        public double BaseValueAdd; // Value added per count
        public double BaseValueCountMul; // Value multiplied per count
        public long CurrentCount = 0; // Number of increases
        public long MaxCount = long.MaxValue; // Max value of Count

        public long GetValue()
            => GetValueForCount(CurrentCount);

        public long GetValueForCount(long count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be >= 0");

            if (count > MaxCount)
                throw new ArgumentOutOfRangeException(nameof(count), $"Count must not exceed MaxCount ({MaxCount})");

            // Special case for 0
            if (count == 0)
                return (long)Math.Ceiling(BaseValue + BaseValueAdd);

            // Compute value: BaseValue * (BaseValueCountMul ^ count)
            double multiplied = BaseValue * System.Math.Pow(BaseValueCountMul, count);

            // Add BaseValueAdd at the end
            double final = multiplied + BaseValueAdd;

            // Round up
            return (long)Math.Ceiling(final);
        }
    }
}
