using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class CriticalStrikeManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelCriticalStrike;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeCriticalStrike;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long chanceNow = (long)Math.Round(ChanceForLevel(level) * 100.0);
            long multNow = level * 10;
            long multNext = (level + 1) * 10;

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelCriticalStrike,
                SaveGame.Members.LevelCriticalStrikeX2,
                UpgradeProgression.InitialPrice_CriticalStrike_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Critical Strike</color></b></size>");
            sb.AppendLine("<color=#dddddd>Chance for any hit to deal bonus damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns $<color=COLOR-ARENA>{Format512.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: $<color=COLOR-ARENA>{Format512.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: $<color=COLOR-ARENA>{Format512.Format(earnedSoFar)}</color>.");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(PriceForNext()));

            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-ARENA>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(priceX2));
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Critical hit chance: <color=COLOR-ARENA>{chanceNow}%</color>");
            sb.AppendLine($"<color=#dddddd>Critical hit damage: <color=COLOR-ARENA>x{multNow}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>x{multNext}</color>");

            return sb.ToString();
        }

        private const double FlatChance = 0.15; // always 15% once any level is owned

        private static double ChanceForLevel(long level)
            => level > 0 ? FlatChance : 0.0;

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_CriticalStrike;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelCriticalStrikeX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelCriticalStrike;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_CriticalStrike, SaveGame.Members.LevelCriticalStrike, SaveGame.Members.LevelCriticalStrikeX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            long level = SaveGame.Members.LevelCriticalStrike;
            PlayerUpgrades.Data.BaseCritChance = (float)ChanceForLevel(level);
            PlayerUpgrades.Data.CritValueMul = level * 10.0f;
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelCriticalStrike += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelCriticalStrike, SaveGame.Members.LevelCriticalStrikeX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_CriticalStrike_X2, SaveGame.Members.LevelCriticalStrikeX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelCriticalStrikeX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelCriticalStrike,
                SaveGame.Members.LevelCriticalStrikeX2,
                UpgradeProgression.InitialPrice_CriticalStrike_X2);

            UpgradeManager.Instance.CriticalStrike.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelCriticalStrike);
        }
    }
}
