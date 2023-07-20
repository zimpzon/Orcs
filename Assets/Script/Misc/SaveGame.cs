using System;
using System.Collections;
using System.Collections.Generic;
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
    public int Money;
    public int MoneySpentInShop;
    public Dictionary<ShopItemType, BoughtItem> BoughtItems = new Dictionary<ShopItemType, BoughtItem>();
    public ShopItemType[] TempBoughtTypes = new ShopItemType[0];
    public BoughtItem[] TempBoughtItems = new BoughtItem[0];

    public int[] Counters = new int[(int)GameCounter.Last];

    public int Version;
    public int BestKills;
    public int BestScore;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;
    public int SelectedHero;
    public string UserId;

    public int OrcsSaved;
    public int EnemiesKilled;
    public int PlayerDeaths;
    public int SecondsPlayed;

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

        PlayerPrefs.SetString(prefs, Members.ToJson());
        PlayerPrefs.Save();
    }

    public static bool RestoreOldPrefs()
    {
        if (PlayerPrefs.HasKey("prefs_"))
        {
            var restoredV1 = SaveGameMembers.FromJson(PlayerPrefs.GetString("prefs_"));
            if (restoredV1.BestScore > 0)
            {
                Members = restoredV1;
                Save();
                return true;
            }
        }

        if (PlayerPrefs.HasKey("prefs"))
        {
            var restoredV0 = SaveGameMembers.FromJson(PlayerPrefs.GetString("prefs"));
            if (restoredV0.BestScore > 0)
            {
                Members = restoredV0;
                Save();
                return true;
            }
        }

        return false;
    }

    static string prefs = "prefs_1";

    public static void Load()
    {
        try
        {
            // hmm, init?
            string prefs = PlayerPrefs.GetString(SaveGame.prefs);

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
        catch (Exception e)
        {
            GameManager.SetDebugOutput("error loading settings", e.Message);
            Members = null;
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
