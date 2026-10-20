using System;

// What the game needs from Steam. SteamStub fakes it (no Steam app yet); the real one will be a Steamworks.NET
// backend under a STEAMWORKS scripting define. SteamManager picks the backend.
public interface ISteamBackend
{
    string Name { get; }

    // True = the game wasn't started through Steam: Steam relaunches it, so this instance must quit right away.
    bool RestartAppIfNecessary(uint appId);
    bool Init(uint appId);
    void RunCallbacks();
    void Shutdown();

    string PersonaName { get; }
    ulong SteamId { get; }

    // False if it was already set.
    bool SetAchievement(string apiName);
    // Stat API names must exist on the Steamworks site (Stats & Achievements), type INT.
    void SetStat(string apiName, int value);
    void StoreStats();

    event Action<bool> OverlayActivated;

    // Inventory (item drops). Steam decides if a drop is due (playtime generator); a drop raises ItemsGranted with
    // the new items, RequestInventory raises InventoryUpdated with everything the player owns.
    void TriggerItemDrop(int generatorDefId);
    void RequestInventory();
    event Action<SteamItem[]> ItemsGranted;
    event Action<SteamItem[]> InventoryUpdated;
}
