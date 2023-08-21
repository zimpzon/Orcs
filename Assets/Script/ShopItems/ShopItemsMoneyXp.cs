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
                Title = "Double Drop",
                Description = "All dropped coins and XP have <color=#00ff00>+#VALUE%</color> chance of being doubled",
                BasePrice = 10,
                PriceMultiplier = 2.0f,
                MaxLevel = 10,
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
                Title = "Pickpocket",
                Description = "Killed enemies have <color=#00ff00>+#VALUE%</color> chance to drop <color=#00ff00>1</color>-<color=#00ff00>3</color> gold",
                BasePrice = 5,
                PriceMultiplier = 2.0f,
                MaxLevel = 10,
                Value = 2,
                ValueScale = 0.01f,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.DropMoneyOnKillChance += bought.Level * bought.Value * bought.ValueScale;
                }
            },

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
