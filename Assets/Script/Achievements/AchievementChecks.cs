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

        public static void CheckChainZap200(List<Achieved> list)
        {
            if (list.Contains(Achieved.ChainZap200)) return;

            if (SaveGame.Members.LevelClickDamage >= 200)
            {
                list.Add(Achieved.ChainZap200);
                NewAchieved(Achieved.ChainZap200);
            }
        }

        public static void CheckArena100(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena100)) return;

            if (SaveGame.Members.ArenaLevel >= 100)
            {
                list.Add(Achieved.Arena100);
                NewAchieved(Achieved.Arena100);
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

        public static void CheckArena1500(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena1500)) return;

            if (SaveGame.Members.ArenaLevel >= 1500)
            {
                list.Add(Achieved.Arena1500);
                NewAchieved(Achieved.Arena1500);
            }
        }

        public static void CheckRebirth1(List<Achieved> list)
        {
            if (list.Contains(Achieved.Rebirth1)) return;

            if (SaveGame.Members.TimesAscended_09_08_2025 >= 1)
            {
                list.Add(Achieved.Rebirth1);
                NewAchieved(Achieved.Rebirth1);
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

        public static void CheckRebirth8(List<Achieved> list)
        {
            if (list.Contains(Achieved.Rebirth8)) return;

            if (SaveGame.Members.TimesAscended_09_08_2025 >= 8)
            {
                list.Add(Achieved.Rebirth8);
                NewAchieved(Achieved.Rebirth8);
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

        public static void CheckChest50(List<Achieved> list)
        {
            if (list.Contains(Achieved.Chest50)) return;

            if (SaveGame.Members.ChestsCollected >= 50)
            {
                list.Add(Achieved.Chest50);
                NewAchieved(Achieved.Chest50);
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

        public static void CheckDiamonds50(List<Achieved> list)
        {
            if (list.Contains(Achieved.Diamonds50)) return;

            if (SaveGame.Members.DiamondCount_09_08_2025 >= 50)
            {
                list.Add(Achieved.Diamonds50);
                NewAchieved(Achieved.Diamonds50);
            }
        }

        public static void CheckDiamonds250(List<Achieved> list)
        {
            if (list.Contains(Achieved.Diamonds250)) return;

            if (SaveGame.Members.DiamondCount_09_08_2025 >= 250)
            {
                list.Add(Achieved.Diamonds250);
                NewAchieved(Achieved.Diamonds250);
            }
        }

        public static void CheckDiamonds1000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Diamonds1000)) return;

            if (SaveGame.Members.DiamondCount_09_08_2025 >= 1000)
            {
                list.Add(Achieved.Diamonds1000);
                NewAchieved(Achieved.Diamonds1000);
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

        public static void CheckMasterWizard(List<Achieved> list)
        {
            if (list.Contains(Achieved.MasterWizardTier)) return;

            if (SaveGame.Members.LevelHoarder >= 1)
            {
                list.Add(Achieved.MasterWizardTier);
                NewAchieved(Achieved.MasterWizardTier);
            }
        }

        public static void CheckX2_25(List<Achieved> list)
        {
            if (list.Contains(Achieved.X2_25)) return;

            if (PlayerUpgrades.Data.NumberOfX2Bought >= 25)
            {
                list.Add(Achieved.X2_25);
                NewAchieved(Achieved.X2_25);
            }
        }

        public static void CheckX2_50(List<Achieved> list)
        {
            if (list.Contains(Achieved.X2_50)) return;

            if (PlayerUpgrades.Data.NumberOfX2Bought >= 50)
            {
                list.Add(Achieved.X2_50);
                NewAchieved(Achieved.X2_50);
            }
        }

        public static void CheckSkins3(List<Achieved> list)
        {
            if (list.Contains(Achieved.Skins3)) return;

            if (SaveGame.Members.Achieved.Count >= 3)
            {
                list.Add(Achieved.Skins3);
                NewAchieved(Achieved.Skins3);
            }
        }

        public static void CheckUpgrades500(List<Achieved> list)
        {
            if (list.Contains(Achieved.Upgrades500)) return;

            if (SaveGame.Members.TotalUpgradesBought >= 500)
            {
                list.Add(Achieved.Upgrades500);
                NewAchieved(Achieved.Upgrades500);
            }
        }

        public static void CheckUpgrades2500(List<Achieved> list)
        {
            if (list.Contains(Achieved.Upgrades2500)) return;

            if (SaveGame.Members.TotalUpgradesBought >= 2500)
            {
                list.Add(Achieved.Upgrades2500);
                NewAchieved(Achieved.Upgrades2500);
            }
        }

        public static void CheckUpgrades10000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Upgrades10000)) return;

            if (SaveGame.Members.TotalUpgradesBought >= 10000)
            {
                list.Add(Achieved.Upgrades10000);
                NewAchieved(Achieved.Upgrades10000);
            }
        }

        public static void CheckBuyCardScaryEarl(List<Achieved> list)
        {
            if (list.Contains(Achieved.BuyCardScaryEarl)) return;

            if (SaveGame.Members.BoughtScaryEarlSkin)
            {
                list.Add(Achieved.BuyCardScaryEarl);
                NewAchieved(Achieved.BuyCardScaryEarl);
            }
        }

        public static void CheckSmartDagger10(List<Achieved> list)
        {
            if (list.Contains(Achieved.SmartDagger10)) return;

            if (SaveGame.Members.LevelSmartDaggers >= 10)
            {
                list.Add(Achieved.SmartDagger10);
                NewAchieved(Achieved.SmartDagger10);
            }
        }

        public static void CheckChestMaster200(List<Achieved> list)
        {
            if (list.Contains(Achieved.ChestMaster200)) return;

            if (SaveGame.Members.LevelChestMaster >= 200)
            {
                list.Add(Achieved.ChestMaster200);
                NewAchieved(Achieved.ChestMaster200);
            }
        }
    }
}
