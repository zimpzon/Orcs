using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class SkullCrusherManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelSkullCrusher;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeSkullCrusher;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            long currentIncrease = (long)Math.Round((ValueForLevel(level) - 1.0) * 100.0);
            long nextIncrease = (long)Math.Round((ValueForLevel(level + 1) - 1.0) * 100.0);

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelSkullCrusher,
                SaveGame.Members.LevelSkullCrusherX2,
                UpgradeProgression.InitialPrice_SkullCrusher_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Skull Crusher</color></b></size>");
            sb.AppendLine("<color=#dddddd>Skulls crush their target for more damage.");
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
            sb.AppendLine($"<color=#dddddd>Skull damage increase: <color=COLOR-ARENA>{Format512.Format(currentIncrease)}%</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{Format512.Format(nextIncrease)}%</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => 1 + 0.3 * level;

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_SkullCrusher;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelSkullCrusherX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelSkullCrusher;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_SkullCrusher, SaveGame.Members.LevelSkullCrusher, SaveGame.Members.LevelSkullCrusherX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.NecromancerSkullCrusherMultiplier = ValueForLevel(SaveGame.Members.LevelSkullCrusher);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSkullCrusher += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelSkullCrusher, SaveGame.Members.LevelSkullCrusherX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_SkullCrusher_X2, SaveGame.Members.LevelSkullCrusherX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelSkullCrusherX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelSkullCrusher,
                SaveGame.Members.LevelSkullCrusherX2,
                UpgradeProgression.InitialPrice_SkullCrusher_X2);

            UpgradeManager.Instance.SkullCrusher.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelSkullCrusher);
        }
    }
}
