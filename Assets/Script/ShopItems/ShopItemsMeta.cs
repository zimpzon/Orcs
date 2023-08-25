using System.Collections.Generic;

public static class ShopItemsMeta
{
    public static List<ShopItem> GetMetaItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.Abundance,
                Title = "Abundance!\r\n",
                Description = "Value of all dropped gold and XP <color=#00ff00>TRIPLED</color> per rank",
                BasePrice = 5000,
                PriceMultiplier = 2.0f,
                MaxLevel = 3,
                Value = 3,
                ValueScale = 1f,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.GoldXpMultiplierBought = bought.Level > 0;
                    PlayerUpgrades.Data.GoldXpMultiplyValue = 3 * bought.Level;
                }
            },
        };
    }
}
