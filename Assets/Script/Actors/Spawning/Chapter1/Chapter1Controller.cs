using Assets.Script.Actors.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PositionUtility;

public class Chapter1Controller : MonoBehaviour, IKillableObject
{
    public ActorReaperBoss BossProto;
    [NonSerialized] public ActorReaperBoss Boss;

    public GameObject Hounds;
    public GameObject BossObjects;
    public GameObject SawbladeProto;
    public GameObject Confetti;

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
    public Color ColorWon;
    public Color FilterWon;

    public void Kill()
    {
        StopAllCoroutines();
        Confetti.SetActive(false);
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
        // The edgy gauards must always be there, no matter when game starts
        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreEdgy,
            startTime: new TimeSpan(0, 0, 5),
            endTime: new TimeSpan(0, 14, 50),
            startingCount: 4,
            endCount: 7,
            maxSpawnCountPerTick: 5,
            timeBetweenTicks: 1.5f,
            SpawnDirection.Top);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreEdgy,
            startTime: new TimeSpan(0, 0, 5),
            endTime: new TimeSpan(0, 14, 50),
            startingCount: 3,
            endCount: 5,
            maxSpawnCountPerTick: 5,
            timeBetweenTicks: 1.5f,
            SpawnDirection.Right);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreEdgy,
            startTime: new TimeSpan(0, 0, 5),
            endTime: new TimeSpan(0, 14, 50),
            startingCount: 4,
            endCount: 7,
            maxSpawnCountPerTick: 5,
            timeBetweenTicks: 1.5f,
            SpawnDirection.Bottom);

        yield return SpawnUtil.SpawnAndMaintain(
            ActorTypeEnum.OgreEdgy,
            startTime: new TimeSpan(0, 0, 5),
            endTime: new TimeSpan(0, 14, 50),
            startingCount: 3,
            endCount: 5,
            maxSpawnCountPerTick: 5,
            timeBetweenTicks: 1.5f,
            SpawnDirection.Left);

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

        GameManager.Instance.Orc.ResetAll();
        G.D.PlayerScript.DisableToggledEffects();

        SaveGame.Members.Chapter1BossStarted++;

        Boss = Instantiate(BossProto);
        Boss.gameObject.SetActive(true);
        HpBar.Owner = Boss.BodyTransform.GetComponent<ActorBase>();

        BossObjects.SetActive(true);
        yield return Chapter1BossIntro.Run(this, skipIntroduction: false);

        Boss.StartFight(this);

        while (!Boss.FightComplete)
            yield return null;

        StopAllCoroutines();

        G.D.PlayerScript.DisableToggledEffects();

        var info = new GameInfo
        {
            Text = "Congratulations!\n\n\nYou have defeated Super Knight - Man in Steel\n\n\nUse the menu to return to title screen",
            Color = Color.green,
            Duration = 60.0f,
            FadeInDuration = 0.5f,
            FadeOutDuration = 2.0f,
            FontSize = 10,
            Position = Vector2.up * -100,
        };

        GameManager.Instance.TextGameInfoViewer.Show(info);

        LeanTween.color(GameManager.Instance.Floor.gameObject, ColorWon, 3);
        LeanTween.color(GameManager.Instance.FloorFilter.gameObject, FilterWon, 3);

        Confetti.SetActive(true);
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

        yield return StartCoroutine(SpawnUtil.ActionAtTime(new TimeSpan(0, 14, 45), () => SpawnUtil.FleeAllActors()));

        while (true)
            yield return null;
    }
}
