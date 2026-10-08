using System;
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

    public int TimeSinceLastAscend;
    public long TimesAscended_09_08_2025;
    public double RebirthIncomeBonus; // set at each rebirth to 5% per hour played (UpgradeProgression.RebirthBonus)
    public long MonsterCredits_09_08_2025;
    public long MonsterCreditsLifetime_09_08_2025;
    public long CreditBonusStartLifetime = -1; // lifetime credits at the last rebirth, drives the credit bonus (-1 = not set yet)
    public long DiamondCount_09_08_2025;

    // Permanent upgrades - SEE ApplyAscendPermanentBonuses for actual values of these
    public bool BoughtPassiveX2_1 = false;
    public bool BoughtPassiveX2_2 = false;
    public bool BoughtPassiveX4_1 = false;
    public bool BoughtPassiveX4_2 = false;
    public bool BoughtPassiveX4_3 = false;
    public bool BoughtPassiveX6_1 = false;
    public bool BoughtPassiveX7_1 = false;
    public bool BoughtFasterMystery = false;
    public bool BoughtFasterArena = false;
    public bool BoughtScaryEarlSkin = false;
    public bool BoughtPercentBonusX10 = false;
    public bool BoughtHalfEnemyHp = false;
    public bool BoughtHaggler1 = false;
    public bool BoughtHaggler2 = false;
    public bool BoughtHaggler3 = false;
    public bool BoughtBeastScholar1 = false;
    public bool BoughtBeastScholar2 = false;
    public bool BoughtX2Mastery1 = false;
    public bool BoughtX2Mastery2 = false;
    public bool BoughtX2Mastery3 = false;
    public bool BoughtFasterArena2 = false;
    public bool BoughtCardDiscount = false;
    public bool BoughtZapLore = false;
    public bool BoughtChestLore = false;
    public bool BoughtVoodooLore = false;
    public bool BoughtWizardLore = false;
    public bool BoughtSkinCollector = false;
    public bool BoughtSkinCollector2 = false;
    public bool BoughtCompletionist = false;
    public bool BoughtHeadStart = false;

    public string AscendCardDebug() =>
        $"{nameof(BoughtPassiveX2_1)} = {SaveGame.Members.BoughtPassiveX2_1} | " +
        $"{nameof(BoughtPassiveX2_2)} = {SaveGame.Members.BoughtPassiveX2_2} | " +
        $"{nameof(BoughtPassiveX4_1)} = {SaveGame.Members.BoughtPassiveX4_1} | " +
        $"{nameof(BoughtPassiveX4_2)} = {SaveGame.Members.BoughtPassiveX4_2} | " +
        $"{nameof(BoughtPassiveX4_3)} = {SaveGame.Members.BoughtPassiveX4_3} | " +
        $"{nameof(BoughtPassiveX6_1)} = {SaveGame.Members.BoughtPassiveX6_1} | " +
        $"{nameof(BoughtPassiveX7_1)} = {SaveGame.Members.BoughtPassiveX7_1} | " +
        $"{nameof(BoughtFasterMystery)} = {SaveGame.Members.BoughtFasterMystery} | " +
        $"{nameof(BoughtFasterArena)} = {SaveGame.Members.BoughtFasterArena} | " +
        $"{nameof(BoughtScaryEarlSkin)} = {SaveGame.Members.BoughtScaryEarlSkin} | " +
        $"{nameof(BoughtPercentBonusX10)} = {SaveGame.Members.BoughtPercentBonusX10} | " +
        $"{nameof(BoughtHalfEnemyHp)} = {SaveGame.Members.BoughtHalfEnemyHp} | " +
        $"{nameof(BoughtHaggler1)} = {SaveGame.Members.BoughtHaggler1} | " +
        $"{nameof(BoughtHaggler2)} = {SaveGame.Members.BoughtHaggler2} | " +
        $"{nameof(BoughtHaggler3)} = {SaveGame.Members.BoughtHaggler3} | " +
        $"{nameof(BoughtBeastScholar1)} = {SaveGame.Members.BoughtBeastScholar1} | " +
        $"{nameof(BoughtBeastScholar2)} = {SaveGame.Members.BoughtBeastScholar2} | " +
        $"{nameof(BoughtX2Mastery1)} = {SaveGame.Members.BoughtX2Mastery1} | " +
        $"{nameof(BoughtX2Mastery2)} = {SaveGame.Members.BoughtX2Mastery2} | " +
        $"{nameof(BoughtX2Mastery3)} = {SaveGame.Members.BoughtX2Mastery3} | " +
        $"{nameof(BoughtFasterArena2)} = {SaveGame.Members.BoughtFasterArena2} | " +
        $"{nameof(BoughtCardDiscount)} = {SaveGame.Members.BoughtCardDiscount} | " +
        $"{nameof(BoughtZapLore)} = {SaveGame.Members.BoughtZapLore} | " +
        $"{nameof(BoughtChestLore)} = {SaveGame.Members.BoughtChestLore} | " +
        $"{nameof(BoughtVoodooLore)} = {SaveGame.Members.BoughtVoodooLore} | " +
        $"{nameof(BoughtWizardLore)} = {SaveGame.Members.BoughtWizardLore} | " +
        $"{nameof(BoughtSkinCollector)} = {SaveGame.Members.BoughtSkinCollector} | " +
        $"{nameof(BoughtSkinCollector2)} = {SaveGame.Members.BoughtSkinCollector2} | " +
        $"{nameof(BoughtCompletionist)} = {SaveGame.Members.BoughtCompletionist} | " +
        $"{nameof(BoughtHeadStart)} = {SaveGame.Members.BoughtHeadStart}";

    // Settings
    public int Version;
    public float VolumeMaster = 1.0f;
    public float VolumeMusic = 0.7f;
    public float VolumeSfx = 1.0f;

    public bool ShowFloatingDamageNumbers = true;
    public bool ShowFloatingGoldNumbers = true;
    public bool UseScientificNotation = false;
    public bool ShowDetailsOnHover = true;
    public bool SoundEnabled = true;
    public SkinAnimation SelectedSkin = SkinAnimation.Default;

    // Progress
    public List<ActorTypeEnum> BeastsSeen = new List<ActorTypeEnum>();
    public List<Achieved> Achieved = new List<Achieved>();

    public double EstimatedOnlineSeconds2 = 0;

    // Stats
    // HOW TO ALIGN left/right: https://discussions.unity.com/t/textmeshpro-right-and-left-align-on-same-line/672190/6
    public long MaxDps = 0;
    public long MaxArena = 1;
    public long MaxUpgradeTiersBought = 0; // permanent: most upgrade tiers ever bought in one run (game completion)
    public long TotalArenas = 0;
    public long TotalArenasWon = 0;
    public long SuperFastClears = 0;
    public Decimal512 MaxIncome;
    public Decimal512 MaxCreditIncome; // best passive income without the mystery buff; floors credit XP (UpgradeProgression)
    public Decimal512 MaxMoney;
    public long MaxCredits;
    public long TotalUpgradesBought = 0;
    public long TotalX2UpgradesBought = 0;

    // ---------------------- Deleted after ascend ----------------------


    // Prevent overwriting save in case of Members reset (happens in editor on crash on code change).
    public bool SaveKillSwitch_CanSave = false;

    // Tracking
    public double QuestionMarkRealTimeLeft = -1;

    // Save identity, to tell which save got loaded. SaveId is a random GUID given to a save when it is created (or
    // found missing on an old save) and kept through rebirth; a save wipe starts a new one. SavedAtUtc is updated on
    // every save (ISO 8601 UTC).
    public string SaveId;
    public string SavedAtUtc;

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
    public Decimal512 TotalIncomeSmartDaggers;
    public Decimal512 TotalIncomeFastFeet;
    public Decimal512 TotalIncomeCryptMaster;
    public Decimal512 TotalIncomeSmartFireballs;
    public Decimal512 TotalIncomeBeefyEarl;
    public Decimal512 TotalIncomeCriticalStrike;
    public Decimal512 TotalIncomePowerZap;
    public Decimal512 TotalIncomeSkullSlicer;
    public Decimal512 TotalIncomeStormLord;

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
    public long LevelSmartDaggers = 0;
    public long LevelFastFeet = 0;
    public long LevelCryptMaster = 0;
    public long LevelSmartFireballs = 0;
    public long LevelBeefyEarl = 0;
    public long LevelCriticalStrike = 0;
    public long LevelPowerZap = 0;
    public long LevelSkullSlicer = 0;
    public long LevelStormLord = 0;

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
    public long LevelSmartDaggersX2 = 0;
    public long LevelFastFeetX2 = 0;
    public long LevelCryptMasterX2 = 0;
    public long LevelSmartFireballsX2 = 0;
    public long LevelBeefyEarlX2 = 0;
    public long LevelCriticalStrikeX2 = 0;
    public long LevelPowerZapX2 = 0;
    public long LevelSkullSlicerX2 = 0;
    public long LevelStormLordX2 = 0;

    // Pct
    public long LevelPctBought = 0;
    public long TotalLevelPctBought = 0;

    // Damage
    public Decimal512 TotalDamageChainZap;
    public Decimal512 TotalDamageDaggerThrow;
    public Decimal512 TotalDamageWitchDoctor;
    public Decimal512 TotalDamageWizard;
    public Decimal512 TotalDamageNecromancer;

    // Game
    public long ArenaLevel = 1;
    public Decimal512 Money = 40;
    public int CurrentSkin = 1;

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

    public static float LastSaveTime;

    public static event Action OnSave;

    public static void Save()
    {
        if (!Members.SaveKillSwitch_CanSave)
        {
            throw new System.Exception("SaveKillSwitch_CanSave is false, Members were reset somehow");
        }

        LastSaveTime = G.D is not null ? G.D.GameTime : 0;

        OnSave?.Invoke();

        EnsureSaveId();
        Members.SavedAtUtc = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);

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

    const string SaveGameKey = "idle-earl-save-v2.json";

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
                // Both available - use the more recently saved one when both have a timestamp, else PlayerPrefs
                // (more recent since it survives localStorage issues).
                string prefsSavedAt = DescribeSave("PlayerPrefs", playerPrefsData, out DateTime prefsTime);
                string jsSavedAt = DescribeSave("JsMappings", jsMappingsData, out DateTime jsTime);
                GameManager.GameLoadInfo.Add(prefsSavedAt);
                GameManager.GameLoadInfo.Add(jsSavedAt);

                bool useJs = jsTime != DateTime.MinValue && prefsTime != DateTime.MinValue && jsTime > prefsTime;
                GameManager.GameLoadInfo.Add(useJs ? "Both saves available, using JsMappings (newer)" : "Both saves available, using PlayerPrefs");
                dataToLoad = useJs ? jsMappingsData : playerPrefsData;
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
    // Gives the current save a SaveId if it has none (new save, or an old save from before SaveId existed).
    static bool EnsureSaveId()
    {
        if (!string.IsNullOrEmpty(Members.SaveId))
            return false;

        Members.SaveId = Guid.NewGuid().ToString();
        return true;
    }

    // "PlayerPrefs: save <id> saved at <time>" for the load log; time is MinValue if unknown.
    static string DescribeSave(string source, string json, out DateTime savedAt)
    {
        savedAt = DateTime.MinValue;
        try
        {
            var m = SaveGameMembers.FromJson(json);
            if (m != null && DateTime.TryParse(m.SavedAtUtc, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind, out var t))
                savedAt = t;
            return $"{source}: save {m?.SaveId ?? "?"} saved at {m?.SavedAtUtc ?? "?"}";
        }
        catch (Exception e)
        {
            return $"{source}: unreadable ({e.Message})";
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

        string loaded = $"Loaded save {(string.IsNullOrEmpty(Members.SaveId) ? "(no id yet)" : Members.SaveId)}, saved at {(string.IsNullOrEmpty(Members.SavedAtUtc) ? "(unknown)" : Members.SavedAtUtc)} UTC";
        GameManager.GameLoadInfo.Add(loaded);
        Debug.Log(loaded);
        if (EnsureSaveId())
            Save();

        if (string.IsNullOrEmpty(Members.UserId))
        {
            Members.UserId = "userId-" + System.Guid.NewGuid().ToString();
            Save();
        }
    }
}
