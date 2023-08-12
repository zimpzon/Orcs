using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Controller : MonoBehaviour
{
    public Color Chapter1TextColor;
    public Color ColorAtStart;
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
        RunAll(Chapter1Minute01.GetEvents());

        const float Min3 = 60 * 3;
        while (GameManager.Instance.GameTime < Min3)
            yield return null;

        RunAll(Chapter1Minute03.GetEvents());

        const float Min6 = 60 * 6;
        while (GameManager.Instance.GameTime < Min6)
            yield return null;

        RunAll(Chapter1Minute06.GetEvents());

        const float Min10 = 60 * 10;
        while (GameManager.Instance.GameTime < Min10)
            yield return null;

        RunAll(Chapter1Minute10.GetEvents());

        const float Min14 = 60 * 14;
        while (GameManager.Instance.GameTime < Min14)
            yield return null;

        RunAll(Chapter1Minute14.GetEvents());
    }

    IEnumerator RunInternal()
    {
        var info = new GameInfo
        {
            Text = "CHAPTER 1\n\nSkeleton Skirmish",
            Color = Chapter1TextColor,
            Duration = 4.0f,
            FadeInDuration = 0.5f,
            FadeOutDuration = 2.0f,
            FontSize = 15,
            Position = Vector2.up * -100,
        };

        GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);
        LeanTween.color(GameManager.Instance.Floor.gameObject, ColorAtStart, 0.5f);

        StartCoroutine(RunSpawnEvents());

        yield return StartCoroutine(Warning(new TimeSpan(0, 5, 0), "The air thickens", ColorAfterFirstWarning, FilterAfterFirstWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 10, 40), "The dead grow uneasy", ColorAfterSecondWarning, FilterAfterSecondWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 14, 50), "A horrible smell arises", ColorAfterFinalWarning, FilterAfterFinalWarning));
        yield return StartCoroutine(Warning(new TimeSpan(0, 15, 2), "actually he is not here yet, sorry.\nworking on it! hope you had fun anyways", ColorAfterFinalWarning, FilterAfterFinalWarning));

        yield return StartCoroutine(SpawnUtil.ActionAtTime(new TimeSpan(0, 13, 57), () => SpawnUtil.FleeAllActors()));
        yield return StartCoroutine(SpawnUtil.ActionAtTime(new TimeSpan(0, 14, 50), () => SpawnUtil.FleeAllActors()));

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
