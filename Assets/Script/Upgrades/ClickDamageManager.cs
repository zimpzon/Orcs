using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ClickDamageManager
    {
        public static string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Damage per click.");
            sb.AppendLine("");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelClickDamage)}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelClickDamage + 1)}</color>");
            return sb.ToString();
        }

        public static long PriceForNext()
        {
            return 10 + (long)Math.Pow(SaveGame.Members.LevelClickDamage, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.ClickDamage = ValueForLevel(SaveGame.Members.LevelClickDamage);
        }

        private static long ValueForLevel(long level)
            => 5 + level;

        public static void OnBuy()
        {
            long priceForNext = PriceForNext();
            if (priceForNext > SaveGame.Members.Money)
                return;

            SaveGame.Members.Money -= priceForNext;
            SaveGame.Members.LevelClickDamage++;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.ClickDamage.UpdateUi(canAfford, priceForNext, SaveGame.Members.LevelClickDamage);
        }
    }
}
