using System;
using System.Collections;
using UnityEngine;

public class GameModeUndeadsSpawner : MonoBehaviour
{
    // 20 minutes
    // 0:00 1:30 min of small ogre, start X, increase to Y
    // 1:00 5 min of ogre, start X, increase to Y
    // 2:00 large ogre formation, 

    public void Run()
    {
        StartCoroutine(Test1());
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    void Casters()
    {
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreShamanStaff,
                startTime: new TimeSpan(0, 2, 0),
                endTime: new TimeSpan(0, 3, 0),
                maintainCount: 1,
                maintainCountIncreasePerSec: 0.05f,
                spawnCountPerTick: 1,
                timeBetweenTicks: 1.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreShamanStaff,
                startTime: new TimeSpan(0, 3, 0),
                endTime: new TimeSpan(0, 15, 0),
                maintainCount: 1,
                maintainCountIncreasePerSec: 0.05f,
                spawnCountPerTick: 2,
                timeBetweenTicks: 1.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );
    }

    void SmallOnes()
    {
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 2, 0),
                maintainCount: 20,
                maintainCountIncreasePerSec: 0.25f,
                spawnCountPerTick: 5,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.TopOrBottom)
            );

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreSmall,
                startTime: new TimeSpan(0, 2, 0),
                endTime: new TimeSpan(0, 15, 0),
                maintainCount: 30,
                maintainCountIncreasePerSec: 0.0f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.TopOrBottom)
            );
    }

    void Standard()
    {
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 45),
                endTime: new TimeSpan(0, 10, 0),
                maintainCount: 5,
                maintainCountIncreasePerSec: 0.3f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 10, 30),
                endTime: new TimeSpan(0, 15, 0),
                maintainCount: 30,
                maintainCountIncreasePerSec: 0.05f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );
    }

    void Bandanas()
    {
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreBandana,
                startTime: new TimeSpan(0, 4, 0),
                endTime: new TimeSpan(0, 15, 0),
                maintainCount: 2,
                maintainCountIncreasePerSec: 0.1f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any)
            );
    }

    IEnumerator Test1()
    {
        Casters();
        SmallOnes();
        Standard();
        Bandanas();

        while (true)
        {
            yield return null;
        }
    }
}
