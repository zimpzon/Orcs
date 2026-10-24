using System;
using UnityEngine;

// Steam item drops: asks Steam for a playtime drop now and then (Steam decides whether one is due, see SteamItemDefs),
// keeps the player's current item list, and announces new drops. Driven by SteamManager; does nothing without Steam.
// What items do in the game is not decided yet - read Items for that.
//
// Countdown to the next drop (SecondsToNextDrop, TextNextSteamDrop): Steam can't be asked how long is left, so it's
// our own estimate. Steam's playtime generator only counts time the game is running, so we count the same: play time
// since the last drop, kept per Steam account in PlayerPrefs (not in the save - Steam's timer survives save wipes and
// rebirths). Every drop resets it to 0, which re-syncs it with Steam; before the first drop it counts from the first
// launch. Must match drop_interval on the playtimegenerator itemdef on Steamworks.
public static class SteamItemDrops
{
    // -steam-fastdrops: 1 minute (see SteamStub).
    public static readonly float DropIntervalMinutes = Environment.CommandLine.Contains("-steam-fastdrops") ? 1f : 180f;

    const float FirstTriggerDelay = 10f;      // after start, so the arena (and any start popup) is up first
    // How often to ask while the countdown runs (in case it's off), and once it says a drop is due.
    static readonly float TriggerIntervalSeconds = Environment.CommandLine.Contains("-steam-fastdrops") ? 20f : 300f;
    const float DueTriggerIntervalSeconds = 20f;
    const float MaxPlayDeltaSeconds = 1f;     // per frame: hitches / sleep don't count, like the stub
    const float SavePlaytimeInterval = 10f;

    public static SteamItem[] Items { get; private set; } = Array.Empty<SteamItem>();
    public static event Action ItemsChanged;

    // Null without Steam (WebGL, Steam not running); 0 = due, the drop comes any moment.
    public static float? SecondsToNextDrop => _playLoaded ? Mathf.Max(0f, DropIntervalMinutes * 60f - _playSeconds) : null;

    static float _nextTrigger = FirstTriggerDelay;
    static bool _inventoryRequested;
    static bool _playLoaded;
    static float _playSeconds;
    static float _nextPlaySave = SavePlaytimeInterval;
    static string _playKey;

    public static void Attach(ISteamBackend backend)
    {
        backend.InventoryUpdated += items =>
        {
            Items = items;
            ItemsChanged?.Invoke();
        };
        backend.ItemsGranted += granted =>
        {
            _playSeconds = 0f;
            SavePlaytime();
            Announce(granted);
        };
    }

    public static void Tick(ISteamBackend backend)
    {
        if (!_playLoaded)
        {
            _playKey = $"SteamDropPlaySeconds_{backend.SteamId}";
            _playSeconds = Mathf.Clamp(PlayerPrefs.GetFloat(_playKey, 0f), 0f, DropIntervalMinutes * 60f);
            _playLoaded = true;
        }
        float dt = Mathf.Min(Time.unscaledDeltaTime, MaxPlayDeltaSeconds);
        _playSeconds = Mathf.Min(_playSeconds + dt, DropIntervalMinutes * 60f);
        _nextPlaySave -= dt;
        if (_nextPlaySave <= 0f)
        {
            _nextPlaySave = SavePlaytimeInterval;
            SavePlaytime();
        }

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
        _nextTrigger = SecondsToNextDrop == 0f ? DueTriggerIntervalSeconds : TriggerIntervalSeconds;
        backend.TriggerItemDrop(SteamItemDefs.PlaytimeGenerator);
    }

    static void SavePlaytime()
    {
        if (_playKey == null)
            return;
        PlayerPrefs.SetFloat(_playKey, _playSeconds);
        PlayerPrefs.Save();
    }

    static void Announce(SteamItem[] granted)
    {
        foreach (var item in granted)
        {
            ItemDropPopup.Show(item.Def);
            if (item.Def != null)
                EventLog.Add($"Item drop! {item.Def.Name} ({item.Def.Rarity}) was added to your Steam inventory.");
        }
    }
}
