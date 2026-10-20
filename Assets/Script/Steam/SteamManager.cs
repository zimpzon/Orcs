using System.Collections.Generic;
using UnityEngine;

// Steam entry point. Starts before the first scene, pumps callbacks every frame, shuts down on quit. Not on WebGL
// (Newgrounds/PlayFab only) - there Active is false and every call does nothing.
// Steam Cloud needs no code: set up Auto-Cloud on the Steamworks site for the save file + backups/ (SaveGame).
// Achievements: the Steam API name is the Achieved enum name - enter the same names on the Steamworks site.
public class SteamManager : MonoBehaviour
{
    // Placeholder: Valve's Spacewar test app. Replace with the real app id (and steam_appid.txt next to the exe for
    // dev builds) once the game has a Steam page.
    public const uint AppId = 480;

    static ISteamBackend _backend;

    public static bool Active => _backend != null;
    // Steam should be there (desktop) but init failed: playing offline, achievements aren't sent to Steam.
    public static bool InitFailed { get; private set; }
    public static string PlatformName => Active ? _backend.Name : "web";
    public static ulong SteamId => Active ? _backend.SteamId : 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Startup()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            return;

        SteamLog.Open();
        var backend = new SteamStub();
        SteamLog.Write($"Starting {backend.Name}, app id {AppId}, {Application.platform}, game {Application.version}");
        SteamLog.Write($"Save folder (Steam Cloud / Auto-Cloud root): {Application.persistentDataPath}");

        if (backend.RestartAppIfNecessary(AppId))
        {
            SteamLog.Write("Not started through Steam: quitting, Steam relaunches the game");
            SteamLog.Close();
            Application.Quit();
            return;
        }

        if (!backend.Init(AppId))
        {
            SteamLog.Write("Init failed (Steam not running?): playing without Steam");
            InitFailed = true;
            return;
        }

        _backend = backend;
        _backend.OverlayActivated += active => SteamLog.Write($"Overlay {(active ? "opened" : "closed")}");

        var go = new GameObject("SteamManager");
        DontDestroyOnLoad(go);
        go.AddComponent<SteamManager>();
    }

    public static void UnlockAchievement(Achieved achieved)
    {
        if (Active && _backend.SetAchievement(achieved.ToString()))
            _backend.StoreStats();
    }

    // At load: everything the save has already earned (offline, before Steam, on another machine via the cloud save).
    public static void SyncAchievements(IEnumerable<Achieved> achieved)
    {
        if (!Active || achieved == null)
            return;
        int count = 0;
        foreach (var a in achieved)
        {
            _backend.SetAchievement(a.ToString());
            ++count;
        }
        SteamLog.Write($"Synced {count} achievements from the save");
        _backend.StoreStats();
    }

    void Update()
    {
        _backend?.RunCallbacks();
    }

    void OnApplicationQuit()
    {
        if (_backend == null)
            return;
        _backend.StoreStats();
        _backend.Shutdown();
        _backend = null;
        SteamLog.Close();
    }
}
