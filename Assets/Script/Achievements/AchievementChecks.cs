using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Achievements
{
    internal static class AchievementChecks
    {
        public static void NewAchieved(Achieved achieved)
        {
            Debug.Log("New achievement: " + achieved);
        }

        public static void CheckWitchDoctor(List<Achieved> list)
        {
            if (list.Contains(Achieved.WitchDoctor75)) return;

            if (SaveGame.Members.LevelWitchDoctor >= 75)
            {
                list.Add(Achieved.WitchDoctor75);
                NewAchieved(Achieved.WitchDoctor75);
            }
        }

        public static void CheckNecro(List<Achieved> list)
        {
            if (list.Contains(Achieved.Necromancer75)) return;

            if (SaveGame.Members.LevelMoneyMaker >= 75)
            {
                list.Add(Achieved.Necromancer75);
                NewAchieved(Achieved.Necromancer75);
            }
        }

        public static void CheckSkullCrusher(List<Achieved> list)
        {
            if (list.Contains(Achieved.SkullCrusher1)) return;

            if (SaveGame.Members.LevelSkullCrusher >= 1)
            {
                list.Add(Achieved.SkullCrusher1);
                NewAchieved(Achieved.SkullCrusher1);
            }
        }

        public static void CheckArena500(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena500)) return;

            if (SaveGame.Members.ArenaLevel >= 500)
            {
                list.Add(Achieved.Arena500);
                NewAchieved(Achieved.Arena500);
            }
        }

        public static void CheckArena1000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena1000)) return;

            if (SaveGame.Members.ArenaLevel >= 1000)
            {
                list.Add(Achieved.Arena1000);
                NewAchieved(Achieved.Arena1000);
            }
        }

        public static void CheckRebirth2(List<Achieved> list)
        {
            if (list.Contains(Achieved.Rebirth2)) return;

            if (SaveGame.Members.TimesAscended_09_08_2025 >= 2)
            {
                list.Add(Achieved.Rebirth2);
                NewAchieved(Achieved.Rebirth2);
            }
        }
    }
}
