using Assets.Script;
using Assets.Script.Enemies;
using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameModeEnum { Undeads };

public class GameManager : MonoBehaviour
{
    public enum State { None, Idle_PresentLevel, Idle_Fighting, Idle_WonFight, Idle_OutOfTime };

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
    public GameInfoViewer TextGameInfoViewer;
    public Text TextRoundKills;
    public Text TextRoundGold;
    public Text TextUser;
    public Text TextShopMoney;
    public Text TextFps;
    public TextMeshProUGUI TextGo;
    public Text TextClock;
    public SpriteRenderer Floor;
    public SpriteRenderer FloorFilter;
    Color floorDefaultColor;
    public Button ButtonPlay;
    public TextMeshProUGUI ButtonRefundAmount;
    public string ColorLocked;
    public string ColorUnlocked;
    public Transform ShopItemsRoot;
    public Canvas CanvasGameOverDefault;
    public HpBarScript HpBarScript;
    public GameModeEnum GameMode;

    public ParticleSystem FlyingBlood;
    public ParticleSystem BloodDrops;
    public ParticleSystem FloorBlood;
    public ParticleSystem PoofClouds;
    public ParticleSystem SpawnPoof;
    public ParticleSystem FlashParticles;
    public ParticleSystem CircleParticles;
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
    public int LayerChest;
    public float TimeSinceStartup;
    public bool PauseGameTime;
    public float GameTime;
    public float GameDeltaTime;
    int livingEnemyCount;

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

    [NonSerialized] public float xpToLevel;
    [NonSerialized] public float currentXp = 0;
    float currentLevel_ = 1;
    float roundStartTime_;

    void EnablePanel(GameObject panel, bool enable)
    {
        // Work-around for Unity SetActive bug (still showing UI components after disable)
        panel.SetActive(enable);
//        panel.transform.localScale = enable ? Vector3.one : Vector3.zero;
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
    }

    IEnumerator ShowInfoText(string text, float delay = 1.0f)
    {
        TextGameInfo.text = text;
        LeanTween.scale(TextGameInfo.gameObject, Vector3.one, 0.2f);
        yield return new WaitForSeconds(delay);
        LeanTween.scale(TextGameInfo.gameObject, Vector3.zero, 0.25f);
    }

    int previousSecondsLeft = 0;
    void ShowSecondsLeft(int seconds)
    {
        if (seconds != previousSecondsLeft)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            TextClock.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            previousSecondsLeft = seconds;
        }
    }

    //  
    IEnumerator GameStateCo()
    {
        while (true)
        {
            GameState = State.Idle_PresentLevel;

            G.D.PlayerScript.StartGame();

            var enemies = EnemySpawner.GetEnemies();
            long totalHitpoints = (long)enemies.Sum(a => a.BaseHp);
            livingEnemyCount = enemies.Count();
            HpBarScript.SetHp(totalHitpoints, totalHitpoints);

            yield return ShowInfoText("ROUND START!");

            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(true);
                yield return null;
            }

            float roundStartTime = G.D.GameTime;
            float roundEndTime = roundStartTime + 60;

            GameState = State.Idle_Fighting;

            while (GameState == State.Idle_Fighting)
            {
                float delta = GameDeltaTime;
                ProjectileManager.Instance.Tick(delta);

                int secondsLeft = (int)(roundEndTime - G.D.GameTime);
                ShowSecondsLeft(secondsLeft);

                if (secondsLeft <= 0)
                    GameState = State.Idle_OutOfTime;

                if (HpBarScript.CurrentHp <= 0)
                    GameState = State.Idle_WonFight;

                yield return null;
            }

            PrepareForNewRound();

            while (GameState == State.Idle_WonFight)
            {
                yield return null;
                GameState = State.Idle_PresentLevel;
            }

            while (GameState == State.Idle_OutOfTime)
            {
                GameState = State.Idle_PresentLevel;

                foreach (var enemy in enemies)
                {
                    enemy.gameObject.SetActive(true);
                    ActorCache.Instance.ReturnObject(enemy.gameObject);
                }

                yield return ShowInfoText("OUT OF TIME", delay: 2);
                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
    }

    public void PlayfabStats()
    {
        var props = new Dictionary<string, object>
        {
            { "score", SaveGame.RoundScore },
            { "level", (int)currentLevel_ },
            { "gold", SaveGame.RoundGold },
            { "kills", SaveGame.RoundKills },
        };
        Playfab.PlayerEvent(Playfab.GameOverEvent, props);

        Rounds++;

        TextGameInfo.text = "";

        float roundTime = GameTime - roundStartTime_;

        int roundSeconds = Mathf.RoundToInt(roundTime);
        SaveGame.Members.TotalSeconds += roundSeconds;

        if (GameTime > SaveGame.Members.MaxSecondsReached)
            SaveGame.Members.MaxSecondsReached = Mathf.RoundToInt(GameTime);

        var dic = new Dictionary<string, int>();
        dic[Playfab.GoldStat] = SaveGame.RoundGold;
        dic[Playfab.LevelStat] = (int)currentLevel_;
        dic[Playfab.KillsStat] = SaveGame.RoundKills;
        dic[Playfab.RoundsCompletedStat] = Rounds;
        dic[Playfab.TotalSeconds] = SaveGame.Members.TotalSeconds;
        dic[Playfab.ChapterBossStartedStat] = SaveGame.Members.Chapter1BossStarted;
        dic[Playfab.ChapterBossKilledStat] = SaveGame.Members.Chapter1BossKilled;
        dic[Playfab.MaxSecondsReached] = SaveGame.Members.MaxSecondsReached;

        Playfab.PlayerStat(dic);

        ProjectileManager.Instance.StopAll();

        CanvasGameOverDefault.enabled = true;

        SaveGame.UpdateFromRound(roundSeconds, reset: true);
        SaveGame.Save();
    }

    static int Rounds = 0;

    public void KillKillableObjects()
    {
        var killables = FindObjectsOfType<MonoBehaviour>().OfType<IKillableObject>();
        foreach (var killable in killables)
        {
            killable.Kill();
        }
    }

    public void KillOnStartGameKillableObjects()
    {
        var killables = FindObjectsOfType<MonoBehaviour>().OfType<IOnStartGameKillableObject>();
        foreach (var killable in killables)
        {
            killable.StartingGame();
        }
    }

    public void ResetGame(bool autoStartGame = false)
    {
        GameTime = 0.0001f;
        GameDeltaTime = 0.0f;

        LeanTween.color(Floor.gameObject, floorDefaultColor, 1.0f);

        Floor.color = floorDefaultColor;
        FloorFilter.color = Color.clear;

        Time.timeScale = 1.0f;
        CameraShaker.Instance.ShakeInstances.Clear();
        Camera.main.transform.parent.position = new Vector3(0.0f, 0.0f, -10.0f);
        Camera.main.orthographicSize = 7.68f;

        CanvasGame.gameObject.SetActive(true);
        PrepareForNewRound();

        StartGame();
    }

    void PrepareForNewRound()
    {
        KillKillableObjects();
        ProjectileManager.Instance.StopAll();
        G.D.PlayerScript.ResetAll();
        BlackboardScript.DestroyAllEnemies();
        ClearParticles();
    }

    public void StartGame()
    {
        KillOnStartGameKillableObjects();
        ActorBase.ResetClosestEnemy();
        SaveGame.ResetRound();

        ResetPickups();

        CanvasGame.gameObject.SetActive(true);
        BlackboardScript.DestroyAllCorpses();
        FloorBlood.Clear();
        RoundUnlockCount = 0;
        roundStartTime_ = Time.time;
        xpToLevel = BaseXpToLevel;
        G.D.PlayerScript.ResetPlayerPos();
        G.D.PlayerScript.StartGame();

        if (PlayerUpgrades.Data.GameStartTime > TimeSpan.Zero)
            GameTime = (float)PlayerUpgrades.Data.GameStartTime.TotalSeconds;

        //MusicManagerScript.Instance.PlayGameMusic(CurrentGameModeData.Music);
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

        // temp
        ThrowPickups(AutoPickUpType.Money, actor.transform.position, 3, value: 1, forceScale: 3.0f);

        if (UnityEngine.Random.value < PlayerUpgrades.Data.DropMoneyOnKillChance)
        {
            int amount = UnityEngine.Random.Range(PlayerUpgrades.Data.DropMoneyOnKillMin, PlayerUpgrades.Data.DropMoneyOnKillMax + 1);
            ThrowPickups(AutoPickUpType.Money, actor.transform.position, amount, value: 1, forceScale: 1.0f);
        }
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
                float xpToColorScale = 4.0f;
                int colorIdx = Mathf.Min(xpColors.Length - 1, (int)(xpValue / xpToColorScale));
                pickup.GetComponent<SpriteRenderer>().color = xpColors[colorIdx];
            }
            pickup.SetActive(true);
        }
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
        int rangeFrom = Mathf.RoundToInt(2 * amount);
        if (rangeFrom > 8)
            rangeFrom = 8;

        FlyingBlood.Emit(UnityEngine.Random.Range(rangeFrom, rangeFrom + 2));

        BloodDrops.transform.position = pos;
        BloodDrops.Emit(Mathf.RoundToInt(1 + (0.1f * amount)));

        if (UnityEngine.Random.value <= floorBloodRnd)
        {
            int bloodAmount = Mathf.RoundToInt(1 + (0.1f * amount));
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
        if (amount < 1)
            amount = 1;

        bool isCrit = UnityEngine.Random.value < PlayerUpgrades.Data.BaseCritChance * PlayerUpgrades.Data.CritChanceMul;
        if (isCrit)
            amount *= PlayerUpgrades.Data.CritValueMul;

        long intAmount = (long)amount;
        if (intAmount > enemy.Hp)
            intAmount = enemy.Hp;

        long overkill = enemy.Hp - intAmount;
        bool isDead = enemy.IsDead;
        enemy.ApplyDamage(intAmount, direction, forceModifier);
        bool wasKilled = !isDead && enemy.Hp <= 0;
        if (wasKilled)
        {
            if (--livingEnemyCount == 0)
            {
                ThrowPickups(AutoPickUpType.Money, enemy.transform.position, amount: 20, value: 1, forceScale: 2.0f);
            }
        }
        HpBarScript.AddHp((long)-intAmount);
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
        return !sizeAdjustedBounds.Contains(pos);
    }

    public Vector3 ClampToBounds(Vector3 pos, float margin)
    {
        pos.x = Mathf.Clamp(pos.x, ArenaBounds.xMin + margin, ArenaBounds.xMax - margin);
        pos.y = Mathf.Clamp(pos.y, ArenaBounds.yMin + margin, ArenaBounds.yMax - margin);
        return pos;
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
        SetDebugOutput("speed", PlayerUpgrades.Data.TimeScale);
        SetDebugOutput("start", PlayerUpgrades.Data.GameStartTime);

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

    void Awake()
    {
        TextGameInfo.text = "";
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

        var bounds = GetComponent<BoxCollider2D>();

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
    }

    private void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        SaveGame.Load();

        MusicManagerScript.Instance.SetVolume(SaveGame.Members.VolumeMusic * SaveGame.Members.VolumeMaster);
        AudioManager.Instance.SetVolume(SaveGame.Members.VolumeSfx * SaveGame.Members.VolumeMaster);

        ResetGame(autoStartGame: true);
        StartCoroutine(GameStateCo());
    }

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

            GameDeltaTime = Math.Min(0.1f, Time.deltaTime * PlayerUpgrades.Data.TimeScale);
            GameTime += GameDeltaTime;
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
}
