using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ShopItemType
{
    DamageAll,
    WeaponRange,
    WeaponDamage,
    WeaponCd,
    MachinegunBulletsPerRound,
    DoubleMoney,
    EnemyMoneyDrop,
    KillXpChanceMul,

    UnlockPoisonDagger,
    BurstOfFrost,
    Sawblade,
    MeleeThrow,
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
    public Func<int, string> GetLevelText;
    public Action<BoughtItem> Apply;

    readonly StringBuilder sb_ = new ();

    public ShopItem()
    {
        GetButtonText = (int level) => $"${GetPrice(level)}";
        GetTitle = (int level) => $"{Title}";
        GetDescription = (int level) => Description;

        GetLevelText = (int level) =>
        {
            char c = '@';

            sb_.Clear();
            if (level > 0)
            {
                sb_.Append("<color=\"#00ff00\">");
                for (int i = 0; i < level; ++i)
                {
                    sb_.Append(c);
                }
                sb_.Append("</color>");
            }

            for (int i = 0; i < MaxLevel - level; ++i)
            {
                sb_.Append(c);
            }

            return sb_.ToString();
        };
    }
}

public static class ShopItems
{
    public static List<ShopItem> Items = new ();
    public static List<ShopItemScript> Scripts = new ();

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
            var shopItem = Items.First(i => i.ItemType == bought.ItemType);
            shopItem.Apply(bought);
        }
    }

    private static void OnBuy(ShopItemType itemType)
    {
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
            bool disableBuyButton = !canAfford || isMaxLevel;

            var script = Scripts[i];
            script.Title.text = item.GetTitle(level);
            script.Description.text = item.GetDescription(level);
            script.Level.text = item.GetLevelText(level);
            script.ButtonText.text = item.GetButtonText(level);
            script.SetIsMaxed(isMaxLevel);
            script.SetDisableButton(disableBuyButton);
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
                BasePrice = 25,
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
        Items.AddRange(ShopItemsUnlock.GetUnlockItems());

        foreach (var item in Items)
            item.Description = item.Description.Replace("#VALUE", item.Value.ToString());
    }
}
