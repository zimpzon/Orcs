using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ArenaGoldManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelMoneyPerGold;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeGoldPerRound;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentValue = (long)(ValueForLevel(level) * 100);
            long nextValue = (long)(ValueForLevel(level + 1) * 100);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelMoneyPerGold,
                SaveGame.Members.LevelMoneyPerGoldX2,
                UpgradeProgression.InitialPrice_GoldValue_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal256 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Gold Value</color></b></size>");
            sb.AppendLine("<color=#dddddd>Get more $ from gold.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");

            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-PASSIVE>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine($"<color=#dddddd>Price: <color={colorX2PriceMet}>${Format256.Format(priceX2)}");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Gold value: <color=COLOR-ARENA>{currentValue}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{nextValue}%</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.25 * level;

        private static Decimal256 BaseIncome()
        {
            Decimal256 baseIncome = UpgradeProgression.BaseIncome_GoldValue;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelMoneyPerGoldX2);
            return baseIncome;
        }

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelMoneyPerGold;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_GoldValue * Math.Pow(1.15, SaveGame.Members.LevelMoneyPerGold);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MoneyPerGold = ValueForLevel(SaveGame.Members.LevelMoneyPerGold);
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyPerGold++;
        }

        public static void OnBuyX2()
        {
            Decimal256 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_GoldValue_X2, SaveGame.Members.LevelMoneyPerGoldX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyPerGoldX2++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelMoneyPerGold,
                SaveGame.Members.LevelMoneyPerGoldX2,
                UpgradeProgression.InitialPrice_GoldValue_X2);

            UpgradeManager.Instance.GoldPerRound.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelMoneyPerGold);
        }
    }
}
