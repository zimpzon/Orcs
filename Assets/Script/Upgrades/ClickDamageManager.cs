using System;

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
            PlayerUpgrades.Data.ClickDamage = 10 + SaveGame.Members.LevelClickDamage;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.ClickDamage.BuyButtonOverlay.enabled = !canAfford;
            UpgradeManager.Instance.ClickDamage.SetPrice(priceForNext);
            UpgradeManager.Instance.ClickDamage.SetLevel(SaveGame.Members.LevelClickDamage);
        }
    }
}
