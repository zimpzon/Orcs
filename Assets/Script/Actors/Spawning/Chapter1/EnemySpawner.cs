using Assets.Script.Actors.Spawning;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawner
{
    public static IEnumerable<ActorBase> GetEnemies()
    {
        //yield return SpawnUtil.Square(ActorTypeEnum.OgreSmall, Vector2.zero, size: 3, spacing: 1);

        //yield return SpawnUtil.Circle(ActorTypeEnum.OgreSmall, Vector2.zero, radius: 3, count: 10);

        //foreach (var go in SpawnUtil.Circle(ActorTypeEnum.OgreSmall, Vector2.right * 2, radius: 8, count: 30))
        //    yield return go;

        foreach (var go in SpawnUtil.Random(ActorTypeEnum.OgreSmall, 30))
            yield return go;

        //foreach(var go in SpawnUtil.Single(ActorTypeEnum.OgreSmall, Vector2.left * 2))
        //    yield return go;
    }
}
