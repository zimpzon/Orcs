using System;

// One item in the player's Steam inventory. InstanceId is unique per owned copy; DefId points into SteamItemDefs
// (= the itemdefs on the Steamworks site).
[Serializable]
public struct SteamItem
{
    public ulong InstanceId;
    public int DefId;

    public SteamItemDefs.Def Def => SteamItemDefs.Get(DefId);
    public override string ToString() => $"#{InstanceId} {Def?.Name ?? "?"} ({Def?.Rarity ?? "?"}, def {DefId})";
}
