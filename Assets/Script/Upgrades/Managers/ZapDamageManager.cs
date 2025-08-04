using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ZapDamageManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelZapDamage;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeZapDamage;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            long currentIncrease = (long)((ValueForLevel(level) - 1.0) * 100.0);
            long nextIncrease = (long)((ValueForLevel(level + 1) - 1.0) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelZapDamage,
                SaveGame.Members.LevelZapDamageX2,
                UpgradeProgression.InitialPrice_ZapDamage_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal256 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Zap Damage</color></b></size>");
            sb.AppendLine("<color=#dddddd>Increases all Zap damage.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.FormatWithDecimals(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.FormatWithDecimals(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.FormatWithDecimals(earnedSoFar)}</color>.");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-PASSIVE>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine($"<color=#dddddd>Price: <color={colorX2PriceMet}>${Format256.Format(priceX2)}");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Zap damage increase: <color=COLOR-ARENA>{Format256.Format(currentIncrease)}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{Format256.Format(nextIncrease)}%</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
        {
            return 1.0 + 0.25 * level;
        }

        private static Decimal256 BaseIncome()
        {
            Decimal256 baseIncome = UpgradeProgression.BaseIncome_ZapDamage;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelZapDamageX2);
            return baseIncome;
        }

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelZapDamage;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_ZapDamage * Math.Pow(1.15, SaveGame.Members.LevelZapDamage);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.ZapDamageUpgrade = ValueForLevel(SaveGame.Members.LevelZapDamage);
        }

        public static void OnBuy()
        {
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelZapDamage++;
        }

        public static void OnBuyX2()
        {
            Decimal256 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_ZapDamage_X2, SaveGame.Members.LevelZapDamageX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelZapDamageX2++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelZapDamage,
                SaveGame.Members.LevelZapDamageX2,
                UpgradeProgression.InitialPrice_ZapDamage_X2);

            UpgradeManager.Instance.ZapDamage.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelZapDamage);
        }
    }
}
