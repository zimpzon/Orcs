using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShopItemType
{
    Refund,
    DamageAll,
    PrimaryWeaponRange,
    PrimaryWeaponDamage,
    PrimaryWeaponCd,
    PrimaryWeaponBulletsPerRound,
    MoreMoney, //todo
    OrcPickupXpMul, // todo
    KillXpChanceMul, // todo, display text, maybe in baseActor?
}

[Serializable]
public class BoughtItem
{
    public ShopItemType ItemType;
    public int Level;
    public float Value;
    public float ValueScale;
}

public class ShopItem
{
    public ShopItemType ItemType;
    public int BasePrice = 100;
    public float PriceMultiplier = 2;
    public string Title;
    public string Description;
    public int MaxLevel = 5;
    public float Value = 1;
    public float ValueScale = 1;
    public bool ShowLevelInTitle = true;

    public int GetPrice(int level) => (int)(BasePrice * Math.Pow(PriceMultiplier, level));
    public Func<int, string> GetButtonText ;
    public Func<int, string> GetTitle;
    public Func<int, string> GetDescription;

    public ShopItem()
    {
        GetButtonText = (int level) => $"${GetPrice(level)}";
        GetTitle = (int level) => $"{Title}{(ShowLevelInTitle ? $" ({level}/{MaxLevel})" : string.Empty)}";
        GetDescription = (int level) => Description;
    }
}

public static class ShopItems
{
    public static List<ShopItem> Items = new List<ShopItem>();
    public static List<ShopItemScript> Scripts = new List<ShopItemScript>();

    public static void CreateItemGoList(ShopItemScript protoScript, Transform parent)
    {
        foreach(var item in Items)
        {
            var go = (GameObject)GameObject.Instantiate(protoScript.gameObject, parent, instantiateInWorldSpace: false);
            var script = go.GetComponent<ShopItemScript>();
            script.ItemType = item.ItemType;
            script.OnClickCallback = OnBuy;
            Scripts.Add(script);
        }
    }

    public static void ApplyToPlayerUpgrades()
    {
        foreach(var pair in SaveGame.Members.BoughtItems)
        {
            var item = pair.Value;
            var itemType = pair.Key;

            if (itemType == ShopItemType.DamageAll)
            {
                PlayerUpgrades.Data.DamageMul += item.Level * item.Value * item.ValueScale;
                GameManager.SetDebugOutput(pair.Key.ToString(), PlayerUpgrades.Data.DamageMul);
            }

            else if (itemType == ShopItemType.PrimaryWeaponBulletsPerRound)
            {
                PlayerUpgrades.Data.MachinegunBulletsAdd += item.Level * (int)item.Value;
                GameManager.SetDebugOutput(pair.Key.ToString(), PlayerUpgrades.Data.MachinegunBulletsAdd);
            }

            else if (itemType == ShopItemType.PrimaryWeaponCd)
            {
                PlayerUpgrades.Data.WeaponsCdMul *= 1.0f - item.Value * item.ValueScale * item.Level;
                GameManager.SetDebugOutput(pair.Key.ToString(), PlayerUpgrades.Data.WeaponsCdMul);
            }

            else if (itemType == ShopItemType.PrimaryWeaponRange)
            {
                PlayerUpgrades.Data.WeaponsRangeMul += item.Level * item.Value * item.ValueScale;
                GameManager.SetDebugOutput(pair.Key.ToString(), PlayerUpgrades.Data.WeaponsRangeMul);
            }

            else if (itemType == ShopItemType.PrimaryWeaponDamage)
            {
                PlayerUpgrades.Data.WeaponsDamageMul += item.Level * item.Value * item.ValueScale;
                GameManager.SetDebugOutput(pair.Key.ToString(), PlayerUpgrades.Data.WeaponsDamageMul);
            }

            else
            {
                GameManager.SetDebugOutput(pair.Key.ToString(), "NOT IMPLEMENTED");
            }
        }
    }

    private static void OnBuy(ShopItemType itemType)
    {
        if (itemType == ShopItemType.Refund)
        {
            SaveGame.Members.BoughtItems.Clear();
            SaveGame.Members.Money += SaveGame.Members.MoneySpentInShop;
            SaveGame.Members.MoneySpentInShop = 0;
            UpdateBoughtItems();
            GameManager.Instance.OnItemBought(itemType);
            return;
        }

        var item = Items.Where(i => i.ItemType == itemType).First();

        SaveGame.Members.BoughtItems.TryGetValue(itemType, out var bought);
        bought ??= new BoughtItem { ItemType = itemType, Value = item.Value, ValueScale = item.ValueScale };

        int price = item.GetPrice(bought.Level);
        if (price > SaveGame.Members.Money)
            return;

        SaveGame.Members.Money -= price;
        SaveGame.Members.MoneySpentInShop += price;
        bought.Level++;
        SaveGame.Members.BoughtItems[itemType] = bought;
        UpdateBoughtItems();
        GameManager.Instance.OnItemBought(itemType);
    }

    public static void UpdateBoughtItems()
    {
        for (int i = 0; i < Items.Count; ++i)
        {
            var item = Items[i];
            SaveGame.Members.BoughtItems.TryGetValue(item.ItemType, out var bought);
            bought ??= new BoughtItem { ItemType = item.ItemType };
            int level = bought.Level;
            int price = item.GetPrice(bought.Level);
            bool canAfford = price <= SaveGame.Members.Money;
            bool isMaxLevel = bought.Level >= item.MaxLevel;
            bool disable = !canAfford || isMaxLevel;

            var script = Scripts[i];
            script.Title.text = item.GetTitle(level);
            script.Description.text = item.GetDescription(level);
            script.ButtonText.text = isMaxLevel ? "max" : item.GetButtonText(level);

            script.BuyButton.interactable = !disable;
            script.SetDisableButton(disable);
            script.SetIsMaxed(isMaxLevel);
        }
    }

    static ShopItems()
    {
        Items = new List<ShopItem>
        {
            new ShopItem
            {
                ItemType = ShopItemType.DamageAll,
                Title = "Damage",
                Description = "Increase all damage by <color=#00ff00>+#VALUE%</color> per rank.",
                Value = 10,
                ValueScale = 0.01f,
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponDamage,
                Title = "Main weapon, damage",
                Description = "Increase main weapon damage by <color=#00ff00>+#VALUE%</color> per rank.",
                Value = 15,
                ValueScale = 0.01f,
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponRange,
                Title = "Main weapon, range",
                Description = "Increase main weapon range by <color=#00ff00>+#VALUE%</color> per rank.",
                Value = 15,
                ValueScale = 0.01f,
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponCd,
                Title = "Main weapon, cooldown",
                Description = "Decrease main weapon cooldown by <color=#00ff00>+#VALUE%</color> per rank.",
                Value = 8,
                ValueScale = 0.01f,
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponBulletsPerRound,
                Title = "Main weapon, bullet count",
                Description = "Increase primary weapon bullets per round by <color=#00ff00>+#VALUE</color> per rank.",
                Value = 1,
                ValueScale = 1,
            },

            new ShopItem
            {
                ItemType = ShopItemType.MoreMoney,
                Title = "More money",
                Description = "Increase income from all sources by <color=#00ff00>+#VALUE%</color> per rank.",
                MaxLevel = 3,
                BasePrice = 1000,
                Value = 20,
                ValueScale = 0.01f,
            },

            new ShopItem
            {
                ItemType = ShopItemType.Refund,
                ShowLevelInTitle = false,
                Title = "Refund",
                BasePrice = 0,
                Description = "Refund all purchases and get your money back.",
                GetButtonText = (_) => "Free",
            },
        };

        foreach (var item in Items)
            item.Description = item.Description.Replace("#VALUE", item.Value.ToString());
    }
}
