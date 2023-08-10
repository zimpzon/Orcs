using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Controller : MonoBehaviour
{
    public Color Chapter1Color;
    public Color ColorAfterFirstWarning;
    public Color FilterAfterFirstWarning;
    public Color ColorAfterSecondWarning;
    public Color FilterAfterSecondWarning;
    public Color ColorAfterFinalWarning;
    public Color FilterAfterFinalWarning;

    public void Run()
    {
        StartCoroutine(RunInternal());
    }

    public void Stop()
    {
        StopAllCoroutines();
    }

    void RunAll(IEnumerable<IEnumerator> coList)
    {
        foreach (var co in coList)
            StartCoroutine(co);
    }

    IEnumerator RunSpawnEvents()
    {
        RunAll(Chapter1Minute1.GetEvents());

        while (GameManager.Instance.GameTime < 60)
            yield return null;

        RunAll(Chapter1Minute2.GetEvents());

        while (GameManager.Instance.GameTime < 120)
            yield return null;

        RunAll(Chapter1Minute3.GetEvents());

        while (GameManager.Instance.GameTime < 180)
            yield return null;

        RunAll(Chapter1Minute4.GetEvents());

        while (GameManager.Instance.GameTime < 240)
            yield return null;
    }

    IEnumerator RunInternal()
    {
        StartCoroutine(SpawnUtil.ActionAtTime(new TimeSpan(0, 4, 57), () => SpawnUtil.FleeAllActors()));

        var info = new GameInfo
        {
            Text = "CHAPTER 1\n\nSkeleton Skirmish",
            Color = Chapter1Color,
            Duration = 4.0f,
            FadeInDuration = 0.5f,
            FadeOutDuration = 2.0f,
            FontSize = 13,
            Position = Vector2.up * -100,
        };

        StartCoroutine(RunSpawnEvents());

        GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);

        yield return StartCoroutine(Warning(new TimeSpan(0, 2, 30), "you sense danger far away", ColorAfterFirstWarning, FilterAfterFirstWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 3, 40), "you feel goosebumps on your armor", ColorAfterSecondWarning, FilterAfterSecondWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 4, 55), "it is time", ColorAfterFinalWarning, FilterAfterFinalWarning));

        while (true)
            yield return null;
    }

    IEnumerator Warning(TimeSpan showTime, string text, Color endColor, Color endFilter)
    {
        while (GameManager.Instance.GameTime < showTime.TotalSeconds)
            yield return null;

        const float TextTime = 10.0f;
        var info = new GameInfo
        {
            Text = text,
            Duration = TextTime,
            FadeInDuration = 0.25f,
            FadeOutDuration = 2.0f,
            FontSize = 10,
        };

        GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);

        LeanTween.color(GameManager.Instance.Floor.gameObject, endColor, TextTime * 0.5f);
        LeanTween.color(GameManager.Instance.FloorFilter.gameObject, endFilter, TextTime * 0.5f);
    }
}
