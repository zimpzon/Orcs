using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Achievements
{
    internal static class AchievementChecks
    {
        private const int V = 100;

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

        public static void CheckNecro200(List<Achieved> list)
        {
            if (list.Contains(Achieved.Necromancer200)) return;

            if (SaveGame.Members.LevelMoneyMaker >= 200)
            {
                list.Add(Achieved.Necromancer200);
                NewAchieved(Achieved.Necromancer200);
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

        public static void CheckArena1000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena1000)) return;

            if (SaveGame.Members.ArenaLevel >= 1000)
            {
                list.Add(Achieved.Arena1000);
                NewAchieved(Achieved.Arena1000);
            }
        }

        public static void CheckArena10000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena10000)) return;

            if (SaveGame.Members.ArenaLevel >= 10000)
            {
                list.Add(Achieved.Arena10000);
                NewAchieved(Achieved.Arena10000);
            }
        }

        public static void CheckArena35000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena35000)) return;

            if (SaveGame.Members.ArenaLevel >= 35000)
            {
                list.Add(Achieved.Arena35000);
                NewAchieved(Achieved.Arena35000);
            }
        }

        public static void CheckArena75000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Arena75000)) return;

            if (SaveGame.Members.ArenaLevel >= 75000)
            {
                list.Add(Achieved.Arena75000);
                NewAchieved(Achieved.Arena75000);
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

        public static void CheckRebirth50(List<Achieved> list)
        {
            if (list.Contains(Achieved.Rebirth20)) return;

            if (SaveGame.Members.TimesAscended_09_08_2025 >= 20)
            {
                list.Add(Achieved.Rebirth20);
                NewAchieved(Achieved.Rebirth20);
            }
        }

        public static void CheckMystery25(List<Achieved> list)
        {
            if (list.Contains(Achieved.Mystery100)) return;

            if (SaveGame.Members.MysteryCollected >= 100)
            {
                list.Add(Achieved.Mystery100);
                NewAchieved(Achieved.Mystery100);
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

        public static void CheckPlayed5Days(List<Achieved> list)
        {
            if (list.Contains(Achieved.Played5Days)) return;

            if (SaveGame.Members.EstimatedOnlineSeconds2 >= 5 * 24 * 60 * 60)
            {
                list.Add(Achieved.Played5Days);
                NewAchieved(Achieved.Played5Days);
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

        public static void CheckDiamonds5000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Diamonds5000)) return;

            if (SaveGame.Members.DiamondCount_09_08_2025 >= 5000)
            {
                list.Add(Achieved.Diamonds5000);
                NewAchieved(Achieved.Diamonds5000);
            }
        }

        static float _nextCompletionCheck;

        // 100% game completion: every enemy, every tier, and every skin except this one (it can't count itself).
        // Throttled, GameCompletion.GetProgress scans the skin objects.
        public static void CheckCompletion100(List<Achieved> list)
        {
            if (list.Contains(Achieved.Completion100)) return;

            if (Time.unscaledTime < _nextCompletionCheck) return;
            _nextCompletionCheck = Time.unscaledTime + 2.0f;

            var p = GameCompletion.GetProgress();
            if (p.EnemiesUnlocked >= p.EnemiesTotal && p.TiersBought >= p.TiersTotal && p.SkinsUnlocked >= p.SkinsTotal - 1)
            {
                list.Add(Achieved.Completion100);
                NewAchieved(Achieved.Completion100);
            }
        }

        public static void CheckVoidgazer(List<Achieved> list)
        {
            if (list.Contains(Achieved.StormLordTier)) return;

            if (SaveGame.Members.LevelStormLord >= 1)
            {
                list.Add(Achieved.StormLordTier);
                NewAchieved(Achieved.StormLordTier);
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

        public static void CheckX2_300(List<Achieved> list)
        {
            if (list.Contains(Achieved.X2_300)) return;

            if (PlayerUpgrades.Data.NumberOfX2Bought >= 300)
            {
                list.Add(Achieved.X2_300);
                NewAchieved(Achieved.X2_300);
            }
        }

        public static void CheckX2_150(List<Achieved> list)
        {
            if (list.Contains(Achieved.X2_150)) return;

            if (PlayerUpgrades.Data.NumberOfX2Bought >= 150)
            {
                list.Add(Achieved.X2_150);
                NewAchieved(Achieved.X2_150);
            }
        }

        public static void CheckSkins20(List<Achieved> list)
        {
            if (list.Contains(Achieved.Skins20)) return;

            if (SaveGame.Members.Achieved.Count >= 20)
            {
                list.Add(Achieved.Skins20);
                NewAchieved(Achieved.Skins20);
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

        public static void Have1Percent50(List<Achieved> list)
        {
            if (list.Contains(Achieved.Have1Percent50)) return;

            if (SaveGame.Members.LevelPctBought >= 50)
            {
                list.Add(Achieved.Have1Percent50);
                NewAchieved(Achieved.Have1Percent50);
            }
        }

        public static void Buy1PercentTotal500(List<Achieved> list)
        {
            if (list.Contains(Achieved.Buy1PercentTotal500)) return;

            if (SaveGame.Members.TotalLevelPctBought>= 500)
            {
                list.Add(Achieved.Buy1PercentTotal500);
                NewAchieved(Achieved.Buy1PercentTotal500);
            }
        }

        public static void CheckArenasLost100(List<Achieved> list)
        {
            if (list.Contains(Achieved.ArenasLost100)) return;

            // White Earl skin. Was "run out of time in 100 arenas" - too rare once the game gets easy - now 25000
            // upgrades bought. Enum name kept so existing saves keep the skin.
            if (SaveGame.Members.TotalUpgradesBought >= 25000)
            {
                list.Add(Achieved.ArenasLost100);
                NewAchieved(Achieved.ArenasLost100);
            }
        }

        public static void CheckSuperClears1000(List<Achieved> list)
        {
            if (list.Contains(Achieved.SuperClears1000)) return;

            if (SaveGame.Members.SuperFastClears >= 1000)
            {
                list.Add(Achieved.SuperClears1000);
                NewAchieved(Achieved.SuperClears1000);
            }
        }

        public static void CheckCredits1000(List<Achieved> list)
        {
            if (list.Contains(Achieved.Credits1000)) return;

            if (SaveGame.Members.MonsterCreditsLifetime_09_08_2025 >= 1000)
            {
                list.Add(Achieved.Credits1000);
                NewAchieved(Achieved.Credits1000);
            }
        }

        public static void CheckStormLord5(List<Achieved> list)
        {
            if (list.Contains(Achieved.StormLord5)) return;

            if (SaveGame.Members.LevelStormLord >= 5)
            {
                list.Add(Achieved.StormLord5);
                NewAchieved(Achieved.StormLord5);
            }
        }

        public static void CheckMystery50(List<Achieved> list)
        {
            if (list.Contains(Achieved.Mystery50)) return;

            if (SaveGame.Members.MysteryCollected >= 50)
            {
                list.Add(Achieved.Mystery50);
                NewAchieved(Achieved.Mystery50);
            }
        }
    }
}
