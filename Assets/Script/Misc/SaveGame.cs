using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameMembers
{
    // ---------------------- Permanent data, stay after ascend ----------------------

    // SaveGameAscend must be kept up to date with this. Should probably create total and per-round stats.

    // Geez one is a left-over. I think only PlayerId is important.
    public string PlayerId = string.Empty;
    public string UserId;

    public string LastSeenUtcStr = string.Empty;

    // Ascending
    public Decimal256 MonsterCreditsXp_09_08_2025;

    public long TimesAscended_09_08_2025;
    public long MonsterCredits_09_08_2025;
    public long MonsterCreditsLifetime_09_08_2025;
    public long DiamondCount_09_08_2025;

    // Permanent upgrades
    public bool BoughtPassiveX2_1 = false;
    public bool BoughtPassiveX2_2 = false;
    public bool BoughtPassiveX4_1 = false;

    // Settings
    public int Version;
    public float VolumeMaster = 1.0f;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;

    public double EstimatedOnlineSeconds2 = 0;

    public List<Achieved> Achieved = new List<Achieved>();

    // ---------------------- Deleted after ascend ----------------------


    // Prevent overwriting save in case of Members reset (happens in editor on crash on code change).
    public bool SaveKillSwitch_CanSave = false;

    // Tracking
    public double QuestionMarkRealTimeLeft = -1;

    // Time
    public string TimeLastSeenUtc;
    public double LifeTimeSessionSeconds;

    // Stats
    public long ChestsCollected = 0;
    public long MysteryCollected = 0;

    public Decimal256 TotalIncomeClickDamage;
    public Decimal256 TotalIncomeGoldPerKnifeThrow;
    public Decimal256 TotalIncomeKnifeCd;
    public Decimal256 TotalIncomeKnifeDamage;
    public Decimal256 TotalIncomeWitchDoctor;
    public Decimal256 TotalIncomeGoldPerRound;
    public Decimal256 TotalIncomeHoarder;
    public Decimal256 TotalIncomeWizard;
    public Decimal256 TotalIncomeZapDamage;
    public Decimal256 TotalIncomeMoneyMaker;
    public Decimal256 TotalIncomeDaggerMaster;
    public Decimal256 TotalIncomeNecroNinja;
    public Decimal256 TotalIncomeSkullCrusher;

    public Decimal256 TotalIncomeArena;
    public Decimal256 TotalIncomePassive;

    public Decimal256 TotalIncomeKnifeThrow;

    public Decimal256 EnemiesKilled;
    public Decimal256 DamageDone;

    // Upgrades
    public long LevelClickDamage = 0;
    public long LevelKnifeCd = 0;
    public long LevelKnifeDamage = 0;
    public long LevelWitchDoctor = 0;
    public long LevelMoneyPerGold = 0;
    public long LevelGoldPerKnifeThrown = 0;
    public long LevelHoarder = 0;
    public long LevelWizard = 0;
    public long LevelZapDamage = 0;
    public long LevelMoneyMaker = 0;
    public long LevelDaggerMaster = 0;
    public long LevelNecroNinja = 0;
    public long LevelSkullCrusher = 0;

    // X2
    public long LevelClickDamageX2 = 0;
    public long LevelKnifeCdX2 = 0;
    public long LevelKnifeDamageX2 = 0;
    public long LevelWitchDoctorX2 = 0;
    public long LevelMoneyPerGoldX2 = 0;
    public long LevelGoldPerKnifeThrownX2 = 0;
    public long LevelHoarderX2 = 0;
    public long LevelWizardX2 = 0;
    public long LevelZapDamageX2 = 0;
    public long LevelMoneyMakerX2 = 0;
    public long LevelDaggerMasterX2 = 0;
    public long LevelNecroNinjaX2 = 0;
    public long LevelSkullCrusherX2 = 0;

    // Pct
    public long LevelPctBought = 0;

    // Damage
    public Decimal256 TotalDamageChainZap;
    public Decimal256 TotalDamageDaggerThrow;
    public Decimal256 TotalDamageWitchDoctor;
    public Decimal256 TotalDamageWizard;
    public Decimal256 TotalDamageNecromancer;

    // Game
    public long ArenaLevel = 1;
    public Decimal256 Money = 40;
    public int CurrentSkin = 0;

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

    public static string GetObfuscatedSaveGame()
    {
        string json = Members.ToJson();
        return Obfuscation.EncodeForSave(json);
    }

    public static bool ImportObfuscatedSaveGame(string obfuscated)
    {
        string errorMsg = "";
        try
        {
            string json = Obfuscation.Decode(obfuscated);
            SaveGame.LoadJson(json, isRestore: true);
            return true;
        }
        catch(FormatException)
        {
            errorMsg = "The save game format was not recognized";
        }
        catch (Exception e)
        {
             errorMsg = e.Message;
        }

        if (!string.IsNullOrWhiteSpace(errorMsg))
            GameCanvasScript.Instance.ShowPopup($"Could not import save game, reason:\n{errorMsg}");
        return false;
    }

    public static void Save()
    {
        if (!Members.SaveKillSwitch_CanSave)
        {
            throw new System.Exception("SaveKillSwitch_CanSave is false, Members were reset somehow");
        }

        string json = Members.ToJson();
        //Debug.Log("saving json: " + json);

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            //Debug.Log("saving WEBGL");
            JsMappings.Save(json);
        }
        else
        {
            //Debug.Log("saving prefs");
            PlayerPrefs.SetString(SaveGameKey, json);
        }
    }

    const string SaveGameKey = "idle-knight-save.json";

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
        LoadJson(prefs);
    }

    public static void LoadJson(string json, bool isRestore = false)
    {
        if (isRestore)
        {
            GameManager.Instance.ResetAllProgress();
        }

        if (!string.IsNullOrWhiteSpace(json))
        {
            Members = SaveGameMembers.FromJson(json);
        }

        Members ??= new SaveGameMembers();
        Members.SaveKillSwitch_CanSave = true; // Guard against lost data if Members are reset.

        if (string.IsNullOrEmpty(Members.UserId))
        {
            Members.UserId = "userId-" + System.Guid.NewGuid().ToString();
            Save();
        }
    }
}
