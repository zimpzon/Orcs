using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute10
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.Swarm(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                spawnCountPerTick: 1,
                timeBetweenTicks: 5,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 10, 0),
            endTime: new TimeSpan(0, 14, 0),
            startingCount: 2,
            endCount: 2,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 8.0f,
            outsideScreen: true,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 5,
                endCount: 5,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 1.2f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 12,
                endCount: 12,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Skeleton,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 2,
                endCount: 4,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 10.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 10, 0),
                endTime: new TimeSpan(0, 14, 0),
                spawnCountPerTick: 2,
                timeBetweenTicks: 5,
                outsideScreen: true,
                SpawnDirection.Any);

        yield break;
    }
}