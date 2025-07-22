using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class GoldPerKnifeThrowManager
    {
        public static string GetText()
        {
            long level = SaveGame.Members.LevelGoldPerKnifeThrown;
            double baseIncome = BaseIncome();
            double totalIncome = PassiveIncome();
            long currentValue = ValueForLevel(level);
            long nextValue = ValueForLevel(level + 1);

            var sb = new StringBuilder();

            sb.AppendLine("<size=+4><b><color=yellow>Gold Per Dagger</color></b></size>");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Passive Income</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Each level earns <color=yellow>${baseIncome:F1}</color>/sec.");
            sb.AppendLine($"<color=#dddddd>Current: <color=yellow>${totalIncome:F1}</color>/sec.");
            sb.AppendLine("");
            sb.AppendLine("<size=+4><i><color=#aaaaff>Arena</color></i></size>");
            sb.AppendLine($"<color=#dddddd>Current gold per dagger: <color=yellow>{currentValue}</color>");
            sb.AppendLine($"<color=#dddddd>Next: <color=yellow>{nextValue}</color>");

            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => level;

        private static double BaseIncome() => 1750;

        public static double PassiveIncome()
            => BaseIncome() * SaveGame.Members.LevelGoldPerKnifeThrown;

        public static long PriceForNext()
        {
            return (long)(1_400_000 * Math.Pow(1.15, SaveGame.Members.LevelGoldPerKnifeThrown));
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
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            GameManager.Instance.DeductMoney(priceForNext);
            SaveGame.Members.LevelGoldPerKnifeThrown++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.GoldPerKnife.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelGoldPerKnifeThrown);
        }
    }
}
