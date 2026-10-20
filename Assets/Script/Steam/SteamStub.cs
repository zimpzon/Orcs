using System;
using System.Collections.Generic;

// Fake Steam until the game has a Steam app: always "running through Steam", init always works, achievements are
// remembered for this run only. Every call is logged (SteamLog) so the integration can be checked in steam.log.
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
        return true;
    }

    public void RunCallbacks()
    {
    }

    public void Shutdown()
    {
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
}
