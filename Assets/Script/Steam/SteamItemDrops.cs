using System;
using UnityEngine;

// Steam item drops: asks Steam for a playtime drop now and then (Steam decides whether one is due, see SteamItemDefs),
// keeps the player's current item list, and announces new drops. Driven by SteamManager; does nothing without Steam.
// What items do in the game is not decided yet - read Items for that.
public static class SteamItemDrops
{
    const float FirstTriggerDelay = 10f;      // after start, so the arena (and any start popup) is up first
    // How often to ask; the real drop rate is set on Steam. -steam-fastdrops: ask often too (see SteamStub).
    static readonly float TriggerIntervalSeconds = Environment.CommandLine.Contains("-steam-fastdrops") ? 20f : 300f;

    public static SteamItem[] Items { get; private set; } = Array.Empty<SteamItem>();
    public static event Action ItemsChanged;

    static float _nextTrigger = FirstTriggerDelay;
    static bool _inventoryRequested;

    public static void Attach(ISteamBackend backend)
    {
        backend.InventoryUpdated += items =>
        {
            Items = items;
            ItemsChanged?.Invoke();
        };
        backend.ItemsGranted += Announce;
    }

    public static void Tick(ISteamBackend backend)
    {
        if (GameCanvasScript.Instance == null)
            return; // still booting

        if (!_inventoryRequested)
        {
            _inventoryRequested = true;
            backend.RequestInventory();
        }

        _nextTrigger -= Time.unscaledDeltaTime;
        if (_nextTrigger > 0f)
            return;
        _nextTrigger = TriggerIntervalSeconds;
        backend.TriggerItemDrop(SteamItemDefs.PlaytimeGenerator);
    }

    static void Announce(SteamItem[] granted)
    {
        foreach (var item in granted)
        {
            var def = item.Def;
            string text = def != null ? $"You found a {def.Rarity} item:\n{def.Name}!" : "You found a new item!";
            if (GameCanvasScript.Instance != null)
                GameCanvasScript.Instance.ShowPopup(text, celebrate: true);
        }
    }
}
