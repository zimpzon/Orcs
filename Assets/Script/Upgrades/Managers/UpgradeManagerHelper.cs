namespace Assets.Script.Upgrades.Managers
{
    internal class UpgradeManagerHelper
    {
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
