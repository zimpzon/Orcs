using Assets.Script;
using EZCameraShake;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameModeEnum { Undeads };

public class GameManager : MonoBehaviour
{
    public enum State { None, Intro, Intro_GameMode, Intro_Unlocks, Intro_Shop, Intro_Settings, Playing, Dead };

    const float XpPerLevelMultiplier = 1.2f;
    const float WinTime = 60 * 15;
    const float BaseXpToLevel = 10;

    public static GameManager Instance;
    public bool UnlockAllGameModes;
    public bool UnlockAllWeapons;
    public bool UnlockAllHeroes;

    public Color[] xpColors = new Color[] { };

    public Text TextLevel;
    public Text TextHp;
    public Text TextTime;
    public Text TextRoundKills;
    public Text TextRoundGold;
    public Text TextGameOverOrcsSaved;
    public Text TextGameOverOrcsSavedBest;
    public Text TextLocked;
    public Text TextRoundEndUnlocks;
    public Text TextUser;
    public Text TextShopMoney;
    public TextMeshProUGUI TextColWepNames;
    public TextMeshProUGUI TextColWepReq;
    public TextMeshProUGUI TextColLocNames;
    public TextMeshProUGUI TextColLocReq;
    public TextMeshProUGUI TextColDamNames;
    public TextMeshProUGUI TextColDamReq;
    public Text ButtonUnlockedText;
    public Text TextFps;
    public Renderer Floor;
    public Button ButtonGo;
    public Button ButtonPlay;
    public string ColorLocked;
    public string ColorUnlocked;
    public ShopItemScript ShopItemProto;
    public Transform ShopItemsRoot;
    public Slider SliderMusic;
    public Slider SliderSfx;
    public Transform PanelChoices;
    public GameObject UpgradeChoice1;
    public GameObject UpgradeChoice2;
    public GameObject UpgradeChoice3;
    public GameObject UpgradeChoice4;
    public TextMeshProUGUI TextGameMode;
    public TextMeshProUGUI TextGameModeInfo;
    public GameObject PanelGameMode;
    public GameObject PanelUnlocks;
    public GameObject PanelSettings;
    public GameObject PanelShop;
    public Canvas CanvasGameOverDefault;
    public GameModeEnum GameMode;

    public TextBlinkScript TextFloatingWeapon;
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
    public float TimeSinceStartup;
    public bool PauseGameTime;
    public float GameTime;

    [NonSerialized] public GameModeData LatestGameModeData = new ();
    [NonSerialized] public GameModeData CurrentGameModeData;
    public GameModeData GameModeDataNursery = new ();
    public GameModeData GameModeDataEarth = new ();
    public GameModeData GameModeDataWind = new ();
    public GameModeData GameModeDataFire = new ();
    public GameModeData GameModeDataStorm = new ();
    public GameModeData GameModeDataHarmony = new ();

    public List<Hero> Heroes = new ();
    public Hero SelectedHero;

    public int SpriteFlashParamId;
    public int SpriteFlashColorParamId;
    public Rect ArenaBounds = new ();
    [NonSerialized] public float TextUnlockBasePos;

    static Dictionary<string, string> DebugValues = new ();

    [NonSerialized] public float UnlockedPct;
    [NonSerialized] public int RoundUnlockCount;

    int lastSecondsLeft = 0;
    int lastHp_ = 0;
    int lastMaxHp_ = 0;
    float xpToLevel_;
    int lastXpShown_ = 0;
    int lastKillsShown_ = 0;
    int lastGoldShown_ = 0;
    float currentXp_ = 0;
    float currentLevel_ = 1;
    float roundStartTime_;

    KeyCode menuBackKey_ = KeyCode.Escape;

    public void SliderMusicChanged()
    {
        float value = SliderMusic.value;
        MusicManagerScript.Instance.SetVolume(value);
        SaveGame.Members.VolumeMusic = value;
    }

    float sfxVolumeChangeLastFeedback_;
    bool skipNextsfxVolumeChangeFeedback_;

    public void SliderSfxChanged()
    {
        float value = SliderSfx.value;
        AudioManager.Instance.SetVolume(value);
        if (!skipNextsfxVolumeChangeFeedback_)
        {
            if (Time.time > sfxVolumeChangeLastFeedback_)
            {
                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerMachinegunFire);
                sfxVolumeChangeLastFeedback_ = Time.time + 0.1f;
            }
        }
        SaveGame.Members.VolumeSfx = value;
        skipNextsfxVolumeChangeFeedback_ = false;
    }

    public void SelectHero(HeroEnum heroType, bool save = false)
    {
        var hero = Heroes.Where(h => h.HeroType == heroType).FirstOrDefault();
        SelectedHero = hero;

        var renderer = PlayerScript.GetComponent<SpriteRenderer>();
        bool isUnlocked = hero.IsUnlocked();
        renderer.sprite = hero.ShowoffSprite;
        renderer.color = isUnlocked ? Color.white : Color.black;
        SaveGame.Members.SelectedHero = (int)hero.HeroType;

        if (save)
            SaveGame.Save();
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

        SliderMusic.value = SaveGame.Members.VolumeMusic;
        skipNextsfxVolumeChangeFeedback_ = true; // Only play feedback sound when user moves slider, not when setting value once right before shown
        SliderSfx.value = SaveGame.Members.VolumeSfx;

        EnablePanel(PanelSettings, true);
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
        EnablePanel(PanelGameMode, true);
    }

    public void OnButtonUnlocks()
    {
        PlayMenuSound();
        GameState = State.Intro_Unlocks;
        EnablePanel(PanelUnlocks, true);
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

    public void OnButtonResetProgress()
    {
        PlayMenuSound();
        float VolumeMusic = SaveGame.Members.VolumeMusic;
        float VolumeSfx = SaveGame.Members.VolumeSfx;

        SaveGame.Members = new SaveGameMembers();
        SaveGame.Members.VolumeMusic = VolumeMusic;
        SaveGame.Members.VolumeSfx = VolumeSfx;

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
    }

    void UpdateMoneyLabels()
    {
        TextShopMoney.text = $"${SaveGame.Members.Money}";
    }

    public void OnItemBought(ShopItemType itemType)
    {
        UpdateMoneyLabels();
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
                if (Input.GetKeyDown(menuBackKey_))
                {
                    PlayMenuSound();
                    GameState = State.Intro;
                    EnablePanel(PanelGameMode, false);
                    break;
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayMenuSound();
                    StartGame();
                }

                yield return null;
            }

            while (GameState == State.Intro_Unlocks)
            {
                if (Input.GetKeyDown(menuBackKey_))
                {
                    PlayMenuSound();
                    GameState = State.Intro;
                    EnablePanel(PanelUnlocks, false);
                    break;
                }

                yield return null;
            }

            while (GameState == State.Intro_Settings)
            {
                if (Input.GetKeyDown(menuBackKey_))
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
                if (Input.GetKeyDown(KeyCode.M))
                {
                    SaveGame.Members.Money += 500;
                    ShopItems.UpdateBoughtItems();
                    UpdateMoneyLabels();
                }

                if (Input.GetKeyDown(menuBackKey_))
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
                float runTime = GameTime;

                int secondsLeft = (int)(WinTime - runTime + 0.5f);
                if (secondsLeft != lastSecondsLeft)
                {
                    TextTime.text = $"{secondsLeft / 60:00}:{secondsLeft % 60:00}";
                    lastSecondsLeft = secondsLeft;
                }

                int hpLeft = (int)(PlayerScript.Hp + 0.5f);
                int maxHp = (int)(PlayerScript.MaxHp + 0.5f);
                if (hpLeft != lastHp_ || maxHp != lastMaxHp_)
                {
                    TextHp.text = $"{hpLeft}/{PlayerScript.MaxHp} HP";
                    lastHp_ = hpLeft;
                    lastMaxHp_ = maxHp;
                }

                if (currentXp_ != lastXpShown_)
                {
                    float pct = Math.Min(100, (currentXp_ / (float)xpToLevel_) * 100);
                    TextLevel.text = $"LEVEL {currentLevel_} ({pct:0.0}%)";
                    lastXpShown_ = (int)currentXp_;
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

                while (currentXp_ >= xpToLevel_)
                    yield return LevelUp();

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
                else if (Input.GetKeyDown(menuBackKey_))
                {
                    PlayerScript.RoundComplete = true;
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

        Time.timeScale = 0.0001f;
        PauseGameTime = true;

        while (!selectionDone)
        {
            yield return null;
        }

        currentXp_ -= xpToLevel_;
        xpToLevel_ = (int)(xpToLevel_ * XpPerLevelMultiplier);
        currentLevel_++;

        PauseGameTime = false;
        Time.timeScale = 1.0f;

        SetChoicesVisible(false);

        AudioListener.pause = false;
        Explosions.Push(PlayerScript.transform.position, 4.0f, 1.0f);
    }

    public void ShowingHowToPlay()
    {
        currentXp_ = 0;
        xpToLevel_ = BaseXpToLevel;
        currentLevel_ = 1;
    }

    public void HidingHowToPlay()
    {
        roundStartTime_ = Time.realtimeSinceStartup;
        PlayerScript.UpgradesActive = true;
    }

    public void ShowGameOver()
    {
        if (GameState == State.Dead)
            return;

        TextFloatingWeapon.Clear();
        GameProgressScript.Instance.Stop();
        ProjectileManager.Instance.StopAll();

        float roundTime = Time.time - roundStartTime_;
        int roundSeconds = Mathf.RoundToInt(roundTime);
        //StartCoroutine(Server.Instance.UpdateStat("RoundSeconds", roundSeconds));

        int bestScore = SaveGame.Members.GetCounter(GameCounter.Max_Score_Any);
        TextGameOverOrcsSaved.text = string.Format("{0}", SaveGame.RoundScore);
        TextGameOverOrcsSavedBest.text = string.Format("{0}", bestScore);

        CanvasGameOverDefault.enabled = true;
        TextRoundEndUnlocks.enabled = RoundUnlockCount > 0;

        SaveGame.UpdateFromRound(roundSeconds, reset: true);
        SaveGame.Save();

        CanvasDead.gameObject.SetActive(true);
        CanvasIntro.gameObject.SetActive(false);

        GameState = State.Dead;
    }

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

        Time.timeScale = 1.0f;
        PanelGameMode.SetActive(false);
        PanelSettings.SetActive(false);
        PanelUnlocks.SetActive(false);
        ProjectileManager.Instance.StopAll();
        PlayerScript.ResetAll();
        BlackboardScript.DestroyAllEnemies();
        GameProgressScript.Instance.Stop();
        Orc.ResetAll();
        CameraShaker.Instance.ShakeInstances.Clear();
        Camera.main.transform.parent.position = new Vector3(0.0f, 0.0f, -10.0f);
        Camera.main.orthographicSize = 7.68f;
        SelectHero((HeroEnum)SaveGame.Members.SelectedHero);
        KillKillableObjects();

        ButtonUnlockedText.text = string.Format("UNLOCKS ({0}%)", Mathf.RoundToInt(UnlockedPct * 100));

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
        ActorBase.ResetClosestEnemy();
        //Cursor.visible = false;
        SaveGame.ResetRound();
        PlayerUpgrades.ResetAll();
        ShopItems.ApplyToPlayerUpgrades();
        UpgradeChoices.InitChoices();
        ResetPickups();
        InitXpText();

        CanvasIntro.gameObject.SetActive(false);
        CanvasDead.gameObject.SetActive(false);
        CanvasGame.gameObject.SetActive(true);
        BlackboardScript.DestroyAllCorpses();
        FloorBlood.Clear();
        GameState = State.Playing;
        RoundUnlockCount = 0;
        roundStartTime_ = Time.time;
        PlayerScript.SetPlayerPos(Vector3.zero);
        Orc.SetPosition(Vector3.up * 3, startingGame: true);

        MusicManagerScript.Instance.PlayGameMusic(CurrentGameModeData.Music);
        GameProgressScript.Instance.Begin(GameModeEnum.Undeads);

        //StartCoroutine(Server.Instance.UpdateStat("RoundStarted", 1));
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

    IEnumerator ServerColdStart()
    {
        //yield return StartCoroutine(Server.Instance.DoServerColdStart());
        // Update SaveGame stats with server stats. They could have been 100% local but
        // since I didn't do that from the start I have to get the stats from the server
        // at least once. So just do it every time, then it also works if local data is
        // deleted.
        int statValue;
        //if (Server.Instance.TryGetStat("OrcsSaved", out statValue))
        //    SaveGame.Members.OrcsSaved = statValue;

        //if (Server.Instance.TryGetStat("Kills", out statValue))
        //    SaveGame.Members.EnemiesKilled = statValue;

        //if (Server.Instance.TryGetStat("RoundStarted", out statValue))
        //    SaveGame.Members.PlayerDeaths = statValue;

        //if (Server.Instance.TryGetStat("RoundSeconds", out statValue))
        //    SaveGame.Members.SecondsPlayed = statValue;
        yield return null;
    }

    KeyCode[] Code = new KeyCode[] { KeyCode.R, KeyCode.E, KeyCode.S, KeyCode.E, KeyCode.T, KeyCode.A, KeyCode.L, KeyCode.L };

    int codeIdx = 0;

    void LateUpdate()
    {
        PruneDeadEnemies();
        ActorBase.ResetClosestEnemy();
    }

    void Update()
    {
        TimeSinceStartup = Time.realtimeSinceStartup;
        if (!PauseGameTime)
            GameTime += Time.deltaTime;

        if (Input.GetKey(Code[codeIdx]))
        {
            codeIdx++;
            if (codeIdx == Code.Length)
            {
                codeIdx = 0;
                SaveGame.ResetAll();
                SaveGame.Save();
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

        ThrowPickups(AutoPickUpType.Xp, actor.transform.position, amount: 1, value: actor.XpValue, forceScale: 0.01f);
    }

    private void OnFirstOrcPickup()
    {
        PlayerScript.OnInitialPickup(WeaponType.Machinegun);
    }

    public void AddXp(int amount)
    {
        currentXp_ += amount;
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
            pickup.GetComponent<AutoPickUpScript>().Throw(UnityEngine.Random.insideUnitCircle, forceScale);
            if (pickupType == AutoPickUpType.Xp)
            {
                float xpValue = value * PlayerUpgrades.Data.XpValueMul;
                float xpToColorScale = 2.0f;
                int colorIdx = Mathf.Min(xpColors.Length, (int)(xpValue / xpToColorScale));
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

        if (PlayerUpgrades.Data.OrcJedisEnabled)
        {
            Orc.SetYoda();
        }

        AudioManager.Instance.PlayClipWithRandomPitch(AudioManager.Instance.AudioData.OrcPickup);

        // TODO: playerUpgrades
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
        Debug.Log("NOT IMPLEMENTED");
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
        Instance = this;
        Application.targetFrameRate = 60;
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

        ArenaBounds = new Rect(-halfX, -halfY, halfX * 2, halfY * 2);

        TextFps.enabled = false;

        ShopItems.CreateItemGoList(ShopItemProto, ShopItemsRoot);
    }

    private void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US"); SaveGame.Load();

        MusicManagerScript.Instance.SetVolume(SaveGame.Members.VolumeMusic);
        AudioManager.Instance.SetVolume(SaveGame.Members.VolumeSfx);

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Confined;
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
        float halfH = sprite == null ? 0.0f : sprite.bounds.extents.y / 2;
        float halfW = sprite == null ? 0.0f : sprite.bounds.extents.x / 2;
        pos.x = Mathf.Clamp(pos.x, ArenaBounds.xMin + halfW, ArenaBounds.xMax - halfW);
        pos.y = Mathf.Clamp(pos.y, ArenaBounds.yMin + halfH * 2, ArenaBounds.yMax - halfH); // Pivot is at head so * 2 for bottom
        return pos;
    }

    void PruneDeadEnemies()
    {
        for (int i = BlackboardScript.DeadEnemies.Count - 1; i >= 0; --i)
            RegisterEnemyDied(BlackboardScript.DeadEnemies[i]);
    }

    void OnGUI()
    {
        //return;
        SetDebugOutput("OnGUI enabled", Time.time);

        if (DebugValues.Count == 0)
            return;

        float y = 50.0f;
        GUI.contentColor = Color.white;
        foreach (var pair in DebugValues)
        {
            GUI.Label(new Rect(10, y, 1000, 20), string.Format("{0} = {1}", pair.Key, pair.Value));
            y += 20;
        }
    }
}
