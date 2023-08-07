using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Playfab
{
    public static string playerId;
    static LoginResult LoginRes;

    public const string GameOverEvent = "game_over";
    public const string ItemBoughtEvent = "item_bought";
    public const string ItemsBoughtStat = "items_bought";
    public const string RoundsCompletedStat = "rounds_completed";
    public const string GoldStat = "round_gold";
    public const string ScoreStat = "round_score";
    public const string LevelStat = "round_level";
    public const string KillsStat = "round_kills";

    const string Version = "1.0";

    public static void Login()
    {
        PlayFabSettings.TitleId = "A45ED";

        string idPath = Path.Combine(Application.persistentDataPath, "playerId");
        if (!File.Exists(idPath))
        {
            string newId = new Guid().ToString();
            File.WriteAllText(idPath, newId);
        }

        string id = File.ReadAllText(idPath);
        var req = new LoginWithCustomIDRequest
        {
            CreateAccount = true,
            CustomId = id,
            TitleId = "A45ED",
        };

        void Callback(LoginResult result)
        {
            LoginRes = result;
            Debug.Log($"login successful, id: {result.PlayFabId}, created: {result.NewlyCreated}");
        }

        void ErrorCallback(PlayFabError result)
        {
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
            Statistics = new List<StatisticUpdate>()
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
