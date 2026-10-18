using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class BeefyEarlManager
    {
        // Dagger damage bonus per level (was 0.25 when it shared one additive sum with Smart Daggers).
        const double DamagePerLevel = 0.5;

        public static string GetText()
        {
            long level = SaveGame.Members.LevelBeefyEarl;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeBeefyEarl;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelBeefyEarl,
                SaveGame.Members.LevelBeefyEarlX2,
                UpgradeProgression.InitialPrice_BeefyEarl_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine($"<size=+4><b><color={TextColors.Green}>Beefy Earl</color></b></size>");
            sb.AppendLine("<color=#dddddd>More daggers, more power!");
            sb.AppendLine("<color=#bbbbbb>___________________________________</color>");
            sb.AppendLine("<size=+4><color=#aaaaff>Income</color></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns $<color=COLOR-ARENA>{Format512.FormatWithDecimals(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: $<color=COLOR-ARENA>{Format512.FormatWithDecimals(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: $<color=COLOR-ARENA>{Format512.FormatWithDecimals(earnedSoFar)}</color>.");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(PriceForNext()));

            sb.AppendLine("<color=#aaaaaa>___________________________________</color>");
            sb.AppendLine("<size=+4><color=#aaaaff>Income X2</color></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-ARENA>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine(UpgradeManagerHelper.FormatPrice(priceX2));

            sb.AppendLine("<color=#aaaaaa>___________________________________</color>");
            sb.AppendLine("<size=+4><color=#aaaaff>Arena</color></size>");
            sb.AppendLine($"<color=#dddddd>Dagger damage: <color=COLOR-ARENA>{UpgradeManagerHelper.FormatFactor(1.0 + level * DamagePerLevel, 1.0 + (level + 1) * DamagePerLevel)}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{UpgradeManagerHelper.FormatFactor(1.0 + (level + 1) * DamagePerLevel, 1.0 + level * DamagePerLevel)}</color>");

            return sb.ToString();
        }

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_BeefyEarl;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelBeefyEarlX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelBeefyEarl;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_BeefyEarl, SaveGame.Members.LevelBeefyEarl, SaveGame.Members.LevelBeefyEarlX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileMultiShotsBeefyEarl = SaveGame.Members.LevelBeefyEarl > 0 ? 1 : 0;
            // Now a separate multiplier on dagger damage (see PlayerUpgrades.MagicMissileEffectiveDamage), was +25%/level.
            PlayerUpgrades.Data.MagicMissileBeefyEarlDamageMultiplier = SaveGame.Members.LevelBeefyEarl * DamagePerLevel;
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelBeefyEarl += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelBeefyEarl, SaveGame.Members.LevelBeefyEarlX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_BeefyEarl_X2, SaveGame.Members.LevelBeefyEarlX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelBeefyEarlX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelBeefyEarl,
                SaveGame.Members.LevelBeefyEarlX2,
                UpgradeProgression.InitialPrice_BeefyEarl_X2);

            UpgradeManager.Instance.BeefyEarl.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelBeefyEarl);
        }
    }
}