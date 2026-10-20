using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Fake Steam until the game has a Steam app: always "running through Steam", init always works, achievements are
// remembered for this run only. Every call is logged (SteamLog) so the integration can be checked in steam.log.
// Item drops: a fake playtime generator - one drop per DropIntervalMinutes of play, rarity rolled from
// SteamItemDefs weights. The inventory is kept in steam-stub-inventory.json next to the save (not in it: like on Steam,
// items survive deleting the save).
public class SteamStub : ISteamBackend
{
    readonly HashSet<string> _achievements = new HashSet<string>();
    bool _pendingStore;

    public string Name => "steam-stub";

    public bool RestartAppIfNecessary(uint appId)
    {
        SteamLog.Write($"RestartAppIfNecessary({appId}) -> false (stub: pretend we were launched by Steam)");
        return false;
    }

    // Test the offline path: start the game with -steam-offline, or set this to true in the Editor.
    const bool SimulateInitFailure = false;

    public bool Init(uint appId)
    {
        if (SimulateInitFailure || System.Environment.CommandLine.Contains("-steam-offline"))
        {
            SteamLog.Write($"Init({appId}) -> false (stub: simulated offline)");
            return false;
        }
        SteamLog.Write($"Init({appId}) -> true (stub), user '{PersonaName}' id {SteamId}");
        LoadInventory();
        return true;
    }

    public void RunCallbacks()
    {
        // Playtime for the drop timer. Capped per frame: waking from sleep must not count the sleep as play.
        Inv.PlaySecondsSinceDrop += Mathf.Min(Time.unscaledDeltaTime, 1f);
    }

    public void Shutdown()
    {
        SaveInventory();
        SteamLog.Write("Shutdown");
    }

    public string PersonaName => "StubPlayer";
    public ulong SteamId => 76561190000000000UL;

    public bool SetAchievement(string apiName)
    {
        if (!_achievements.Add(apiName))
            return false;
        SteamLog.Write($"SetAchievement({apiName})");
        _pendingStore = true;
        return true;
    }

    public void StoreStats()
    {
        if (!_pendingStore)
            return;
        _pendingStore = false;
        SteamLog.Write($"StoreStats ({_achievements.Count} achievements set this run)");
    }

#pragma warning disable CS0067 // the stub has no overlay
    public event Action<bool> OverlayActivated;
#pragma warning restore CS0067

    // ---- Inventory ----

    // Test faster: start the game with -steam-fastdrops (one drop per minute of play).
    static float DropIntervalMinutes => Environment.CommandLine.Contains("-steam-fastdrops") ? 1f : 30f;

    [Serializable]
    class StubInventory
    {
        public List<SteamItem> Items = new List<SteamItem>();
        public float PlaySecondsSinceDrop;
        public ulong NextInstanceId = 1;
    }

    StubInventory _inv;

    public event Action<SteamItem[]> ItemsGranted;
    public event Action<SteamItem[]> InventoryUpdated;

    static string InventoryPath => Path.Combine(Application.persistentDataPath, "steam-stub-inventory.json");

    StubInventory Inv
    {
        get
        {
            if (_inv == null)
                LoadInventory();
            return _inv;
        }
    }

    void LoadInventory()
    {
        _inv = new StubInventory();
        try
        {
            if (File.Exists(InventoryPath))
                _inv = JsonUtility.FromJson<StubInventory>(File.ReadAllText(InventoryPath)) ?? new StubInventory();
            SteamLog.Write($"Stub inventory loaded: {_inv.Items.Count} items, {_inv.PlaySecondsSinceDrop / 60f:0.0} min played toward next drop");
        }
        catch (Exception e)
        {
            SteamLog.Write($"Stub inventory unreadable, starting empty: {e.Message}");
            _inv = new StubInventory();
        }
    }

    void SaveInventory()
    {
        if (_inv == null)
            return;
        try
        {
            File.WriteAllText(InventoryPath, JsonUtility.ToJson(_inv, true));
        }
        catch (Exception e)
        {
            SteamLog.Write($"Stub inventory not saved: {e.Message}");
        }
    }

    public void TriggerItemDrop(int generatorDefId)
    {
        var inv = Inv;
        float needed = DropIntervalMinutes * 60f;
        if (inv.PlaySecondsSinceDrop < needed)
        {
            SteamLog.Write($"TriggerItemDrop({generatorDefId}) -> no drop due ({inv.PlaySecondsSinceDrop / 60f:0.0} / {DropIntervalMinutes:0.#} min)");
            SaveInventory();
            return;
        }

        inv.PlaySecondsSinceDrop -= needed;
        if (inv.PlaySecondsSinceDrop > needed)
            inv.PlaySecondsSinceDrop = needed; // like Steam's drop window: no backlog of many drops at once

        var item = new SteamItem { InstanceId = inv.NextInstanceId++, DefId = RollDef() };
        inv.Items.Add(item);
        SaveInventory();
        SteamLog.Write($"TriggerItemDrop({generatorDefId}) -> granted {item}");
        ItemsGranted?.Invoke(new[] { item });
        InventoryUpdated?.Invoke(inv.Items.ToArray());
    }

    static int RollDef()
    {
        int total = 0;
        foreach (var d in SteamItemDefs.All)
            total += d.Weight;
        int roll = UnityEngine.Random.Range(0, total);
        foreach (var d in SteamItemDefs.All)
        {
            if (roll < d.Weight)
                return d.Id;
            roll -= d.Weight;
        }
        return SteamItemDefs.All[0].Id;
    }

    public void RequestInventory()
    {
        var items = Inv.Items.ToArray();
        SteamLog.Write($"RequestInventory -> {items.Length} items" + (items.Length > 0 ? ": " + string.Join(", ", items) : ""));
        InventoryUpdated?.Invoke(items);
    }
}
