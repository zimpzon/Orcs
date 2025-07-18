using Assets.Script.Misc;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GameCounter
{
    DamageClicks,
    //Player_Death,
    //Kill_Any, Kill_Small, Kill_BigWalker, Kill_Caster,
    //unlocked_paintball,
    //Score_Nursery_Sum, Score_Earth_Sum, Score_Wind_Sum, Score_Fire_Sum, Score_Storm_Sum, score_Harmony_Sum, Score_Any_Sum,
    //Max_First,
    //Max_Score_Nursery, Max_Score_Earth, Max_Score_Wind, Max_Score_Fire, Max_Score_Storm, Max_score_Harmony, Max_Score_Any,
    //Max_Last,
    //unlocked_sniper, unlocked_slug, unlocked_rambo, unlocked_staff, unlocked_staff2, unlocked_orcs_revenge, Last
};

public class SaveGameMembers
{
    // Stats
    public long CountDamageClicks = 0;

    // Values
    public CalculatedValue ClickDamageProgress = new CalculatedValue
    {
        BaseValue = 5,
        BaseValueAdd = 1,
    };

    public CalculatedValue MoneyPerCoinProgress = new CalculatedValue
    {
        BaseValue = 1,
        BaseValueCountMul = 1.2,
    };

    public CalculatedValue EnemyHpProgress = new CalculatedValue
    {
        BaseValue = 100,
        BaseValueCountMul = 1.2,
    };

    public CalculatedValue KnifeDamageProgress = new CalculatedValue
    {
        BaseValue = 10,
        BaseValueCountMul = 1.2,
    };

    public HardcodedValue KnifeCdProgress = new HardcodedValue
    {
        Values = new List<HardcodedValue.Item>
        {
            new() { RequiredLevel = 1, Value = 2.0f },
            new() { RequiredLevel = 2, Value = 1.5f },
            new() { RequiredLevel = 3, Value = 1.0f },
            new() { RequiredLevel = 5, Value = 0.8f },
            new() { RequiredLevel = 8, Value = 0.6f },
            new() { RequiredLevel = 12, Value = 0.4f },
            new() { RequiredLevel = 20, Value = 0.2f },
            new() { RequiredLevel = 30, Value = 0.1f },
            new() { RequiredLevel = 50, Value = 0.05f },
        }
    };

    public CalculatedValue KnifeGoldPerThrow = new CalculatedValue
    {
        BaseValue = 1,
        BaseValueAdd = 2,
    };

    // Game
    public long Money = 0;
    public long CurrentLevel = 1;
    public long AscendLevel = 1;
    public long MaxCompletedLevel = 1;
    public long SecondsPerRound = 30;

    // Settings
    public int Version;
    public float VolumeMaster = 1.0f;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;
    public int SelectedHero;
    public string UserId;

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

    public static IEnumerator SaveCo()
    {
        Save();
        yield break;
    }

    public static void Save()
    {
        string json = Members.ToJson();
        Debug.Log("saving json: " + json);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("saving WEBGL");
            JsMappings.Save(json);
        }
        else
        {
            Debug.Log("saving prefs");
            PlayerPrefs.SetString(SaveGameKey, json);
        }
    }

    const string SaveGameKey = "idle-knight-save.json";

    static string GetPath()
    {
        return Path.Combine(Application.persistentDataPath, SaveGameKey);
    }

    public static void Load()
    {
        Debug.Log("PLATFORM: " + Application.platform);

        string prefs = string.Empty;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("loading from JS");
            prefs = JsMappings.Load();
        }
        else
        {
            Debug.Log("loading from PREFS");
            if (PlayerPrefs.HasKey(SaveGameKey))
                prefs = PlayerPrefs.GetString(SaveGameKey);
        }

        Debug.Log("json = " + prefs);
        if (!string.IsNullOrWhiteSpace(prefs))
        {
            Members = SaveGameMembers.FromJson(prefs);
        }

        Members ??= new SaveGameMembers();

        if (string.IsNullOrEmpty(Members.UserId))
        {
            Members.UserId = "userId-" + System.Guid.NewGuid().ToString();
            Save();
        }
    }
}
