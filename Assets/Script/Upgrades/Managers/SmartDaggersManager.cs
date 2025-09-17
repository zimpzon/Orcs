using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class SmartDaggersManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelSmartDaggers;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeSmartDaggers;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentValue = (long)Math.Round((ValueForLevel(level)) * 100.0);
            long nextValue = (long)Math.Round((ValueForLevel(level + 1)) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelSmartDaggers,
                SaveGame.Members.LevelSmartDaggersX2,
                UpgradeProgression.InitialPrice_SmartDaggers_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Smart Daggers</color></b></size>");
            sb.AppendLine("<color=#dddddd>Increased dagger damage and daggers continue on to nearby targets.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns $<color=COLOR-ARENA>{Format512.FormatWithDecimals(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: $<color=COLOR-ARENA>{Format512.FormatWithDecimals(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: $<color=COLOR-ARENA>{Format512.FormatWithDecimals(earnedSoFar)}</color>.");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(PriceForNext()));
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-ARENA>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(priceX2));
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Bonus Dagger damage: +<color=COLOR-ARENA>{Format512.Format(currentValue)}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: +<color=COLOR-ARENA>{Format512.Format(nextValue)}%</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
        {
            return level * 0.15;
        }

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_SmartDaggers;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelSmartDaggersX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelSmartDaggers;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_SmartDaggers, SaveGame.Members.LevelSmartDaggers, SaveGame.Members.LevelSmartDaggersX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileJumpDamageMul = SaveGame.Members.LevelSmartDaggers > 0 ? 1.0f : 0.0f;
            PlayerUpgrades.Data.MagicMissileDamageMulSmartDaggers = ValueForLevel(SaveGame.Members.LevelSmartDaggers);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSmartDaggers += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelSmartDaggers, SaveGame.Members.LevelSmartDaggersX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_SmartDaggers_X2, SaveGame.Members.LevelSmartDaggersX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSmartDaggersX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelSmartDaggers,
                SaveGame.Members.LevelSmartDaggersX2,
                UpgradeProgression.InitialPrice_SmartDaggers_X2);

            UpgradeManager.Instance.SmartDaggers.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelSmartDaggers);
        }
    }
}
