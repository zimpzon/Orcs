using System.Collections.Generic;

public static class ShopItemsWeapons
{
    public static List<ShopItem> GetWeaponItems()
    {
        return new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.WeaponRange,
                Title = "Main weapon, range",
                Description = "Increase main weapon range by <color=#00ff00>+#VALUE%</color> per rank.",
                BasePrice = 250,
                PriceMultiplier = 2.0f,
                MaxLevel = 3,
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
                Title = "Main weapon, cooldown",
                Description = "Decrease main weapon cooldown by <color=#00ff00>+#VALUE%</color> per rank.",
                BasePrice = 500,
                PriceMultiplier = 2.0f,
                MaxLevel = 3,
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
                Title = "Main weapon, bullet count",
                Description = "Increase primary weapon bullets per round by <color=#00ff00>+#VALUE</color> per rank.",
                BasePrice = 250,
                PriceMultiplier = 2.0f,
                MaxLevel = 5,
                Value = 1,
                ValueScale = 1,
                Apply = (bought ) =>
                {
                    PlayerUpgrades.Data.MagicMissileBulletsAdd += bought.Level * (int)bought.Value;
                }
            },
        };
    }
}
