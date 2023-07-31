using System.Collections.Generic;

public static class ShopItemsWeapons
{
    public static List<ShopItem> GetWeaponItems()
    {
        const string Name = "Knife of Nobility";
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.WeaponRange,
                Title = $"{Name}, Range",
                Description = $"Increase range of your {Name} by <color=#00ff00>+#VALUE%</color> per rank",
                BasePrice = 250,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 10,
                ValueScale = 0.01f,
                Apply = (bought ) =>
                {
                    PlayerUpgrades.Data.MagicMissileRangeMul += bought.Level * bought.Value * bought.ValueScale;
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.WeaponCd,
                Title = $"{Name}, Speed",
                Description = $"Wait <color=#00ff00>#VALUE%</color> less time before next ${Name}",
                BasePrice = 500,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 8,
                ValueScale = 0.01f,
                Apply = (bought ) =>
                {
                    PlayerUpgrades.Data.MagicMissileCdMul *= 1.0f - bought.Value * bought.ValueScale * bought.Level;
                }
            },

            new ShopItem
            {
                ItemType = ShopItemType.MachinegunBulletsPerRound,
                Title = $"{Name}, Salvo",
                Description = "Increase primary weapon bullets per round by <color=#00ff00>+#VALUE</color> per rank.",
                BasePrice = 250,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 1,
                ValueScale = 1,
                Apply = (bought ) =>
                {
                    PlayerUpgrades.Data.MagicMissileBulletsMul += bought.Level * (int)bought.Value;
                }
            },
        };
    }
}
