using System.Collections;
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

    // Upgrades
    public long LevelClickDamage = 0;
    public long LevelGoldPerKnifeThrown = 0;
    public long LevelKnifeDamage = 0;
    public long LevelKnifeCooldown = 0;

    // Game
    public long Money = 0;
    public long CurrentLevel = 1;
    public long AscendLevel = 1;

    // Settings
    public int Version;
    public float VolumeMaster = 1.0f;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;
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
    public static SaveGameMembers Members = new();

    internal static void ResetAll()
    {
        Members = new();
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
