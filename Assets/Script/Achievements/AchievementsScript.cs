using Assets.Script.Achievements;
using System;
using UnityEngine;

[Serializable]
public enum Achieved
{
    WitchDoctor25,
    Necromancer25,
    SkullCrusher5,
    Arena500,
    Rebirth2,

    Arena1000,
    Rebirth3,
    Mystery25,
    Chest25,
    Diamonds10,
};

public class AchievementsScript : MonoBehaviour
{
    void Update()
    {
        var list = SaveGame.Members.Achieved;
        AchievementChecks.CheckWitchDoctor(list);
        AchievementChecks.CheckNecro(list);
        AchievementChecks.CheckSkullCrusher5(list);
        AchievementChecks.CheckArena500(list);
        AchievementChecks.CheckArena1000(list);
        AchievementChecks.CheckRebirth2(list);
        AchievementChecks.CheckRebirth3(list);
        AchievementChecks.CheckMystery25(list);
        AchievementChecks.CheckChest25(list);
        AchievementChecks.CheckDiamonds10(list);
    }
}
