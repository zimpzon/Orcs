using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;

public static class EnemySpawner
{
    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        const long MaxEnemies = 100;
        long HpPerLevel = 150;
        long HpBase = 50;

        if (level <= 4)
        {
            HpPerLevel = 50;
            HpBase = 0;
        }

        long hpTargetForRound = (HpPerLevel * level) + HpBase;
        long baseHpBat = 20;
        long baseHpOgreSmall = 50;
        long baseHpOgreLarge = 1000;

        long hpBat = baseHpBat;
        long hpOgreSmall = baseHpOgreSmall;
        long hpOgreLarge = baseHpOgreLarge;

        while (true)
        {
            long remainingHp = hpTargetForRound;
            long totalEnemies = 0;

            long ogreLargeCount = remainingHp / hpOgreLarge;
            ogreLargeCount -= UnityEngine.Random.Range(0, (int)(ogreLargeCount / 5) + 1);
            ogreLargeCount = Math.Max(0, ogreLargeCount);

            long clampedLarge = Math.Min(ogreLargeCount, MaxEnemies - totalEnemies);
            totalEnemies += clampedLarge;
            remainingHp -= clampedLarge * hpOgreLarge;

            long ogreSmallCount = remainingHp / hpOgreSmall;
            ogreSmallCount -= UnityEngine.Random.Range(0, (int)(ogreSmallCount / 5) + 1);
            ogreSmallCount = Math.Max(0, ogreSmallCount);

            long clampedSmall = Math.Min(ogreSmallCount, MaxEnemies - totalEnemies);
            totalEnemies += clampedSmall;
            remainingHp -= clampedSmall * hpOgreSmall;

            long batCount = remainingHp / hpBat;
            long clampedBat = Math.Min(batCount, MaxEnemies - totalEnemies);
            totalEnemies += clampedBat;

            if (totalEnemies <= MaxEnemies)
            {
                // Now safe to yield: calculations done
                foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreLarge, (int)clampedLarge))
                {
                    go.GetComponent<ActorBase>().BaseHp = hpOgreLarge;
                    yield return go;
                }

                foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, (int)clampedSmall))
                {
                    go.GetComponent<ActorBase>().BaseHp = hpOgreSmall;
                    yield return go;
                }

                foreach (var go in SpawnUtil.Random(ActorTypeEnum.BatRed, (int)clampedBat))
                {
                    go.GetComponent<ActorBase>().BaseHp = hpBat;
                    yield return go;
                }

                yield break;
            }

            // Retry with double HP values
            hpBat *= 2;
            hpOgreSmall *= 2;
            hpOgreLarge *= 2;
        }
    }
}
