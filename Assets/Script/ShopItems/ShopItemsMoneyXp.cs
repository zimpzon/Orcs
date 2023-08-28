using System;
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
                ItemType = ShopItemType.PickupRange,
                Title = "Magnetic Armor",
                Description = "Attract gold and XP from much further away",
                BasePrice = 1,
                PriceMultiplier = 1.0f,
                MaxLevel = 1,
                Value = 1,
                ValueScale = 1f,
                Apply = (BoughtItem bought) =>
                {
                    if (bought.Level > 0)
                        PlayerUpgrades.Data.GoldXpAttractRange *= 1.5f;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)new TimeSpan(0, 6, 0).TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(60 * 15 - SaveGame.Members.MaxSecondsReached);
                        return $"Reach {G.ColorTimeNeutral(new TimeSpan(0, 15 - 6, 0))}\n\nBest: {G.ColorTimeNeutral(best)}";
                    }
                    else
                    {
                        return shopItem.Description;
                    }
                }
            },
       };
    }
}
