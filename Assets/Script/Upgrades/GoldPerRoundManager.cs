using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class GoldPerRoundManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Extra gold received after completing a round.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelGoldPerRoundAdd)}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelGoldPerRoundAdd + 1)}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 50 + (long)Math.Pow(SaveGame.Members.LevelGoldPerRoundAdd, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        private static long ValueForLevel(long level)
            => level * 10;

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.GoldPerRoundAdd = ValueForLevel(SaveGame.Members.LevelGoldPerRoundAdd);
        }

        public static void OnBuy()
        {
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            SaveGame.Members.Money -= priceForNext;
            SaveGame.Members.LevelGoldPerRoundAdd++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.GoldPerRound.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelGoldPerRoundAdd);
        }
    }
}
