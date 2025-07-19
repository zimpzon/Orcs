using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class GoldPerKnifeThrowManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Gold earned for each knife thrown.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelGoldPerKnifeThrown)}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelGoldPerKnifeThrown + 1)}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 10 + (long)Math.Pow(SaveGame.Members.LevelGoldPerKnifeThrown, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        private static long ValueForLevel(long level)
            => level;

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.GoldPerKnifeThrown = ValueForLevel(SaveGame.Members.LevelGoldPerKnifeThrown);
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.GoldPerKnife.SetCanAfford(canAfford, priceForNext, SaveGame.Members.LevelGoldPerKnifeThrown);
        }
    }
}
