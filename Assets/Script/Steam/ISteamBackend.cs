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
    void StoreStats();

    event Action<bool> OverlayActivated;
}
