using Assets.Script.Actors.Spawning;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static int MaxRound = 3;

    public static IEnumerable<ActorBase> GetEnemies(long level)
    {
        const long HpPerLevelBase = 200;
        long hpTargetForRound = HpPerLevelBase * level;

        // All spawns should stay within Rect ArenaBounds.
        Rect bounds = GameManager.ArenaBounds;
        long hpBat = 20;
        long hpOgreSmall = 50;
        long hpOgreLarge = 1000;

        // Examples:
        //foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreLarge, 3))
        //    yield return go;

        //foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, 3))
        //    yield return go;

        //foreach (var go in SpawnUtil.Random(ActorTypeEnum.BatRed, 3))
        //    yield return go;

        //foreach (var go in SpawnUtil.Square(ActorTypeEnum.OgreSmall, Vector2.zero, size: 3, spacing: 1))
        //    yield return go;

        //foreach (var go in SpawnUtil.Circle(ActorTypeEnum.BatRed, Vector2.zero, radius: 3, count: 10))
        //    yield return go;
    }
}
