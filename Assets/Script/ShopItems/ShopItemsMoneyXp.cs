using System.Collections.Generic;

public static class ShopItemsMoneyXp
{
    public static List<ShopItem> GetMoneyXpItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.DoubleMoney,
                Title = "Chance to drop double gold",
                Description = "Any gold dropped has a <color=#00ff00>+#VALUE%</color> chance to be doubled.",
                BasePrice = 25,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 2,
                ValueScale = 0.01f,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.MoneyDoubleChance += bought.Level * bought.Value * bought.ValueScale;
                }
            },
        };
    }
}
