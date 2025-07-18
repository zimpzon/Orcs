using System;

namespace Assets.Script.Upgrades
{
    public static class KnifeCdManager
    {
        public static long PriceForNext()
        {
            return 10 + (long)Math.Pow(SaveGame.Members.LevelKnifeCooldown, 2.8f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.MagicMissileBaseCd = 2.0f - SaveGame.Members.LevelKnifeCooldown * 0.1f;
            PlayerUpgrades.Data.MagicMissileBaseCd = Math.Clamp(PlayerUpgrades.Data.MagicMissileBaseCd, 0.1f, 100);
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.KnifeCd.BuyButtonOverlay.enabled = !canAfford;
            UpgradeManager.Instance.KnifeCd.SetPrice(priceForNext);
            UpgradeManager.Instance.KnifeCd.SetLevel(SaveGame.Members.LevelKnifeCooldown, maxLevel: 20);
        }
    }
}
