using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ShopItemType
{
    Refund,
    DamageAll,
    WeaponRange,
    WeaponDamage,
    WeaponCd,
    MachinegunBulletsPerRound,
    DoubleMoney,
    EnemyMoneyDrop,
    OrcPickupXpMul,
    KillXpChanceMul,
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

    public int GetPrice(int level) => (int)(BasePrice * Math.Pow(PriceMultiplier, level));
    public Func<int, string> GetButtonText;
    public Func<int, string> GetTitle;
    public Func<int, string> GetDescription;
    public Action<BoughtItem> Apply;

    public ShopItem()
    {
        GetButtonText = (int level) => $"${GetPrice(level)}";
        GetTitle = (int level) => $"{Title}";
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
            var bought = pair.Value;
            var shopItem = ShopItems.Items.First(i => i.ItemType == bought.ItemType);
            shopItem.Apply(bought);
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
                BasePrice = 500,
                PriceMultiplier = 2.0f,
                MaxLevel = 3,
                Value = 5,
                ValueScale = 0.01f,
                Apply = (bought) =>
                {
                    PlayerUpgrades.Data.DamageMul += bought.Level * bought.Value * bought.ValueScale;
                }
            },
        };

        Items.AddRange(ShopItemsWeapons.GetWeaponItems());
        Items.AddRange(ShopItemsMoneyXp.GetMoneyXpItems());

        // Refund last
        Items.Add(new ShopItem
        {
            ItemType = ShopItemType.Refund,
            Title = "Refund",
            BasePrice = 0,
            Description = "Refund all purchases and get your money back.",
            GetButtonText = (_) => "Free",
            Apply = (_) => { }
        });

        foreach (var item in Items)
            item.Description = item.Description.Replace("#VALUE", item.Value.ToString());
    }
}
