using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Playfab
{
    public static string playerId;
    public static string DisplayStatus = "not logged in";
    static LoginResult LoginRes;

    public const string GameOverEvent = "game_over";
    public const string ItemBoughtEvent = "item_bought";
    public const string ItemsBoughtStat = "items_bought";
    public const string RoundsCompletedStat = "rounds_completed";
    public const string GoldStat = "round_gold";
    public const string LevelStat = "round_level";
    public const string KillsStat = "round_kills";
    public const string SecondsLeftStat = "round_seconds_left";
    public const string TotalSeconds = "total_seconds";
    public const string ChapterBossStartedStat = "chapter_1_boss_started";
    public const string ChapterBossKilledStat = "chapter_1_boss_killed";
    public const string MaxSecondsReached = "max_seconds_reached";

    const string Version = "1.0";

    const string PlayerIdKey = "playerId";

    static void RecoverSettingsFromOldFileLocation()
    {
        string idPath = Path.Combine(Application.persistentDataPath, PlayerIdKey);
        if (File.Exists(idPath))
        {
            string id = File.ReadAllText(idPath);
            PlayerPrefs.SetString(PlayerIdKey, id);
            File.Delete(idPath);
        }
    }

    public static void Login()
    {
        PlayFabSettings.TitleId = "A45ED";

        RecoverSettingsFromOldFileLocation();

        void CreateNewId()
        {
            string newId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PlayerIdKey, newId);
        }

        if (!PlayerPrefs.HasKey(PlayerIdKey))
            CreateNewId();

        string id = PlayerPrefs.GetString(PlayerIdKey);
        playerId = id;

        var req = new LoginWithCustomIDRequest
        {
            CreateAccount = true,
            CustomId = id,
            TitleId = "A45ED",
        };

        void Callback(LoginResult result)
        {
            LoginRes = result;
            DisplayStatus = "logged in";
            GameManager.Instance.TextUser.text = DisplayStatus;
            Debug.Log($"login successful, id: {result.PlayFabId}, created: {result.NewlyCreated}");
        }

        void ErrorCallback(PlayFabError result)
        {
            DisplayStatus = "error logging in";
            GameManager.Instance.TextUser.text = DisplayStatus;

            Debug.LogError($"login error: {result}");
        }

        PlayFabClientAPI.LoginWithCustomID(req, Callback, ErrorCallback);
    }

    public static void PlayerEvent(string eventName, Dictionary<string, object> properties)
    {
        var req = new WriteClientPlayerEventRequest
        {
            Body = properties,
            CustomTags = new Dictionary<string, string> { { "version", Version } },
            EventName = eventName,
            AuthenticationContext = new PlayFabAuthenticationContext
            {
                ClientSessionTicket = LoginRes.SessionTicket,
                PlayFabId = LoginRes.PlayFabId,
            }
        };

        void Callback(WriteEventResponse res)
        {
            Debug.Log($"event {eventName} sent");
        }

        void ErrorCallback(PlayFabError result)
        {
            Debug.LogError($"error sending event {eventName}: {result}");
        }

        PlayFabClientAPI.WritePlayerEvent(req, Callback, ErrorCallback);
    }

    public static void PlayerStat(Dictionary<string, int> stats)
    {
        var req = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>(),
            AuthenticationContext = new PlayFabAuthenticationContext
            {
                ClientSessionTicket = LoginRes.SessionTicket,
                PlayFabId = LoginRes.PlayFabId,
            }
        };

        foreach(var pair in stats)
        {
            req.Statistics.Add(new StatisticUpdate { StatisticName = pair.Key, Value = pair.Value });
        }

        void Callback(UpdatePlayerStatisticsResult res)
        {
            Debug.Log($"stats {stats} sent");
        }

        void ErrorCallback(PlayFabError result)
        {
            Debug.LogError($"error sending stats {stats}: {result}");
        }

        PlayFabClientAPI.UpdatePlayerStatistics(req, Callback, ErrorCallback);
    }

    public static void PlayerStat(string statName, int value)
    {
        PlayerStat(new Dictionary<string, int> { { statName, value } });
    }
}
