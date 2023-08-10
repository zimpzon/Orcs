using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PositionUtility;

public static class Chapter1Minute1
{
    public static IEnumerable<IEnumerator> GetEvents()
    {
        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 0, 50),
                maintainCount: 10,
                maintainCountIncreasePerSec: 0.2f,
                spawnCountPerTick: 5,
                timeBetweenTicks: 2.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 10),
                endTime: new TimeSpan(0, 0, 50),
                maintainCount: 10,
                maintainCountIncreasePerSec: 0.2f,
                spawnCountPerTick: 5,
                timeBetweenTicks: 5.0f,
                outsideScreen: true,
                SpawnDirection.Any);

        yield return SpawnUtil.SpawnFormation(
            ActorTypeEnum.OgreShamanStaff,
            despawnAtDestination: true, breakFreeAtDamage: false,
            new TimeSpan(0, 0, 5),
            fromPos: LeftMidOut,
            target: Vector2.zero,
            targetType: ActorForcedTargetType.Direction,
            w: 2, h: 1,
            stepX: 3.0f, stepY: 0.0f,
            pivotX: 1.0f, pivotY: 0.5f,
            skewX: 0.0f, skewY: 0.0f
        );
    }
}
