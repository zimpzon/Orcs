using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class GoldPerKnifeThrowManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Gold earned for each dagger thrown.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelGoldPerKnifeThrown)}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelGoldPerKnifeThrown + 1)}</color>");
            return sb.ToString();
        }

        private static long ValueForLevel(long level)
            => level;

        public static double PassiveIncome()
            => 6 * SaveGame.Members.LevelGoldPerKnifeThrown;

        public static long PriceForNext()
        {
            return 5000 + (long)Math.Pow(SaveGame.Members.LevelGoldPerKnifeThrown, 2.5f);
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

            SaveGame.Members.Money -= priceForNext;
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
