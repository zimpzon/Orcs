using System.Collections.Generic;

public static class ShopItemsProgress
{
    public static List<ShopItem> GeProgressItems()
    {
        return new List<ShopItem>
        {
            // TODO
            //new ShopItem
            //{
            //    ItemType = ShopItemType.DoubleTime,
            //    Title = "Time Traveller",
            //    Description = "All dropped coins and XP have <color=#00ff00>+#VALUE%</color> chance of being doubled",
            //    BasePrice = 25,
            //    PriceMultiplier = 2.0f,
            //    MaxLevel = 10,
            //    Value = 2,
            //    ValueScale = 0.01f,
            //    Apply = (BoughtItem bought) =>
            //    {
            //        PlayerUpgrades.Data.MoneyDoubleChance += bought.Level * bought.Value * bought.ValueScale;
            //    }
            //},

            //new ShopItem
            //{
            //    ItemType = ShopItemType.EnemyMoneyDrop,
            //    Title = "Pickpocket",
            //    Description = "Killed enemies have <color=#00ff00>+#VALUE%</color> chance to drop <color=#00ff00>1</color>-<color=#00ff00>3</color> gold",
            //    BasePrice = 20,
            //    PriceMultiplier = 2.0f,
            //    MaxLevel = 10,
            //    Value = 2,
            //    ValueScale = 0.01f,
            //    Apply = (BoughtItem bought) =>
            //    {
            //        PlayerUpgrades.Data.DropMoneyOnKillChance += bought.Level * bought.Value * bought.ValueScale;
            //    }
            //},
        };
    }
}
