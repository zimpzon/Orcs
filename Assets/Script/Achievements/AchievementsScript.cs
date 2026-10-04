using Assets.Script.Achievements;
using System;
using UnityEngine;

[Serializable]
public enum Achieved
{
    WitchDoctor25,
    Necromancer200,
    SkullCrusher5,
    Arena10000,
    Rebirth2,
    Arena35000,
    Rebirth3,
    Mystery100,
    Chest50,
    Diamonds10,
    StormLordTier,
    Arena81000,
    MasterWizardTier,
    Diamonds250,
    Rebirth8,
    ChainZap200,
    Played5Days,
    X2_300,
    X2_150,
    Diamonds50,
    Arena1000,
    Skins20,
    Rebirth1,
    Upgrades500,
    Upgrades2500,
    Upgrades100000,
    BuyCardScaryEarl,
    SmartDagger10,
    Diamonds1000,
    ChestMaster200,
    Have1Percent50,
    Buy1PercentTotal500,
    Rebirth20,
    Diamonds5000,
    Completion100,
};

public class AchievementsScript : MonoBehaviour
{
    // Don't celebrate unlocks that happen while the save loads (e.g. thresholds an old save already meets).
    const float AnnounceAfterSeconds = 3f;
    readonly System.Collections.Generic.List<Achieved> _newThisFrame = new();

    void Update()
    {
        var list = SaveGame.Members.Achieved;
        int countBefore = list.Count;
        RunChecks(list);

        if (list.Count > countBefore && Time.timeSinceLevelLoad > AnnounceAfterSeconds)
            AnnounceNewSkins(list, countBefore);
    }

    // Floating "New skin unlocked!" text (same effect as the X2 rank-up), for every skin the new achievements unlocked.
    void AnnounceNewSkins(System.Collections.Generic.List<Achieved> list, int countBefore)
    {
        _newThisFrame.Clear();
        for (int i = countBefore; i < list.Count; ++i)
            _newThisFrame.Add(list[i]);

        foreach (SkinAnimation skin in System.Enum.GetValues(typeof(SkinAnimation)))
        {
            (bool isUnlocked, string hoverText) status;
            try { status = SkinScript.GetUnlockStatus(skin); }
            catch (System.ArgumentException) { continue; } // enum entries with no skin (GhostEarl)
            if (!status.isUnlocked)
                continue;

            // Unlocked now - was it unlocked without this frame's achievements?
            list.RemoveRange(countBefore, _newThisFrame.Count);
            bool wasUnlocked = SkinScript.GetUnlockStatus(skin).isUnlocked;
            list.AddRange(_newThisFrame);
            if (wasUnlocked)
                continue;

            int colon = status.hoverText.IndexOf(':');
            string skinName = colon > 0 ? status.hoverText.Substring(0, colon) : status.hoverText;
            AnnounceAtCursorOrCenter($"New skin unlocked!\n<size=80%>{skinName}</size>");
        }
    }

    static void AnnounceAtCursorOrCenter(string text)
    {
        var anchor = UpgradeManager.Instance != null ? UpgradeManager.Instance.ClickDamage : null;
        if (anchor == null)
            return;

        // Unlocks can happen passively (arena level, time played), so fall back to the screen center when the
        // cursor isn't over the game.
        Vector2 pos = Input.mousePosition;
        if (pos.x < 0 || pos.y < 0 || pos.x > Screen.width || pos.y > Screen.height)
            pos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        X2RankUpEffect.SpawnText(anchor, pos, text);
    }

    void RunChecks(System.Collections.Generic.List<Achieved> list)
    {
        AchievementChecks.CheckWitchDoctor(list);
        AchievementChecks.CheckNecro200(list);
        AchievementChecks.CheckSkullCrusher5(list);
        AchievementChecks.CheckArena1000(list);
        AchievementChecks.CheckArena10000(list);
        AchievementChecks.CheckArena35000(list);
        AchievementChecks.CheckArena81000(list);
        AchievementChecks.CheckMystery25(list);
        AchievementChecks.CheckChest50(list);
        AchievementChecks.CheckVoidgazer(list);
        AchievementChecks.CheckMasterWizard(list);
        AchievementChecks.CheckChainZap200(list);
        AchievementChecks.CheckPlayed5Days(list);
        AchievementChecks.CheckRebirth1(list);
        AchievementChecks.CheckRebirth2(list);
        AchievementChecks.CheckRebirth3(list);
        AchievementChecks.CheckRebirth8(list);
        AchievementChecks.CheckRebirth50(list);
        AchievementChecks.CheckX2_300(list);
        AchievementChecks.CheckX2_150(list);
        AchievementChecks.CheckSkins20(list);
        AchievementChecks.CheckUpgrades500(list);
        AchievementChecks.CheckUpgrades2500(list);
        AchievementChecks.CheckUpgrades10000(list);
        AchievementChecks.CheckBuyCardScaryEarl(list);
        AchievementChecks.CheckSmartDagger10(list);
        AchievementChecks.CheckDiamonds10(list);
        AchievementChecks.CheckDiamonds50(list);
        AchievementChecks.CheckDiamonds250(list);
        AchievementChecks.CheckDiamonds1000(list);
        AchievementChecks.CheckDiamonds5000(list);
        AchievementChecks.CheckCompletion100(list);
        AchievementChecks.CheckChestMaster200(list);
        AchievementChecks.Have1Percent50(list);
        AchievementChecks.Buy1PercentTotal500(list);
    }
}
