// The game's copy of the Steam item definitions. Must match the itemdefs JSON uploaded on the Steamworks site: there
// a "playtimegenerator" item with id PlaytimeGenerator holds the drop table ("1x60;2x25;3x12;4x3" style weights),
// drop_interval in minutes of play, drop_window and drop_limit. Placeholder items until the real ones are designed.
public static class SteamItemDefs
{
    public const int PlaytimeGenerator = 1000;

    public class Def
    {
        public int Id;
        public string Name;
        public string Rarity;
        public int Weight; // drop chance relative to the others (only the stub rolls with it; Steam uses its own table)
    }

    public static readonly Def[] All =
    {
        new Def { Id = 1, Name = "Rusty Trinket", Rarity = "Common", Weight = 60 },
        new Def { Id = 2, Name = "Polished Charm", Rarity = "Uncommon", Weight = 25 },
        new Def { Id = 3, Name = "Gilded Relic", Rarity = "Rare", Weight = 12 },
        new Def { Id = 4, Name = "Crown Shard", Rarity = "Legendary", Weight = 3 },
    };

    public static Def Get(int id)
    {
        foreach (var d in All)
            if (d.Id == id)
                return d;
        return null;
    }
}
