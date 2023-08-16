using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Controller : MonoBehaviour, IKillableObject
{
    public ActorReaperBoss Boss;
    public GameObject Hounds;
    public GameObject BossObjects;
    public GameObject SawbladeProto;
    public ParticleSystem Acid;
    public ParticleSystem AcidSpawnEffect;
    public AcidFlaskScript AcidFlaskProto;
    public HpBarScript HpBar;
    public float HoundsHiddenX = 14;
    public float HoundsShownX = 12.2f;
    public Vector2 BossOffsetY = Vector2.up * 1;

    public Color Chapter1TextColor;
    public Color ColorAtStart;
    public Color FilterAtStart;
    public Color ColorBoss;
    public Color FilterBoss;
    public Color ColorHounds;
    public Color FilterHounds;

    public void Kill()
    {
        gameObject.SetActive(false);
    }

    public void Run()
    {
        gameObject.SetActive(true);
        BossObjects.SetActive(false);

        StartCoroutine(RunInternal());
    }

    void RunAll(IEnumerable<IEnumerator> coList)
    {
        foreach (var co in coList)
            StartCoroutine(co);
    }

    IEnumerator RunSpawnEvents()
    {
        if (G.D.GameTime < 5)
            RunAll(Chapter1Minute01.GetEvents());

        const float Min3 = 60 * 3;
        while (GameManager.Instance.GameTime < Min3)
            yield return null;

        if (G.D.GameTime < Min3 + 5)
            RunAll(Chapter1Minute03.GetEvents());


        const float Min6 = 60 * 6;
        while (GameManager.Instance.GameTime < Min6)
            yield return null;

        if (G.D.GameTime < Min6 + 5)
            RunAll(Chapter1Minute06.GetEvents());


        const float Min10 = 60 * 10;
        while (GameManager.Instance.GameTime < Min10)
            yield return null;

        if (G.D.GameTime < Min10 + 5)
            RunAll(Chapter1Minute10.GetEvents());


        const float Min14 = 60 * 14;
        while (GameManager.Instance.GameTime < Min14)
            yield return null;

        if (G.D.GameTime < Min14 + 5)
            RunAll(Chapter1Minute14.GetEvents());


        const float BossTime = 60 * 15;
        while (GameManager.Instance.GameTime < BossTime)
            yield return null;

        // disable orc
        GameManager.Instance.Orc.ResetAll();

        BossObjects.SetActive(true);
        yield return Chapter1BossIntro.Run(this, skipIntroduction: false);

        yield return Chapter1BossFight.Run(this);
    }

    IEnumerator RunInternal()
    {
        var info = new GameInfo
        {
            Text = "Skeleton Skirmish",
            Color = Chapter1TextColor,
            Duration = 4.0f,
            FadeInDuration = 0.5f,
            FadeOutDuration = 2.0f,
            FontSize = 15,
            Position = Vector2.up * -100,
        };

        if (G.D.GameTime < 30)
        {
            GameManager.Instance.TextGameInfo.GetComponent<GameInfoViewer>().Show(info);
            LeanTween.color(GameManager.Instance.Floor.gameObject, ColorAtStart, time: 0.5f);
        }

        StartCoroutine(RunSpawnEvents());

        yield return StartCoroutine(Warning(new TimeSpan(0, 5, 0), "The air thickens", ColorAtStart, FilterAtStart));
        yield return StartCoroutine(Warning(new TimeSpan(0, 10, 40), "The dead grow uneasy", ColorAtStart, FilterAtStart));
        yield return StartCoroutine(Warning(new TimeSpan(0, 14, 50), "A horrible smell draws closer", ColorAtStart, FilterAtStart));

        yield return StartCoroutine(SpawnUtil.ActionAtTime(new TimeSpan(0, 14, 55), () => SpawnUtil.FleeAllActors()));

        while (true)
            yield return null;
    }

    IEnumerator Warning(TimeSpan showTime, string text, Color endColor, Color endFilter)
    {
        while (G.D.GameTime < showTime.TotalSeconds)
            yield return null;

        if (G.D.GameTime > showTime.TotalSeconds + 5)
            yield break;

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
