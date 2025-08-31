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
    public Decimal512 MonsterCreditsXp_09_08_2025;

    public long TimesAscended_09_08_2025;
    public long MonsterCredits_09_08_2025;
    public long MonsterCreditsLifetime_09_08_2025;
    public long DiamondCount_09_08_2025;

    // Permanent upgrades
    public bool BoughtPassiveX2_1 = false;
    public bool BoughtPassiveX2_2 = false;
    public bool BoughtPassiveX4_1 = false;
    public bool BoughtFasterMystery = false;

    // Settings
    public int Version;
    public float VolumeMaster = 1.0f;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;

    public bool ShowFloatingDamageNumbers = true;
    public bool ShowFloatingGoldNumbers = true;

    // Progress
    public List<ActorTypeEnum> BeastsSeen = new List<ActorTypeEnum>();
    public List<Achieved> Achieved = new List<Achieved>();

    public double EstimatedOnlineSeconds2 = 0;

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

    public Decimal512 TotalIncomeClickDamage;
    public Decimal512 TotalIncomeGoldPerKnifeThrow;
    public Decimal512 TotalIncomeKnifeCd;
    public Decimal512 TotalIncomeKnifeDamage;
    public Decimal512 TotalIncomeWitchDoctor;
    public Decimal512 TotalIncomeGoldPerRound;
    public Decimal512 TotalIncomeHoarder;
    public Decimal512 TotalIncomeWizard;
    public Decimal512 TotalIncomeZapDamage;
    public Decimal512 TotalIncomeMoneyMaker;
    public Decimal512 TotalIncomeDaggerMaster;
    public Decimal512 TotalIncomeNecroNinja;
    public Decimal512 TotalIncomeSkullCrusher;
    public Decimal512 TotalIncomeChestMaster;
    public Decimal512 TotalIncomeVoidgazer;

    public Decimal512 TotalIncomeArena;
    public Decimal512 TotalIncomePassive;

    public Decimal512 TotalIncomeKnifeThrow;

    public Decimal512 EnemiesKilled;
    public Decimal512 DamageDone;

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
    public long LevelChestMaster = 0;
    public long LevelVoidgazer = 0;

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
    public long LevelChestMasterX2 = 0;
    public long LevelVoidgazerX2 = 0;

    // Pct
    public long LevelPctBought = 0;

    // Damage
    public Decimal512 TotalDamageChainZap;
    public Decimal512 TotalDamageDaggerThrow;
    public Decimal512 TotalDamageWitchDoctor;
    public Decimal512 TotalDamageWizard;
    public Decimal512 TotalDamageNecromancer;

    // Game
    public long ArenaLevel = 1;
    public Decimal512 Money = 40;
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

    //public static IEnumerator SaveCo()
    //{
    //    Save();
    //    yield break;
    //}

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
            //Debug.Log("saving WEBGL - dual save for safety");
            // Save to both systems for redundancy
            // PlayerPrefs survives localStorage clearing but gets wiped on updates
            // JsMappings/localStorage survives updates but can randomly disappear

            bool playerPrefsSuccess = false;
            bool jsMappingsSuccess = false;

            try
            {
                PlayerPrefs.SetString(SaveGameKey, json);
                PlayerPrefs.Save();
                playerPrefsSuccess = true;
                //Debug.Log("PlayerPrefs save successful");
            }
            catch (System.Exception e)
            {
                Debug.LogError("PlayerPrefs save failed: " + e.Message);
            }

            try
            {
                JsMappings.Save(json);
                jsMappingsSuccess = true;
                //Debug.Log("JsMappings save successful");
            }
            catch (System.Exception e)
            {
                Debug.LogError("JsMappings save failed: " + e.Message);
            }

            if (!playerPrefsSuccess && !jsMappingsSuccess)
            {
                Debug.LogError("Both save methods failed! Data may be lost.");
            }
        }
        else
        {
            //Debug.Log("saving prefs");
            PlayerPrefs.SetString(SaveGameKey, json);
        }
    }

    const string SaveGameKey = "idle-earl-save-v1.json";

    public static void Load()
    {
        GameManager.GameLoadInfo.Add("Loading game, platform: " + Application.platform);
        string playerPrefsData = string.Empty;
        string jsMappingsData = string.Empty;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Try to load from both sources
            bool playerPrefsAvailable = false;
            bool jsMappingsAvailable = false;

            // Check PlayerPrefs
            try
            {
                if (PlayerPrefs.HasKey(SaveGameKey))
                {
                    playerPrefsData = PlayerPrefs.GetString(SaveGameKey);
                    if (!string.IsNullOrEmpty(playerPrefsData))
                    {
                        playerPrefsAvailable = true;
                        GameManager.GameLoadInfo.Add("PlayerPrefs data available");
                    }
                }
            }
            catch (System.Exception e)
            {
                GameManager.GameLoadInfo.Add("PlayerPrefs load failed: " + e.Message);
            }

            // Check JsMappings
            try
            {
                jsMappingsData = JsMappings.Load();
                if (!string.IsNullOrEmpty(jsMappingsData))
                {
                    jsMappingsAvailable = true;
                    GameManager.GameLoadInfo.Add("JsMappings data available");
                }
            }
            catch (System.Exception e)
            {
                GameManager.GameLoadInfo.Add("JsMappings load failed: " + e.Message);
            }

            // Decide which data to use
            string dataToLoad = string.Empty;

            if (playerPrefsAvailable && jsMappingsAvailable)
            {
                // Both available - prefer PlayerPrefs (more recent since it survives localStorage issues)
                GameManager.GameLoadInfo.Add("Both saves available, using PlayerPrefs");
                dataToLoad = playerPrefsData;

                // Optional: You could compare timestamps or save versions here if you store that info
            }
            else if (playerPrefsAvailable)
            {
                GameManager.GameLoadInfo.Add("Only PlayerPrefs available (JsMappings likely cleared)");
                dataToLoad = playerPrefsData;
            }
            else if (jsMappingsAvailable)
            {
                GameManager.GameLoadInfo.Add("Only JsMappings available (likely after version update)");
                dataToLoad = jsMappingsData;
            }
            else
            {
                GameManager.GameLoadInfo.Add("No save data found in either location");
            }

            Debug.Log("json = " + dataToLoad);
            LoadJson(dataToLoad);
        }
        else
        {
            Debug.Log("loading from PREFS");
            if (PlayerPrefs.HasKey(SaveGameKey))
            {
                string prefs = PlayerPrefs.GetString(SaveGameKey);
                Debug.Log("json = " + prefs);
                LoadJson(prefs);
            }
            else
            {
                Debug.Log("No save data found");
                LoadJson(string.Empty);
            }
        }
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
