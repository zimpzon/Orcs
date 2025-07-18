using System;
using System.Text;

namespace Assets.Script.Upgrades
{
    public static class ClickDamageManager
    {
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
            => 10 + level;

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.ClickDamage.BuyButtonOverlay.enabled = !canAfford;
            UpgradeManager.Instance.ClickDamage.SetPrice(priceForNext);
            UpgradeManager.Instance.ClickDamage.SetLevel(SaveGame.Members.LevelClickDamage);
        }

        public static string GetDescription()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Add more damage per click.");
            sb.AppendLine("Current:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelClickDamage)}</color>");
            sb.AppendLine("Next:");
            sb.AppendLine($"    <color=#ffff00>{ValueForLevel(SaveGame.Members.LevelClickDamage) + 1}</color>");
            return sb.ToString();
        }
    }
}
