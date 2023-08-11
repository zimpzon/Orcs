using System.Collections.Generic;

public static class ShopItemsProgress
{
    public static List<ShopItem> GeProgressItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.TimeScale,
                Title = "Fast Forward",
                Description = $"Time passes <color=#afff00>10%</color> faster per rank",
                BasePrice = 1,
                PriceMultiplier = 1,
                MaxLevel = 3,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.TimeScale += 0.1f * bought.Level;
                }
            },
        };
    }
}
