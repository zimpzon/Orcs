using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class HeroRunSpeedManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("How fast the hero runs.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelHeroRunspeed)}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelHeroRunspeed + 1)}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 30 + (long)Math.Pow(SaveGame.Members.LevelHeroRunspeed, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        private static float ValueForLevel(long level)
            => 3 + 0.1f * level;

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MoveSpeedAdd = ValueForLevel(SaveGame.Members.LevelHeroRunspeed);
        }

        public static void OnBuy()
        {
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            SaveGame.Members.Money -= priceForNext;
            SaveGame.Members.LevelHeroRunspeed++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.HeroRunspeed.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelHeroRunspeed, maxLevel: 30);
        }
    }
}
