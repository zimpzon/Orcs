using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class GoldPerKnifeThrowManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelGoldPerKnifeThrown;
            Decimal256 earnedSoFar = SaveGame.Members.TotalIncomeGoldPerKnifeThrow;
            Decimal256 baseIncome = BaseIncome();
            Decimal256 totalIncome = PassiveIncome();
            Decimal256 currentValue = ValueForLevel(level) * 100.0;
            Decimal256 nextValue = ValueForLevel(level + 1) * 100.0;

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Gold Per Dagger</color></b></size>");
            sb.AppendLine("<color=#dddddd>Get gold per dagger thrown. Higher dagger damage means higher reward.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=COLOR-PASSIVE>${Format256.Format(baseIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-PASSIVE>${Format256.Format(totalIncome)}</color> per second.");
            sb.AppendLine($"<color=#dddddd>Earned so far: <color=COLOR-PASSIVE>${Format256.Format(earnedSoFar)}</color>.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current: <color=COLOR-ARENA>{Format256.Format(currentValue)}% of dagger damage</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=COLOR-ARENA>{Format256.Format(nextValue)}%</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => (long)(1 + (level - 1) * 1.1);

        private static Decimal256 BaseIncome() => UpgradeProgression.BaseIncome_GoldPerKnifeThrown;

        public static Decimal256 PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelGoldPerKnifeThrown;

        public static Decimal256 PriceForNext()
        {
            return UpgradeProgression.InitialPrice_GoldPerKnifeThrown * Math.Pow(1.15, SaveGame.Members.LevelGoldPerKnifeThrown);
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
            Decimal256 priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelGoldPerKnifeThrown++;
        }

        public static void UpdateUi()
        {
            Decimal256 priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.GoldPerKnife.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelGoldPerKnifeThrown);
        }
    }
}
