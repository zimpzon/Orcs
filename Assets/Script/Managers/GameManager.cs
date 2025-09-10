using Assets.Script;
using Assets.Script.Misc;
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
    public const int MajorVersion = 0;

    // 1: added versions
    // 2: added mystery bonus
    // 3: radial progress bar mystery counters
    // 4: added Necromancer
    // 5: started ascension
    // 6: percentage upgrades added
    // 7: ascend almost done
    // 8: export added, diamonds gives +10%
    // 9: rebirth can be enabled
    // 10: import/export
    // 11: Rebirth released
    // 12: new tiers
    // 13: new colors
    // 14: skins
    // 15: monster credits much more expensive
    // 16: new enemies
    // 17: toggle damage + gold numbers
    // 18: added bestiary
    // 19: bonus income for unlocked beasts
    // 20: new save method
    // 21: save gone?
    // 22: reverted to old save type
    // 23: now using both local storage and playerprefs for save games
    // 24: named monsters
    // 25: enemies, skin, upgrade tier
    // 26: overflow bug
    // 27: two new enemies
    // 28: new tier + skins
    // 29: four new skins
    // 30: fixes
    // 31: three new enemies, halved beast unlock xp
    // 32: bugfix: mystery and chest collections did not carry over rebirth
    // 33: fixed bug where Arena has gotten way too easy
    // 34: increased high level upgrade prices
    // 35: arena a bit harder, again
    // 36: style changes
    // 37: X2 bonuses
    // 38: 3 new skins
    // 39: updated away check
    // 40: refactored Decimal256 to use double instead of decimal for temp values
    // 41: extended to 512bit plus some long to doubles
    // 42: title + probably bugfix for coins hanging over head (overflow value in cast to int)
    // 43: a lot of new stats
    // 44: fixed gold per knife scale bug + nerf
    // 45: minor improvements
    // 46: three new skins
    // 47: rebirth timer, colors
    // 48: new skins
    // 49: faster arena rebirth card
    // 50: scary Earl skin for diamonds
    // 51: Arena super jump on fast clear
    // 52: New upgrade tier
    // 53: Two new skins
    // 53: New skins
    // 54: Skin Gangster Earl
    // 55: Added buy multiple buttons
    // 56: Added new enemy
    // 57: Added scientific notation
    public const int MinorVersion = 57;

    public enum State { None, Idle_Starting_Game, Idle_PresentLevel, Idle_Fighting, Idle_WonFight, Idle_OutOfTime, Idle_RestartRound };

    const float BaseXpToLevel = 14;
    const int RoundTimeSeconds = 30;
    const int AutoSaveInterval = 5;
    const int SendStatsInterval = 60 * 10;
    const float MoneyUpdateDelay = 0.02f;

    public string GameVersion;
    public static GameManager Instance;
    public bool UnlockAllGameModes;
    public bool UnlockAllWeapons;
    public bool UnlockAllHeroes;

    public Color[] xpColors = new Color[] { };
    public TMP_FontAsset FontDefaultFloatingText;
    public TMP_FontAsset FontTarragon;

    public BoxCollider2D ArenaBoundsCollider;
    public Vector2 ArenaCenter => ArenaBoundsCollider.bounds.center;
    public LeanTween Tween;
    public Text TextFps;
    public TextMeshProUGUI TextGameInfo;
    public TextMeshProUGUI TextClock;
    public TextMeshProUGUI TextLevel;
    public TextMeshProUGUI TextMoney;
    public TextMeshProUGUI TextPassiveIncome;
    public TextMeshProUGUI TextVersion;

    public TextMeshProUGUI TextTotalIncome;
    public TextMeshProUGUI TextTotalKilled;
    public TextMeshProUGUI TextTimeThisSession;

    public Color ColorDamageNumbersDagger = Color.red;
    public Color ColorDamageNumbersZap = Color.red;
    public Color ColorGoldCollect = Color.yellow;

    public SpriteRenderer Floor;
    Color floorDefaultColor;
    public string ColorLocked;
    public string ColorUnlocked;
    public HpBarScript HpBarScript;
    public GameModeEnum GameMode;

    public Button ButtonQuit;
    public ParticleSystem FlyingBlood;
    public ParticleSystem BloodDrops;
    public ParticleSystem FloorBlood;
    public ParticleSystem PoofClouds;
    public ParticleSystem SpawnPoof;
    public ParticleSystem FlashParticles;
    public ParticleSystem CircleParticles;
    public GrenadeScript Grenade;
    public int SortLayerTopEffects;
    public State GameState = State.Idle_Starting_Game;
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
    public float ArenaScale => ArenaRoot.transform.localScale.x;
    public GameObject ArenaRoot;
    public int CurrentRound = 1;
    public int MaxRound = 10;
    public Decimal512 TotalPassiveIncome;
    [NonSerialized] public static List<string> GameLoadInfo = new List<string>();

    int livingEnemyCount;

    [NonSerialized] public GameModeData LatestGameModeData = new();
    [NonSerialized] public GameModeData CurrentGameModeData;
    public GameModeData GameModeDataNursery = new();
    public GameModeData GameModeDataEarth = new();
    public GameModeData GameModeDataWind = new();
    public GameModeData GameModeDataFire = new();
    public GameModeData GameModeDataStorm = new();
    public GameModeData GameModeDataHarmony = new();

    public int SpriteFlashParamId;
    public int SpriteFlashColorParamId;
    public static Rect ArenaBounds = new();
    public static Rect TopRect = new();
    public static Rect BottomRect = new();
    public static Rect LeftRect = new();
    public static Rect RightRect = new();
    [NonSerialized] public float TextUnlockBasePos;

    static Dictionary<string, string> DebugValues = new();

    public Button ButtonBuy1;
    public Button ButtonBuy10;
    public Button ButtonBuy100;

    [NonSerialized] public int BuyAmount = 1;

    [NonSerialized] public float UnlockedPct;
    [NonSerialized] public int RoundUnlockCount;
    [NonSerialized] public double xpToLevel;
    [NonSerialized] public double currentXp = 0;
    [NonSerialized] private DateTime? _timeStartSessionUtc = null;

    public void ResetAllProgress(bool ascend = false)
    {
        if (ascend)
        {
            SaveGameAscend.AscendSaveGame();
        }
        else
        {
            SaveGame.Members = new SaveGameMembers();
        }

        GameState = State.Idle_RestartRound;

        PrepareForNewRound();
        KillKillOnSaveWipeObjects();
        GameEvents.RaiseSaveWiped(ascend ? GameEvents.SaveWipeReason.Ascended : GameEvents.SaveWipeReason.UserWipe);

        ScrollToTopOnStart.Instance.ScrollToTopNow();

        SaveGame.Members.SaveKillSwitch_CanSave = true;
        SaveGame.Save();

        SetBuyMultiple(1);
    }

    public IEnumerator ShowInfoTextFlashy(string text, float delay = 1.0f)
    {
        var go = TextGameInfo.gameObject;
        var canvasGroup = go.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = go.AddComponent<CanvasGroup>();

        TextGameInfo.text = text;
        canvasGroup.alpha = 1;
        go.transform.localScale = Vector3.zero;

        LeanTween.cancel(go);

        // Pop in with punch-like bounce
        LeanTween.scale(go, Vector3.one * 1.2f, 0.1f)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                LeanTween.scale(go, Vector3.one, 0.1f).setEaseInQuad();
            });

        yield return new WaitForSeconds(delay);

        // Fade out and scale down
        LeanTween.alphaCanvas(canvasGroup, 0f, 0.3f);
        LeanTween.scale(go, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack);
        yield return new WaitForSeconds(0.35f);
    }

    IEnumerator ShowInfoText(string text, float delay = 1.0f)
    {
        TextGameInfo.text = text;
        LeanTween.scale(TextGameInfo.gameObject, Vector3.one, 0.2f);
        yield return new WaitForSeconds(delay);
        LeanTween.scale(TextGameInfo.gameObject, Vector3.zero, 0.25f);
    }

    int _secondsLeft = 0;
    int _previousSecondsLeft = 0;

    void ShowSecondsLeft(int seconds)
    {
        if (seconds != _previousSecondsLeft)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            TextClock.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            _previousSecondsLeft = seconds;
            _secondsLeft = seconds;
        }
    }

    long _roundTotalHp = -1;

    private bool TryZapEnemy(Vector2 from, ActorBase enemy, ActorDamageSource damageSource)
    {
        if (PlayerUpgrades.Data.EffectiveZapDamage == 0)
            return false;

        if (enemy is null)
            return false;

        bool success = Zapper.TryZapEnemy(from, enemy, PlayerUpgrades.Data.EffectiveZapDamage, damageSource);
        if (!success)
            return false;

        MakeFlash(from, 2.0f);
        MakePoof(from, 3, 0.5f);

        for (int i = 0; i < 10; ++i)
        {
            Particles.I.ClickTrail.transform.position = from + UnityEngine.Random.insideUnitCircle * 0.5f;
            Particles.I.ClickTrail.Emit(1);
        }

        return true;
    }

    const float ZapInterval = 3;
    float _nextZap;
    public List<ActorBase> ZapTargetIgnoreList = new List<ActorBase>();

    void ShakeArenaBackground()
    {
        RectTransform rt = ArenaBoundsCollider.GetComponent<RectTransform>();
        Vector3 originalPos = rt.anchoredPosition;

        LeanTween.value(rt.gameObject, 0f, 1f, time: 0.1f)
            .setOnUpdate((float val) =>
            {
                float shakeStrength = 0.1f;
                rt.anchoredPosition = originalPos + (Vector3)UnityEngine.Random.insideUnitCircle * shakeStrength;
            })
            .setOnComplete(() =>
            {
                rt.anchoredPosition = originalPos;
            });
    }

    void CheckZapping(ActorDamageSource damageSource)
    {
        if (PlayerUpgrades.Data.EffectiveZapDamage > 0 && G.D.GameTime > _nextZap)
        {
            var firstTarget = ActorBase.PlayerClosestEnemyActor;
            if (firstTarget != null)
            {
                var direction = (firstTarget.transform.position - G.D.PlayerPos).normalized;
                G.D.PlayerScript.AddForce(-direction * 0.5f);

                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, volumeScale: 1.0f, pitch: 1.0f);
                ShakeArenaBackground();

                _nextZap = G.D.GameTime + ZapInterval;
                StartCoroutine(ZapChainCoroutine(firstTarget, damageSource));
            }
        }
    }

    IEnumerator ZapChainCoroutine(ActorBase firstTarget, ActorDamageSource damageSource)
    {
        ZapTargetIgnoreList.Clear();

        if (!TryZapEnemy(G.D.PlayerPos, firstTarget, damageSource))
            yield break;

        ZapTargetIgnoreList.Add(firstTarget);
        var prevEnemy = firstTarget;

        const int MaxJumps = 3;
        const float JumpDelay = 0.001f;

        for (int i = 0; i < MaxJumps; ++i)
        {
            yield return new WaitForSeconds(JumpDelay);

            var nextEnemy = BlackboardScript.GetClosestEnemy(
                prevEnemy.transform.position,
                radius: 4.0f,
                ZapTargetIgnoreList);

            if (!TryZapEnemy(prevEnemy.transform.position, nextEnemy, damageSource))
                break;

            GameManager.Instance.ShakeCamera(1.0f);

            ZapTargetIgnoreList.Add(nextEnemy);
            prevEnemy = nextEnemy;
        }
    }

    void UpdateBeastsSeen(List<ActorBase> actors)
    {
        foreach (var actor in actors)
        {
            if (!SaveGame.Members.BeastsSeen.Contains(actor.ActorType))
            {
                // New beast
                SaveGame.Members.BeastsSeen.Add(actor.ActorType);
                TitleTextScript.Instance.UdateTitle();
            }
        }
    }

    // main loop
    IEnumerator GameStateCo()
    {
        GameCanvasScript.Instance.ShowPopup(
            "<color=yellow>Welcome to Idle Earl Earl'y Access</color>\n<size=-3><color=#c0c0d0>Game is saved every 5 sec</color></size>\n\n" +
            "<size=-2>Recent updates:\n<size=-3><color=#d0d0e0>" +
            " - new enemy\n" +
            " - buy multiple buttons\n" +
            " - new skins");

        Decimal512 v1 = 1_234_456;
        Decimal512 v2 = 5_000_000;
        Decimal512 v3 = 2_100_000;

        string s1 = Format512.Format(v1);
        string s2 = Format512.Format(v2);
        string s3 = Format512.Format(v3);

        string s4 = Format512.FormatWithDecimals(v1);
        string s5 = Format512.FormatWithDecimals(v2);
        string s6 = Format512.FormatWithDecimals(v3);

        string s7 = Format512.FormatWithDecimals(v1, alwaysThreeDecimalsForLargeNumbers: true);
        string s8 = Format512.FormatWithDecimals(v2, alwaysThreeDecimalsForLargeNumbers: true);
        string s9 = Format512.FormatWithDecimals(v3, alwaysThreeDecimalsForLargeNumbers: true);

        _timeStartSessionUtc = DateTime.UtcNow;

        Playfab.Login();

        while (true)
        {
            PrepareForNewRound();

            UpdateMoneyText();

            TextLevel.text = $"ARENA {SaveGame.Members.ArenaLevel}";
            GameState = State.Idle_PresentLevel;

            G.D.PlayerScript.StartGame();
            ShowSecondsLeft(RoundTimeSeconds);

            var enemies = EnemySpawner.GetEnemies(SaveGame.Members.ArenaLevel).ToList();
            UpdateBeastsSeen(enemies);

            long totalHitpoints = enemies.Sum(a => a.BaseHp);
            livingEnemyCount = enemies.Count();
            HpBarScript.SetHp(totalHitpoints, totalHitpoints);
            _roundTotalHp = totalHitpoints;

            AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.NewRound);
            yield return ShowInfoTextFlashy("ROUND START!");
            if (GameState == State.Idle_RestartRound)
                continue;

            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(true);
                yield return null;
            }

            float roundStartTime = G.D.GameTime;
            float roundEndTime = roundStartTime + RoundTimeSeconds;

            GameState = State.Idle_Fighting;
            _nextZap = G.D.GameTime + ZapInterval;

            ResetAwayTimestamp();

            while (GameState == State.Idle_Fighting)
            {
                if (RestartRoundIfWasAway())
                {
                    // Probably throttled by browser.
                    GameState = State.Idle_RestartRound;
                    continue;
                }

                CheckZapping(ActorDamageSource.ChainZap);

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

            if (GameState == State.Idle_RestartRound)
            {
                // Nothing to do here, we just restart same round.
            }

            if (GameState == State.Idle_OutOfTime)
            {
                // Killing last enemy will throw round gold, but on timeout that doesn't happen, so do it now.
                // It must be before PrepareForNewRound since that resets round results.
                PresentRoundGold(ArenaCenter);
            }

            PrepareForNewRound();

            SaveGame.Members.TotalArenas++;

            long arenaStep = SaveGame.Members.BoughtFasterArena ? 5L : 1L;
            int secondsLeftAtRoundEnd = (int)(roundEndTime - G.D.GameTime);
            // If faster Arena and >= 20 seconds left take a big jump.
            if (secondsLeftAtRoundEnd >= 20 && SaveGame.Members.BoughtFasterArena && GameState == State.Idle_WonFight)
            {
                arenaStep = 25;
                Vector2 superStepPos = new Vector2(ArenaBounds.center.x, ArenaBounds.center.y);
                FloatingTextSpawner.Instance.Spawn(
                    superStepPos,
                    $"<size=+3>Super fast Arena clear! Making super jump!",
                    new Color(0.8f, 0.8f, 0.8f),
                    speed: 0.05f,
                    timeToLive: 3.0f,
                    fadeTime: 0.5f,
                    fontStyle: TMPro.FontStyles.Bold,
                    FontTarragon);
            }

            if (GameState == State.Idle_WonFight)
            {
                // Last enemy already threw round gold, NOT done here.
                SaveGame.Members.ArenaLevel += arenaStep;
                
                if (arenaStep > 1)
                {
                    // snap to nearest multiple of 5 (rounding down).
                    SaveGame.Members.ArenaLevel -= SaveGame.Members.ArenaLevel % arenaStep;
                    if (SaveGame.Members.ArenaLevel < 1)
                        SaveGame.Members.ArenaLevel = 1;
                }

                SaveGame.Members.TotalArenasWon++;
                SaveGame.Members.MaxArena = Math.Max(SaveGame.Members.ArenaLevel, SaveGame.Members.MaxArena);

                yield return ShowInfoTextFlashy($"ARENA {SaveGame.Members.ArenaLevel}", delay: 1);
                yield return new WaitForSeconds(0.25f);
                yield return null;
            }

            if (GameState == State.Idle_OutOfTime)
            {
                yield return ShowInfoTextFlashy("OUT OF TIME", delay: 1);
                yield return new WaitForSeconds(0.5f);

                // Go back one arena level on timeout
                if (SaveGame.Members.ArenaLevel > 1)
                    SaveGame.Members.ArenaLevel -= arenaStep;

                if (SaveGame.Members.ArenaLevel < 1)
                    SaveGame.Members.ArenaLevel = 1;
            }

            GameState = State.Idle_PresentLevel;

            yield return null;
        }
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

    public void KillKillOnSaveWipeObjects()
    {
        var killables = FindObjectsOfType<MonoBehaviour>().OfType<IKillOnSaveWipe>();
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
        GameDeltaTime = 0.001f;

        LeanTween.color(Floor.gameObject, floorDefaultColor, 1.0f);

        Floor.color = floorDefaultColor;

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
        //KillKillableObjects();
        ProjectileManager.Instance.StopAll();
        G.D.PlayerScript.ResetAll();
        BlackboardScript.DestroyAllEnemies();
        ClearParticles();
    }

    public void StartGame()
    {
        KillOnStartGameKillableObjects();
        ActorBase.ResetClosestEnemy();

        ResetPickups();

        CanvasGame.gameObject.SetActive(true);
        BlackboardScript.DestroyAllCorpses();
        FloorBlood.Clear();
        RoundUnlockCount = 0;
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

    public void AddGold(bool isLargeCoin, Decimal512 value)
    {
        Decimal512 moneyAdded = value;
        SaveGame.Members.TotalIncomeArena += moneyAdded;

        AddMoney(moneyAdded);

        Vector2 playerPos = G.D.PlayerPos;
        Vector2 textPos = playerPos + Vector2.up * 0.75f + RndUtil.RandomInsideUnitCircle();

        if (SaveGame.Members.ShowFloatingGoldNumbers)
        {
            Decimal512 displayMoney = moneyAdded;
            FloatingTextSpawner.Instance.Spawn(
                textPos,
                $"${Format512.Format(displayMoney)}",
                ColorGoldCollect,
                speed: 2.0f,
                timeToLive: 1.0f,
                fontStyle: TMPro.FontStyles.Bold);
        }
    }

    public void AddMoney(Decimal512 amount)
    {
        SaveGame.Members.Money += amount;

        if (SaveGame.Members.Money > SaveGame.Members.MaxMoney)
            SaveGame.Members.MaxMoney = SaveGame.Members.Money;
    }

    public void DeductMoney(Decimal512 amount)
    {
        SaveGame.Members.Money -= amount;
    }

    public void AddXp(double amount)
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

    public void ThrowPickups(AutoPickUpType pickupType, Vector2 pos, int amount, double value, float forceScale = 1.0f, bool isLargeCoin = false)
    {
        for (int i = 0; i < amount; ++i)
        {
            var pickup = PickUpManagerScript.Instance.GetPickUpFromCache(pickupType);
            pickup.transform.position = pos;

            var pickupScript = pickup.GetComponent<AutoPickUpScript>();
            pickupScript.Value = value;
            pickupScript.Throw(UnityEngine.Random.insideUnitCircle, forceScale, isLargeCoin);

            if (pickupType == AutoPickUpType.Xp)
            {
                throw new NotImplementedException();
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
        pos.z = 0;
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
        if (!PositionUtility.IsPointInsideArena(pos))
            return;

        FlyingBlood.transform.position = pos;
        FlyingBlood.Emit(1);

        BloodDrops.transform.position = pos;
        BloodDrops.Emit(1);

        FloorBlood.transform.position = pos;
        FloorBlood.Emit(1);
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

    long CalculateRoundCompleteGold(int totalSeconds, int secondsLeft, long totalDamage)
    {
        // This directly affects how much gold arena makes. On top we have dagger throws, which usually does more.
        const double HpToGoldPct = 0.25 / EnemySpawner.HpScale;

        // Faster completion means more gold. But we will end at lowest (0.75%) all the time since
        // time will almost always be used up at higher levels.
        float timeFraction = (secondsLeft + 0.0001f) / totalSeconds;
        float penaltyMul = 0.75f + 0.75f * timeFraction; // Linear scaling
        long goldWon = (long)Math.Ceiling(totalDamage * penaltyMul * HpToGoldPct);
        if (goldWon <= 0) goldWon = 1;
        return (long)(goldWon * PlayerUpgrades.Data.MoneyPerGold);
    }

    void PresentRoundGold(Vector2 position)
    {
        long damageDone = _roundTotalHp - HpBarScript.CurrentHp;

        long goldWon = CalculateRoundCompleteGold(RoundTimeSeconds, _secondsLeft, damageDone);
        Vector2 endRoundGoldSummaryPos = new Vector2(ArenaBounds.center.x, ArenaBounds.center.y - 4);

        bool knifeThrowGoldEnabled = SaveGame.Members.LevelGoldPerKnifeThrown > 0;
        if (knifeThrowGoldEnabled)
        {
            // bonus = DaggerDamage * daggersThrown * goldPerDagger
            long knifeThrownBonus = (long)(
                PlayerUpgrades.Data.MagicMissileEffectiveDamage *
                G.D.PlayerScript.DaggersThrown *
                PlayerUpgrades.Data.GoldPerKnifeThrown *
                PlayerUpgrades.Data.MoneyPerGold);

            if (knifeThrownBonus > 0)
            {
                SaveGame.Members.TotalIncomeKnifeThrow += knifeThrownBonus;

                FloatingTextSpawner.Instance.Spawn(
                    endRoundGoldSummaryPos + Vector2.down * 0.7f,
                    $"<size=+2>Dagger throws: +<color=#8DBE4C>{Format512.Format(knifeThrownBonus)}</color> gold",
                    new Color(0.8f, 0.8f, 0.8f),
                    speed: 0.05f,
                    timeToLive: 5.0f,
                    fadeTime: 0.5f,
                    fontStyle: TMPro.FontStyles.Bold,
                    FontTarragon);

                ThrowGoldSplit(knifeThrownBonus, position);
            }
        }

        ThrowGoldSplit(goldWon, position);

        long secondsSpent = RoundTimeSeconds - _secondsLeft;
        long dps = (long)(damageDone / (double)secondsSpent);
        SaveGame.Members.MaxDps = Math.Max(dps, SaveGame.Members.MaxDps);

        // Happens after user was away for a while and we manually reset round.
        if (goldWon <= 0 || damageDone <= 0 || secondsSpent <= 0)
            return;

        FloatingTextSpawner.Instance.Spawn(
            endRoundGoldSummaryPos,
            $"<color=#F0F0F0><size=+2><color=#8DBE4C>{Format512.Format(damageDone)}</color> dmg in <color=#8DBE4C>{secondsSpent}</color> sec (<color=#8DBE4C>{Format512.Format(dps)}</color> dps), +<color=#8DBE4C>{Format512.Format(goldWon)}</color> gold",
            new Color(0.8f, 0.8f, 0.8f),
            speed: 0.05f,
            timeToLive: 5.0f,
            fadeTime: 0.5f,
            fontStyle: TMPro.FontStyles.Bold,
            FontTarragon);
    }

    void OnLastEnemyKilled(ActorBase lastEnemy)
    {
        HpBarScript.SetHp(0, HpBarScript.MaxHp);
        PresentRoundGold(lastEnemy.transform.position);
    }

    public void OnEnemyKill(ActorBase actor)
    {
        long value = (long)Math.Ceiling(SaveGame.Members.ArenaLevel * PlayerUpgrades.Data.MoneyPerGold);
        ThrowPickups(AutoPickUpType.Money, actor.transform.position, amount: 1, value, isLargeCoin: false);

        if (UnityEngine.Random.value > 0.75)
        {
            float pitch = 1.6f + UnityEngine.Random.value * 0.2f;
            float volume = 0.7f + UnityEngine.Random.value * 0.1f;
            AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.EnemyDie, volume, pitch);
        }

        SaveGame.Members.EnemiesKilled += 1;

        bool wasLastEnemy = --livingEnemyCount == 0;
        if (wasLastEnemy)
        {
            OnLastEnemyKilled(actor);
        }
    }

    void ThrowGoldSplit(long goldWon, Vector3 position, bool isLargeCoin = true)
    {
        if (goldWon <= 0)
            return;

        // Smooth scale: 1 coin at low amounts, 10 at 1000 or more
        int coinCount = Mathf.Clamp(Mathf.RoundToInt(goldWon / 50f), 1, 10);

        long baseValue = goldWon / coinCount;
        long remainder = goldWon % coinCount;

        for (int i = 0; i < coinCount; i++)
        {
            long value = baseValue + (i < remainder ? 1 : 0);
            ThrowPickups(AutoPickUpType.Money, position, amount: 1, value, forceScale: 4.0f, isLargeCoin);
        }
    }

    void AddToTotalDamage(ActorDamageSource damageSource, long damage)
    {
        switch (damageSource)
        {
            case ActorDamageSource.ChainZap:
                SaveGame.Members.TotalDamageChainZap += damage;
                break;

            case ActorDamageSource.DaggerThrow:
                SaveGame.Members.TotalDamageDaggerThrow += damage;
                break;

            case ActorDamageSource.WitchDoctor:
                SaveGame.Members.TotalDamageWitchDoctor += damage;
                break;

            case ActorDamageSource.Wizard:
                SaveGame.Members.TotalDamageWizard += damage;
                break;

            case ActorDamageSource.Necromancer:
                SaveGame.Members.TotalDamageNecromancer += damage;
                break;

            default:
                throw new Exception($"DamageSource {damageSource} cannot be added to any known total");
        }
    }

    private Color GetDamageColorFromSource(ActorDamageSource source)
    {
        return source switch
        {
            ActorDamageSource.ChainZap or ActorDamageSource.WitchDoctor or ActorDamageSource.Wizard => ColorDamageNumbersZap,
            _ => ColorDamageNumbersDagger,
        };
    }

    public void DamageEnemy(ActorBase enemy, double amount, Vector3 direction, float forceModifier, ActorDamageSource damageSource)
    {
        if (enemy.Hp <= 0 || enemy.IsDead)
            return;

        direction = direction.normalized;

        amount *= PlayerUpgrades.Data.DamageMul;
        if (amount < 1)
            amount = 1;

        long intAmount = (long)amount;
        SaveGame.Members.DamageDone += intAmount;

        // Diplay the full damager number, without truncating to enemy health.
        if (SaveGame.Members.ShowFloatingDamageNumbers)
        {
            Vector2 randomTextOffset = UnityEngine.Random.insideUnitCircle * 1.0f;
            FloatingTextSpawner.Instance.Spawn(
                (Vector2)enemy.transform.position + Vector2.up * 1.0f + randomTextOffset,
                $"-{Format64.Format(intAmount)}",
                GetDamageColorFromSource(damageSource),
                speed: 0.75f,
                timeToLive: 1.0f,
                fontStyle: TMPro.FontStyles.Bold);
        }

        // Now truncate to enemy health.
        if (intAmount > enemy.Hp)
            intAmount = enemy.Hp;

        // Count total without overkill
        AddToTotalDamage(damageSource, intAmount);

        enemy.ApplyDamage2(intAmount, direction, forceModifier * 1.5f);

        HpBarScript.AddHp(-intAmount);

        MakeCircle(enemy.transform.position, 1.0f);

        // PWE: Colossal hack for now. HP does often not end at 0 when all enemies are dead...
        if (HpBarScript.CurrentHp < 0)
        {
            HpBarScript.SetHp(0, HpBarScript.MaxHp);
        }
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

    public Vector3 ClampToBounds(Vector3 pos, float margin = 0.5f)
    {
        pos.x = Mathf.Clamp(pos.x, ArenaBounds.xMin + margin, ArenaBounds.xMax - margin);
        pos.y = Mathf.Clamp(pos.y, ArenaBounds.yMin + margin, ArenaBounds.yMax - margin);
        return pos;
    }

    public Vector3 ClampToBounds(Vector3 pos, Sprite sprite, float margin = 0.5f)
    {
        float halfH = sprite == null ? 0.0f : sprite.bounds.extents.y + margin;
        float halfW = sprite == null ? 0.0f : sprite.bounds.extents.x + margin;
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

    bool _firstSaveGameLoadComplete = false;

    public void OnQuit()
    {
        SaveGame.Save();
        Application.Quit();
    }

    void Awake()
    {
        //bool showQuitButton = Application.platform != RuntimePlatform.WebGLPlayer;
        //ButtonQuit.gameObject.SetActive(showQuitButton);

        if (GetPlayFabStats().Count > 25)
            throw new InvalidOperationException($"Too many PlayFab stats! Max 25, actual: {GetPlayFabStats().Count}");

        TextVersion.text = $"V {MajorVersion}.{MinorVersion}";
        SaveGame.Load();
        _firstSaveGameLoadComplete = true;

        TextGameInfo.text = "";

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

        //float arenaHeight = bounds.size.y;
        //float arenaWidth = arenaHeight * AspectUtility.WantedAspectRatio;
        //float halfX = arenaWidth / 2;
        //float halfY = arenaHeight / 2;

        //const float Size = 2;
        //ArenaBounds = new Rect(-halfX + 0.5f, -halfY + 0.35f, halfX * 2 - 1.0f, halfY * 2 - 0.5f);
        //TopRect = new Rect(ArenaBounds.x, ArenaBounds.yMax - Size, ArenaBounds.width, Size);
        //BottomRect = new Rect(ArenaBounds.x, ArenaBounds.yMin, ArenaBounds.width, Size);

        //LeftRect = new Rect(ArenaBounds.x, ArenaBounds.y, Size, ArenaBounds.height);
        //RightRect = new Rect(ArenaBounds.xMax - Size, ArenaBounds.y, Size, ArenaBounds.height);

        var bounds = ArenaBoundsCollider.bounds;

        const float BorderSize = 2;
        ArenaBounds = new Rect(
            ArenaBoundsCollider.transform.position.x - bounds.extents.x,
            ArenaBoundsCollider.transform.position.y - bounds.extents.y,
            bounds.extents.x * 2,
            bounds.extents.y * 2);

        TopRect = new Rect(ArenaBounds.x, ArenaBounds.yMax - BorderSize, ArenaBounds.width, BorderSize);
        BottomRect = new Rect(ArenaBounds.x, ArenaBounds.yMin, ArenaBounds.width, BorderSize);

        LeftRect = new Rect(ArenaBounds.x, ArenaBounds.y, BorderSize, ArenaBounds.height);
        RightRect = new Rect(ArenaBounds.xMax - BorderSize, ArenaBounds.y, BorderSize, ArenaBounds.height);

        TextFps.enabled = false;
    }

    private void Start()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        SaveGame.OnSave += OnSaveGame;

        UpgradeManager.Instance.UpdateAllUpgrades();

        MusicManagerScript.Instance.SetVolume(SaveGame.Members.VolumeMusic * SaveGame.Members.VolumeMaster);
        AudioManager.Instance.SetVolume(SaveGame.Members.VolumeSfx * SaveGame.Members.VolumeMaster);

        ResetGame(autoStartGame: true);
        SetBuyMultiple(1);

        StartCoroutine(GameStateCo());
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit, saving...");
        TrySaveGame(forceSave: true);
    }

    Decimal512 _prevMoney = 999999;
    Decimal512 _prevPassiveIncome = 999999;
    float _timePrevPassiveIncomeUpdate = -1f;
    float _deltaRealTime = 0f;

    public float GetIncomeFactorPerFrame() => _deltaRealTime;

    private void ResetAwayTimestamp()
    {
        _timeLastSeen = DateTime.MinValue;
    }

    DateTime _timeLastSeen = DateTime.MinValue;

    private bool WasAway(int minSeconds, out TimeSpan awayTime)
    {
        awayTime = TimeSpan.Zero;
        if (_timeLastSeen == DateTime.MinValue)
        {
            _timeLastSeen = DateTime.UtcNow;
            return false;
        }

        awayTime = DateTime.UtcNow - _timeLastSeen;
        _timeLastSeen = DateTime.UtcNow;

        return awayTime.TotalSeconds > minSeconds;
    }

    bool RestartRoundIfWasAway()
    {
        bool wasAway = WasAway(minSeconds: 1, out TimeSpan awayTime);
        if (wasAway)
        {
            Debug.Log($"was away for {awayTime}");

            switch (GameState)
            {
                case State.Idle_Starting_Game:
                    break;
                case State.Idle_PresentLevel:
                case State.Idle_Fighting: // restart round if fighting
                    GameState = State.Idle_RestartRound;
                    break;
                case State.Idle_WonFight:
                    break;
                case State.Idle_OutOfTime:
                    break;
                case State.Idle_RestartRound:
                    break;
                default:
                    throw new Exception($"Need to handle {nameof(GameState)} {GameState} after being away");
            }
        }

        return wasAway;
    }

    static void ShowWelcomeBackDialog(TimeSpan ts)
    {
        int d = ts.Days;
        int h = ts.Hours;
        int m = ts.Minutes;

        GameCanvasScript.Instance.ShowPopup($"You were away for {FormatTime.Format(d, h, m)}\nWelcome back!");
    }

    float _nextPassiveIncomeUpdate;
    void UpdatePassiveIncome()
    {
        float currentTime = G.D.RealTime;

        if (_timePrevPassiveIncomeUpdate < 0f)
        {
            _timePrevPassiveIncomeUpdate = currentTime;
            _deltaRealTime = 0f;
            return;
        }

        _deltaRealTime = currentTime - _timePrevPassiveIncomeUpdate;
        if (_deltaRealTime <= 0f)
            return;

        // Important: even when on sleep realtime will be reflected when we
        // wake back up. This means we get a huge deltatime and actually get
        // income for all the time it was asleep. When throttled we should
        // handle at least 5-10 seconds, though, so set max delta time to
        // something reasonable.
        const float MaxRealtimeDelta = 60 * 5;
        if (_deltaRealTime > MaxRealtimeDelta)
        {
            var ts = TimeSpan.FromSeconds(_deltaRealTime);
            ShowWelcomeBackDialog(ts);
            Debug.Log($"Realtime delta too high ({(int)_deltaRealTime} sec), was probably on sleep. Clamped to {(int)MaxRealtimeDelta}");
            _deltaRealTime = MaxRealtimeDelta;
        }

        _timePrevPassiveIncomeUpdate = currentTime;

        // TotalPassiveIncome is the per/sec income.
        TotalPassiveIncome = UpgradeManager.Instance.GetTotalPassiveIncome();
        if (TotalPassiveIncome > SaveGame.Members.MaxIncome)
            SaveGame.Members.MaxIncome = TotalPassiveIncome;

        // MoneyToAdd is the scaled by fps income.
        float incomeFactorPerFrame = GetIncomeFactorPerFrame();
        Decimal512 moneyToAdd = TotalPassiveIncome * (Decimal512)incomeFactorPerFrame;
        SaveGame.Members.TotalIncomePassive += moneyToAdd;

        AddMoney(moneyToAdd);

        if (_prevPassiveIncome != TotalPassiveIncome || G.D.GameTime > _nextPassiveIncomeUpdate)
        {
            _nextPassiveIncomeUpdate = G.D.GameTime + 0.2f;
            _prevPassiveIncome = TotalPassiveIncome;

            string textIncome = SaveGame.Members.UseScientificNotation ?
                FormatScientific.Format(TotalPassiveIncome) :
                Format512.FormatWithDecimals(TotalPassiveIncome);

            textIncome = $"{textIncome} per second";
            if (textIncome != TextPassiveIncome.text)
            {
                TextPassiveIncome.text = textIncome;
                PopText(TextPassiveIncome);
            }
        }
    }

    void PopText(TextMeshProUGUI text)
    {
        var rect = text.rectTransform;
        LeanTween.cancel(rect);
        rect.localScale = Vector3.one;
        //var colorGreen = new Color(0.553f, 0.785f, 0.298f); // The green used in dps etc
        var colorGreyish = new Color32(240, 240, 240, 255); // The grayish used for standard text

        text.color = Color.white;

        LeanTween.scale(rect, Vector3.one * 1.1f, 0.1f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(rect, Vector3.one, 0.3f)
                    .setEase(LeanTweenType.easeOutBounce);
            });

        LeanTween.value(gameObject, Color.white, colorGreyish, 0.4f)
            .setOnUpdate((Color col) => text.color = col);
    }

    float _prevMoneyTextNextUpdate = 0f;

    void UpdateMoneyText()
    {
        if (SaveGame.Members.Money == _prevMoney)
            return;

        if (G.D.GameTime < _prevMoneyTextNextUpdate)
            return;

        _prevMoneyTextNextUpdate = G.D.GameTime + MoneyUpdateDelay;
        _prevMoney = SaveGame.Members.Money;

        string textMoney = SaveGame.Members.UseScientificNotation ?
            $"${FormatScientific.Format(SaveGame.Members.Money)}" :
            $"${Format512.FormatWithDecimals(SaveGame.Members.Money, abbreviate: false, alwaysThreeDecimalsForLargeNumbers: true)}";

        TextMoney.text = textMoney;
    }

    float _timeNextTotalIncomeUpdate;

    Color? _textTimeThisSessionOriginalColor;
    void OnSaveGame()
    {
        if (_textTimeThisSessionOriginalColor == null)
            _textTimeThisSessionOriginalColor = TextTimeThisSession.color;

        Color flash = Color.white;

        LeanTween.value(TextTimeThisSession.gameObject, _textTimeThisSessionOriginalColor.Value, flash, 0.1f)
            .setOnUpdate((Color val) =>
            {
                TextTimeThisSession.color = val;
            })
            .setLoopPingPong(1); // go there and back once
    }

    public void SetBuyMultiple(int count)
    {
        ButtonBuy1.interactable = true;
        ButtonBuy10.interactable = true;
        ButtonBuy100.interactable = true;
        if (count == 1)
        {
            ButtonBuy1.interactable = false;
            BuyAmount = 1;
        }
        else if (count == 10)
        {
            ButtonBuy10.interactable = false;
            BuyAmount = 10;
        }
        else if (count == 100)
        {
            ButtonBuy100.interactable = false;
            BuyAmount = 100;
        }
        UpgradeManager.Instance.UpdateUpgradeUi();
    }

    void UpdateBottomStats()
    {
        Decimal512 total = SaveGame.Members.TotalIncomePassive + SaveGame.Members.TotalIncomeArena;

        if (G.D.GameTime < _timeNextTotalIncomeUpdate)
            return;

        _timeNextTotalIncomeUpdate = G.D.GameTime + 0.1f;

        TextTotalIncome.text =
            $"Total: ${Format512.FormatWithDecimals(total, alwaysThreeDecimalsForLargeNumbers: true)}";

        TextTotalKilled.text =
            $"Enemies killed: {Format512.FormatWithDecimals(SaveGame.Members.EnemiesKilled, alwaysThreeDecimalsForLargeNumbers: true)} | " +
            $"Damage done: {Format512.FormatWithDecimals(SaveGame.Members.DamageDone, alwaysThreeDecimalsForLargeNumbers: true)}";

        long timeSinceLastSaveTime = (long)(G.D.GameTime - SaveGame.LastSaveTime);
        if (timeSinceLastSaveTime <= 7 && timeSinceLastSaveTime >= 0)
        {
            TextTimeThisSession.text = $"Game saved <5 sec ago    |";
        }
        else if (timeSinceLastSaveTime <= 20)
        {
            TextTimeThisSession.text = $"Game saved <20 sec ago    |";
        }
        else
        {
            TextTimeThisSession.text = $"Game saved >20 sec ago    |";
        }
    }

    float _nextSave;

    public void TrySaveGame(bool forceSave = false)
    {
        // Make sure we never save before load, this would wipe existing save game.
        if (!_firstSaveGameLoadComplete)
            return;

        if (forceSave || Time.realtimeSinceStartup > _nextSave)
        {
            //Debug.Log("Saving game...");
            SaveGame.Save();
            _nextSave = Time.realtimeSinceStartup + AutoSaveInterval;
        }
    }

    float _nextSendStats;
    void TrySendStats()
    {
        if (Time.realtimeSinceStartup > _nextSendStats)
        {
            // Update estimated time every time we save, we only need it for stats anyways (for now...)
            SaveGame.Members.EstimatedOnlineSeconds2 += SendStatsInterval;
            if (SaveGame.Members.TimesAscended_09_08_2025 > 0)
            {
                SaveGame.Members.TimeSinceLastAscend += SendStatsInterval;
            }

            Debug.Log("Sending stats...");
            UpdatePlayFabStats();
            _nextSendStats = Time.realtimeSinceStartup + SendStatsInterval;
        }
    }

    Dictionary<string, int> GetPlayFabStats()
    {
        // NB NB NB: only 25 stats are allowed! GameManager Awake() will throw if more.
        var dic = new Dictionary<string, int>()
        {
            { "level_pct_bought", (int)SaveGame.Members.LevelPctBought },

            { Playfab.ArenaLevel, (int)SaveGame.Members.ArenaLevel },
            { "times_ascended_09_08_2025", (int)SaveGame.Members.TimesAscended_09_08_2025 },
            { "diamond_count_09_08_2025", (int)SaveGame.Members.DiamondCount_09_08_2025 },
            { "monster_credits_lifetime_09_08_2025", (int)SaveGame.Members.MonsterCreditsLifetime_09_08_2025 },

            { "chests_collected", (int)SaveGame.Members.ChestsCollected },
            { "mystery_collected", (int)SaveGame.Members.MysteryCollected },
            { "estimated_online_seconds_2", (int)SaveGame.Members.EstimatedOnlineSeconds2 },

            { "current_skin", (int)SaveGame.Members.CurrentSkin },
            // 15 below this

            { "level_zap", (int)SaveGame.Members.LevelClickDamage },
            { "level_knife_damage", (int)SaveGame.Members.LevelKnifeDamage },
            { "level_gold_value", (int)SaveGame.Members.LevelMoneyPerGold },
            { "level_dagger_cd", (int)SaveGame.Members.LevelKnifeCd },
            { "level_witchdoctor", (int)SaveGame.Members.LevelWitchDoctor },
            { "level_gold_per_dagger", (int)SaveGame.Members.LevelGoldPerKnifeThrown },
            { "level_wizard", (int)SaveGame.Members.LevelWizard },
            { "level_hoarder", (int)SaveGame.Members.LevelHoarder },
            { "level_zap_damage", (int)SaveGame.Members.LevelZapDamage },
            { "level_moneymaker", (int)SaveGame.Members.LevelMoneyMaker },
            { "level_dagger_master", (int)SaveGame.Members.LevelDaggerMaster },
            { "level_necro_ninja", (int)SaveGame.Members.LevelNecroNinja },
            { "level_skull_crusher", (int)SaveGame.Members.LevelSkullCrusher},
            { "level_chest_master", (int)SaveGame.Members.LevelChestMaster},
            { "level_voidgazer", (int)SaveGame.Members.LevelVoidgazer},
        };
        return dic;
    }

    public void UpdatePlayFabStats()
    {
        var dic = GetPlayFabStats();
        Playfab.PlayerStat(dic);
    }

    void LateUpdate()
    {
        PruneDeadEnemies();
        ActorBase.ResetClosestEnemy();
    }

    void Update()
    {
        TrySaveGame();
        TrySendStats();

        UpdatePassiveIncome();
        UpdateMoneyText();
        UpdateBottomStats();

        TimeSinceStartup = Time.realtimeSinceStartup;
        GameDeltaTime = Math.Min(0.5f, Time.deltaTime * PlayerUpgrades.Data.TimeScale);
        GameTime += GameDeltaTime;
        SaveGame.Members.LastSeenUtcStr = FormatTime.DateTimeToString(DateTime.UtcNow);


        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        // CHEATS

        // C is taken
        //if (G.GetCheatKeyDown(KeyCode.C) && G.GetCheatKey(KeyCode.RightControl))
        //{
        //    Playfab.LoginRes.SessionTicket = null;
        //    _nextSendStats = 0;
        //    TrySendStats();
        //}

        if (G.GetCheatKeyDown(KeyCode.L) && G.GetCheatKey(KeyCode.RightShift))
        {
            GameCanvasScript.Instance.ShowPopup(string.Join("\n", GameLoadInfo));
        }

        if (G.GetCheatKeyDown(KeyCode.C) && G.GetCheatKey(KeyCode.RightControl))
        {
            SaveGame.Members.DiamondCount_09_08_2025 = 0;
            SaveGame.Members.MonsterCredits_09_08_2025 += 3;
            SaveGame.Members.MonsterCreditsXp_09_08_2025 = 200000;
        }

        if (G.GetCheatKeyDown(KeyCode.Q) && G.GetCheatKey(KeyCode.RightControl))
        {
            QuestionmarkScript.Instance.ForceReady();
        }

        if (G.GetCheatKeyDown(KeyCode.Q) && G.GetCheatKey(KeyCode.RightShift))
        {
            PopupChestScript.Instance.ShowNow();
        }

        if (G.GetCheatKeyDown(KeyCode.M) && G.GetCheatKey(KeyCode.RightControl))
        {
            SaveGame.Members.Money += 900_000_000_000_000_000;
        }

        if (G.GetCheatKeyDown(KeyCode.M) && G.GetCheatKey(KeyCode.RightControl) && G.GetCheatKey(KeyCode.RightShift))
        {
            SaveGame.Members.Money = 0;
        }

        if (G.GetCheatKeyDown(KeyCode.A) && G.GetCheatKey(KeyCode.RightControl))
        {
            SaveGame.Members.ArenaLevel += 25;
        }

        if (G.GetCheatKeyDown(KeyCode.S) && G.GetCheatKey(KeyCode.RightControl))
        {
            SaveGame.Members.ArenaLevel -= 25;
            if (SaveGame.Members.ArenaLevel <= 1)
                SaveGame.Members.ArenaLevel = 1;
        }

        if (G.GetCheatKeyDown(KeyCode.RightArrow) && G.GetCheatKey(KeyCode.RightControl))
        {
            PlayerUpgrades.Data.TimeScale += 0.1f;
        }

        if (G.GetCheatKeyDown(KeyCode.LeftArrow) && G.GetCheatKey(KeyCode.RightControl))
        {
            PlayerUpgrades.Data.TimeScale -= 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            TextFps.enabled = !TextFps.enabled;
        }

        if (TextFps.enabled)
        {
            TextFps.text = string.Format("{0} fps", Mathf.RoundToInt(1.0f / Time.unscaledDeltaTime));
        }

        if (Input.GetKeyDown(KeyCode.Z) && G.GetCheatKey(KeyCode.RightControl))
        {
            SaveGame.Members.LevelClickDamage = 1000;
            SaveGame.Members.LevelGoldPerKnifeThrown = 1000;
            SaveGame.Members.LevelHoarder = 1000;
            SaveGame.Members.LevelDaggerMaster = 1000;
            SaveGame.Members.LevelKnifeDamage = 1000;
            SaveGame.Members.LevelMoneyPerGold = 1000;
            SaveGame.Members.LevelPctBought = 250;
            SaveGame.Members.LevelNecroNinja = 1000;
            SaveGame.Members.LevelVoidgazer = 1000;
            SaveGame.Members.LevelMoneyPerGold = 1000;
            SaveGame.Members.LevelGoldPerKnifeThrown = 1000;
            SaveGame.Members.ArenaLevel = 15000;
        }

        //if (Input.GetKeyDown(KeyCode.F4))
        //{
        //    var saw = WeaponBase.GetWeapon(WeaponType.Sawblade);
        //    saw.Eject(Vector2.zero, Vector2.right, Color.white);
        //}
    }
}
