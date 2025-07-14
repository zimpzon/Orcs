using Assets.Script.Actors.Spawning;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static int MaxRound = 3;

    public static IEnumerable<ActorBase> GetEnemies(int round)
    {
        //yield return SpawnUtil.Square(ActorTypeEnum.OgreSmall, Vector2.zero, size: 3, spacing: 1);

        //yield return SpawnUtil.Circle(ActorTypeEnum.OgreSmall, Vector2.zero, radius: 3, count: 10);

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, 5))
            yield return go;

        //if (round == 1)
        //{
        //    foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, 5))
        //        yield return go;
        //}
        //else if (round == 2)
        //{
        //    foreach (var go in SpawnUtil.Circle(ActorTypeEnum.OgreSmall, Vector2.right * 3, radius: 4, count: 10))
        //        yield return go;
        //}
        //if (round == 3)
        //{
        //    foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreLarge, 2))
        //        yield return go;
        //}
        //else
        //{
        //    throw new ArgumentException();
        //}

        //foreach(var go in SpawnUtil.Single(ActorTypeEnum.OgreSmall, Vector2.left * 2))
        //    yield return go;
    }
}
