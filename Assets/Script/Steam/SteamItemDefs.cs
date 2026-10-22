using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary }

// The Steam item definitions: Earl and Scary Earl (legendary) plus every bestiary beast, rarer the later it shows up
// in the arena. Single source for the game (drops, item drop dialog) and the editor export (SteamAssetExport, which
// writes the icons and the itemdefs JSON uploaded on the Steamworks site). Ids: Earl 1, Scary Earl 2, beasts
// BeastIdOffset + ActorTypeEnum value; never change an id once it's on Steam.
public static class SteamItemDefs
{
    public const int PlaytimeGenerator = 1000; // the "playtimegenerator" item holding the drop table
    public const int BeastIdOffset = 100;

    public static readonly Color32[] RarityColors =
    {
        new Color32(157, 165, 176, 255), // Common: steel grey
        new Color32(79, 191, 90, 255),   // Uncommon: green
        new Color32(61, 139, 255, 255),  // Rare: blue
        new Color32(168, 85, 247, 255),  // Epic: purple
        new Color32(245, 184, 46, 255),  // Legendary: gold
    };

    // Drop weight per item of that rarity (relative). The stub rolls with these; on Steam the itemdefs bundle does.
    public static readonly int[] RarityWeights = { 100, 35, 12, 4, 1 };

    public class Def
    {
        public int Id;
        public string Name;
        public string Description;
        public ItemRarity Rarity;
        public SkinAnimation Skin = SkinAnimation.NotSet;   // Earl / Scary Earl: icon art from this skin
        public ActorTypeEnum Beast = ActorTypeEnum.None;    // beasts: icon art from this enemy

        public int Weight => RarityWeights[(int)Rarity];
        public Color32 Color => RarityColors[(int)Rarity];
        public string IconName => $"item_{Id:000}_{FileSafe(Name)}"; // sprite / png name (Texture/ItemDrops)
    }

    static Def[] _all;
    public static Def[] All => _all ??= Build();

    static Def[] Build()
    {
        var items = new List<Def>
        {
            new Def { Id = 1, Name = "Earl", Description = "Good ol' Earl. The hero himself - the rarest find of all.",
                Rarity = ItemRarity.Legendary, Skin = SkinAnimation.Default },
            new Def { Id = 2, Name = "Scary Earl", Description = "What hides behind the fog. Almost nobody finds this one.",
                Rarity = ItemRarity.Legendary, Skin = SkinAnimation.ScaryEarl },
        };

        // Beasts in arena order (ActorTypeEnum order); rarity bands: first 40% common, 30% uncommon, 20% rare, last 10% epic.
        var beasts = ((ActorTypeEnum[])Enum.GetValues(typeof(ActorTypeEnum)))
            .Where(t => t != ActorTypeEnum.None && t != ActorTypeEnum.Any)
            .ToArray();
        for (int i = 0; i < beasts.Length; ++i)
        {
            float t = (i + 0.5f) / beasts.Length;
            var rarity = t < 0.4f ? ItemRarity.Common : t < 0.7f ? ItemRarity.Uncommon : t < 0.9f ? ItemRarity.Rare : ItemRarity.Epic;
            var type = beasts[i];
            string name = ActorBase.Names.TryGetValue(type, out var n) ? n : type.ToString();
            items.Add(new Def
            {
                Id = BeastIdOffset + (int)type,
                Name = name,
                Description = $"{name}, a beast from the arena.",
                Rarity = rarity,
                Beast = type,
            });
        }
        return items.ToArray();
    }

    public static Def Get(int id)
    {
        foreach (var d in All)
            if (d.Id == id)
                return d;
        return null;
    }

    public static string FileSafe(string s) => new string(s.Where(char.IsLetterOrDigit).ToArray());
}
