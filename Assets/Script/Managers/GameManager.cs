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
    public enum State { None, Idle_PresentLevel, Idle_Fighting, Idle_WonFight, Idle_OutOfTime };

    const float BaseXpToLevel = 14;
    const int RoundTimeSeconds = 30;
    const int AutoSaveInterval = 10;
    const int SendStatsInterval = 60;

    public string GameVersion;
    public static GameManager Instance;
    public bool UnlockAllGameModes;
    public bool UnlockAllWeapons;
    public bool UnlockAllHeroes;

    public Color[] xpColors = new Color[] { };

    public BoxCollider2D ArenaBoundsCollider;
    public LeanTween Tween;
    public Text TextVersion;
    public Text TextUser;
    public Text TextFps;
    public TextMeshProUGUI TextGameInfo;
    public TextMeshProUGUI TextClock;
    public TextMeshProUGUI TextLevel;
    public TextMeshProUGUI TextArenaTotalIncome;
    public TextMeshProUGUI TextPassiveTotalIncome;
    public TextMeshProUGUI TextMoney;
    public TextMeshProUGUI TextPassiveIncome;
    public SpriteRenderer Floor;
    Color floorDefaultColor;
    public string ColorLocked;
    public string ColorUnlocked;
    public HpBarScript HpBarScript;
    public GameModeEnum GameMode;

    public ParticleSystem FlyingBlood;
    public ParticleSystem BloodDrops;
    public ParticleSystem FloorBlood;
    public ParticleSystem PoofClouds;
    public ParticleSystem SpawnPoof;
    public ParticleSystem FlashParticles;
    public ParticleSystem CircleParticles;
    public GrenadeScript Grenade;
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
    public float ArenaScale => ArenaRoot.transform.localScale.x;
    public GameObject ArenaRoot;
    public int CurrentRound = 1;
    public int MaxRound = 10;

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

        SaveGame.Members = new();
        SaveGame.Members.VolumeMaster = VolumeMaster;
        SaveGame.Members.VolumeMusic = VolumeMusic;
        SaveGame.Members.VolumeSfx = VolumeSfx;

        SaveGame.Members.SaveKillSwitch_CanSave = true;

        SaveGame.Save();
    }

    IEnumerator ShowInfoTextFlashy(string text, float delay = 1.0f)
    {
        var go = TextGameInfo.gameObject;
        var canvasGroup = go.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = go.AddComponent<CanvasGroup>();

        TextGameInfo.text = text;
        TextGameInfo.color = Color.yellow;
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

    private bool TryZapEnemy(Vector2 from, ActorBase enemy)
    {
        if (PlayerUpgrades.Data.ZapDamage == 0)
            return false;

        if (enemy is null)
            return false;

        bool success = Zapper.TryZapEnemy(from, enemy, PlayerUpgrades.Data.ZapDamage);
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
                float shakeStrength = 0.075f;
                rt.anchoredPosition = originalPos + (Vector3)UnityEngine.Random.insideUnitCircle * shakeStrength;
            })
            .setOnComplete(() =>
            {
                rt.anchoredPosition = originalPos;
            });
    }

    void CheckZapping()
    {
        if (PlayerUpgrades.Data.ZapDamage > 0 && G.D.GameTime > _nextZap)
        {
            var firstTarget = ActorBase.PlayerClosestEnemyActor;
            if (firstTarget != null)
            {
                var direction = (firstTarget.transform.position - G.D.PlayerPos).normalized;
                G.D.PlayerScript.AddForce(-direction * 0.5f);

                AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerStaffHit, volumeScale: 1.0f, pitch: 1.0f);
                ShakeArenaBackground();

                _nextZap = G.D.GameTime + ZapInterval;
                StartCoroutine(ZapChainCoroutine(firstTarget));
            }
        }
    }

    IEnumerator ZapChainCoroutine(ActorBase firstTarget)
    {
        ZapTargetIgnoreList.Clear();

        if (!TryZapEnemy(G.D.PlayerPos, firstTarget))
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

            if (!TryZapEnemy(prevEnemy.transform.position, nextEnemy))
                break;

            GameManager.Instance.ShakeCamera(1.0f);

            ZapTargetIgnoreList.Add(nextEnemy);
            prevEnemy = nextEnemy;
        }
    }

    // main loop
    IEnumerator GameStateCo()
    {
        //Decimal256 v1 = 1_234_456;
        //Decimal256 v2 = 10.123;
        //Decimal256 v3 = 1000.456;

        //string s1 = Format256.Format(v1);
        //string s2 = Format256.Format(v2);
        //string s3 = Format256.Format(v3);

        while (true)
        {
            UpdateMoneyText();
            UpdateTotalIncomeArenaText();

            TextLevel.text = $"ARENA {SaveGame.Members.ArenaLevel}";
            GameState = State.Idle_PresentLevel;

            G.D.PlayerScript.StartGame();
            ShowSecondsLeft(RoundTimeSeconds);

            var enemies = EnemySpawner.GetEnemies(SaveGame.Members.ArenaLevel).ToList();
            long totalHitpoints = enemies.Sum(a => a.BaseHp);
            livingEnemyCount = enemies.Count();
            HpBarScript.SetHp(totalHitpoints, totalHitpoints);
            _roundTotalHp = totalHitpoints;

            AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.NewRound);
            yield return ShowInfoTextFlashy("ROUND START!");
            
            foreach (var enemy in enemies)
            {
                enemy.gameObject.SetActive(true);
                yield return null;
            }

            float roundStartTime = G.D.GameTime;
            float roundEndTime = roundStartTime + RoundTimeSeconds;

            GameState = State.Idle_Fighting;
            _nextZap = G.D.GameTime + ZapInterval;

            while (GameState == State.Idle_Fighting)
            {
                CheckZapping();

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
                // Last enemy already threw round gold, NOT done here
                SaveGame.Members.ArenaLevel++;
                yield return ShowInfoTextFlashy($"ARENA {SaveGame.Members.ArenaLevel}", delay: 1);
                yield return new WaitForSeconds(0.25f);
                yield return null;
                GameState = State.Idle_PresentLevel;
            }

            while (GameState == State.Idle_OutOfTime)
            {
                GameState = State.Idle_PresentLevel;

                // We didn't kill all enemies so present gold in the middle
                PresentRoundGold(Vector2.zero);

                foreach (var enemy in enemies)
                {
                    enemy.ReturnToCache();
                }

                yield return ShowInfoTextFlashy("OUT OF TIME", delay: 1);
                yield return new WaitForSeconds(0.5f);

                // Go back one arena level on timeout
                SaveGame.Members.ArenaLevel--;
            }
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

    public void AddGold(bool isLargeCoin, Decimal256 value)
    {
        Decimal256 moneyAdded = value;
        SaveGame.Members.TotalIncomeArena += moneyAdded;

        AddMoney(moneyAdded);

        Vector2 playerPos = G.D.PlayerPos;
        Vector2 textPos = playerPos + Vector2.up * 0.75f + RndUtil.RandomInsideUnitCircle();

        FloatingTextSpawner.Instance.Spawn(
            textPos,
            $"${Format256.Format(moneyAdded)}",
            Color.yellow,
            speed: 2.0f,
            timeToLive: 1.0f,
            fontStyle: TMPro.FontStyles.Bold);
    }

    public void AddMoney(Decimal256 amount)
    {
        SaveGame.Members.Money += amount;
    }

    public void DeductMoney(Decimal256 amount)
    {
        SaveGame.Members.Money -= amount;
    }

    public void AddXp(long amount)
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

    public void ThrowPickups(AutoPickUpType pickupType, Vector2 pos, int amount, long value, float forceScale = 1.0f, bool isLargeCoin = false)
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
        const double HpToGoldPct = 0.1 / EnemySpawner.HpScale;

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
            // DaggerDamage * daggersThrown * goldPerDagger
            long knifeThrownBonus = (long)(
                PlayerUpgrades.Data.MagicMissileEffectiveDamage *
                G.D.PlayerScript.DaggersThrown *
                PlayerUpgrades.Data.GoldPerKnifeThrown);

            if (knifeThrownBonus > 0)
            {
                FloatingTextSpawner.Instance.Spawn(
                    endRoundGoldSummaryPos + Vector2.down * 0.5f,
                    $"Dagger throws: +<color=yellow>{Format256.Format(knifeThrownBonus)})/color>G",
                    Color.white,
                    speed: 0.05f,
                    timeToLive: 2.0f,
                    fontStyle: TMPro.FontStyles.Bold);

                ThrowGoldSplit(knifeThrownBonus, position);
            }
        }

        ThrowGoldSplit(goldWon, position);

        long secondsSpent = RoundTimeSeconds - _secondsLeft;
        long dps = (long)(damageDone / (double)secondsSpent);

        FloatingTextSpawner.Instance.Spawn(
            endRoundGoldSummaryPos,
            $"{damageDone} dam in {secondsSpent} sec ({dps} DPS), +<color=yellow>{Format256.Format(goldWon)}</color>G",
            Color.white,
            speed: 0.05f,
            timeToLive: 3.0f,
            fontStyle: TMPro.FontStyles.Bold);
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

        // Smooth scale: 1 coin at low amounts, 20 at 1000 or more
        int coinCount = Mathf.Clamp(Mathf.RoundToInt(goldWon / 50f), 1, 20);

        long baseValue = goldWon / coinCount;
        long remainder = goldWon % coinCount;

        for (int i = 0; i < coinCount; i++)
        {
            long value = baseValue + (i < remainder ? 1 : 0);
            ThrowPickups(AutoPickUpType.Money, position, amount: 1, (int)value, forceScale: 4.0f, isLargeCoin);
        }
    }

    public void DamageEnemy(ActorBase enemy, double amount, Vector3 direction, float forceModifier)
    {
        if (enemy.Hp <= 0 || enemy.IsDead)
            return;

        direction = direction.normalized;

        amount *= PlayerUpgrades.Data.DamageMul;
        if (amount < 1)
            amount = 1;

        long intAmount = (long)amount;

        // Diplay the full damager number, without truncating to enemy health.
        Vector2 randomTextOffset = UnityEngine.Random.insideUnitCircle * 1.5f;
        FloatingTextSpawner.Instance.Spawn(
            (Vector2)enemy.transform.position + Vector2.up * 0.25f + randomTextOffset,
            $"-{Format64.Format(intAmount)}",
            Color.red,
            speed: 0.75f,
            timeToLive: 1.0f,
            fontStyle: TMPro.FontStyles.Bold);

        // Now truncate to enemy health.
        if (intAmount > enemy.Hp)
            intAmount = enemy.Hp;

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

    void Awake()
    {
        SaveGame.Load();
        _firstSaveGameLoadComplete = true;

        TextGameInfo.text = "";
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

        UpgradeManager.Instance.UpdateAllUpgrades();

        MusicManagerScript.Instance.SetVolume(SaveGame.Members.VolumeMusic * SaveGame.Members.VolumeMaster);
        AudioManager.Instance.SetVolume(SaveGame.Members.VolumeSfx * SaveGame.Members.VolumeMaster);

        ResetGame(autoStartGame: true);
        StartCoroutine(GameStateCo());
    }

    private void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit, saving...");
        TrySaveGame(forceSave: true);
    }

    void LateUpdate()
    {
        PruneDeadEnemies();
        ActorBase.ResetClosestEnemy();
    }

    Decimal256 _prevMoney = 999999;
    Decimal256 _prevPassiveIncome = 999999;
    float _timePrevPassiveIncomeUpdate = -1f;
    float _deltaRealTime = 0f;

    public float GetIncomeFactorPerFrame() => _deltaRealTime;

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

        _timePrevPassiveIncomeUpdate = currentTime;

        Decimal256 totalPassiveIncome = UpgradeManager.Instance.GetTotalPassiveIncome();
        SaveGame.Members.Money += totalPassiveIncome * (Decimal256)_deltaRealTime;

        if (_prevPassiveIncome != totalPassiveIncome)
        {
            TextPassiveIncome.text = $"{Format256.FormatWithDecimals(totalPassiveIncome)} per second";
            _prevPassiveIncome = totalPassiveIncome;
            PopText(TextPassiveIncome);
        }
    }

    void PopText(TextMeshProUGUI text)
    {
        var rect = text.rectTransform;
        LeanTween.cancel(rect);
        rect.localScale = Vector3.one;
        text.color = Color.yellow;

        LeanTween.scale(rect, Vector3.one * 1.5f, 0.1f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween.scale(rect, Vector3.one, 0.3f)
                    .setEase(LeanTweenType.easeOutBounce);
            });

        LeanTween.value(gameObject, Color.yellow, Color.white, 0.4f)
            .setOnUpdate((Color col) => text.color = col);
    }

    void UpdateMoneyText()
    {
        if (SaveGame.Members.Money == _prevMoney)
            return;

        _prevMoney = SaveGame.Members.Money;
        TextMoney.text = $"${Format256.FormatWithDecimals(SaveGame.Members.Money, abbreviate: false)}";
    }

    Decimal256 _prevTotalIncomeArena = 999999;
    void UpdateTotalIncomeArenaText()
    {
        if (SaveGame.Members.TotalIncomeArena == _prevTotalIncomeArena)
            return;

        _prevTotalIncomeArena = SaveGame.Members.TotalIncomeArena;
        TextArenaTotalIncome.text = $"Arena earned: ${Format256.Format(SaveGame.Members.TotalIncomeArena, abbreviate: false)}";
    }

    Decimal256 _prevTotalIncomePassive = 999999;
    void UpdateTotalIncomePassiveText()
    {
        if (SaveGame.Members.TotalIncomePassive == _prevTotalIncomePassive)
            return;

        _prevTotalIncomePassive = SaveGame.Members.TotalIncomePassive;
        TextPassiveTotalIncome.text = $"Passive earned: ${Format256.Format(SaveGame.Members.TotalIncomePassive, abbreviate: false)}";
    }

    void UpdateTimeSeen()
    {
        SaveGame.Members.TotalGameTimeAccumulated += G.D.GameTime - SaveGame.Members.LastGameTimeSeen;
        SaveGame.Members.TotalRealTimeAccumulated += G.D.RealTime - SaveGame.Members.LastRealTimeSeen;
        SaveGame.Members.LastGameTimeSeen = G.D.GameTime;
        SaveGame.Members.LastRealTimeSeen = G.D.RealTime;
    }

    float _nextSave;

    public void TrySaveGame(bool forceSave = false)
    {
        // Make sure we never save before load, this would wipe existing save game.
        if (!_firstSaveGameLoadComplete)
            return;

        if (forceSave || Time.realtimeSinceStartup > _nextSave)
        {
            Debug.Log("Saving game...");
            SaveGame.Save();
            _nextSave = Time.realtimeSinceStartup + AutoSaveInterval;
        }
    }

    float _nextSendStats;

    void TrySendStats()
    {
        if (Time.realtimeSinceStartup > _nextSendStats)
        {
            Debug.Log("Sending stats...");
            UpdatePlayFabStats();
            _nextSendStats = Time.realtimeSinceStartup + SendStatsInterval;
        }
    }

    public void UpdatePlayFabStats()
    {
        Playfab.PlayerEvent(Playfab.SendStatsEvent, new Dictionary<string, object>());
        var dic = new Dictionary<string, int>()
        {
            { Playfab.ArenaLevel, (int)SaveGame.Members.ArenaLevel },
            { Playfab.GameTimeAccumulated, (int)SaveGame.Members.TotalGameTimeAccumulated },
            { Playfab.RealTimeAccumulated, (int)SaveGame.Members.TotalRealTimeAccumulated },
            { "level_zap", (int)SaveGame.Members.LevelClickDamage },
            { "level_knife_damage", (int)SaveGame.Members.LevelKnifeDamage },
            { "level_gold_value", (int)SaveGame.Members.LevelMoneyPerGold },
            { "level_dagger_cd", (int)SaveGame.Members.LevelKnifeCd },
            { "level_witchdoctor", (int)SaveGame.Members.LevelWitchDoctor },
            { "level_gold_per_dagger", (int)SaveGame.Members.LevelGoldPerKnifeThrown},
        };

        Playfab.PlayerStat(dic);
    }

    void Update()
    {
        TrySaveGame();
        TrySendStats();

        UpdateTimeSeen();
        UpdatePassiveIncome();
        UpdateMoneyText();
        UpdateTotalIncomeArenaText();
        UpdateTotalIncomePassiveText();

        TimeSinceStartup = Time.realtimeSinceStartup;
        GameDeltaTime = Math.Min(0.1f, Time.deltaTime * PlayerUpgrades.Data.TimeScale);
        GameTime += GameDeltaTime;


        if (Input.GetKeyDown(KeyCode.F))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }


        // CHEATS


        if (G.GetCheatKeyDown(KeyCode.R) && G.GetCheatKey(KeyCode.RightShift))
        {
            ResetAllProgress();
        }

        if (G.GetCheatKeyDown(KeyCode.M) && G.GetCheatKey(KeyCode.LeftShift))
        {
            SaveGame.Members.Money += 100_000_000_000;
        }

        if (G.GetCheatKeyDown(KeyCode.RightArrow) && G.GetCheatKey(KeyCode.RightShift))
        {
            PlayerUpgrades.Data.TimeScale += 0.1f;
        }

        if (G.GetCheatKeyDown(KeyCode.LeftArrow) && G.GetCheatKey(KeyCode.RightShift))
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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var closestEnemy = BlackboardScript.GetClosestEnemy(G.D.PlayerPos, radius: 20);
            long damage = PlayerUpgrades.Data.ZapDamage;
            Zapper.TryZapEnemy(G.D.PlayerPos, closestEnemy, damage);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerUpgrades.Data.BaseMoveSpeed += Input.GetKey(KeyCode.LeftShift) ? 1f : -1f;
            Debug.Log("BaseMoveSpeed : " + PlayerUpgrades.Data.BaseMoveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerUpgrades.Data.MagicMissileBaseCd += Input.GetKey(KeyCode.LeftShift) ? -0.25f : 0.25f;
            Debug.Log("MagicMissileBaseCd : " + PlayerUpgrades.Data.MagicMissileBaseCd);
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerUpgrades.Data.MagicMissileBaseDamage += Input.GetKey(KeyCode.LeftShift) ? 10f : -10f;
            Debug.Log("MagicMissileBaseDamage : " + PlayerUpgrades.Data.MagicMissileBaseDamage);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            var saw = WeaponBase.GetWeapon(WeaponType.Sawblade);
            saw.Eject(Vector2.zero, Vector2.right, Color.white, 1.0f);
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame.Members.ArenaLevel++;
        }
    }
}
