using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShopItemType { Refund, DamageAll, PrimaryWeaponDamage, PrimaryWeaponCd, PrimaryWeaponBulletsPerRound, Rambo, MoreMoney }

[Serializable]
public class BoughtItem
{
    public ShopItemType ItemType;
    public int Level = 0;
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

        SaveGame.Members.BoughtItems.TryGetValue(itemType, out var bought);
        bought ??= new BoughtItem { ItemType = itemType };

        var item = Items.Where(i => i.ItemType == itemType).First();
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
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponDamage,
                Title = "Main weapon, damage",
                Description = "Increase main weapon damage by <color=#00ff00>+#VALUE%</color> per rank.",
                Value = 15,
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponCd,
                Title = "Main weapon, cooldown",
                Description = "Decrease main weapon cooldown by <color=#00ff00>+#VALUE%</color> per rank."
            },

            new ShopItem
            {
                ItemType = ShopItemType.PrimaryWeaponBulletsPerRound,
                Title = "Main weapon, bullets",
                Description = "Increase primary weapon bullets per round by <color=#00ff00>+#VALUE</color> per rank."
            },

            new ShopItem
            {
                ItemType = ShopItemType.Rambo,
                Title = "Sir Rambo",
                Description = "Randomly go beserk for a damage burst.",
                BasePrice = 1000,
            },

            new ShopItem
            {
                ItemType = ShopItemType.MoreMoney,
                Title = "More money",
                Description = "Increase income from all sources by <color=#00ff00>+#VALUE%</color> per rank."
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
