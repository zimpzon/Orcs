using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Playfab
{
    public static string playerId;
    public static string DisplayStatus = "not logged in";
    static LoginResult LoginRes;
    static bool LoginComplete => LoginRes is not null;

    public const string SendStatsEvent = "send_stats_event";

    public const string GameTimeAccumulated = "game_time_accumulated";
    public const string RealTimeAccumulated = "real_time_accumulated";
    public const string ArenaLevel = "arena_level";
    public const string ArenaTotalIncome = "arena_total_income";
    public const string PassiveTotalIncome = "psssive_total_income";

    const string Version = "1.0";

    const string PlayerIdKey = "playerId";

    public static void Login()
    {
        PlayFabSettings.TitleId = "4EE3"; // Haps

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
            TitleId = PlayFabSettings.TitleId,
        };

        void Callback(LoginResult result)
        {
            LoginRes = result;
            DisplayStatus = "logged in";
            //GameManager.Instance.TextUser.text = DisplayStatus;

            Debug.Log($"login successful, id: {result.PlayFabId}, created: {result.NewlyCreated}");
            Debug.Log($"Sending platform info ({Application.platform})");
            SendPlatformInfo();
        }

        void ErrorCallback(PlayFabError result)
        {
            DisplayStatus = "error logging in";
            GameManager.Instance.TextUser.text = DisplayStatus;

            Debug.LogError($"login error: {result}");
        }

        PlayFabClientAPI.LoginWithCustomID(req, Callback, ErrorCallback);
    }

    static void SendPlatformInfo()
    {
        var data = new Dictionary<string, string>
        {
            { "Platform", Application.platform.ToString() },
            { "DeviceModel", SystemInfo.deviceModel },
            { "OS", SystemInfo.operatingSystem },
            { "UnityVersion", Application.unityVersion },
        };

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = data,
            Permission = UserDataPermission.Public // Optional, makes it visible in dashboard
        },
        result => Debug.Log("Platform info sent to PlayFab"),
        error => Debug.LogError("Failed to send platform info: " + error.GenerateErrorReport()));
    }

    public static void PlayerEvent(string eventName, Dictionary<string, object> properties)
    {
        if (!LoginComplete)
        {
            Debug.Log("Cannot send event, login not complete.");
            return;
        }

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
        if (!LoginComplete)
        {
            Debug.Log("Cannot send player stats, login not complete.");
            return;
        }

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
