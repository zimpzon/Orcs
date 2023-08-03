using System.Collections.Generic;

public static class ShopItemsWeapons
{
    public static List<ShopItem> GetWeaponItems()
    {
        const string Name = "Your trusty knife";
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.WeaponRange,
                Title = $"{Name}, Range",
                Description = $"Increase range of {Name} by <color=#00ff00>+#VALUE%</color> per rank",
                BasePrice = 25,
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
                Description = $"<color=#00ff00>#VALUE%</color> less time between attacks",
                BasePrice = 50,
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
                Description = "<color=#00ff00>+#VALUE</color> more knives per rank.",
                BasePrice = 35,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 0.5f,
                ValueScale = 1,
                Apply = (bought ) =>
                {
                    PlayerUpgrades.Data.MagicMissileBulletsMul += bought.Level * (int)bought.Value;
                }
            },
        };
    }
}
