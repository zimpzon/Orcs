using Assets.Script.Upgrades.Managers;
using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class GoldPerKnifeThrowManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelGoldPerKnifeThrown;
            Decimal512 earnedSoFar = SaveGame.Members.TotalIncomeGoldPerKnifeThrow;
            Decimal512 baseIncome = BaseIncome();
            Decimal512 totalIncome = PassiveIncome();
            Decimal512 currentValue = ValueForLevel(level) * 100.0;
            Decimal512 nextValue = ValueForLevel(level + 1) * 100.0;

            UpgradeManagerHelper.GetX2Calculated(
                SaveGame.Members.LevelGoldPerKnifeThrown,
                SaveGame.Members.LevelGoldPerKnifeThrownX2,
                UpgradeProgression.InitialPrice_GoldPerKnifeThrown_X2,
                out long x2LevelsBought,
                out long x2LevelRequirement,
                out Decimal512 priceX2,
                out bool x2LevelMet,
                out bool x2PriceMet,
                out string colorX2LevelMet,
                out string colorX2PriceMet);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=#8DBE4C>Gold Per Dagger</color></b></size>");
            sb.AppendLine("<color=#dddddd>Get gold per dagger thrown. Higher Dagger damage equals higher reward.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format512.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format512.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format512.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income X2</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Purchased: <color=COLOR-PASSIVE>{x2LevelsBought}");
            sb.AppendLine($"<color=#dddddd>Level required: <color={colorX2LevelMet}>{x2LevelRequirement}");
            sb.AppendLine($"<color=#dddddd>Price: <color={colorX2PriceMet}>${Format512.Format(priceX2)}");
            sb.AppendLine("");

            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-ARENA>{Format512.Format(currentValue)}%<color=#dddddd> of Dagger damage</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{Format512.Format(nextValue)}%</color>");
            sb.AppendLine("");
            sb.AppendLine($"<color=#dddddd>Dagger throws earned: <color=COLOR-ARENA>{Format512.Format(SaveGame.Members.TotalIncomeKnifeThrow)}</color>");

            return sb.ToString();
        }

        private static double ValueForLevel(long level)
            => ((1 + (level - 1) * 1.1)) / 8.0;

        private static Decimal512 BaseIncome()
        {
            Decimal512 baseIncome = UpgradeProgression.BaseIncome_GoldPerKnifeThrown;

            // Apply global modifiers
            baseIncome *= PlayerUpgrades.Data.PassiveIncomeEffectiveMultiplier;

            // Apply X2 bonuses
            baseIncome = baseIncome * Math.Pow(2, SaveGame.Members.LevelGoldPerKnifeThrownX2);
            return baseIncome;
        }

        public static Decimal512 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelGoldPerKnifeThrown;

        public static Decimal512 PriceForNext()
        {
            return UpgradeProgression.PriceForNextUpgrade(UpgradeProgression.InitialPrice_GoldPerKnifeThrown, SaveGame.Members.LevelGoldPerKnifeThrown, SaveGame.Members.LevelGoldPerKnifeThrownX2);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.GoldPerKnifeThrown = ValueForLevel(SaveGame.Members.LevelGoldPerKnifeThrown);
        }

        public static void OnBuy()
        {
            Decimal512 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelGoldPerKnifeThrown += UpgradeProgression.GetActualBuyAmountFromSelectedBuyAmount(SaveGame.Members.LevelGoldPerKnifeThrown, SaveGame.Members.LevelGoldPerKnifeThrownX2);
        }

        public static void OnBuyX2()
        {
            Decimal512 priceForNext = UpgradeProgression.PriceX2(UpgradeProgression.InitialPrice_GoldPerKnifeThrown_X2, SaveGame.Members.LevelGoldPerKnifeThrownX2 + 1);
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelGoldPerKnifeThrownX2++;
        }

        public static void UpdateUi()
        {
            Decimal512 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;
            bool enableBtnX2 = UpgradeManagerHelper.X2RequirementsMet(
                SaveGame.Members.LevelGoldPerKnifeThrown,
                SaveGame.Members.LevelGoldPerKnifeThrownX2,
                UpgradeProgression.InitialPrice_GoldPerKnifeThrown_X2);

            UpgradeManager.Instance.GoldPerKnife.UpdateUi(canAfford, enableBtnX2, priceForNext, SaveGame.Members.LevelGoldPerKnifeThrown);
        }
    }
}
