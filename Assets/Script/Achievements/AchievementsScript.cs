using Assets.Script.Achievements;
using System;
using UnityEngine;

[Serializable]
public enum Achieved
{
    WitchDoctor75,
    Necromancer75,
    SkullCrusher1,
    Arena500,
    Arena1000,
    Rebirth2,
};

public class AchievementsScript : MonoBehaviour
{
    void Update()
    {
        var list = SaveGame.Members.Achieved;
        AchievementChecks.CheckWitchDoctor(list);
        AchievementChecks.CheckNecro(list);
        AchievementChecks.CheckSkullCrusher(list);
        AchievementChecks.CheckArena500(list);
        AchievementChecks.CheckArena1000(list);
        AchievementChecks.CheckRebirth2(list);
    }
}
