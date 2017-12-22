using Assets.Script;
using EZCameraShake;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameModeEnum { Nursery, Earth, Wind, Fire, Storm, Harmony, LastSelectable, Deed };

public class GameManager : MonoBehaviour
{
    public enum State { None, Intro, Intro_GameMode, Intro_Unlocks, Intro_Deeds, Playing, Dead };

    public static GameManager Instance;
    public bool UnlockAllGameModes;
    public bool UnlockAllWeapons;
    public bool UnlockAllHeroes;

    public Text TextScore;
    public Text TextGameOverOrcsSaved;
    public Text TextGameOverOrcsSavedBest;
    public Text TextGameOverDeedScore;
    public Text TextGameOverDeedComment;
    public Text TextLocked;
    public Text TextNewRecord;
    public Text TextNewUnlock;
    public Text TextRoundEndUnlocks;
    public Text TextDeedScore;
    public Text TextGameWarning;
    public TextMeshProUGUI TextHeroUnlock;
    public TextMeshProUGUI TextHeroName;
    public TextMeshProUGUI TextColWepNames;
    public TextMeshProUGUI TextColWepReq;
    public TextMeshProUGUI TextColLocNames;
    public TextMeshProUGUI TextColLocReq;
    public TextMeshProUGUI TextColDamNames;
    public TextMeshProUGUI TextColDamReq;
    public Text ButtonUnlockedText;
    public Text ButtonDeedText;
    public Renderer Floor;
    public GameObject DeedPrefab;
    public Transform DeedItemParent;

    public GameObject TextDeedsLocked;
    public Button ButtonGo;
    public Button ButtonPlay;
    public string ColorLocked;
    public string ColorUnlocked;

    public TextMeshProUGUI TextGameMode;
    public TextMeshProUGUI TextGameModeInfo;
    public GameObject PanelGameMode;
    public GameObject PanelUnlocks;
    public GameObject PanelDeeds;
    public GameObject PanelResetProgress;
    public Canvas CanvasGameOverDefault;
    public Canvas CanvasGameOverDeed;
    public GameModeEnum GameMode;

    public TextBlinkScript TextFloatingWeapon;
    public Transform Crosshair;
    public ParticleSystem FlyingBlood;
    public ParticleSystem BloodDrops;
    public ParticleSystem FloorBlood;
    public ParticleSystem PoofClouds;
    public ParticleSystem SpawnPoof;
    public ParticleSystem FlashParticles;
    public ParticleSystem CircleParticles;
    public ParticleSystem FlameParticles;
    public ParticleSystem NpcFlameParticles;
    public OrcController Orc;
    public int SortLayerTopEffects;
    public State GameState;
    public Transform PlayerTrans;
    public PlayerScript PlayerScript;
    public Canvas CanvasIntro;
    public Canvas CanvasGame;
    public Canvas CanvasDead;
    public int LayerPlayer;
    public int LayerPlayerProjectile;
    public int LayerEnemyProjectile;
    public int LayerEnemyCorpse;
    public int LayerEnemy;
    public int LayerNeutral;
    public int LayerXpPill;
    public int LayerOrc;

    [NonSerialized] public GameModeData LatestGameModeData = new GameModeData();
    [NonSerialized] public GameModeData CurrentGameModeData;
    [NonSerialized] public DeedData CurrentDeedData = new DeedData();
    public GameModeData GameModeDataNursery = new GameModeData();
    public GameModeData GameModeDataEarth = new GameModeData();
    public GameModeData GameModeDataWind = new GameModeData();
    public GameModeData GameModeDataFire = new GameModeData();
    public GameModeData GameModeDataStorm = new GameModeData();
    public GameModeData GameModeDataHarmony = new GameModeData();
    public GameModeData GameModeDataDeed = new GameModeData();

    public List<DeedData> Deeds = new List<DeedData>();
    public List<DeedUI> DeedItems = new List<DeedUI>();
    public List<Hero> Heroes = new List<Hero>();
    public Hero SelectedHero;

    public int SpriteFlashParamId;
    public int SpriteFlashColorParamId;
    public Rect ArenaBounds = new Rect();
    [NonSerialized] public float TextUnlockBasePos;
    [NonSerialized] public Vector3 CrosshairWorldPosition;

    static Dictionary<string, string> DebugValues = new Dictionary<string, string>();

    [NonSerialized] public float UnlockedPct;
    [NonSerialized] public float DeedDonePct;
    [NonSerialized] public int RoundUnlockCount;

    float roundStartTime_;
    int roundStartBestScore_;

    public void SelectHero(HeroEnum heroType, bool save = false)
    {
        var hero = Heroes.Where(h => h.HeroType == heroType).FirstOrDefault();
        SelectedHero = hero;

        var renderer = PlayerScript.GetComponent<SpriteRenderer>();
        bool isUnlocked = hero.IsUnlocked();
        renderer.sprite = hero.ShowoffSprite;
        renderer.color = isUnlocked ? Color.white : Color.black;
        string unlockText = isUnlocked ? hero.Description : string.Format(GameEvents.ActionDisplayString(hero.GameCounter), SaveGame.Members.GetCounter(hero.GameCounter), hero.Req);
        TextHeroUnlock.text = unlockText;
        TextHeroName.text = hero.Name;
        Orc.Mood = hero.OrcMood;
        SaveGame.Members.SelectedHero = (int)hero.HeroType;

        UpdateButtonStates();

        if (save)
            SaveGame.Save();
    }

    public void HeroChange(int direction = 0)
    {
        if (direction != 0)
            PlayMenuSound();

        HeroEnum nextHero = SelectedHero.HeroType + direction;
        if (nextHero < 0)
            nextHero = HeroEnum.Last - 1;

        if (nextHero >= HeroEnum.Last)
            nextHero = HeroEnum.StarterKnight;

        SelectHero(nextHero, save: true);
    }

    void EnablePanel(GameObject panel, bool enable)
    {
        // Work-around for Unity SetActive bug (still showing UI components after disable)
        panel.SetActive(true);
        panel.transform.localScale = enable ? Vector3.one : Vector3.zero;
    }

    void UpdateButtonStates()
    {
        bool heroIsUnlocked = SelectedHero.IsUnlocked();
        bool gameModeIsUnlocked = GameEvents.IsUnlocked(GameMode);
        bool canDoDeeds = Unlocks.WeaponIsUnlocked(WeaponType.Rambo);

        ButtonPlay.interactable = heroIsUnlocked;
        ButtonGo.interactable = heroIsUnlocked && gameModeIsUnlocked;
        ButtonUnlockedText.transform.parent.GetComponent<Button>().interactable = UnlockedPct > 0.0f;
        ButtonDeedText.transform.parent.GetComponent<Button>().interactable = canDoDeeds && heroIsUnlocked;
        TextDeedsLocked.SetActive(!canDoDeeds);
    }

    public void OnButtonGo()
    {
        PlayMenuSound();
        EnablePanel(PanelGameMode, false);
        StartGame();
    }

    public void OnButtonStart()
    {
        PlayMenuSound();
        GameState = State.Intro_GameMode;
        GameModeChange();
        EnablePanel(PanelGameMode, true);
    }

    public void OnButtonUnlocks()
    {
        PlayMenuSound();
        GameState = State.Intro_Unlocks;
        EnablePanel(PanelUnlocks, true);
        UpdateUnlocksPanel();
    }

    public void OnButtonDeeds()
    {
        PlayMenuSound();
        GameState = State.Intro_Deeds;
        UpdateDeedsPanel();
        EnablePanel(PanelDeeds, true);
    }

    void InitDeedItems()
    {
        if (DeedItems.Count == 0)
        {
            float y = 90.0f;
            float spacing = 95;
            for (int i = 0; i < Deeds.Count; ++i)
            {
                var fab = Instantiate<GameObject>(DeedPrefab);
                var script = fab.GetComponent<DeedUI>();
                DeedItems.Add(script);
                fab.transform.position = Vector3.down * y;
                fab.transform.SetParent(DeedItemParent, false);
                y += spacing;
            }
        }
    }

    void OnStartDeed(DeedData deedData)
    {
        deedData.Reset();
        SetCurrentGameModeData(GameModeEnum.Deed, remember: false);
        CurrentDeedData = deedData;
        GameModeData.UpdateWithDeedData(CurrentGameModeData, CurrentDeedData);
        StartGame();
    }

    void UpdateDeedsPanel()
    {
        for (int i = 0; i < Deeds.Count; ++i)
        {
            DeedItems[i].UpdateFromDeed(Deeds[i], OnStartDeed);
        }
    }

    void UpdateUnlocksPanel()
    {
        TextColWepNames.text = string.Join(Environment.NewLine, GameEvents.FormattedWepNames().ToArray());
        TextColWepReq.text = string.Join(Environment.NewLine, GameEvents.FormattedWepReqs().ToArray());

        TextColLocNames.text = string.Join(Environment.NewLine, GameEvents.FormattedGameModeNames().ToArray());
        TextColLocReq.text = string.Join(Environment.NewLine, GameEvents.FormattedGameModeReqs().ToArray());

        TextColDamNames.text = string.Join(Environment.NewLine, GameEvents.FormattedUpgradeNames().ToArray());
        TextColDamReq.text = string.Join(Environment.NewLine, GameEvents.FormattedUpgradeReqs().ToArray());
    }

    public void GameModeChange(int direction = 0)
    {
        if(direction != 0)
            PlayMenuSound();

        GameMode += direction;
        if (GameMode < 0)
            GameMode = GameModeEnum.LastSelectable - 1;

        if (GameMode >= GameModeEnum.LastSelectable)
            GameMode = GameModeEnum.Nursery;

        SetGameModeText(GameMode);
        SetCurrentGameModeData(GameMode);
    }

    void SetCurrentGameModeData(GameModeEnum gameMode, bool remember = true)
    {
        switch (gameMode)
        {
            case GameModeEnum.Nursery: CurrentGameModeData = GameModeDataNursery; break;
            case GameModeEnum.Earth: CurrentGameModeData = GameModeDataEarth; break;
            case GameModeEnum.Wind: CurrentGameModeData = GameModeDataWind; break;
            case GameModeEnum.Fire: CurrentGameModeData = GameModeDataFire; break;
            case GameModeEnum.Storm: CurrentGameModeData = GameModeDataStorm; break;
            case GameModeEnum.Harmony: CurrentGameModeData = GameModeDataHarmony; break;
            case GameModeEnum.Deed: CurrentGameModeData = GameModeDataDeed; break;
            default: CurrentGameModeData = GameModeDataNursery; break;
        }
        LatestGameModeData = CurrentGameModeData;

        Floor.material.color = CurrentGameModeData.BackgroundTint;
        UpdateButtonStates();
    }

    void SetGameModeText(GameModeEnum gameMode)
    {
        bool isUnlocked = GameEvents.IsUnlocked(gameMode);
        TextGameMode.text = GameEvents.WrapInColor(GameEvents.GameModeDisplayName(gameMode), isUnlocked);
        TextGameModeInfo.text = GameEvents.GameModeInfo(gameMode);
        TextLocked.enabled = !isUnlocked;
    }

    IEnumerator GameStateCo()
    {
        while (true)
        {
            while (GameState == State.Intro)
            {
                yield return null;
            }

            while (GameState == State.Intro_GameMode)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PlayMenuSound();
                    GameState = State.Intro;
                    EnablePanel(PanelGameMode, false);
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (GameEvents.IsUnlocked(CurrentGameModeData.GameMode))
                    {
                        PlayMenuSound();
                        StartGame();
                    }
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    GameModeChange(-1);
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    GameModeChange(1);
                }

                yield return null;
            }

            while (GameState == State.Intro_Unlocks)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PlayMenuSound();
                    GameState = State.Intro;
                    EnablePanel(PanelUnlocks, false);
                    break;
                }

                yield return null;
            }

            while (GameState == State.Intro_Deeds)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PlayMenuSound();
                    GameState = State.Intro;
                    EnablePanel(PanelDeeds, false);
                    break;
                }

                yield return null;
            }

            while (GameState == State.Playing)
            {
                float delta = Time.deltaTime;
                ProjectileManager.Instance.Tick(delta);
                yield return null;
            }

            while (GameState == State.Dead)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerScript.RoundComplete = true;
                    ShowTitle(autoStartGame: true);
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PlayerScript.RoundComplete = true;
                    ShowTitle();
                }

                yield return null;
            }

            yield return null;
        }
    }

    public void ShowGameOver()
    {
        if (GameState == State.Dead)
            return;

        bool wasDeed = CurrentDeedData.Deed != DeedEnum.None;

        TextGameWarning.enabled = false;
        TextFloatingWeapon.Clear();
        GameProgressScript.Instance.Stop();
        ProjectileManager.Instance.StopAll();
        StartCoroutine(Server.Instance.UpdateStat("OrcsSaved", SaveGame.RoundScore));
        StartCoroutine(Server.Instance.UpdateStat("Kills", SaveGame.RoundKills));
        switch (CurrentGameModeData.GameMode)
        {
            case GameModeEnum.Nursery: StartCoroutine(Server.Instance.UpdateStat("ScoreNursery", SaveGame.RoundScore)); break;
            case GameModeEnum.Earth: StartCoroutine(Server.Instance.UpdateStat("ScoreEarth", SaveGame.RoundScore)); break;
            case GameModeEnum.Wind: StartCoroutine(Server.Instance.UpdateStat("ScoreWind", SaveGame.RoundScore)); break;
            case GameModeEnum.Fire: StartCoroutine(Server.Instance.UpdateStat("ScoreFire", SaveGame.RoundScore)); break;
            case GameModeEnum.Storm: StartCoroutine(Server.Instance.UpdateStat("ScoreStorm", SaveGame.RoundScore)); break;
            case GameModeEnum.Harmony: StartCoroutine(Server.Instance.UpdateStat("ScoreHarmony", SaveGame.RoundScore)); break;
            default: break;
        }

        //switch(CurrentDeedData.Deed)
        //{
        //    case DeedEnum.SnipersParadise: StartCoroutine(Server.Instance.UpdateStat("DeedSnipersParadise", CurrentDeedData.DeedCurrentScore)); break;
        //    case DeedEnum.MachinegunMadness: StartCoroutine(Server.Instance.UpdateStat("DeedTheEnd", CurrentDeedData.DeedCurrentScore)); break;
        //    case DeedEnum.LittleMonsters: StartCoroutine(Server.Instance.UpdateStat("DeedLittleMonsters", CurrentDeedData.DeedCurrentScore)); break;
        //    case DeedEnum.WhiteWalkers: StartCoroutine(Server.Instance.UpdateStat("DeedWhiteWalkers", CurrentDeedData.DeedCurrentScore)); break;
        //    default: break;
        //}

        float roundTime = Time.time - roundStartTime_;
        StartCoroutine(Server.Instance.UpdateStat("RoundSeconds", Mathf.RoundToInt(roundTime)));

        int bestScore = SaveGame.Members.GetCounter(GameCounter.Max_Score_Any);
        TextGameOverOrcsSaved.text = string.Format("{0}", SaveGame.RoundScore);
        TextGameOverOrcsSavedBest.text = string.Format("{0}", bestScore);

        TextDeedScore.text = string.Format("{0} / {1}", CurrentDeedData.DeedCurrentScore, CurrentDeedData.KillReq);
        TextGameOverDeedScore.text = TextDeedScore.text;
        TextGameOverDeedComment.text = CurrentDeedData.DeedComplete ?
            "Victory! Your Heroic Feat Will Be Remembered!" : "Defeat. Your Heroic Efforts Were In Vain.";
        TextGameOverDeedComment.color = CurrentDeedData.DeedComplete ? Color.green : Color.red;

        if (wasDeed && CurrentDeedData.DeedComplete)
            SaveGame.Members.SetCounter(CurrentDeedData.CompletionCounter, 1);

        bool newRecord = bestScore > roundStartBestScore_;
        TextNewRecord.GetComponent<TextBlinkScript>().enabled = newRecord;
        TextNewRecord.enabled = newRecord;

        CanvasGameOverDefault.enabled = !wasDeed;
        CanvasGameOverDeed.enabled = wasDeed;

        TextRoundEndUnlocks.enabled = RoundUnlockCount > 0;
        if (RoundUnlockCount > 0)
        {
            TextRoundEndUnlocks.text = Unlocks.LatestUnlockText;
        }
        else
        {
            // Hacky. Show progress for first weapon (sniper) as teaser
            var unlockInfo = GameEvents.WeaponUnlockInfo.Where(wep => wep.Type == WeaponType.Sniper).FirstOrDefault();
            int needed = unlockInfo.Requirement - SaveGame.Members.GetCounter(unlockInfo.Counter);
            if (needed > 0)
            {
                TextRoundEndUnlocks.enabled = true;
                TextRoundEndUnlocks.text = string.Format("Save {0} More To Unlock Next Weapon: {1}!", needed, WeaponBase.WeaponDisplayName(unlockInfo.Type));
            }
        }

        SaveGame.UpdateFromRound(reset: true);
        SaveGame.Save();

        CanvasDead.gameObject.SetActive(true);
        CanvasIntro.gameObject.SetActive(false);

        GameState = State.Dead;
    }

    void UpdateUnlockedPct()
    {
        int unlockedCount = Unlocks.CountUnlocked();
        int possibleUnlocks = Unlocks.CountPossibleUnlocks();
        UnlockedPct = unlockedCount / (float)possibleUnlocks;
        StartCoroutine(Server.Instance.UpdateStat("PctUnlocked", Mathf.RoundToInt(UnlockedPct * 100)));

        int deedDoneCount = Unlocks.CountDoneDeeds();
        int possibleDeedCount = Unlocks.CountPossibleDeeds();
        DeedDonePct = deedDoneCount / (float)possibleDeedCount;
        StartCoroutine(Server.Instance.UpdateStat("DeedPct", Mathf.RoundToInt(DeedDonePct * 100)));
    }

    public void ShowTitle(bool autoStartGame = false)
    {
        if (GameState == State.Intro)
            return;

        TextGameWarning.enabled = false;
        PanelGameMode.SetActive(false);
        PanelUnlocks.SetActive(false);
        PanelDeeds.SetActive(false);
        ProjectileManager.Instance.StopAll();
        PlayerScript.ResetAll();
        BlackboardScript.DestroyAllEnemies();
        GameProgressScript.Instance.Stop();
        Orc.Hide();
        Time.timeScale = 1.0f;
        CameraShaker.Instance.ShakeInstances.Clear();
        Camera.main.transform.parent.position = new Vector3(0.0f, 0.0f, -10.0f);
        Camera.main.orthographicSize = 7.68f;
        SelectHero((HeroEnum)SaveGame.Members.SelectedHero);

        UpdateUnlockedPct();
        ButtonUnlockedText.text = string.Format("UNLOCKS ({0}%)", Mathf.RoundToInt(UnlockedPct * 100));
        ButtonDeedText.text = string.Format("HEROIC FEATS ({0}%)", Mathf.RoundToInt(DeedDonePct * 100));

        UpdateButtonStates();
        ClearParticles();
        CanvasIntro.gameObject.SetActive(true);
        CanvasGame.gameObject.SetActive(false);
        CanvasDead.gameObject.SetActive(false);

        System.GC.Collect();
        if (autoStartGame)
        {
            StartGame();
        }
        else
        {
            SetCurrentGameModeData(LatestGameModeData.GameMode);
            CurrentDeedData = new DeedData();
            GameModeData.UpdateWithDeedData(CurrentGameModeData, CurrentDeedData);
            GameState = State.Intro;
            MusicManagerScript.Instance.PlayIntroMusic();
        }
    }

    public void StartGame()
    {
        if (GameState == State.Playing)
            return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // TODO PE: Only works as a reponse to user action (browser security)
        SaveGame.ResetRound();

        CurrentDeedData.Reset();
        TextDeedScore.text = string.Format("{0} / {1}", CurrentDeedData.DeedCurrentScore, CurrentDeedData.KillReq);
        TextScore.text = SaveGame.RoundScore.ToString();
        CanvasIntro.gameObject.SetActive(false);
        CanvasDead.gameObject.SetActive(false);
        CanvasGame.gameObject.SetActive(true);
        BlackboardScript.DestroyAllCorpses();
        FloorBlood.Clear();
        GameState = State.Playing;
        RoundUnlockCount = 0;
        roundStartTime_ = Time.time;
        roundStartBestScore_ = SaveGame.Members.GetCounter(GameCounter.Max_Score_Any);
        PlayerScript.SetPlayerPos(Vector3.zero);
        Orc.SetPosition(Vector3.up * 3);
        TextUnlockBasePos = TextNewUnlock.rectTransform.anchoredPosition.y;

        if (CurrentDeedData.Deed != DeedEnum.None && CurrentDeedData.WeaponRestrictions.Count != 0)
        {
            // Start with a weapon when running a deed with weapon restrictions (mostly for the ones where player movement is ZERO)
            PlayerScript.SetWeapon(CurrentDeedData.WeaponRestrictions[0]);
        }

        MusicManagerScript.Instance.PlayGameMusic(CurrentGameModeData.Music);
        GameProgressScript.Instance.Begin();
        StartCoroutine(Server.Instance.UpdateStat("RoundStarted", 1));
    }

    void ClearParticles()
    {
        // Except blood on floor
        FlyingBlood.Clear();
        BloodDrops.Clear();
        PoofClouds.Clear();
        SpawnPoof.Clear();
        FlashParticles.Clear();
        FlameParticles.Clear();
        NpcFlameParticles.Clear();
    }

    IEnumerator ServerColdStart()
    {
        yield return StartCoroutine(Server.Instance.DoServerColdStart());
//        Server.Instance.LastResult
    }

    void LateUpdate()
    {
        PruneDeadEnemies();
    }

    KeyCode[] Code = new KeyCode[] { KeyCode.R, KeyCode.E, KeyCode.S, KeyCode.E, KeyCode.T, KeyCode.A, KeyCode.L, KeyCode.L };
    KeyCode[] CodeRestore = new KeyCode[] { KeyCode.R, KeyCode.E, KeyCode.S, KeyCode.T, KeyCode.O, KeyCode.R, KeyCode.E };

    int codeIdx = 0;
    int restoreIdx = 0;

    void Update()
    {
        if (Input.GetKey(Code[codeIdx]))
        {
            codeIdx++;
            if (codeIdx == Code.Length)
            {
                codeIdx = 0;
                SaveGame.ResetAll();
                SaveGame.Save();
                Unlocks.RefreshUnlocked();
                return;
            }
        }

        if (Input.GetKey(CodeRestore[restoreIdx]))
        {
            restoreIdx++;
            if (restoreIdx == CodeRestore.Length)
            {
                restoreIdx = 0;
                if (SaveGame.RestoreOldPrefs())
                    TextHeroName.text = "Old data restored";
                else
                    TextHeroName.text = "No old data found";
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MusicManagerScript.Instance.ToggleMusic();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        //var mousePos = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0);
        //SetDebugOutput("axis", mousePos);
        Vector3 worldPos = Input.mousePosition;
        worldPos.z = 0;
        Crosshair.position = worldPos;
        CrosshairWorldPosition = Camera.main.ScreenToWorldPoint(worldPos);
        CrosshairWorldPosition.z = 0.0f;
    }

    public Vector2 UiPositionFromWorld(Vector3 world)
    {
        Vector2 result = Camera.main.WorldToViewportPoint(world);
        const float ReferenceWidth = 800;
        const float ReferenceHeight = 600;
        result.x = -ReferenceWidth * 0.5f + result.x * ReferenceWidth;
        result.y = -ReferenceHeight * 0.5f + result.y * ReferenceHeight;
        return result;
    }

    void PlayMenuSound()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.MiscAudioSource, AudioManager.Instance.AudioData.Menu);
    }

    public void OnEnemyKill(ActorBase actor)
    {
        SaveGame.RoundKills++;
        GameEvents.CounterEvent(GameCounter.Kill_Any, 1);

        if (actor.ActorType == ActorTypeEnum.SmallWalker || actor.ActorType == ActorTypeEnum.SmallCharger)
            GameEvents.CounterEvent(GameCounter.Kill_Small, 1);
        else if (actor.ActorType == ActorTypeEnum.LargeWalker)
            GameEvents.CounterEvent(GameCounter.Kill_BigWalker, 1);
        else if (actor.ActorType == ActorTypeEnum.Caster)
            GameEvents.CounterEvent(GameCounter.Kill_Caster, 1);

        if (CurrentDeedData.OnKill(actor.ActorType))
        {
            TextDeedScore.text = string.Format("{0} / {1}", CurrentDeedData.DeedCurrentScore, CurrentDeedData.KillReq);
        }
    }

    public void OnOrcPickup(Vector3 pos)
    {
        SaveGame.RoundScore++;
        GameEvents.CounterEvent(GameCounter.Score_Any_Sum, 1);
        GameEvents.CounterEvent(GameCounter.Max_Score_Any, SaveGame.RoundScore);

        if (GameMode == GameModeEnum.Nursery)
        {
            GameEvents.CounterEvent(GameCounter.Score_Nursery_Sum, 1);
            GameEvents.CounterEvent(GameCounter.Max_Score_Nursery, SaveGame.RoundScore);
        }
        else if (GameMode == GameModeEnum.Earth)
        {
            GameEvents.CounterEvent(GameCounter.Score_Earth_Sum, 1);
            GameEvents.CounterEvent(GameCounter.Max_Score_Earth, SaveGame.RoundScore);
        }
        else if (GameMode == GameModeEnum.Wind)
        {
            GameEvents.CounterEvent(GameCounter.Score_Wind_Sum, 1);
            GameEvents.CounterEvent(GameCounter.Max_Score_Wind, SaveGame.RoundScore);
        }
        else if (GameMode == GameModeEnum.Fire)
        {
            GameEvents.CounterEvent(GameCounter.Score_Fire_Sum, 1);
            GameEvents.CounterEvent(GameCounter.Max_Score_Fire, SaveGame.RoundScore);
        }
        else if (GameMode == GameModeEnum.Storm)
        {
            GameEvents.CounterEvent(GameCounter.Score_Storm_Sum, 1);
            GameEvents.CounterEvent(GameCounter.Max_Score_Storm, SaveGame.RoundScore);
        }
        else if (GameMode == GameModeEnum.Harmony)
        {
            GameEvents.CounterEvent(GameCounter.score_Harmony_Sum, 1);
            GameEvents.CounterEvent(GameCounter.Max_score_Harmony, SaveGame.RoundScore);
        }

        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.MiscAudioSource, AudioManager.Instance.AudioData.OrcPickup);

        Vector2 uiPos = UiPositionFromWorld(PlayerTrans.position + Vector3.up * 0.5f);
        var rectTransform = TextFloatingWeapon.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = uiPos;

        TextScore.text = SaveGame.RoundScore.ToString();

        Unlocks.SetRandomWeapon();
        TextFloatingWeapon.SetText(WeaponBase.WeaponDisplayName(PlayerScript.Weapon.Type), 2.0f);

        if (CurrentDeedData.ShowOrcs)
        {
            Vector3 bestPos = Vector3.zero;
            float bestDistance = 0.0f;
            for (int i = 0; i < 5; ++i)
            {
                Vector3 newPos = PositionUtility.GetPointInsideArena(1.0f, 0.9f);
                float distance = Vector3.Distance(newPos, pos);
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    bestPos = newPos;
                }

                // TODO PE: Does not seem to work? I've seen it behind the score.
                // Let's try not placing the orc around the UI
                bool nearUi = newPos.y > 4.5f && (newPos.x > -3.5f && newPos.x < 3.5f);
                if (nearUi)
                    continue;

                if (distance > 5.0f)
                    break;
            }

            Orc.SetPosition(bestPos);
            MakeFlash(bestPos, 2.0f);
        }
        else
        {
            // Hide the orc
            Orc.SetPosition(Vector3.left * 10000);
        }
    }

    public static void TrySendEvent(string category, string value)
    {
        // Could be a PlayFab event?
    }

    public static void SetDebugOutput(string key, object value)
    {
        DebugValues[key] = value.ToString();
    }

    public void MakePoof(Vector3 pos, int count, float size = 1.0f)
    {
        PoofClouds.transform.position = pos;
        var main = PoofClouds.main;
        main.startSize = size;
        PoofClouds.Emit(count);
    }

    public void MakeSpawnPoof(Vector3 pos, int count)
    {
        SpawnPoof.transform.position = pos;
        var main = SpawnPoof.main;
        SpawnPoof.Emit(count);
    }

    public void MakeFlash(Vector3 pos, float size = 1.0f)
    {
        FlashParticles.transform.position = pos;
        var main = FlashParticles.main;
        main.startSize = size;
        FlashParticles.Emit(1);
    }

    public void MakeCircle(Vector3 pos, float size = 1.0f)
    {
        CircleParticles.transform.position = pos;
        var main = CircleParticles.main;
        main.startSize = size;
        CircleParticles.Emit(1);
    }

    public void EmitFlame(Vector3 pos, float size = 1.0f)
    {
        FlameParticles.transform.position = pos;
        var main = FlameParticles.main;
        main.startSize = size;
        FlameParticles.Emit(1);
    }

    public void TriggerBlood(Vector3 pos, float amount)
    {
        FlyingBlood.transform.position = pos;
        int rangeFrom = Mathf.RoundToInt(10 * amount);
        FlyingBlood.Emit(UnityEngine.Random.Range(rangeFrom, rangeFrom + 5));

        BloodDrops.transform.position = pos;
        BloodDrops.Emit(Mathf.RoundToInt(1 + (0.25f * amount)));

        FloorBlood.transform.position = pos;
        FloorBlood.Emit(Mathf.RoundToInt(1 + (0.25f * amount)));
    }

    public void RegisterEnemy(ActorBase enemy)
    {
        BlackboardScript.Enemies.Add(enemy);
    }

    public void RegisterEnemyDied(ActorBase enemy)
    {
        BlackboardScript.Enemies.Remove(enemy);
        BlackboardScript.DeadEnemies.Add(enemy);
    }

    public void ShakeCamera(float force)
    {
        // Create two or more (set DontDelete)
        // Add them to the list somehow
        // At every shake update the shakes 'something' directly, so it overwrites
        CameraShakeInstance c = CameraShaker.Instance.ShakeOnce(magn * force, rough, fadeIn, fadeOut);
        c.PositionInfluence = posInf;
        c.RotationInfluence = rotInf;
    }

    Vector3 posInf = new Vector3(0.5f, 0.3f, 0.0f);
    Vector3 rotInf = new Vector3(0, 0, 0);
    float magn = 1.0f, rough = 10, fadeIn = 0.5f, fadeOut = 0.5f;

    public void DamageEnemy(ActorBase enemy, float amount, Vector3 direction, float forceModifier, bool headshot = false)
    {
        if (headshot)
            amount *= 1.5f;

        enemy.ApplyDamage(amount, direction, forceModifier, headshot);
    }

    void Awake()
    {
        Instance = this;

        SortLayerTopEffects = SortingLayer.NameToID("TopEffects");
        LayerPlayer = LayerMask.NameToLayer("Player");
        LayerEnemyProjectile = LayerMask.NameToLayer("EnemyProjectile");
        LayerEnemyCorpse = LayerMask.NameToLayer("EnemyCorpse");
        LayerPlayerProjectile = LayerMask.NameToLayer("PlayerProjectile");
        LayerEnemy = LayerMask.NameToLayer("Enemy");
        LayerNeutral = LayerMask.NameToLayer("Neutral");
        LayerXpPill = LayerMask.NameToLayer("XpPill");
        LayerOrc = LayerMask.NameToLayer("Orc");

        SpriteFlashParamId = Shader.PropertyToID("_FlashAmount");
        SpriteFlashColorParamId = Shader.PropertyToID("_FlashColor");

        var bounds = GetComponent<BoxCollider2D>();
        float halfX = bounds.size.x / 2;
        float halfY = bounds.size.y / 2;
        ArenaBounds = new Rect(-halfX, -halfY, halfX * 2, halfY * 2);
        InitDeedItems();

        SaveGame.Load();

        //// One-shot message when old prefs are deleted
        //if (SaveGame.OldPrefsPresent())
        //{
        //    EnablePanel(PanelResetProgress, true);
        //}

        Unlocks.RefreshUnlocked();
    }

    public void OnPrefsDeletedOk()
    {
        EnablePanel(PanelResetProgress, false);
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        StartCoroutine(ServerColdStart());
        ShowTitle();
        StartCoroutine(GameStateCo());
    }

    public bool IsInsideBounds(Vector3 pos, Sprite sprite)
    {
        float halfH = sprite.bounds.extents.y;
        float halfW = sprite.bounds.extents.x;
        Rect sizeAdjustedBounds = Rect.MinMaxRect(ArenaBounds.xMin + halfW, ArenaBounds.yMin + halfH * 2, ArenaBounds.xMax - halfW, ArenaBounds.yMax - halfH); // Pivot is at head so * 2 for bottom
        return sizeAdjustedBounds.Contains(pos);
    }

    public Vector3 ClampToBounds(Vector3 pos, Sprite sprite)
    {
        float halfH = sprite == null ? 0.0f : sprite.bounds.extents.y;
        float halfW = sprite == null ? 0.0f : sprite.bounds.extents.x;
        pos.x = Mathf.Clamp(pos.x, ArenaBounds.xMin + halfW, ArenaBounds.xMax - halfW);
        pos.y = Mathf.Clamp(pos.y, ArenaBounds.yMin + halfH * 2, ArenaBounds.yMax - halfH); // Pivot is at head so * 2 for bottom
        return pos;
    }

    void OnGUI()
    {
        if (DebugValues.Count == 0)
            return;

        float y = 200.0f;
        GUI.contentColor = Color.white;
        foreach (var pair in DebugValues)
        {
            GUI.Label(new Rect(10, y, 1000, 20), string.Format("{0} = {1}", pair.Key, pair.Value));
            y += 20;
        }
    }

    void PruneDeadEnemies()
    {
        for (int i = BlackboardScript.Enemies.Count - 1; i >= 0; --i)
        {
            if (BlackboardScript.Enemies[i].Hp <= 0)
                RegisterEnemyDied(BlackboardScript.Enemies[i]);
        }
    }
}
