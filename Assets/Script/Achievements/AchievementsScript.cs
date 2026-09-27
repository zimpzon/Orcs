using Assets.Script.Achievements;
using System;
using UnityEngine;

[Serializable]
public enum Achieved
{
    WitchDoctor25,
    Necromancer200,
    SkullCrusher5,
    Arena5000,
    Rebirth2,
    Arena25000,
    Rebirth3,
    Mystery100,
    Chest50,
    Diamonds10,
    CriticalStrikeTier,
    Arena50000,
    MasterWizardTier,
    Diamonds250,
    Rebirth8,
    ChainZap200,
    Chest100,
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
    Diamonds50000,
};

public class AchievementsScript : MonoBehaviour
{
    void Update()
    {
        var list = SaveGame.Members.Achieved;
        AchievementChecks.CheckWitchDoctor(list);
        AchievementChecks.CheckNecro200(list);
        AchievementChecks.CheckSkullCrusher5(list);
        AchievementChecks.CheckArena1000(list);
        AchievementChecks.CheckArena5000(list);
        AchievementChecks.CheckArena25000(list);
        AchievementChecks.CheckArena1500(list);
        AchievementChecks.CheckMystery25(list);
        AchievementChecks.CheckChest50(list);
        AchievementChecks.CheckVoidgazer(list);
        AchievementChecks.CheckMasterWizard(list);
        AchievementChecks.CheckChainZap200(list);
        AchievementChecks.CheckChest100(list);
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
        AchievementChecks.CheckDiamonds50000(list);
        AchievementChecks.CheckChestMaster200(list);
        AchievementChecks.Have1Percent50(list);
        AchievementChecks.Buy1PercentTotal500(list);
    }
}
