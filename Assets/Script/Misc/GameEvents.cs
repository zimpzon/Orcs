using System.Collections.Generic;
using System.Text;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Assets.Script
{
    public enum DeedEnum { None, SnipersParadise, MachinegunMadness, LittleMonsters, WhiteWalkers };

    public class DamageUnlockInfo
    {
        public int Amount;
        public GameCounter Counter;
        public int Requirement;
    }

    public class WeaponUnlockInfo
    {
        public WeaponType Type;
        public GameCounter Counter;
        public int Requirement;
    }

    public class GameModeUnlockInfo
    {
        public GameModeEnum GameMode;
        public GameCounter Counter;
        public int Requirement;
    }

    public static class GameEvents
    {
        public static List<DamageUnlockInfo> DamageUnlockInfo = new List<DamageUnlockInfo>
        {
            new DamageUnlockInfo { Amount = 10, Counter = GameCounter.Kill_Any, Requirement = 100 },
            new DamageUnlockInfo { Amount = 10, Counter = GameCounter.Kill_Any, Requirement = 500 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.Kill_Any, Requirement = 5000 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.Kill_Any, Requirement = 10000 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.Score_Any_Sum, Requirement = 500 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.Score_Any_Sum, Requirement = 1000 },
            new DamageUnlockInfo { Amount = 10, Counter = GameCounter.Kill_BigWalker, Requirement = 100 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.Kill_Caster, Requirement = 200 },
            new DamageUnlockInfo { Amount = 10, Counter = GameCounter.Max_Score_Nursery, Requirement = 25 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.Max_Score_Nursery, Requirement = 35 },
            new DamageUnlockInfo { Amount = 30, Counter = GameCounter.Max_Score_Nursery, Requirement = 45 },
            new DamageUnlockInfo { Amount = 20, Counter = GameCounter.score_Harmony_Sum, Requirement = 50 },
        };

        public static List<WeaponUnlockInfo> WeaponUnlockInfo = new List<WeaponUnlockInfo>
        {
            new WeaponUnlockInfo { Type = WeaponType.Sniper, Counter = GameCounter.Score_Any_Sum, Requirement = 20 },
            new WeaponUnlockInfo { Type = WeaponType.ShotgunSlug, Counter = GameCounter.Score_Any_Sum, Requirement = 100 },
            new WeaponUnlockInfo { Type = WeaponType.Horn, Counter = GameCounter.Score_Any_Sum, Requirement = 200 },
            new WeaponUnlockInfo { Type = WeaponType.Paintball, Counter = GameCounter.Score_Any_Sum, Requirement = 300 },
            new WeaponUnlockInfo { Type = WeaponType.Staff, Counter = GameCounter.Max_Score_Earth, Requirement = 15 },
            new WeaponUnlockInfo { Type = WeaponType.SuperShotgun, Counter = GameCounter.Max_Score_Wind, Requirement = 15 },
            new WeaponUnlockInfo { Type = WeaponType.SawedShotgun, Counter = GameCounter.Score_Fire_Sum, Requirement = 15 },
            new WeaponUnlockInfo { Type = WeaponType.Rambo, Counter = GameCounter.score_Harmony_Sum, Requirement = 75 },
            new WeaponUnlockInfo { Type = WeaponType.Staff2, Counter = GameCounter.Kill_Any, Requirement = 2500 },
        };

        public static List<GameModeUnlockInfo> GameModeUnlockInfo = new List<GameModeUnlockInfo>
        {
            new GameModeUnlockInfo { GameMode = GameModeEnum.Earth, Counter = GameCounter.Max_Score_Nursery, Requirement = 40 },
            new GameModeUnlockInfo { GameMode = GameModeEnum.Wind, Counter = GameCounter.Max_Score_Earth, Requirement = 40 },
            new GameModeUnlockInfo { GameMode = GameModeEnum.Fire, Counter = GameCounter.Max_Score_Wind, Requirement = 35 },
            new GameModeUnlockInfo { GameMode = GameModeEnum.Storm, Counter = GameCounter.Max_Score_Fire, Requirement = 30 },
            new GameModeUnlockInfo { GameMode = GameModeEnum.Harmony, Counter = GameCounter.unlocked_paintball, Requirement = 1 },
        };

        public static bool IsUnlocked(GameModeEnum gameMode)
        {
            var gm = GameModeUnlockInfo.Where(x => x.GameMode == gameMode).FirstOrDefault();
            return gm == null || SaveGame.Members.ReqMet(gm.Requirement, gm.Counter);
        }

        public static string GameModeInfo(GameModeEnum gameMode)
        {
            var gm = GameModeUnlockInfo.Where(x => x.GameMode == gameMode).FirstOrDefault();
            bool isUnlocked = gm == null || SaveGame.Members.ReqMet(gm.Requirement, gm.Counter);
            if (isUnlocked)
            {
                switch(gameMode)
                {
                    case GameModeEnum.Nursery: return "So cute they are!";
                    case GameModeEnum.Earth: return "A Rotten Stench Is In The Air";
                    case GameModeEnum.Wind: return "Sprinting In Full Armor Is Impressive";
                    case GameModeEnum.Fire: return "Fire. So much fire.";
                    case GameModeEnum.Storm: return "Madness.";
                    case GameModeEnum.Harmony: return "No Violence, please.";
                    default: return "unknown: " + gameMode.ToString();
                }
            }
            else
            {
                return string.Format(ActionDisplayString(gm.Counter), SaveGame.Members.GetCounter(gm.Counter), gm.Requirement);
            }
        }

        public static string GameModeDisplayName(GameModeEnum gameMode)
        {
            switch (gameMode)
            {
                case GameModeEnum.Nursery: return "Orc Nursery";
                case GameModeEnum.Earth: return "Plane Of Earth";
                case GameModeEnum.Wind: return "Plane Of Wind";
                case GameModeEnum.Fire: return "Plane Of Fire";
                case GameModeEnum.Storm: return "Perfect Storm";
                case GameModeEnum.Harmony: return "Plane Of Harmony";
                default: return "Unknown: " + gameMode.ToString();
            }
        }

        public static string ActionDisplayString(GameCounter counter)
        {
            switch (counter)
            {
                case GameCounter.unlocked_paintball:return "Paintball required (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Player_Death:      return "Look On The Bright Side Of Death (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";

                case GameCounter.Kill_Any:          return "Kill Enemies Of Any Type (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Kill_BigWalker:    return "Kill Big Enemies (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Kill_Caster:       return "Kill Caster Enemies (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";

                case GameCounter.Score_Nursery_Sum: return "Score a Total Of <#ffffff>{1}</color> at Orc Nursery (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Score_Earth_Sum:   return "Score a Total Of <#ffffff>{1}</color> at Plane Of Earth (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Score_Wind_Sum:    return "Score a Total Of <#ffffff>{1}</color> at Plane Of Wind (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Score_Fire_Sum:    return "Score a Total Of <#ffffff>{1}</color> at Plane Of Fire (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Score_Storm_Sum:   return "Score a Total Of <#ffffff>{1}</color> at Perfect Storm (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.score_Harmony_Sum: return "Score a Total Of <#ffffff>{1}</color> at Plane Of Harmony (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Score_Any_Sum:     return "Score a Total Of <#ffffff>{1}</color> Anywhere (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";

                case GameCounter.Max_Score_Nursery: return "Reach a Score Of <#ffffff>{1}</color> at Orc Nursery (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Max_Score_Earth:   return "Reach a Score Of <#ffffff>{1}</color> at Plane Of Earth (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Max_Score_Wind:    return "Reach a Score Of <#ffffff>{1}</color> at Plane Of Wind (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Max_Score_Fire:    return "Reach a Score Of <#ffffff>{1}</color> at Plane Of Fire (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Max_Score_Storm:   return "Reach a Score Of <#ffffff>{1}</color> at Perfect Storm (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Max_score_Harmony: return "Reach a Score Of <#ffffff>{1}</color> at Plane Of Harmony (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";
                case GameCounter.Max_Score_Any:     return "Reach a Score Of <#ffffff>{1}</color> Anywhere (<#ffffff>{0}</color>/<#ffffff>{1}</color>)";

                default: return "Unknown: " + counter.ToString();
            }
        }

        static string FormatCounterAndRequirement(string format, int req, int counter)
        {
            if (counter > req)
                counter = req;

            return string.Format(format, counter, req);
        }

        public static string WrapInColor(string str, bool reqMet)
        {
            return string.Format("<{0}>{1}</color>", reqMet ? GameManager.Instance.ColorUnlocked : GameManager.Instance.ColorLocked, str);
        }

        public static IEnumerable<string> FormattedWepReqs()
        {
            foreach (var wu in WeaponUnlockInfo)
                yield return FormatCounterAndRequirement(ActionDisplayString(wu.Counter), wu.Requirement, SaveGame.Members.GetCounter(wu.Counter));
        }

        public static IEnumerable<string> FormattedWepNames()
        {
            foreach (var wu in WeaponUnlockInfo)
                yield return WrapInColor(WeaponBase.WeaponDisplayName(wu.Type), SaveGame.Members.ReqMet(wu.Requirement, wu.Counter));
        }

        public static IEnumerable<string> FormattedGameModeReqs()
        {
            foreach (var gm in GameModeUnlockInfo)
                yield return FormatCounterAndRequirement(ActionDisplayString(gm.Counter), gm.Requirement, SaveGame.Members.GetCounter(gm.Counter));
        }

        public static IEnumerable<string> FormattedGameModeNames()
        {
            foreach (var gm in GameModeUnlockInfo)
                yield return WrapInColor(GameModeDisplayName(gm.GameMode), SaveGame.Members.ReqMet(gm.Requirement, gm.Counter));
        }

        public static IEnumerable<string> FormattedUpgradeReqs()
        {
            foreach (var dmg in DamageUnlockInfo)
                yield return FormatCounterAndRequirement(ActionDisplayString(dmg.Counter), dmg.Requirement, SaveGame.Members.GetCounter(dmg.Counter));
        }

        public static IEnumerable<string> FormattedUpgradeNames()
        {
            foreach (var dmg in DamageUnlockInfo)
                yield return WrapInColor(string.Format("+{0}%", dmg.Amount), SaveGame.Members.ReqMet(dmg.Requirement, dmg.Counter));
        }

        static string MaskString(string str, bool doMask)
        {
            if (!doMask)
                return str;

            var sb = new StringBuilder(20);
            for (int i = 0; i < str.Length; ++i)
                sb.Append(str[i] == ' ' ? ' ' : '*');

            return sb.ToString();
        }

        // Important: All counter events must go through here so unlocks can be checked
        public static void CounterEvent(GameCounter counter, int amount)
        {
            if (SaveGame.Members.IsMaxCounter(counter))
                SaveGame.Members.TryUpdateMaxValue(counter, amount);
            else
                SaveGame.Members.UpdateCounter(counter, amount);

            const float DisplayTime = 3.0f;

            var blink = GameManager.Instance.TextNewUnlock.GetComponent<TextBlinkScript>();
            var blinkTrans = GameManager.Instance.TextNewUnlock.transform;

            // This is all pretty ugly. Redundant formatting in dam, overwriting each other etc.

            // Check for new damage
            var newDamage = Unlocks.UnlockEarnedDamage(onlyExactMatch: true);
            foreach (var newDam in newDamage)
                blink.SetText(string.Format("{0} Unlocked!", string.Format("+{0}% Damage", newDam.Amount), SaveGame.Members.ReqMet(newDam.Requirement, newDam.Counter)), DisplayTime);

            // Check for new weapons
            var newWeps = Unlocks.UnlockEarnedWeapons(onlyExactMatch: true);
            foreach (var newWep in newWeps)
                blink.SetText(string.Format("{0} Unlocked!", WeaponBase.WeaponDisplayName(newWep.Type)), DisplayTime);

            // Check for new game modes
            var newModes = Unlocks.UnlockEarnedGameModes(onlyExactMatch: true);
            foreach (var newMode in newModes)
                blink.SetText(string.Format("{0} Unlocked!", GameModeDisplayName(newMode.GameMode)), DisplayTime);

            // Check for new heroes
            var newHeroes = Unlocks.UnlockEarnedHeroes(onlyExactMatch: true);
            foreach (var newHero in newHeroes)
                blink.SetText(string.Format("{0} Unlocked!", newHero.Name), DisplayTime);

            GameManager.Instance.RoundUnlockCount += newDamage.Count + newWeps.Count + newModes.Count + newHeroes.Count;
            bool hadNewUnlock = newDamage.Count > 0 || newWeps.Count > 0 || newModes.Count > 0 || newHeroes.Count > 0;
            if (hadNewUnlock)
            {
                Unlocks.LatestUnlockText = blink.Text.text;

                float basePos = GameManager.Instance.TextUnlockBasePos;
                ((RectTransform)blinkTrans).DOAnchorPosY(basePos - 300, 0.0f);
                ((RectTransform)blinkTrans).DOAnchorPosY(basePos, 0.5f);

                blinkTrans.DOScale(0.5f, 0.0f);
                blinkTrans.DOScale(1.0f, 0.5f);
            }

            // TODO PE: This is sort of hacked. It might be ok for now. If X was present in the newly unlocked weapons then set the corresponding GameCounter.
            UpdateUnlockCounters(newWeps, WeaponType.Paintball, GameCounter.unlocked_paintball);
            UpdateUnlockCounters(newWeps, WeaponType.ShotgunSlug, GameCounter.unlocked_slug);
            UpdateUnlockCounters(newWeps, WeaponType.Rambo, GameCounter.unlocked_rambo);
            UpdateUnlockCounters(newWeps, WeaponType.Staff, GameCounter.unlocked_staff);
        }

        static void UpdateUnlockCounters(List<WeaponUnlockInfo> newWeapons, WeaponType ifThisWasJustUnlocked, GameCounter thenSetThisCounter)
        {
            if (newWeapons.Where(wep => wep.Type == ifThisWasJustUnlocked).Count() == 1)
                GameEvents.CounterEvent(thenSetThisCounter, 1);
        }
    }
}
