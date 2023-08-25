using System;
using System.Collections.Generic;

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
                Description = $"A large or a small treasure chest will show up at {G.ColorTimePositive(new TimeSpan(0, 5, 0))}",
                BasePrice = 100,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    if (bought.Level <= 0)
                        return;

                    PlayerUpgrades.Data.SpawnChestUnlocked = true;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)PlayerUpgrades.Data.SpawnChestUnlockCriteria_GameTime.TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {G.ColorTimeNeutral(PlayerUpgrades.Data.SpawnChestUnlockCriteria_GameTime)}\n\nBest: {G.ColorTimeNeutral(best)}";
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
                Description = $"Skip to {G.ColorTimePositive(new TimeSpan(0, 3, 0))}",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    if (bought.Level <= 0)
                        return;

                    var time = new TimeSpan(0, 3, 0);
                    if (PlayerUpgrades.Data.GameStartTime < time)
                        PlayerUpgrades.Data.GameStartTime = time;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)new TimeSpan(0, 7, 0).TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {G.ColorTimeNeutral(new TimeSpan(0, 7, 0))}\n\nBest: {G.ColorTimeNeutral(best)}";
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
                Description = $"Skip to {G.ColorTimePositive(new TimeSpan(0, 6, 0))}",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    if (bought.Level <= 0)
                        return;

                    var time = new TimeSpan(0, 6, 0);
                    if (PlayerUpgrades.Data.GameStartTime < time)
                        PlayerUpgrades.Data.GameStartTime = time;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.MaxSecondsReached < (int)new TimeSpan(0, 9, 0).TotalSeconds;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {G.ColorTimeNeutral(new TimeSpan(0, 9, 0))}\n\nBest: {G.ColorTimeNeutral(best)}";
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
                Description = $"Skip to {G.ColorTimePositive(new TimeSpan(0, 10, 0))}",
                BasePrice = 1,
                MaxLevel = 1,
                Apply = (BoughtItem bought) =>
                {
                    if (bought.Level <= 0)
                        return;

                    var time = new TimeSpan(0, 10, 0);
                    if (PlayerUpgrades.Data.GameStartTime < time)
                        PlayerUpgrades.Data.GameStartTime = time;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.Chapter1BossStarted <= 0;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"Reach {G.D.UpgradeNeutralColorHex}final boss</color> at {G.ColorTimeNeutral(new TimeSpan(0, 15, 0))}\n\nBest: {G.ColorTimeNeutral(best)}";
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
                Title = "It's good to be king!",
                Description = $"Get what you deserve",
                BasePrice = 1,
                MaxLevel = 1,
                IsCosmetic = true,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.CosmeticKing = bought.Level > 0;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.EnemiesKilled < G.D.CosmeticArmorKills;

                    if (shopItem.IsLocked)
                    {
                        var best = TimeSpan.FromSeconds(SaveGame.Members.MaxSecondsReached);
                        return $"KIll a total of {G.D.UpgradeNeutralColorHex}{G.D.CosmeticArmorKills}</color> enemies\n\nCurrent: {G.D.UpgradeNeutralColorHex}{SaveGame.Members.EnemiesKilled}</color>";
                    }
                    else
                    {
                        return shopItem.Description;
                    }
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.CosmeticHearts,
                Title = "Heart Attack",
                Description = $"Let it all out",
                BasePrice = 1,
                MaxLevel = 1,
                IsCosmetic = true,
                Apply = (BoughtItem bought) =>
                {
                    PlayerUpgrades.Data.CosmeticHearts = bought.Level > 0;
                },
                Customize = (shopItem) =>
                {
                    shopItem.IsLocked = SaveGame.Members.OrcsSaved < G.D.CosmeticHeartsSaves;

                    if (shopItem.IsLocked)
                    {
                        return $"Save a total of {G.D.UpgradeNeutralColorHex}{G.D.CosmeticHeartsSaves}</color> ducks\n\nCurrent: {G.D.UpgradeNeutralColorHex}{SaveGame.Members.OrcsSaved}</color>";
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
