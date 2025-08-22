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
            if (list.Contains(Achieved.WitchDoctor25)) return;

            if (SaveGame.Members.LevelWitchDoctor >= 25)
            {
                list.Add(Achieved.WitchDoctor25);
                NewAchieved(Achieved.WitchDoctor25);
            }
        }

        public static void CheckNecro(List<Achieved> list)
        {
            if (list.Contains(Achieved.Necromancer25)) return;

            if (SaveGame.Members.LevelMoneyMaker >= 25)
            {
                list.Add(Achieved.Necromancer25);
                NewAchieved(Achieved.Necromancer25);
            }
        }

        public static void CheckSkullCrusher5(List<Achieved> list)
        {
            if (list.Contains(Achieved.SkullCrusher5)) return;

            if (SaveGame.Members.LevelSkullCrusher >= 5)
            {
                list.Add(Achieved.SkullCrusher5);
                NewAchieved(Achieved.SkullCrusher5);
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

        public static void CheckArena5000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena5000)) return;

            if (SaveGame.Members.ArenaLevel >= 5000)
            {
                list.Add(Achieved.Arena5000);
                NewAchieved(Achieved.Arena5000);
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

        public static void CheckRebirth3(List<Achieved> list)
        {
            if (list.Contains(Achieved.Rebirth3)) return;

            if (SaveGame.Members.TimesAscended_09_08_2025 >= 3)
            {
                list.Add(Achieved.Rebirth3);
                NewAchieved(Achieved.Rebirth3);
            }
        }

        public static void CheckMystery25(List<Achieved> list)
        {
            if (list.Contains(Achieved.Mystery25)) return;

            if (SaveGame.Members.MysteryCollected >= 25)
            {
                list.Add(Achieved.Mystery25);
                NewAchieved(Achieved.Mystery25);
            }
        }

        public static void CheckChest25(List<Achieved> list)
        {
            if (list.Contains(Achieved.Chest25)) return;

            if (SaveGame.Members.ChestsCollected >= 25)
            {
                list.Add(Achieved.Chest25);
                NewAchieved(Achieved.Chest25);
            }
        }

        public static void CheckDiamonds10(List<Achieved> list)
        {
            if (list.Contains(Achieved.Diamonds10)) return;

            if (SaveGame.Members.DiamondCount_09_08_2025 >= 10)
            {
                list.Add(Achieved.Diamonds10);
                NewAchieved(Achieved.Diamonds10);
            }
        }

        public static void CheckVoidgazer(List<Achieved> list)
        {
            if (list.Contains(Achieved.VoidgazerTier)) return;

            if (SaveGame.Members.LevelVoidgazer >= 1)
            {
                list.Add(Achieved.VoidgazerTier);
                NewAchieved(Achieved.VoidgazerTier);
            }
        }
    }
}
