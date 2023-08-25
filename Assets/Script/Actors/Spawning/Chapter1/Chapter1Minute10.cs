using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using static PositionUtility;

public static class Chapter1Minute10
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnFormation(ActorTypeEnum.OgreBandanaGun, despawnAtDestination: true, breakFreeAtDamage: false,
            time: new TimeSpan(0, 10, 1), BottomMidOut, TopMidOut, ActorForcedTargetType.Direction, w: 5, h: 2, stepX: 1, stepY: 1, pivotX: 0.5f, pivotY: 0.0f, skewX: 0.0f, skewY: 0.2f);

        yield return SpawnUtil.Single(
                ActorTypeEnum.OgreShamanStaffLarge,
                time: new TimeSpan(0, 13, 0),
                SpawnDirection.Top);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.OgreLarge,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                spawnCountPerTick: 1,
                timeBetweenTicks: 4,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                spawnCountPerTick: 2,
                timeBetweenTicks: 2,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreShamanStaff,
            startTime: new TimeSpan(0, 10, 0),
            endTime: new TimeSpan(0, 14, 0),
            startingCount: 4,
            endCount: 5,
            maxSpawnCountPerTick: 1,
            timeBetweenTicks: 8.0f,
            SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.BatWhite,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 10,
                endCount: 15,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 1.2f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 35,
                endCount: 40,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Skeleton,
                startTime: new TimeSpan(0, 10, 10),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 5,
                endCount: 7,
                maxSpawnCountPerTick: 1,
                timeBetweenTicks: 10.0f,
                SpawnDirection.Any);

        yield return SpawnUtil.Swarm(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 10, 0),
                endTime: new TimeSpan(0, 14, 0),
                spawnCountPerTick: 4,
                timeBetweenTicks: 3,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.BatRed,
                startTime: new TimeSpan(0, 10, 5),
                endTime: new TimeSpan(0, 14, 0),
                startingCount: 20,
                endCount: 35,
                maxSpawnCountPerTick: 10,
                timeBetweenTicks: 3.0f,
                SpawnDirection.Any);

        yield break;
    }
}
