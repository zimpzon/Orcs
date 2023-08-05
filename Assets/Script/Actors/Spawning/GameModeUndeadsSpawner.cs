using System;
using System.Collections;
using UnityEngine;

public class GameModeUndeadsSpawner : MonoBehaviour
{
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
                endTime: new TimeSpan(0, 4, 0),
                maintainCount: 1,
                maintainCountIncreasePerSec: 0.0f,
                spawnCountPerTick: 1,
                timeBetweenTicks: 8.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.LeftOrRight)
            );

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.OgreShamanStaff,
                startTime: new TimeSpan(0, 4, 0),
                endTime: new TimeSpan(0, 15, 0),
                maintainCount: 1,
                maintainCountIncreasePerSec: 0.05f,
                spawnCountPerTick: 2,
                timeBetweenTicks: 5.0f,
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
                maintainCount: 3,
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
                maintainCount: 20,
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
            PositionUtility.Swarm(
                ActorTypeEnum.Ogre,
                new TimeSpan(0, 0, 10),
                new TimeSpan(0, 15, 0),
                spawnCountPerTick: 2,
                timeBetweenTicks: 3.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any));

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 0),
                endTime: new TimeSpan(0, 0, 30),
                maintainCount: 25,
                maintainCountIncreasePerSec: 0.0f,
                spawnCountPerTick: 5,
                timeBetweenTicks: 2.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any)
            );

        StartCoroutine(
            PositionUtility.Swarm(
                ActorTypeEnum.Ogre,
                new TimeSpan(0, 0, 50),
                new TimeSpan(0, 15, 0),
                spawnCountPerTick: 5,
                timeBetweenTicks: 5.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any));

        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 40),
                endTime: new TimeSpan(0, 10, 0),
                maintainCount: 20,
                maintainCountIncreasePerSec: 0.1f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any)
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

    void Large()
    {
        StartCoroutine(
            PositionUtility.Swarm(
                ActorTypeEnum.OgreLarge,
                new TimeSpan(0, 0, 30),
                new TimeSpan(0, 15, 0),
                spawnCountPerTick: 1,
                timeBetweenTicks: 30.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any));
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
        Large();
        Bandanas();

        while (true)
        {
            yield return null;
        }
    }
}
