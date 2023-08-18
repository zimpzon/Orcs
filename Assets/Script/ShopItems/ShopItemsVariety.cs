using System;
using System.Collections.Generic;

public static class ShopItemsVariety
{
    static string ColorTimeNeutral(TimeSpan time)
        => $"{G.D.UpgradeNeutralColorHex}{time.Minutes:00}</color><color=#f0f0f0>:</color>{G.D.UpgradeNeutralColorHex}{time.Seconds:00}</color>";

    static string ColorTimePositive(TimeSpan time)
        => $"{G.D.UpgradePositiveColorHex}{time.Minutes:00}</color><color=#f0f0f0>:</color>{G.D.UpgradePositiveColorHex}{time.Seconds:00}</color>";

    public static List<ShopItem> GetVarietyItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.SpawnChest,
                Title = "Duck's Treasure",
                Description = $"A large or a small treasure chest will show up at {ColorTimePositive(new TimeSpan(0, 5, 0))}",
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
                        return $"Reach {ColorTimeNeutral(PlayerUpgrades.Data.SpawnChestUnlockCriteria_GameTime)}\n\nBest: {ColorTimeNeutral(best)}";
                    }
                    else
                    {
                        return shopItem.Description;
                    }
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.TimeTravellerA,
                Title = "Time Traveller A",
                Description = $"Start game at {ColorTimePositive(new TimeSpan(0, 3, 0))}",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.GameStartTime = new TimeSpan(0, 3, 0);
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)new TimeSpan(0, 7, 0).TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {ColorTimeNeutral(new TimeSpan(0, 7, 0))}\n\nBest: {ColorTimeNeutral(best)}";
                    }
                    else
                    {
                        return shopItem.Description;
                    }
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.TimeTravellerB,
                Title = "Time Traveller B",
                Description = $"Start game at {ColorTimePositive(new TimeSpan(0, 6, 0))}",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.GameStartTime = new TimeSpan(0, 6, 0);
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)new TimeSpan(0, 9, 0).TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {ColorTimeNeutral(new TimeSpan(0, 9, 0))}\n\nBest: {ColorTimeNeutral(best)}";
                    }
                    else
                    {
                        return shopItem.Description;
                    }
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.TimeTravellerC,
                Title = "Time Traveller C",
                Description = $"Start game at {ColorTimePositive(new TimeSpan(0, 10, 0))}",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.GameStartTime = new TimeSpan(0, 10, 0);
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = !SaveGame.Members.Chapter1BossReached;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {G.D.UpgradeNeutralColorHex}final boss</color> at {ColorTimeNeutral(new TimeSpan(0, 15, 0))}\n\nBest: {ColorTimeNeutral(best)}";
                    }
                    else
                    {
                        return shopItem.Description;
                    }
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.CosmeticArmor,
                Title = "It's good to be the king!",
                Description = $"Get what you deserve",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    // cosmetics also show on titlescreen so much be checked whereever used
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.EnemiesKilled < G.D.CosmeticArmorKills;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"(Cosmetic)\n\nKIll a total of {G.D.UpgradeNeutralColorHex}{G.D.CosmeticArmorKills}</color> enemies\n\nCurrent: {G.D.UpgradeNeutralColorHex}{SaveGame.Members.EnemiesKilled}</color>";
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
