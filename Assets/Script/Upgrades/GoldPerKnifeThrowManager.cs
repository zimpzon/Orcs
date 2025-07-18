using System;

namespace Assets.Script.Upgrades
{
    public static class GoldPerKnifeThrowManager
    {
        public static long PriceForNext()
        {
            return 10 + (long)Math.Pow(SaveGame.Members.LevelGoldPerKnifeThrown, 2.5f);
        }

        public static void UpdateAll()
        {
            UpdatePlayerUpgrades();
            UpdateUi();
        }

        public static void UpdatePlayerUpgrades()
        {
            PlayerUpgrades.Data.GoldPerKnifeThrown = SaveGame.Members.LevelGoldPerKnifeThrown;
        }

        public static void UpdateUi()
        {
            long priceForNext = PriceForNext();
            bool canAfford = priceForNext <= SaveGame.Members.Money;

            UpgradeManager.Instance.GoldPerKnife.BuyButtonOverlay.enabled = !canAfford;
            UpgradeManager.Instance.GoldPerKnife.SetPrice(priceForNext);
            UpgradeManager.Instance.GoldPerKnife.SetLevel(SaveGame.Members.LevelGoldPerKnifeThrown);
        }
    }
}
