using System;
using System.Collections;
using UnityEngine;

public class GameModeUndeadsSpawner : MonoBehaviour
{
    public Color ColorAfterFirstWarning;
    public Color FilterAfterFirstWarning;
    public Color ColorAfterSecondWarning;
    public Color FilterAfterSecondWarning;
    public Color ColorAfterFinalWarning;
    public Color FilterAfterFinalWarning;

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
                endTime: new TimeSpan(0, 5, 0),
                maintainCount: 1,
                maintainCountIncreasePerSec: 0.05f,
                spawnCountPerTick: 2,
                timeBetweenTicks: 10.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any)
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
                endTime: new TimeSpan(0, 5, 0),
                maintainCount: 20,
                maintainCountIncreasePerSec: 0.0f,
                spawnCountPerTick: 10,
                timeBetweenTicks: 0.5f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any)
            );
    }

    void Standard()
    {
        StartCoroutine(
            PositionUtility.SpawnAndMaintain(
                ActorTypeEnum.Ogre,
                startTime: new TimeSpan(0, 0, 10),
                endTime: new TimeSpan(0, 3, 10),
                maintainCount: 15,
                maintainCountIncreasePerSec: 0.05f,
                spawnCountPerTick: 5,
                timeBetweenTicks: 2.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any)
            );

        StartCoroutine(
            PositionUtility.Swarm(
                ActorTypeEnum.Ogre,
                new TimeSpan(0, 3, 0),
                new TimeSpan(0, 5, 0),
                spawnCountPerTick: 5,
                timeBetweenTicks: 5.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any));

        StartCoroutine(
            PositionUtility.Swarm(
                ActorTypeEnum.Ogre,
                new TimeSpan(0, 3, 0),
                new TimeSpan(0, 3, 10),
                spawnCountPerTick: 10,
                timeBetweenTicks: 30.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any));

        StartCoroutine(
            PositionUtility.Swarm(
                ActorTypeEnum.Ogre,
                new TimeSpan(0, 4, 0),
                new TimeSpan(0, 4, 10),
                spawnCountPerTick: 20,
                timeBetweenTicks: 30.0f,
                outsideScreen: true,
                PositionUtility.SpawnDirection.Any));
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
                startTime: new TimeSpan(0, 3, 0),
                endTime: new TimeSpan(0, 5, 0),
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
        //StartCoroutine(
        //    PositionUtility.Single(
        //        ActorTypeEnum.OgreShamanStaffLarge,
        //        new TimeSpan(0, 0, 2),
        //        outsideScreen: true,
        //        PositionUtility.SpawnDirection.Right));

        //StartCoroutine(
        //    PositionUtility.Single(
        //        ActorTypeEnum.OgreShamanStaffLarge,
        //        new TimeSpan(0, 0, 2),
        //        outsideScreen: true,
        //        PositionUtility.SpawnDirection.Left));

        //StartCoroutine(
        //    PositionUtility.Single(
        //        ActorTypeEnum.OgreShamanStaffLarge,
        //        new TimeSpan(0, 0, 2),
        //        outsideScreen: true,
        //        PositionUtility.SpawnDirection.Top));

        //StartCoroutine(
        //    PositionUtility.Single(
        //        ActorTypeEnum.OgreShamanStaffLarge,
        //        new TimeSpan(0, 0, 2),
        //        outsideScreen: true,
        //        PositionUtility.SpawnDirection.Bottom));

        Casters();
        SmallOnes();
        Standard();
        Large();
        Bandanas();

        // first warning

        yield return StartCoroutine(Warning(new TimeSpan(0, 0, 4), "you sense danger", ColorAfterFirstWarning, FilterAfterFirstWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 0, 16), "you feel goosebumps on your armor", ColorAfterSecondWarning, FilterAfterSecondWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 0, 30), "you shiver.. THEY ARE HERE", ColorAfterFinalWarning, FilterAfterFinalWarning));

        while (true)
        {
            yield return null;
        }
    }

    IEnumerator Warning(TimeSpan showTime, string text, Color endColor, Color endFilter)
    {
        while (GameManager.Instance.GameTime < showTime.TotalSeconds)
            yield return null;

        const float TextTime = 10.0f;
        GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(text, TextTime);
        LeanTween.color(GameManager.Instance.Floor.gameObject, endColor, TextTime * 0.5f);
        LeanTween.color(GameManager.Instance.FloorFilter.gameObject, endFilter, TextTime * 0.5f);
    }
}
