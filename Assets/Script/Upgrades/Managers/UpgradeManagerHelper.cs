using Assets.Script.Misc;
using System;

namespace Assets.Script.Upgrades.Managers
{
    internal class UpgradeManagerHelper
    {
        // TimeSpan.MaxValue is about 10,675,199 days or ~29,247 years
        private static readonly double MaxTimeSpanSeconds = TimeSpan.MaxValue.TotalSeconds;

        public static string FormatPrice(Decimal512 priceForNext)
        {
            var money = SaveGame.Members.Money;
            string onlyPriceStr = $"<color=#dddddd>Price: $<color={GetColorPriceX2(x2PriceMet: true)}>{Format512.Format(priceForNext)}";

            if (GameManager.Instance.TotalPassiveIncome < 0.001 || priceForNext <= money)
                return onlyPriceStr;

            Decimal512 priceLeft = priceForNext - SaveGame.Members.Money;
            double secondsLeft = (priceLeft / GameManager.Instance.TotalPassiveIncome).ToDouble();

            // Handle edge cases before creating TimeSpan
            if (secondsLeft <= 0 || double.IsInfinity(secondsLeft) || double.IsNaN(secondsLeft))
                return onlyPriceStr;

            string timeStr;
            if (secondsLeft > MaxTimeSpanSeconds)
            {
                // Handle extremely long times without creating TimeSpan
                timeStr = "forget it"; // or "Never" or ">29k years"
            }
            else
            {
                TimeSpan ts = TimeSpan.FromSeconds(secondsLeft);
                timeStr = Format.FormatTimeShort(ts);
            }

            return $"<color=#dddddd>Price: $<color={GetColorPriceX2(x2PriceMet: false)}>{Format512.Format(priceForNext)} ({timeStr})";
        }

        public static string FormatTimeLeft(Decimal512 valueForNext, Decimal512 current, double MaxSeconds = double.MaxValue)
        {
            var money = SaveGame.Members.Money;

            if (GameManager.Instance.TotalPassiveIncome < 0.01)
                return string.Empty;

            // Handle case where current >= valueForNext (already achieved or exceeded the target)
            if (current >= valueForNext)
                return string.Empty;

            Decimal512 amountLeft = valueForNext - current;
            double secondsLeft = (amountLeft / GameManager.Instance.TotalPassiveIncome).ToDouble();

            // Handle edge cases before creating TimeSpan
            if (secondsLeft <= 0 || secondsLeft > MaxSeconds || double.IsInfinity(secondsLeft) || double.IsNaN(secondsLeft))
                return string.Empty;

            string timeStr;
            if (secondsLeft > MaxTimeSpanSeconds)
            {
                // Handle extremely long times without creating TimeSpan
                timeStr = "forget it"; // or "Never" or ">29k years"
            }
            else
            {
                TimeSpan ts = TimeSpan.FromSeconds(secondsLeft);
                timeStr = Format.FormatTimeShort(ts);
            }

            return timeStr;
        }

        public static void GetX2Calculated(
            long upgradeLevel,
            long levelX2,
            Decimal512 initialPriceX2,
            out long x2LevelsBought,
            out long x2LevelRequirement,
            out Decimal512 priceX2,
            out bool x2LevelMet,
            out bool x2PriceMet,
            out string colorX2LevelMet,
            out string colorX2PriceMet)
        {
            x2LevelsBought = levelX2;
            x2LevelRequirement = UpgradeProgression.LevelRequirementX2(levelX2 + 1);
            priceX2 = UpgradeProgression.PriceX2(initialPriceX2, levelX2 + 1);
            x2LevelMet = upgradeLevel >= x2LevelRequirement;
            x2PriceMet = SaveGame.Members.Money >= priceX2;
            colorX2LevelMet = GetColorLevelX2(x2LevelMet);
            colorX2PriceMet = GetColorPriceX2(x2PriceMet);
        }

        public static bool X2RequirementsMet(long upgradeLevel, long levelX2, Decimal512 initialPriceX2)
        {
            long x2LevelRequirement = UpgradeProgression.LevelRequirementX2(levelX2 + 1);
            Decimal512 priceX2 = UpgradeProgression.PriceX2(initialPriceX2, levelX2 + 1);
            bool x2LevelMet = upgradeLevel >= x2LevelRequirement;
            bool x2PriceMet = SaveGame.Members.Money >= priceX2;
            return x2LevelMet && x2PriceMet;
        }

        public static string GetColorLevelX2(bool x2LevelMet)
            => x2LevelMet ? "#8DBE4C" : "#DF8749";

        public static string GetColorPriceX2(bool x2PriceMet)
            => x2PriceMet ? "#8DBE4C" : "#DF8749";
    }
}
