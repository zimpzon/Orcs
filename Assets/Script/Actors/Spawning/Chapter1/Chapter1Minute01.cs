using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute01
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreEdgy,
            startTime: new TimeSpan(0, 0, 5),
            endTime: new TimeSpan(0, 14, 50),
            startingCount: 12,
            endCount: 20,
            maxSpawnCountPerTick: 5,
            timeBetweenTicks: 1.5f,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 3, 0),
                startingCount: 10,
                endCount: 30,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 1.2f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 20),
                endTime: new TimeSpan(0, 3, 0),
                startingCount: 20,
                endCount: 30,
                maxSpawnCountPerTick: 5,
                timeBetweenTicks: 3.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 20),
                endTime: new TimeSpan(0, 3, 0),
                spawnCountPerTick: 1,
                timeBetweenTicks: 2,
                SpawnDirection.Any);

        yield return SpawnUtil.Single(
            ActorTypeEnum.Skeleton,
            time: new TimeSpan(0, 1, 30),
            SpawnDirection.Any);

        yield return SpawnUtil.Single(
            ActorTypeEnum.Skeleton,
            time: new TimeSpan(0, 2, 0),
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 1, 30), LeftMidOut, RightMidOut, ActorForcedTargetType.Direction, w: 3, h: 2, stepX: 1, stepY: 1, pivotX: 1, pivotY: 0.5f, skewX: 0.2f);
    }
}
