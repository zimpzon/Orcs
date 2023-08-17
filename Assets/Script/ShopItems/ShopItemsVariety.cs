using System;
using System.Collections.Generic;
using System.Linq;

public static class ShopItemsVariety
{
    public static List<ShopItem> GetVarietyItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.SpawnChest,
                Title = "Duck's Treasure",
                Description = $"A large or a small treasure chest will show up at 5:00",
                BasePrice = 100,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.SpawnChestUnlocked = true;
                    PlayerUpgrades.Data.SpawnChestChance = bought.Value * bought.ValueScale * bought.Level;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)PlayerUpgrades.Data.SpawnChestUnlockCriteria_GameTime.TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {PlayerUpgrades.Data.SpawnChestUnlockCriteria_GameTime.Minutes:00}:{PlayerUpgrades.Data.SpawnChestUnlockCriteria_GameTime.Seconds:00} to unlock\n\nBest: {best.Minutes:00}:{best.Seconds:00}";
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
