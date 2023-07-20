using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets.Script
{
    public static class Unlocks
    {
        public static int ForceWeaponCount;
        public static WeaponType ForceWeaponType;

        public static string LatestUnlockText = string.Empty;

        static List<WeaponType> unlockedWeapons_ = new List<WeaponType>();
        static List<GameModeEnum> unlockedGameModes_ = new List<GameModeEnum>();
        static List<DamageUnlockInfo> unlockedDamage_ = new List<DamageUnlockInfo>();
        static List<HeroEnum> unlockedHeroes_ = new List<HeroEnum>();

        public static void RefreshUnlocked()
        {
            CheckDebugUnlocks();

            unlockedWeapons_.Clear();
            //UnlockEarnedWeapons();
            //UnlockWeapon(WeaponType.Unarmed);
            //UnlockWeapon(WeaponType.Shotgun);
            //UnlockWeapon(WeaponType.Grenade);
            //UnlockWeapon(WeaponType.Paintball);
            UnlockWeapon(WeaponType.Machinegun);
            //UnlockWeapon(WeaponType.Yoda);
            //UnlockWeapon(WeaponType.Rambo);
            //UnlockWeapon(WeaponType.Sawblade);
            //UnlockWeapon(WeaponType.Staff);
            //UnlockWeapon(WeaponType.Staff2);

            unlockedGameModes_.Clear();
            UnlockEarnedGameModes();

            unlockedDamage_.Clear();

            unlockedHeroes_.Clear();
            UnlockEarnedHeroes();
        }

        static void CheckDebugUnlocks()
        {
            if (GameManager.Instance.UnlockAllGameModes)
            {
                GameManager.SetDebugOutput("All game modes unlocked", Time.time);
                foreach (var gm in GameEvents.GameModeUnlockInfo)
                {
                    if (SaveGame.Members.IsMaxCounter(gm.Counter))
                        SaveGame.Members.TryUpdateMaxValue(gm.Counter, gm.Requirement);
                    else
                        SaveGame.Members.UpdateCounter(gm.Counter, gm.Requirement);
                }
            }

            if (GameManager.Instance.UnlockAllWeapons)
            {
                GameManager.SetDebugOutput("All weapons unlocked", Time.time);
                foreach (var wep in GameEvents.WeaponUnlockInfo)
                {
                    if (SaveGame.Members.IsMaxCounter(wep.Counter))
                        SaveGame.Members.TryUpdateMaxValue(wep.Counter, wep.Requirement);
                    else
                        SaveGame.Members.UpdateCounter(wep.Counter, wep.Requirement);
                }
            }

            if (GameManager.Instance.UnlockAllHeroes)
            {
                GameManager.SetDebugOutput("All heroes unlocked", Time.time);
                foreach (var hero in GameManager.Instance.Heroes)
                {
                    if (SaveGame.Members.IsMaxCounter(hero.GameCounter))
                        SaveGame.Members.TryUpdateMaxValue(hero.GameCounter, hero.Req);
                    else
                        SaveGame.Members.UpdateCounter(hero.GameCounter, hero.Req);
                }
            }
        }

        public static int CountPossibleUnlocks()
        {
            return GameEvents.WeaponUnlockInfo.Count + GameEvents.GameModeUnlockInfo.Count + GameEvents.DamageUnlockInfo.Count;
        }

        public static int CountUnlocked()
        {
            return CountEarnedWeapons() + CountEarnedGameModes() + CountEarnedDamage();
        }

        // Game modes
        public static int CountEarnedGameModes()
        {
            return GameEvents.GameModeUnlockInfo.Where(gm => SaveGame.Members.GetCounter(gm.Counter) >= gm.Requirement).Count();
        }

        static List<GameModeUnlockInfo> newGameModeUnlocks_ = new List<GameModeUnlockInfo>();

        public static List<GameModeUnlockInfo> UnlockEarnedGameModes(bool onlyExactMatch = false)
        {
            newGameModeUnlocks_.Clear();
            foreach (var gm in GameEvents.GameModeUnlockInfo)
            {
                int counterValue = SaveGame.Members.GetCounter(gm.Counter);
                bool isMatch = (counterValue >= gm.Requirement && !onlyExactMatch) || (counterValue == gm.Requirement);
                if (isMatch && !unlockedGameModes_.Contains(gm.GameMode))
                {
                    newGameModeUnlocks_.Add(gm);
                    unlockedGameModes_.Add(gm.GameMode);
//                    Debug.LogFormat("Unlocked GameMode {0}", gm.Counter.ToString());
                }
            }
            return newGameModeUnlocks_;
        }

        // Weapons
        public static int CountEarnedWeapons()
        {
            return GameEvents.WeaponUnlockInfo.Where(wep => SaveGame.Members.GetCounter(wep.Counter) >= wep.Requirement).Count();
        }

        static List<WeaponUnlockInfo> newWeaponUnlocks_ = new List<WeaponUnlockInfo>();

        public static List<WeaponUnlockInfo> UnlockEarnedWeapons(bool onlyExactMatch = false)
        {
            newWeaponUnlocks_.Clear();
            foreach (var wep in GameEvents.WeaponUnlockInfo)
            {
                int counterValue = SaveGame.Members.GetCounter(wep.Counter);
                bool isMatch = (counterValue >= wep.Requirement && !onlyExactMatch) || (counterValue == wep.Requirement);
                if (isMatch && !unlockedWeapons_.Contains(wep.Type))
                {
//                    Debug.LogFormat("Unlocked weapon {0}", wep.Type);
                    UnlockWeapon(wep.Type, forceSwitch: onlyExactMatch);
                    newWeaponUnlocks_.Add(wep);
                }
            }
            return newWeaponUnlocks_;
        }

        // Damage
        public static int CountEarnedDamage()
        {
            return GameEvents.DamageUnlockInfo.Where(dam => SaveGame.Members.GetCounter(dam.Counter) >= dam.Requirement).Count();
        }

        static List<DamageUnlockInfo> newDamageUnlocks_ = new List<DamageUnlockInfo>();

        // Heroes
        static List<Hero> newHeroUnlocks_ = new List<Hero>();

        public static List<Hero> UnlockEarnedHeroes(bool onlyExactMatch = false)
        {
            newHeroUnlocks_.Clear();
            foreach (var hero in GameManager.Instance.Heroes)
            {
                int counterValue = SaveGame.Members.GetCounter(hero.GameCounter);
                bool isMatch = (counterValue >= hero.Req && !onlyExactMatch) || (counterValue == hero.Req);
                if (isMatch && !unlockedHeroes_.Contains(hero.HeroType))
                {
//                    Debug.LogFormat("Unlocked hero {0}", hero.HeroType);
                    unlockedHeroes_.Add(hero.HeroType);
                    newHeroUnlocks_.Add(hero);
                }
            }
            return newHeroUnlocks_;
        }

        public static WeaponType PickRandomWeaponFromlist(List<WeaponType> list, WeaponType notThis)
        {
            WeaponType orThis = WeaponType.None;

            // If only 1 in list we have no choice
            if (list.Count == 1)
                return list[0];

            // Don't go from unarmed to paintball or vice versa. It is frustrating.
            switch(notThis)
            {
                case WeaponType.Unarmed: orThis = WeaponType.Paintball; break;
                case WeaponType.Paintball: orThis = WeaponType.Unarmed; break;
            }

            for (int i = 0; i < 100; ++i)
            {
                int idx = UnityEngine.Random.Range(0, list.Count);
                WeaponType rndWeapon = list[idx];

                // Make Paintball more rare
                if (rndWeapon == WeaponType.Paintball && Random.value < 0.5f)
                    continue;

                if (rndWeapon != notThis && rndWeapon != orThis && WeaponIsUnlocked(rndWeapon))
                    return rndWeapon;
            }
            // Failed to find a new one. Have to return notThis.
            return notThis;
        }

        public static bool AllWeaponsUnlocked()
        {
            return GameEvents.WeaponUnlockInfo.Where(wep => !WeaponIsUnlocked(wep.Type)).Count() == 0;
        }

        public static bool WeaponIsUnlocked(WeaponType type)
        {
            return unlockedWeapons_.Contains(type);
        }

        public static void UnlockWeapon(WeaponType wep, bool forceSwitch = false)
        {
            if (unlockedWeapons_.Contains(wep))
                return;

            if (forceSwitch)
            {
                ForceWeaponCount = 2;
                ForceWeaponType = wep;
            }

            unlockedWeapons_.Add(wep);
        }
    }
}
