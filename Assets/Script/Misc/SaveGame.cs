﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GameCounter {
    Player_Death,
    Kill_Any, Kill_Small, Kill_BigWalker, Kill_Caster,
    unlocked_paintball,
    Score_Nursery_Sum, Score_Earth_Sum, Score_Wind_Sum, Score_Fire_Sum, Score_Storm_Sum, score_Harmony_Sum, Score_Any_Sum,
    Max_First,
    Max_Score_Nursery, Max_Score_Earth, Max_Score_Wind, Max_Score_Fire, Max_Score_Storm, Max_score_Harmony, Max_Score_Any,
    Max_Last,
    unlocked_sniper, unlocked_slug, unlocked_rambo, unlocked_staff, unlocked_staff2, unlocked_orcs_revenge, Last
};

public class SaveGameMembers
{
    public int Money = 0;
    public int MoneySpentInShop;
    public Dictionary<ShopItemType, BoughtItem> BoughtItems = new Dictionary<ShopItemType, BoughtItem>();
    public ShopItemType[] TempBoughtTypes = new ShopItemType[0];
    public BoughtItem[] TempBoughtItems = new BoughtItem[0];

    public int[] Counters = new int[(int)GameCounter.Last];

    public int Version;
    public int BestKills;
    public int BestScore;
    public float VolumeMaster = 1.0f;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;
    public int SelectedHero;
    public string UserId;

    public int MaxSecondsReached;
    public bool Chapter1BossReached;
    public int TotalSeconds;
    public int OrcsSaved;
    public int EnemiesKilled;
    public int PlayerDeaths;
    public int SecondsPlayed;
    public int Chapter1BossStarted;
    public int Chapter1BossKilled;

    public bool IsMaxCounter(GameCounter counter)
    {
        return counter > GameCounter.Max_First && counter < GameCounter.Max_Last;
    }

    public bool ReqMet(int req, GameCounter counter)
    {
        return Counters[(int)counter] >= req;
    }

    public bool TryUpdateMaxValue(GameCounter maxValue, int newValue)
    {
        Debug.Assert(IsMaxCounter(maxValue));

        if (newValue <= Counters[(int)maxValue])
            return false;

        Counters[(int)maxValue] = newValue;
        return true;
    }

    public void UpdateCounter(GameCounter counter, int amount)
    {
        Debug.Assert(!IsMaxCounter(counter));

        Counters[(int)counter] += amount;
    }

    public void SetCounter(GameCounter counter, int amount)
    {
        Counters[(int)counter] = amount;
    }

    public int GetCounter(GameCounter counter)
    {
        return Counters[(int)counter];
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static SaveGameMembers FromJson(string json)
    {
        return JsonUtility.FromJson<SaveGameMembers>(json);
    }
}

public static class SaveGame
{
    public static SaveGameMembers Members = new SaveGameMembers();

    internal static void ResetAll()
    {
        Members = new SaveGameMembers();
    }

    public static int RoundScore;
    public static int RoundKills;
    public static int RoundGold;

    public static void UpdateFromRound(int roundSeconds, bool reset)
    {
        if (RoundScore > Members.BestScore)
            Members.BestScore = RoundScore;

        if (RoundKills > Members.BestKills)
            Members.BestKills = RoundKills;

        Members.OrcsSaved += RoundScore;
        Members.EnemiesKilled += RoundKills;
        Members.PlayerDeaths++;
        Members.SecondsPlayed += roundSeconds;

        ResetRound();
    }

    public static void ResetRound()
    {
        RoundScore = 0;
        RoundKills = 0;
        RoundGold = 0;
    }

    public static IEnumerator SaveCo()
    {
        Save();
        yield break;
    }

    public static void Save()
    {
        // workaround for dictionary not serialized
        Members.TempBoughtTypes = new ShopItemType[Members.BoughtItems.Count];
        Members.TempBoughtItems = new BoughtItem[Members.BoughtItems.Count];
        Members.BoughtItems.Keys.CopyTo(Members.TempBoughtTypes, 0);
        Members.BoughtItems.Values.CopyTo(Members.TempBoughtItems, 0);

        PlayerPrefs.SetString(SaveGameKey, Members.ToJson());
    }

    const string SaveGameKey = "save.json";

    static string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, SaveGameKey);
    }

    static void RecoverSaveFromOldFileLocation()
    {
        string path = GetPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerPrefs.SetString(SaveGameKey, json);
            File.Delete(path);
        }
    }

    public static void Load()
    {
        RecoverSaveFromOldFileLocation();

        string prefs = string.Empty;
        if (PlayerPrefs.HasKey(SaveGameKey))
            prefs = PlayerPrefs.GetString(SaveGameKey);

        if (!string.IsNullOrWhiteSpace(prefs))
        {
            Members = SaveGameMembers.FromJson(prefs);
            if (Members.Counters.Length != (int)GameCounter.Last)
            {
                // More counters were added since last save.
                var backup = Members.Counters;
                Members.Counters = new int[(int)GameCounter.Last];
                backup.CopyTo(Members.Counters, 0);

                // Enum GameCounters has changed since this was saved. Reset save.
                // Might want to put in a bunch of pladeholders to make this less likely in a release.
                Debug.Log("New counters detected, expanding array");
            }

            // workaround for dictionary not serialized
            Members.BoughtItems.Clear();
            for (int i = 0; i < Members.TempBoughtTypes.Length; ++i)
                Members.BoughtItems[Members.TempBoughtTypes[i]] = Members.TempBoughtItems[i];
        }

        if (Members == null)
            Members = new SaveGameMembers();

        if (string.IsNullOrEmpty(Members.UserId))
        {
            Members.UserId = "timing_" + System.Guid.NewGuid().ToString();
            Save();
        }
    }
}
