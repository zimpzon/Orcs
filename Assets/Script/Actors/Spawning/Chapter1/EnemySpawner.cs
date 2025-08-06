using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
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
        long hpPigtail = 5_000_000 * HpScale;

        bool allowLarge = hpTargetForRound >= hpOgreLarge;
        bool allowHeroChaser = hpTargetForRound >= hpHeroChaser;
        bool allowGreen = hpTargetForRound >= hpGreen;
        bool allowRaven = hpTargetForRound >= hpRaven;
        bool allowRed = hpTargetForRound >= hpRed;
        bool allowPigtail = hpTargetForRound >= hpPigtail;

        List<ActorBase> enemies = new();
        int safety = 100;
        while (true)
        {
            long totalEnemies = 0;
            long remainingHp = hpTargetForRound;
            enemies.Clear();

            // 1. Pigtail
            long clampedPigtail = 0;
            if (allowPigtail)
            {
                clampedPigtail = Math.Min(remainingHp / hpPigtail, MaxEnemies - totalEnemies);
                clampedPigtail -= UnityEngine.Random.Range(0, 2);
                clampedPigtail = Math.Max(0, clampedPigtail);
                remainingHp -= clampedPigtail * hpPigtail;
                totalEnemies += clampedPigtail;
            }

            // 2. Red
            long clampedRed = 0;
            if (allowRed)
            {
                clampedRed = Math.Min(remainingHp / hpRed, MaxEnemies - totalEnemies);
                clampedRed -= UnityEngine.Random.Range(0, 2);
                clampedRed = Math.Max(0, clampedRed);
                remainingHp -= clampedRed * hpRed;
                totalEnemies += clampedRed;
            }

            // 3. Ravens
            long clampedRaven = 0;
            if (allowRaven)
            {
                clampedRaven = Math.Min(remainingHp / hpRaven, MaxEnemies - totalEnemies);
                clampedRaven -= UnityEngine.Random.Range(0, 2);
                clampedRaven = Math.Max(0, clampedRaven);
                remainingHp -= clampedRaven * hpRaven;
                totalEnemies += clampedRaven;
            }

            // 4. Green
            long clampedGreen = 0;
            if (allowGreen)
            {
                clampedGreen = Math.Min(remainingHp / hpGreen, MaxEnemies - totalEnemies);
                clampedGreen -= UnityEngine.Random.Range(0, 2);
                clampedGreen = Math.Max(0, clampedGreen);
                remainingHp -= clampedGreen * hpGreen;
                totalEnemies += clampedGreen;
            }

            // 5. Hero Chasers
            long clampedHeroChaser = 0;
            if (allowHeroChaser)
            {
                clampedHeroChaser = Math.Min(remainingHp / hpHeroChaser, MaxEnemies - totalEnemies);
                clampedHeroChaser -= UnityEngine.Random.Range(0, 2);
                clampedHeroChaser = Math.Max(0, clampedHeroChaser);
                remainingHp -= clampedHeroChaser * hpHeroChaser;
                totalEnemies += clampedHeroChaser;
            }

            // 6. Large Ogres
            long clampedLarge = 0;
            if (allowLarge)
            {
                clampedLarge = Math.Min(remainingHp / hpOgreLarge, MaxEnemies - totalEnemies);
                clampedLarge -= UnityEngine.Random.Range(0, 2);
                clampedLarge = Math.Max(0, clampedLarge);
                remainingHp -= clampedLarge * hpOgreLarge;
                totalEnemies += clampedLarge;
            }

            // 7. Small Ogres
            long clampedSmall = Math.Min(remainingHp / hpOgreSmall, MaxEnemies - totalEnemies);
            clampedSmall -= UnityEngine.Random.Range(0, 2);
            clampedSmall = Math.Max(0, clampedSmall);
            remainingHp -= clampedSmall * hpOgreSmall;
            totalEnemies += clampedSmall;

            // 8. Bats
            long clampedBat = Math.Min(remainingHp / hpBat, MaxEnemies - totalEnemies);
            clampedBat = Math.Max(0, clampedBat);
            remainingHp -= clampedBat * hpBat;
            totalEnemies += clampedBat;

            if (totalEnemies == 0)
            {
                clampedBat = 1;
                remainingHp -= hpBat;
            }

            // Add Pigtail
            foreach (var actor in SpawnUtil.Random(ActorTypeEnum.Pigtail, (int)clampedPigtail))
            {
                actor.BaseHp = hpPigtail;
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
            hpPigtail *= 10;
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