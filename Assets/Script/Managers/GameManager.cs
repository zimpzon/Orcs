using Assets.Script;
using EZCameraShake;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.Actors.Spawning;

public enum GameModeEnum { Undeads };

public class GameManager : MonoBehaviour
{
    public enum State { None, Intro, Intro_GameMode, Intro_Shop, Intro_Settings, Playing, Dead };

    const float XpPerLevelMultiplier = 1.5f;
    const float BaseXpToLevel = 14;

    public string GameVersion;
    public static GameManager Instance;
    public bool UnlockAllGameModes;
    public bool UnlockAllWeapons;
    public bool UnlockAllHeroes;

    public Color[] xpColors = new Color[] { };

    public LeanTween Tween;
    public Text TextVersion;
    public Text TextGameInfo;
    public Text TextLevel;
    public Text TextHp;
    public Text TextTime;
    public Text TextTimeScale;
    public Text TextRoundKills;
    public Text TextRoundGold;
    public Text TextUser;
    public Text TextShopMoney;
    public Text TextFps;
    public Text TextGameOverGold;
    public SpriteRenderer Floor;
    public SpriteRenderer FloorFilter;
    Color floorDefaultColor;
    public Button ButtonPlay;
    public TextMeshProUGUI ButtonRefundAmount;
    public string ColorLocked;
    public string ColorUnlocked;
    public ShopItemScript ShopItemProto;
    public Transform ShopItemsRoot;
    public Slider SliderMaster;
    public Slider SliderMusic;
    public Slider SliderSfx;
    public Transform PanelChoices;
    public Transform PanelPaused;
    public GameObject UpgradeChoice1;
    public GameObject UpgradeChoice2;
    public GameObject UpgradeChoice3;
    public GameObject UpgradeChoice4;
    public GameObject PanelSettings;
    public GameObject PanelShop;
    public Canvas CanvasGameOverDefault;
    public GameModeEnum GameMode;

    public ParticleSystem FlyingBlood;
    public ParticleSystem BloodDrops;
    public ParticleSystem FloorBlood;
    public ParticleSystem PoofClouds;
    public ParticleSystem SpawnPoof;
    public ParticleSystem FlashParticles;
    public ParticleSystem CircleParticles;
    public OrcController Orc;
    public int SortLayerTopEffects;
    public State GameState;
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
    public float TimeSinceStartup;
    public bool PauseGameTime;
    public float GameTime;
    public float GameDeltaTime;

    [NonSerialized] public GameModeData LatestGameModeData = new ();
    [NonSerialized] public GameModeData CurrentGameModeData;
    public GameModeData GameModeDataNursery = new ();
    public GameModeData GameModeDataEarth = new ();
    public GameModeData GameModeDataWind = new ();
    public GameModeData GameModeDataFire = new ();
    public GameModeData GameModeDataStorm = new ();
    public GameModeData GameModeDataHarmony = new ();

    public int SpriteFlashParamId;
    public int SpriteFlashColorParamId;
    public static Rect ArenaBounds = new();
    public static Rect TopRect = new();
    public static Rect BottomRect = new();
    public static Rect LeftRect = new();
    public static Rect RightRect = new();
    [NonSerialized] public float TextUnlockBasePos;

    static Dictionary<string, string> DebugValues = new ();

    [NonSerialized] public float UnlockedPct;
    [NonSerialized] public int RoundUnlockCount;

    public int lastGameSeconds = 0;
    int lastHp_ = 0;
    int lastMaxHp_ = 0;
    [NonSerialized] public float xpToLevel;
    int lastXpShown_ = 0;
    int lastKillsShown_ = 0;
    int lastGoldShown_ = 0;
    [NonSerialized] public float currentXp = 0;
    float currentLevel_ = 1;
    float roundStartTime_;
    float chapterTime_;

    public void SliderMasterChanged()
    {
        float value = SliderMaster.value;
        SaveGame.Members.VolumeMaster = value;
        MusicManagerScript.Instance.SetVolume(SaveGame.Members.VolumeMusic * SaveGame.Members.VolumeMaster);
        AudioManager.Instance.SetVolume(SaveGame.Members.VolumeSfx * SaveGame.Members.VolumeMaster);
    }

    public void SliderMusicChanged()
    {
        float value = SliderMusic.value;
        MusicManagerScript.Instance.SetVolume(value * SaveGame.Members.VolumeMaster);
        SaveGame.Members.VolumeMusic = value;
    }

    float sfxVolumeChangeLastFeedback_;
    bool skipNextsfxVolumeChangeFeedback_;

    public void SliderSfxChanged()
    {
        float value = SliderSfx.value;
        AudioManager.Instance.SetVolume(value * SaveGame.Members.VolumeMaster);
        if (!skipNextsfxVolumeChangeFeedback_)
        {
            if (G.D.GameTime > sfxVolumeChangeLastFeedback_)
            {
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.Oink);
                sfxVolumeChangeLastFeedback_ = G.D.GameTime + 0.1f;
            }
        }
        SaveGame.Members.VolumeSfx = value;
        skipNextsfxVolumeChangeFeedback_ = false;
    }

    void EnablePanel(GameObject panel, bool enable)
    {
        // Work-around for Unity SetActive bug (still showing UI components after disable)
        panel.SetActive(enable);
//        panel.transform.localScale = enable ? Vector3.one : Vector3.zero;
    }

    public void OnButtonSettings()
    {
        PlayMenuSound();
        GameState = State.Intro_Settings;

        SliderMaster.value = SaveGame.Members.VolumeMaster;
        SliderMusic.value = SaveGame.Members.VolumeMusic;
        skipNextsfxVolumeChangeFeedback_ = true; // Only play feedback sound when user moves slider, not when setting value once right before shown
        SliderSfx.value = SaveGame.Members.VolumeSfx;

        EnablePanel(PanelSettings, true);
    }

    public void OnButtonStart()
    {
        PlayMenuSound();
        GameState = State.Intro_GameMode;
    }

    public void OnButtonRefund()
    {
        PlayMenuSound();
        SaveGame.Members.BoughtItems.Clear();
        SaveGame.Members.Money += SaveGame.Members.MoneySpentInShop;
        SaveGame.Members.MoneySpentInShop = 0;
        ShopItems.UpdateBoughtItems();
        UpdateMoneyLabels();
    }

    public void ResetAllProgress()
    {
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerDie);

        float VolumeMaster = SaveGame.Members.VolumeMaster;
        float VolumeMusic = SaveGame.Members.VolumeMusic;
        float VolumeSfx = SaveGame.Members.VolumeSfx;
        int totalSeconds = SaveGame.Members.TotalSeconds;

        SaveGame.Members = new ();
        SaveGame.Members.VolumeMaster = VolumeMaster;
        SaveGame.Members.VolumeMusic = VolumeMusic;
        SaveGame.Members.VolumeSfx = VolumeSfx;
        SaveGame.Members.TotalSeconds = totalSeconds;

        SaveGame.Save();
        ShopItems.UpdateBoughtItems();
        UpdateMoneyLabels();
    }

    public void OnButtonShop()
    {
        PlayMenuSound();
        UpdateMoneyLabels();
        GameState = State.Intro_Shop;
        EnablePanel(PanelShop, true);
        ShopItems.UpdateBoughtItems();
        resetAllCodeIdx = 0;
    }

    void UpdateMoneyLabels()
    {
        TextShopMoney.text = $"${SaveGame.Members.Money}";
        ButtonRefundAmount.text = $"(${SaveGame.Members.MoneySpentInShop})";
    }

    static int BuyCount = 0;

    public void OnItemRefunded(ShopItemType _)
    {
        UpdateMoneyLabels();
    }

    public void OnItemBought(ShopItemType itemType)
    {
        UpdateMoneyLabels();

        BuyCount++;

        var props = new Dictionary<string, object>
        {
            { "type", itemType.ToString() },
            { "goldLeft", (int)SaveGame.Members.Money },
            { "goldSpent", (int)SaveGame.Members.MoneySpentInShop },
        };

        Playfab.PlayerEvent(Playfab.ItemBoughtEvent, props);
        Playfab.PlayerStat(Playfab.ItemsBoughtStat, BuyCount);
    }

    static bool BackButtonClicked = false;

    public void OnBackButtonClick()
    {
        BackButtonClicked = true;
    }

    bool GoBack()
    {
        if (BackButtonClicked)
        {
            BackButtonClicked = false;
            return true;
        }

        return Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace);
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
                if (GoBack())
                {
                    PlayMenuSound();
                    GameState = State.Intro;
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayMenuSound();
                    StartGame();
                }

                yield return null;
            }

            while (GameState == State.Intro_Settings)
            {
                if (GoBack())
                {
                    PlayMenuSound();
                    SaveGame.Save();
                    GameState = State.Intro;
                    EnablePanel(PanelSettings, false);
                    break;
                }

                yield return null;
            }

            while (GameState == State.Intro_Shop)
            {
                if (G.GetCheatKey(KeyCode.R) && G.GetCheatKey(KeyCode.RightShift) && G.GetCheatKey(KeyCode.LeftControl))
                {
                    ResetAllProgress();
                }

                if (G.GetCheatKeyDown(KeyCode.M) && G.GetCheatKey(KeyCode.RightShift))
                {
                    SaveGame.Members.Money += 500;
                    ShopItems.UpdateBoughtItems();
                    UpdateMoneyLabels();
                }

                if (GoBack())
                {
                    PlayMenuSound();
                    SaveGame.Save();
                    GameState = State.Intro;
                    EnablePanel(PanelShop, false);
                    break;
                }

                yield return null;
            }

            while (GameState == State.Playing)
            {
                int gameSeconds = (int)(chapterTime_ + 0.5f);
                if (gameSeconds != lastGameSeconds)
                {
                    TextTime.text = $"{gameSeconds / 60:00}:{gameSeconds % 60:00}";
                    lastGameSeconds = gameSeconds;
                }

                int hpLeft = (int)(G.D.PlayerScript.Hp + 0.5f);
                int maxHp = (int)(G.D.PlayerScript.MaxHp + 0.5f);
                if (hpLeft != lastHp_ || maxHp != lastMaxHp_)
                {
                    TextHp.text = $"{hpLeft}/{G.D.PlayerScript.MaxHp} HP";
                    lastHp_ = hpLeft;
                    lastMaxHp_ = maxHp;
                }

                if (currentXp != lastXpShown_)
                {
                    float pct = Math.Min(100, (currentXp / xpToLevel) * 100);
                    TextLevel.text = $"LEVEL {currentLevel_} ({pct:0.0}%)";
                    lastXpShown_ = (int)currentXp;
                }

                if (SaveGame.RoundKills != lastKillsShown_)
                {
                    TextRoundKills.text = SaveGame.RoundKills.ToString();
                    lastKillsShown_ = SaveGame.RoundKills;
                }

                if (SaveGame.RoundGold != lastGoldShown_)
                {
                    TextRoundGold.text = SaveGame.RoundGold.ToString();
                    lastGoldShown_ = SaveGame.RoundGold;
                }

                while (currentXp >= xpToLevel)
                    yield return LevelUp();

                if (GoBack())
                {
                    PauseGameTime = true;
                    while (PauseGameTime)
                        yield return Pause();

                    if (!G.D.PlayerScript.RoundComplete)
                        MakeFlash(G.D.PlayerPos, 5.0f);
                }

                float delta = GameDeltaTime;
                ProjectileManager.Instance.Tick(delta);
                yield return null;
            }

            while (GameState == State.Dead)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    G.D.PlayerScript.RoundComplete = true;
                    ShowTitle(autoStartGame: true);
                }
                else if (GoBack())
                {
                    G.D.PlayerScript.RoundComplete = true;
                    ShowTitle();
                }

                yield return null;
            }

            yield return null;
        }
    }

    void SetChoicesVisible(bool visible)
    {
        PanelChoices.gameObject.SetActive(visible);
    }

    public void PauseResume()
    {
        Unpause();
    }

    public void PauseQuitToMenu()
    {
        Unpause();
        G.D.PlayerScript.RoundComplete = true;
        ShowTitle(autoStartGame: false);
    }

    private void Unpause()
    {
        EnablePanel(PanelPaused.gameObject, false);
        PauseGameTime = false;
        Time.timeScale = 1.0f;
        SetChoicesVisible(false);
        AudioListener.pause = false;
    }

    IEnumerator Pause()
    {
        PlayMenuSound();
        EnablePanel(PanelPaused.gameObject, true);

        AudioListener.pause = true;

        Time.timeScale = 0.0f;
        PauseGameTime = true;

        while (PauseGameTime)
        {
            yield return null;

            if (GoBack())
            {
                PlayMenuSound();
                Unpause();
                yield break;
            }
        }
    }

    IEnumerator LevelUp()
    {
        bool selectionDone = false;

        void SelectionCallback(Choice choice)
        {
            selectionDone = true;
            choice.Apply();
        }

        SetChoicesVisible(true);

        UpgradeChoice1.GetComponent<UpgradeChoiceScript>().SelectionCallback = SelectionCallback;
        UpgradeChoice2.GetComponent<UpgradeChoiceScript>().SelectionCallback = SelectionCallback;
        UpgradeChoice3.GetComponent<UpgradeChoiceScript>().SelectionCallback = SelectionCallback;
        UpgradeChoice4.GetComponent<UpgradeChoiceScript>().SelectionCallback = SelectionCallback;

        AudioListener.pause = true;
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.LevelUp, volumeScale: 0.5f, pitch: 1.0f, ignoreListenerPause: true);

        Time.timeScale = 0.0f;
        PauseGameTime = true;

        while (!selectionDone)
        {
            yield return null;
        }

        currentXp -= xpToLevel;
        xpToLevel = (int)(xpToLevel * XpPerLevelMultiplier);
        currentLevel_++;

        PauseGameTime = false;
        Time.timeScale = 1.0f;

        SetChoicesVisible(false);

        AudioListener.pause = false;
        Explosions.Push(G.D.PlayerPos, 4.0f, 1.0f);
        MakeFlash(G.D.PlayerPos, 5.0f);
    }

    public void ShowingHowToPlay()
    {
        currentXp = 0;
        xpToLevel = BaseXpToLevel;
        currentLevel_ = 1;
    }

    public void HidingHowToPlay()
    {
        roundStartTime_ = GameTime;
        G.D.PlayerScript.UpgradesActive = true;
    }

    public void ShowGameOver()
    {
        if (GameState == State.Dead)
            return;

        var props = new Dictionary<string, object>
        {
            { "score", SaveGame.RoundScore },
            { "level", (int)currentLevel_ },
            { "gold", SaveGame.RoundGold },
            { "kills", SaveGame.RoundKills },
            { "game_seconds", lastGameSeconds },
        };
        Playfab.PlayerEvent(Playfab.GameOverEvent, props);

        Rounds++;

        TextGameInfo.gameObject.SetActive(false);

        float roundTime = GameTime - roundStartTime_;
        int roundSeconds = Mathf.RoundToInt(roundTime);
        SaveGame.Members.TotalSeconds += roundSeconds;

        var dic = new Dictionary<string, int>();
        dic[Playfab.GoldStat] = SaveGame.RoundGold;
        dic[Playfab.LevelStat] = (int)currentLevel_;
        dic[Playfab.KillsStat] = SaveGame.RoundKills;
        dic[Playfab.ScoreStat] = SaveGame.RoundScore;
        dic[Playfab.RoundsCompletedStat] = Rounds;
        dic[Playfab.SecondsLeftStat] = lastGameSeconds;
        dic[Playfab.TotalSeconds] = SaveGame.Members.TotalSeconds;

        Playfab.PlayerStat(dic);

        GameProgressScript.Instance.Stop();
        ProjectileManager.Instance.StopAll();

        CanvasGameOverDefault.enabled = true;

        SaveGame.UpdateFromRound(roundSeconds, reset: true);
        SaveGame.Save();

        CanvasDead.gameObject.SetActive(true);
        CanvasIntro.gameObject.SetActive(false);

        if (SaveGame.Members.Money < 20)
            TextGameOverGold.text = $"Go to the shop and spend your... <color=#ffd700>{SaveGame.Members.Money}</color> gold... Nevermind, go collect some more!";
        else
            TextGameOverGold.text = $"Go to the shop and spend your <color=#ffd700>{SaveGame.Members.Money}</color> gold!";

        GameState = State.Dead;
    }

    static int Rounds = 0;

    void KillKillableObjects()
    {
        var killables = FindObjectsOfType<MonoBehaviour>().OfType<IKillableObject>();
        foreach (var killable in killables)
        {
            killable.Kill();
        }
    }

    public void ShowTitle(bool autoStartGame = false)
    {
        if (GameState == State.Intro)
            return;

        LeanTween.color(Floor.gameObject, floorDefaultColor, 1.0f);

        Floor.color = floorDefaultColor;
        FloorFilter.color = Color.clear;

        Time.timeScale = 1.0f;
        PanelSettings.SetActive(false);
        ProjectileManager.Instance.StopAll();
        G.D.PlayerScript.ResetAll();
        BlackboardScript.DestroyAllEnemies();
        GameProgressScript.Instance.Stop();
        Orc.ResetAll();
        CameraShaker.Instance.ShakeInstances.Clear();
        Camera.main.transform.parent.position = new Vector3(0.0f, 0.0f, -10.0f);
        Camera.main.orthographicSize = 7.68f;
        KillKillableObjects();

        ClearParticles();
        CanvasIntro.gameObject.SetActive(true);
        CanvasGame.gameObject.SetActive(false);
        CanvasDead.gameObject.SetActive(false);

        if (autoStartGame)
        {
            StartGame();
        }
        else
        {
            GameState = State.Intro;
            MusicManagerScript.Instance.PlayIntroMusic();
        }
    }

    public void StartGame()
    {
        if (GameState == State.Playing)
            return;

        GameTime = 0.0001f;
        GameDeltaTime = 0.0f;
        chapterTime_ = 0;

        ActorBase.ResetClosestEnemy();
        SaveGame.ResetRound();
        PlayerUpgrades.ResetAll();
        ShopItems.ApplyToPlayerUpgrades();
        UpgradeChoices.InitChoices();
        ResetPickups();
        InitXpText();

        TextTimeScale.enabled = PlayerUpgrades.Data.TimeScale > 1;
        TextTimeScale.text = $"+{(int)Math.Round((PlayerUpgrades.Data.TimeScale - 1) * 100)}%";

        CanvasIntro.gameObject.SetActive(false);
        CanvasDead.gameObject.SetActive(false);
        CanvasGame.gameObject.SetActive(true);
        BlackboardScript.DestroyAllCorpses();
        FloorBlood.Clear();
        GameState = State.Playing;
        RoundUnlockCount = 0;
        roundStartTime_ = Time.time;
        G.D.PlayerScript.SetPlayerPos(Vector3.zero);
        G.D.PlayerScript.StartGame();
        Orc.SetPosition(Vector3.up * 3, startingGame: true);

        MusicManagerScript.Instance.PlayGameMusic(CurrentGameModeData.Music);
        GameProgressScript.Instance.Begin(GameModeEnum.Undeads);
    }

    void ClearParticles()
    {
        // Except blood on floor
        FlyingBlood.Clear();
        BloodDrops.Clear();
        PoofClouds.Clear();
        SpawnPoof.Clear();
        FlashParticles.Clear();
    }

    KeyCode[] Code = new KeyCode[] { KeyCode.R, KeyCode.E, KeyCode.S, KeyCode.E, KeyCode.T, KeyCode.A, KeyCode.L, KeyCode.L };

    int resetAllCodeIdx = 0;

    void LateUpdate()
    {
        PruneDeadEnemies();
        ActorBase.ResetClosestEnemy();
    }

    void Update()
    {
        TimeSinceStartup = Time.realtimeSinceStartup;
        if (PauseGameTime)
        {
            GameDeltaTime = 0;
        }
        else
        {
            if (G.GetCheatKeyDown(KeyCode.T) && G.GetCheatKey(KeyCode.RightShift))
            {
                chapterTime_ += 30;
                GameTime += 30;
            }

            if (G.GetCheatKeyDown(KeyCode.L) && G.GetCheatKey(KeyCode.RightShift))
            {
                ThrowPickups(AutoPickUpType.Xp, Vector2.zero, 20, 10);
            }

            if (G.GetCheatKeyDown(KeyCode.RightArrow) && G.GetCheatKey(KeyCode.RightShift))
            {
                PlayerUpgrades.Data.TimeScale += 0.1f;
            }

            if (G.GetCheatKeyDown(KeyCode.LeftArrow) && G.GetCheatKey(KeyCode.RightShift))
            {
                PlayerUpgrades.Data.TimeScale -= 0.1f;
            }

            if (G.GetCheatKeyDown(KeyCode.F) && G.GetCheatKey(KeyCode.RightShift))
            {
                SpawnUtil.FleeAllActors();
            }


            GameDeltaTime = Math.Min(0.1f, Time.deltaTime * PlayerUpgrades.Data.TimeScale);

            if (SaveGame.RoundScore > 0)
            {
                chapterTime_ += GameDeltaTime;
                GameTime += GameDeltaTime;
            }
        }

        if (GameState == State.Intro_Shop && Input.GetKeyDown(Code[resetAllCodeIdx]))
        {
            resetAllCodeIdx++;
            if (resetAllCodeIdx == Code.Length)
            {
                resetAllCodeIdx = 0;
                ResetAllProgress();
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            TextFps.enabled = !TextFps.enabled;
        }

        if (TextFps.enabled)
        {
            TextFps.text = string.Format("{0} fps", Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
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
        AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.Menu);
    }

    public void OnEnemyKill(ActorBase actor)
    {
        SaveGame.RoundKills++;

        if (UnityEngine.Random.value < PlayerUpgrades.Data.DropMoneyOnKillChance)
        {
            int amount = UnityEngine.Random.Range(PlayerUpgrades.Data.DropMoneyOnKillMin, PlayerUpgrades.Data.DropMoneyOnKillMax + 1);
            ThrowPickups(AutoPickUpType.Money, actor.transform.position, amount, value: 1, forceScale: 1.0f);
        }

        ThrowPickups(AutoPickUpType.Money, actor.transform.position, actor.GoldCount, value: 1, forceScale: 1.0f);
        ThrowPickups(AutoPickUpType.Xp, actor.transform.position, amount: actor.XpCount, value: actor.XpValue, forceScale: 1.0f);
    }

    private void OnFirstOrcPickup()
    {
        G.D.PlayerScript.OnInitialPickup(WeaponType.Machinegun);
    }

    public void AddXp(int amount)
    {
        currentXp += amount;
    }

    void ResetPickups()
    {
        // pickups
        var pickups = Resources.FindObjectsOfTypeAll(typeof(AutoPickUpScript)).Cast<AutoPickUpScript>().ToList();
        foreach (var pickup in pickups)
        {
            if (pickup.isActiveAndEnabled)
                pickup.Die();
        }
    }

    public void ThrowPickups(AutoPickUpType pickupType, Vector2 pos, int amount, int value, float forceScale = 1.0f)
    {
        float doubleChance = pickupType == AutoPickUpType.Money ? PlayerUpgrades.Data.MoneyDoubleChance : PlayerUpgrades.Data.XpDoubleChance;

        if (UnityEngine.Random.value < doubleChance)
            amount *= 2;

        for (int i = 0; i < amount; ++i)
        {
            var pickup = PickUpManagerScript.Instance.GetPickUpFromCache(pickupType);
            pickup.transform.position = pos;
            
            var pickupScript = pickup.GetComponent<AutoPickUpScript>();
            pickupScript.Value = value;
            pickupScript.Throw(UnityEngine.Random.insideUnitCircle, forceScale);

            if (pickupType == AutoPickUpType.Xp)
            {
                float xpValue = value * PlayerUpgrades.Data.XpValueMul;
                float xpToColorScale = 2.0f;
                int colorIdx = Mathf.Min(xpColors.Length - 1, (int)(xpValue / xpToColorScale));
                pickup.GetComponent<SpriteRenderer>().color = xpColors[colorIdx];
            }
            pickup.SetActive(true);
        }
    }

    public void OnOrcPickup(Vector3 pos)
    {
        SaveGame.RoundScore++;

        if (SaveGame.RoundScore == 1)
            OnFirstOrcPickup();

        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.OrcPickup);
        G.D.PlayerScript.AddHp(PlayerUpgrades.Data.RescueDuckHp, alwaysShow: true);
        ThrowPickups(AutoPickUpType.Xp, pos, 1 + SaveGame.RoundScore / 5, value: 1, forceScale: 2.0f);
        ThrowPickups(AutoPickUpType.Money, pos, 2 + SaveGame.RoundScore / 2, value: 1, forceScale: 1.1f);
    }

    private void InitXpText()
    {
        TextLevel.text = $"LEVEL 1 (0%)";
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
        SpawnPoof.Emit(count);
    }

    public void MakeFlash(Vector3 pos, Color color, float size = 1.0f)
    {
        FlashParticles.transform.position = pos;
        var main = FlashParticles.main;
        main.startSize = size;
        main.startColor = color;
        FlashParticles.Emit(1);
    }

    public void MakeFlash(Vector3 pos, float size = 1.0f)
    {
        MakeFlash(pos, Color.white, size);
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
        Debug.LogError("NOT IMPLEMENTED");
    }

    public void TriggerBlood(Vector3 pos, float amount, float floorBloodRnd = 1.0f)
    {
        FlyingBlood.transform.position = pos;
        int rangeFrom = Mathf.RoundToInt(10 * amount);
        if (rangeFrom > 8)
            rangeFrom = 8;
        FlyingBlood.Emit(UnityEngine.Random.Range(rangeFrom, rangeFrom + 5));

        BloodDrops.transform.position = pos;
        BloodDrops.Emit(Mathf.RoundToInt(1 + (0.25f * amount)));

        if (UnityEngine.Random.value <= floorBloodRnd)
        {
            int bloodAmount = Mathf.RoundToInt(1 + (0.25f * amount));
            if (bloodAmount > 8)
                bloodAmount = 8;

            FloorBlood.transform.position = pos;
            FloorBlood.Emit(bloodAmount);
        }
    }

    public void RegisterEnemyDied(ActorBase enemy)
    {
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

    public void DamageEnemy(ActorBase enemy, float amount, Vector3 direction, float forceModifier)
    {
        amount *= PlayerUpgrades.Data.DamageMul;
        bool isCrit = UnityEngine.Random.value < PlayerUpgrades.Data.BaseCritChance * PlayerUpgrades.Data.CritChanceMul;
        if (isCrit)
            amount *= PlayerUpgrades.Data.CritValueMul;

        enemy.ApplyDamage(amount, direction, forceModifier);

        MakeFlash(enemy.transform.position, 1.5f);

        string text = string.Concat("-", ((int)(amount + 0.5f)).ToString());

        FloatingTextSpawner.Instance.Spawn(
            enemy.transform.position + Vector3.up * 0.3f,
            text,
            isCrit ? Color.yellow : Color.red,
            speed: isCrit ? 0.4f : 0.25f,
            timeToLive: 0.5f, isCrit ? FontStyles.Bold : FontStyles.Normal);
    }

    void Awake()
    {
        TextVersion.text = GameVersion;
        Playfab.Login();

        Instance = this;
        Application.targetFrameRate = 60;
        floorDefaultColor = Floor.color;
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

        CurrentGameModeData = GameModeDataNursery;

        SetChoicesVisible(false);

        var bounds = GetComponent<BoxCollider2D>();

        // adjust arena to be 16:10 since on 16:9 we add black bars left/right
        float arenaHeight = bounds.size.y;
        float arenaWidth = arenaHeight * AspectUtility.WantedAspectRatio;
        float halfX = arenaWidth / 2;
        float halfY = arenaHeight / 2;

        const float Size = 2;
        ArenaBounds = new Rect(-halfX + 0.5f, -halfY + 0.35f, halfX * 2 - 1.0f, halfY * 2 - 0.5f);
        TopRect = new Rect(ArenaBounds.x, ArenaBounds.yMax - Size, ArenaBounds.width, Size);
        BottomRect = new Rect(ArenaBounds.x, ArenaBounds.yMin, ArenaBounds.width, Size);

        LeftRect = new Rect(ArenaBounds.x, ArenaBounds.y, Size, ArenaBounds.height);
        RightRect = new Rect(ArenaBounds.xMax - Size, ArenaBounds.y, Size, ArenaBounds.height);

        TextFps.enabled = false;

        ShopItems.CreateItemGoList(ShopItemProto, ShopItemsRoot);
    }

    private void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        
        SaveGame.Load();

        MusicManagerScript.Instance.SetVolume(SaveGame.Members.VolumeMusic * SaveGame.Members.VolumeMaster);
        AudioManager.Instance.SetVolume(SaveGame.Members.VolumeSfx * SaveGame.Members.VolumeMaster);

        ShowTitle();
        StartCoroutine(GameStateCo());
    }

    public bool IsInsideBounds(Vector3 pos, Sprite sprite)
    {
        float halfH = sprite.bounds.extents.y / 2;
        float halfW = sprite.bounds.extents.x / 2;
        Rect sizeAdjustedBounds = Rect.MinMaxRect(ArenaBounds.xMin + halfW, ArenaBounds.yMin + halfH * 2, ArenaBounds.xMax - halfW, ArenaBounds.yMax - halfH);
        return sizeAdjustedBounds.Contains(pos);
    }

    public bool IsOutsideBounds(Vector3 pos)
    {
        Rect sizeAdjustedBounds = Rect.MinMaxRect(ArenaBounds.xMin - 2, ArenaBounds.yMin - 2, ArenaBounds.xMax + 2, ArenaBounds.yMax + 2);
        return sizeAdjustedBounds.Contains(pos);
    }

    public Vector3 ClampToBounds(Vector3 pos, Sprite sprite)
    {
        float halfH = sprite == null ? 0.0f : sprite.bounds.extents.y;
        float halfW = sprite == null ? 0.0f : sprite.bounds.extents.x;
        pos.x = Mathf.Clamp(pos.x, ArenaBounds.xMin + halfW, ArenaBounds.xMax - halfW);
        pos.y = Mathf.Clamp(pos.y, ArenaBounds.yMin + halfH, ArenaBounds.yMax - halfH);
        return pos;
    }

    void PruneDeadEnemies()
    {
        for (int i = BlackboardScript.DeadEnemies.Count - 1; i >= 0; --i)
            RegisterEnemyDied(BlackboardScript.DeadEnemies[i]);
    }

    void OnGUI()
    {
        return;
        SetDebugOutput("OnGUI enabled", G.D.GameTime);

        if (DebugValues.Count == 0)
            return;

        float y = 150.0f;
        GUI.contentColor = Color.white;
        foreach (var pair in DebugValues)
        {
            GUI.Label(new Rect(10, y, 1000, 20), string.Format("{0} = {1}", pair.Key, pair.Value));
            y += 20;
        }
    }
}
