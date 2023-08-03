using System.Collections.Generic;

public static class ShopItemsUnlock
{
    public static List<ShopItem> GetUnlockItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.Sawblade,
                Title = ChoicesSawblade.Name,
                Description = $"You may now discover {ChoicesSawblade.Name} on your journey",
                BasePrice = 50,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.SawBladeBought = true;
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.UnlockPoisonDagger,
                Title = ChoicesPoison.Name,
                Description = $"You may now discover {ChoicesPoison.Name} on your journey",
                BasePrice = 200,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.PaintballBought = true;
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.BurstOfFrost,
                Title = ChoicesBurstOfFrost.Name,
                Description = $"You may now discover {ChoicesBurstOfFrost.Name} on your journey",
                BasePrice = 200,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.BurstOfFrostBought = true;
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.MeleeThrow,
                Title = ChoicesMeleeThrow.Name,
                Description = $"You may now discover {ChoicesMeleeThrow.Name} on your journey",
                BasePrice = 200,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.MeleeThrowBought = true;
                }
            },

        };
    }
}
