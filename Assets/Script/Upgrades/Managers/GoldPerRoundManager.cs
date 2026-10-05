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
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeGoldPerRound;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentBonus = ChestBonusForLevel(level);
            long nextBonus = ChestBonusForLevel(level + 1);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelMoneyPerGold,
                SaveGame.Members.LevelMoneyPerGoldX2,
                UpgradeProgression.InitialPrice_GoldValue_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Richer Chests</color></b></size>");
            sb.AppendLine("<color=#dddddd>Chests pay more: each level adds +1 to the chest reward.");
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
            sb.AppendLine($"<color=#dddddd>Chest reward bonus: <color=COLOR-ARENA>+{currentBonus}</color>");
            sb.AppendLine(currentBonus >= MaxChestBonus
                ? $"<color=#dddddd>Maxed (+{MaxChestBonus})"
                : $"<color=#dddddd>Next: <color=COLOR-ARENA>+{nextBonus}</color> (max +{MaxChestBonus})");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.25 * level;

        // Each level adds +1 second of income to chest rewards (PopupChestScript: 100-199 x income), capped so it
        // stays a small bonus however high the level gets.
        public const int MaxChestBonus = 30;

        private static int ChestBonusForLevel(long level)
            => (int)Math.Min(Math.Max(level, 0), MaxChestBonus);

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_GoldValue;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelMoneyPerGoldX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelMoneyPerGold;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_GoldValue, SaveGame.Members.LevelMoneyPerGold, SaveGame.Members.LevelMoneyPerGoldX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MoneyPerGold = ValueForLevel(SaveGame.Members.LevelMoneyPerGold);
            PlayerUpgrades.Data.ChestBonusSeconds = ChestBonusForLevel(SaveGame.Members.LevelMoneyPerGold);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyPerGold += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelMoneyPerGold, SaveGame.Members.LevelMoneyPerGoldX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_GoldValue_X2, SaveGame.Members.LevelMoneyPerGoldX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelMoneyPerGoldX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelMoneyPerGold,
                SaveGame.Members.LevelMoneyPerGoldX2,
                UpgradeProgression.InitialPrice_GoldValue_X2);

            UpgradeManager.Instance.GoldPerRound.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelMoneyPerGold);
        }
    }
}
