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

            new ShopItem
            {
                ItemType = ShopItemType.EnemyMoneyDrop,
                Title = "Gold drop",
                Description = "Killed enemies have a <color=#00ff00>+#VALUE%</color> chance to drop <color=#00ff00>1</color>-<color=#00ff00>3</color> gold.",
                BasePrice = 25,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 2,
                ValueScale = 0.01f,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.DropMoneyOnKillChance += bought.Level * bought.Value * bought.ValueScale;
                }
            },
        };
    }
}
