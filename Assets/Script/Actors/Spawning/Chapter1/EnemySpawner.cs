using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EnemySpawner
{
    public const long HpScale = 5;

    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        const long MaxEnemies = 100;
        long HpBase = 40;
        long HpBasePerLevel = 60;

        long hpTargetForRound = HpBase + ((level - 1) * HpBasePerLevel) + ((level - 1) * (level - 1) * 10);
        hpTargetForRound *= HpScale;

        // Base HP values (do NOT scale arbitrarily)
        long hpBat = 20 * HpScale;
        long hpOgreSmall = 50 * HpScale;
        long hpOgreLarge = 1000 * HpScale;
        long hpHeroChaser = 10_000 * HpScale;
        long hpGreen = 50_000 * HpScale;
        long hpRaven = 250_000 * HpScale;
        long hpRed = 1_000_000 * HpScale;
        long hpPig = 2_500_000 * HpScale;
        long hpPigtail = 5_000_000 * HpScale;
        long hpWhite = 10_000_000 * HpScale;
        long hpFez = 20_000_000 * HpScale;
        long hpHelmet = 50_000_000 * HpScale;
        long hpSwede = 100_000_000 * HpScale;
        long hpPigHat = 200_000_000 * HpScale;
        long hpFreakyWiz = 400_000_000 * HpScale;
        long hpUndeadPirate = 800_000_000 * HpScale;
        long hpWannabeNecro = 1_600_000_000 * HpScale;

        bool allowLarge = hpTargetForRound >= hpOgreLarge;
        bool allowHeroChaser = hpTargetForRound >= hpHeroChaser;
        bool allowGreen = hpTargetForRound >= hpGreen;
        bool allowRaven = hpTargetForRound >= hpRaven;
        bool allowRed = hpTargetForRound >= hpRed;
        bool allowPig = hpTargetForRound >= hpPig;
        bool allowPigtail = hpTargetForRound >= hpPigtail;
        bool allowWhite = hpTargetForRound >= hpWhite;
        bool allowFez = hpTargetForRound >= hpFez;
        bool allowHelmet = hpTargetForRound >= hpHelmet;
        bool allowSwede = hpTargetForRound >= hpSwede;
        bool allowPigHat = hpTargetForRound >= hpPigHat;
        bool allowFreakyWiz = hpTargetForRound >= hpFreakyWiz;
        bool allowUndeadPirate = hpTargetForRound >= hpUndeadPirate;
        bool allowWannabeNecro = hpTargetForRound >= hpWannabeNecro;

        //var tester = SpawnUtil.Single(ActorTypeEnum.WannabeNecro, Vector2.zero).First();
        //tester.BaseHp = hpFez;
        //yield return tester;

        List<ActorBase> enemies = new();
        int safety = 100;
        while (true)
        {
            long totalEnemies = 0;
            long remainingHp = hpTargetForRound;
            enemies.Clear();

            // 1. WannabeNecro
            long clampedWannabeNecro = 0;
            if (allowWannabeNecro)
            {
                clampedWannabeNecro = Math.Min(remainingHp / hpWannabeNecro, MaxEnemies - totalEnemies);
                clampedWannabeNecro -= UnityEngine.Random.Range(0, 2);
                clampedWannabeNecro = Math.Max(0, clampedWannabeNecro);
                remainingHp -= clampedWannabeNecro * hpWannabeNecro;
                totalEnemies += clampedWannabeNecro;
            }

            // 2. UndeadPirate
            long clampedUndeadPirate = 0;
            if (allowUndeadPirate)
            {
                clampedUndeadPirate = Math.Min(remainingHp / hpUndeadPirate, MaxEnemies - totalEnemies);
                clampedUndeadPirate -= UnityEngine.Random.Range(0, 2);
                clampedUndeadPirate = Math.Max(0, clampedUndeadPirate);
                remainingHp -= clampedUndeadPirate * hpUndeadPirate;
                totalEnemies += clampedUndeadPirate;
            }

            // 3. FreakyWiz
            long clampedFreakyWiz = 0;
            if (allowFreakyWiz)
            {
                clampedFreakyWiz = Math.Min(remainingHp / hpFreakyWiz, MaxEnemies - totalEnemies);
                clampedFreakyWiz -= UnityEngine.Random.Range(0, 2);
                clampedFreakyWiz = Math.Max(0, clampedFreakyWiz);
                remainingHp -= clampedFreakyWiz * hpFreakyWiz;
                totalEnemies += clampedFreakyWiz;
            }

            // 4. PigHat
            long clampedPigHat = 0;
            if (allowPigHat)
            {
                clampedPigHat = Math.Min(remainingHp / hpPigHat, MaxEnemies - totalEnemies);
                clampedPigHat -= UnityEngine.Random.Range(0, 2);
                clampedPigHat = Math.Max(0, clampedPigHat);
                remainingHp -= clampedPigHat * hpPigHat;
                totalEnemies += clampedPigHat;
            }

            // 5. Swede
            long clampedSwede = 0;
            if (allowSwede)
            {
                clampedSwede = Math.Min(remainingHp / hpSwede, MaxEnemies - totalEnemies);
                clampedSwede -= UnityEngine.Random.Range(0, 2);
                clampedSwede = Math.Max(0, clampedSwede);
                remainingHp -= clampedSwede * hpSwede;
                totalEnemies += clampedSwede;
            }

            // 6. Helmet
            long clampedHelmet = 0;
            if (allowHelmet)
            {
                clampedHelmet = Math.Min(remainingHp / hpHelmet, MaxEnemies - totalEnemies);
                clampedHelmet -= UnityEngine.Random.Range(0, 2);
                clampedHelmet = Math.Max(0, clampedHelmet);
                remainingHp -= clampedHelmet * hpHelmet;
                totalEnemies += clampedHelmet;
            }

            // 7. Fez
            long clampedFez = 0;
            if (allowFez)
            {
                clampedFez = Math.Min(remainingHp / hpFez, MaxEnemies - totalEnemies);
                clampedFez -= UnityEngine.Random.Range(0, 2);
                clampedFez = Math.Max(0, clampedFez);
                remainingHp -= clampedFez * hpFez;
                totalEnemies += clampedFez;
            }

            // 8. White
            long clampedWhite = 0;
            if (allowWhite)
            {
                clampedWhite = Math.Min(remainingHp / hpWhite, MaxEnemies - totalEnemies);
                clampedWhite -= UnityEngine.Random.Range(0, 2);
                clampedWhite = Math.Max(0, clampedWhite);
                remainingHp -= clampedWhite * hpWhite;
                totalEnemies += clampedWhite;
            }

            // 9. Pigtail
            long clampedPigtail = 0;
            if (allowPigtail)
            {
                clampedPigtail = Math.Min(remainingHp / hpPigtail, MaxEnemies - totalEnemies);
                clampedPigtail -= UnityEngine.Random.Range(0, 2);
                clampedPigtail = Math.Max(0, clampedPigtail);
                remainingHp -= clampedPigtail * hpPigtail;
                totalEnemies += clampedPigtail;
            }

            // 10. Pig
            long clampedPig = 0;
            if (allowPig)
            {
                clampedPig = Math.Min(remainingHp / hpPig, MaxEnemies - totalEnemies);
                clampedPig -= UnityEngine.Random.Range(0, 2);
                clampedPig = Math.Max(0, clampedPig);
                remainingHp -= clampedPig * hpPig;
                totalEnemies += clampedPig;
            }

            // 11. Red
            long clampedRed = 0;
            if (allowRed)
            {
                clampedRed = Math.Min(remainingHp / hpRed, MaxEnemies - totalEnemies);
                clampedRed -= UnityEngine.Random.Range(0, 2);
                clampedRed = Math.Max(0, clampedRed);
                remainingHp -= clampedRed * hpRed;
                totalEnemies += clampedRed;
            }

            // 12. Ravens
            long clampedRaven = 0;
            if (allowRaven)
            {
                clampedRaven = Math.Min(remainingHp / hpRaven, MaxEnemies - totalEnemies);
                clampedRaven -= UnityEngine.Random.Range(0, 2);
                clampedRaven = Math.Max(0, clampedRaven);
                remainingHp -= clampedRaven * hpRaven;
                totalEnemies += clampedRaven;
            }

            // 13. Green
            long clampedGreen = 0;
            if (allowGreen)
            {
                clampedGreen = Math.Min(remainingHp / hpGreen, MaxEnemies - totalEnemies);
                clampedGreen -= UnityEngine.Random.Range(0, 2);
                clampedGreen = Math.Max(0, clampedGreen);
                remainingHp -= clampedGreen * hpGreen;
                totalEnemies += clampedGreen;
            }

            // 14. Hero Chasers
            long clampedHeroChaser = 0;
            if (allowHeroChaser)
            {
                clampedHeroChaser = Math.Min(remainingHp / hpHeroChaser, MaxEnemies - totalEnemies);
                clampedHeroChaser -= UnityEngine.Random.Range(0, 2);
                clampedHeroChaser = Math.Max(0, clampedHeroChaser);
                remainingHp -= clampedHeroChaser * hpHeroChaser;
                totalEnemies += clampedHeroChaser;
            }

            // 15. Large Ogres
            long clampedLarge = 0;
            if (allowLarge)
            {
                clampedLarge = Math.Min(remainingHp / hpOgreLarge, MaxEnemies - totalEnemies);
                clampedLarge -= UnityEngine.Random.Range(0, 2);
                clampedLarge = Math.Max(0, clampedLarge);
                remainingHp -= clampedLarge * hpOgreLarge;
                totalEnemies += clampedLarge;
            }

            // 16. Small Ogres
            long clampedSmall = Math.Min(remainingHp / hpOgreSmall, MaxEnemies - totalEnemies);
            clampedSmall -= UnityEngine.Random.Range(0, 2);
            clampedSmall = Math.Max(0, clampedSmall);
            remainingHp -= clampedSmall * hpOgreSmall;
            totalEnemies += clampedSmall;

            // 17. Bats
            long clampedBat = Math.Min(remainingHp / hpBat, MaxEnemies - totalEnemies);
            clampedBat = Math.Max(0, clampedBat);
            remainingHp -= clampedBat * hpBat;
            totalEnemies += clampedBat;

            if (totalEnemies == 0)
            {
                clampedBat = 1;
                remainingHp -= hpBat;
            }

            // Add WannabeNecro
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.WannabeNecro, (int)clampedWannabeNecro))
            {
                actor.BaseHp = hpWannabeNecro;
                enemies.Add(actor);
            }

            // Add UndeadPirate
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.UndeadPirate, (int)clampedUndeadPirate))
            {
                actor.BaseHp = hpUndeadPirate;
                enemies.Add(actor);
            }

            // Add FreakyWiz
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.FreakyWiz, (int)clampedFreakyWiz))
            {
                actor.BaseHp = hpFreakyWiz;
                enemies.Add(actor);
            }

            // Add PigHat
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.PigHat, (int)clampedPigHat))
            {
                actor.BaseHp = hpPigHat;
                enemies.Add(actor);
            }

            // Add Swede
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Swede, (int)clampedSwede))
            {
                actor.BaseHp = hpSwede;
                enemies.Add(actor);
            }

            // Add Helmet
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Helmet, (int)clampedHelmet))
            {
                actor.BaseHp = hpHelmet;
                enemies.Add(actor);
            }

            // Add Fez
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Fez, (int)clampedFez))
            {
                actor.BaseHp = hpFez;
                enemies.Add(actor);
            }

            // Add White
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.White, (int)clampedWhite))
            {
                actor.BaseHp = hpWhite;
                enemies.Add(actor);
            }

            // Add Pigtail
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Pigtail, (int)clampedPigtail))
            {
                actor.BaseHp = hpPigtail;
                enemies.Add(actor);
            }

            // Add Pig
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Pig, (int)clampedPig))
            {
                actor.BaseHp = hpPig;
                enemies.Add(actor);
            }

            // Add Red
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Red, (int)clampedRed))
            {
                actor.BaseHp = hpRed;
                enemies.Add(actor);
            }

            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Raven, (int)clampedRaven))
            {
                actor.BaseHp = hpRaven;
                enemies.Add(actor);
            }

            // Add Green
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Green, (int)clampedGreen))
            {
                actor.BaseHp = hpGreen;
                enemies.Add(actor);
            }

            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.HeroChaser, (int)clampedHeroChaser))
            {
                actor.BaseHp = hpHeroChaser;
                enemies.Add(actor);
            }

            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.OgreLarge, (int)clampedLarge))
            {
                actor.BaseHp = hpOgreLarge;
                enemies.Add(actor);
            }

            bool tryCircle = UnityEngine.Random.value < 0.25;
            if (tryCircle && clampedSmall > 5)
            {
                float circleHeight = (GameManager.ArenaBounds.height - 2) / 2;
                Vector2 center = GameManager.ArenaBounds.center + Vector2.right * 3;
                foreach (var actor in SpawnUtil.Circle(ActorTypeEnum.OgreSmall, center, radius: circleHeight, (int)clampedSmall))
                {
                    actor.BaseHp = hpOgreSmall;
                    enemies.Add(actor);
                }
            }
            else
            {
                foreach (var actor in SpawnUtil.Random(ActorTypeEnum.OgreSmall, (int)clampedSmall))
                {
                    actor.BaseHp = hpOgreSmall;
                    enemies.Add(actor);
                }
            }

            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.BatWhite, (int)clampedBat))
            {
                actor.BaseHp = hpBat;
                enemies.Add(actor);
            }

            if (enemies.Count < MaxEnemies)
                break;

            // Increase HP budgets and retry
            hpWannabeNecro *= 10;
            hpUndeadPirate *= 10;
            hpFreakyWiz *= 10;
            hpPigHat *= 10;
            hpSwede *= 10;
            hpHelmet *= 10;
            hpFez *= 10;
            hpWhite *= 10;
            hpPigtail *= 10;
            hpPig *= 10;
            hpRed *= 10;
            hpRaven *= 10;
            hpGreen *= 10;
            hpHeroChaser *= 10;
            hpOgreLarge *= 10;
            hpOgreSmall *= 10;
            hpBat *= 10;

            if (--safety <= 0)
                throw new Exception("Tried spawning <100 enemies too many times");
        }

        foreach (var actor in enemies)
            yield return actor;
    }
}