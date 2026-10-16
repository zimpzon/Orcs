using Assets.Script.Upgrades;
using Assets.Script.Upgrades.Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscendProgressScript : MonoBehaviour
{
    public AscendProgressScript Instance;
    public TextMeshProUGUI TextCredits;
    public TextMeshProUGUI TextTimeLeft;
    public Image ProgressBarImage;
    public GameObject AscendRoot;
    private float _nextCreditUpdate;
    private float _lastCreditCommitTime;
    private double _creditAllowance = 1.0; // credits allowed by the speed limit right now (MinSecondsPerCredit)

    [NonSerialized] public bool RebirthEnabled = true;

    static AscendProgressScript _active;
    public static bool RebirthDialogOpen => _active != null && _active.AscendRoot.activeInHierarchy;

    private void Awake()
    {
        Instance = this;
        _active = this;
        _lastCreditCommitTime = G.D.RealTime;
    }

    public void OnShowClick()
    {
        AscendRoot.SetActive(true);
        AudioManager.Instance.PlayDialogShow();
        if (SaveGame.Members.MonsterCredits_09_08_2025 > 0)
            SaveGame.Members.ViewedRebirthDialog = true; // big rebirth button pulse -> small (CrownPulse)
    }

    public void OnCloseClick()
    {
        if (AscendRoot.activeSelf)
            AudioManager.Instance.PlayDialogClose();
        AscendRoot.SetActive(false);
    }

    static void ApplyAscendPermanentBonuses()
    {
        // Additive
        // TODO: THESE are rebalanced but the names are not changed. Cannot just change due to save file JSON. Recover?
        double passiveMultiplier = 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX2_1 ? 2 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX2_2 ? 2 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX4_1 ? 2 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX4_2 ? 2 : 1;
        //passiveMultiplier *= SaveGame.Members.BoughtPassiveX4_3 ? 3 : 1;
        //passiveMultiplier *= SaveGame.Members.BoughtPassiveX6_1 ? 3 : 1;
        //passiveMultiplier *= SaveGame.Members.BoughtPassiveX7_1 ? 4 : 1;

        PlayerUpgrades.Data.PassiveIncomeAscendMultiplier = passiveMultiplier;
        PlayerUpgrades.Data.PassiveIncomeDiamondBonus = UpgradeProgression.DiamondIncomeBonus();
    }

    void Update()
    {
        // Apply ascend bonuses
        ApplyAscendPermanentBonuses();

        // Passive income (UpgradeProgression.CreditXpPerSecond), times the rebirth credit bonus (x1.0 after a rebirth,
        // falling to x0.1; UpgradeProgression.CreditBonusMultiplier).
        Decimal512 creditXpPerSecond = UpgradeProgression.CreditXpPerSecond() * UpgradeProgression.CreditBonusMultiplier();

        // The speed limits apply to the default speed: a mystery boost (x10 income, x2.5 faster time) goes past them by
        // its factor. So the soft limit is worked out on the unboosted rate, then the boost is put back on top.
        double boost = Math.Max(1.0, PlayerUpgrades.Data.PassiveIncomeTempMultiplier);
        Decimal512 unboosted = creditXpPerSecond / boost;
        // Last tiers: soft speed limit, so credits approach the tier limit instead of being cut off at it.
        creditXpPerSecond = unboosted * UpgradeProgression.CreditSoftLimitFactor(unboosted) * boost;

        const float CreditUpdateRate = 1.0f;
        const float MaxRealtimeDelta = 60 * 5; // 5 minutes

        // Commit actual XP/credit gain once per second (the display below updates every frame). Speed limit: at most one
        // credit per UpgradeProgression.MinSecondsPerCredit() (1 s at the last tier, +0.5 s per tier below); XP beyond the next
        // credit's cost is dropped, not banked. The allowance can't be saved up beyond one credit (or one per limit period
        // of a stall).
        double minSecondsPerCredit = UpgradeProgression.MinSecondsPerCredit() / boost; // boosted: past the limit too
        if (G.D.RealTime >= _nextCreditUpdate)
        {
            // Calculate how much time actually passed since last update
            double delta = G.D.RealTime - _nextCreditUpdate + CreditUpdateRate;

            // Clamp it so we don't get a huge spike after sleep
            if (delta > MaxRealtimeDelta)
                delta = MaxRealtimeDelta;

            // Schedule next update
            _nextCreditUpdate = G.D.RealTime + CreditUpdateRate;

            // Add scaled income
            SaveGame.Members.MonsterCreditsXp_09_08_2025 +=
                creditXpPerSecond * delta;

            double allowanceGain = delta / minSecondsPerCredit;
            _creditAllowance = Math.Min(_creditAllowance + allowanceGain, Math.Max(1.0, allowanceGain));
            while (_creditAllowance >= 1.0)
            {
                Decimal512 xpForNextLevelCommit = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
                if (SaveGame.Members.MonsterCreditsXp_09_08_2025 < xpForNextLevelCommit)
                    break;

                SaveGame.Members.MonsterCreditsXp_09_08_2025 -= xpForNextLevelCommit;
                SaveGame.Members.MonsterCreditsLifetime_09_08_2025++;
                SaveGame.Members.MonsterCredits_09_08_2025++;
                _creditAllowance -= 1.0;
            }

            // Only when the speed limit held a credit back (XP is past the cost): keep XP at the cost so no backlog builds
            // up; the credit lands as soon as the limit allows. Progress below the cost is never cut. (Capping at 99.9%
            // every commit threw away early progress: with gain < 0.1% of the cost per second, credit 1 never landed.)
            Decimal512 xpCap = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
            if (SaveGame.Members.MonsterCreditsXp_09_08_2025 > xpCap)
                SaveGame.Members.MonsterCreditsXp_09_08_2025 = xpCap;
            SaveGame.Members.MaxCredits = Math.Max(SaveGame.Members.MaxCredits, SaveGame.Members.MonsterCredits_09_08_2025);

            _lastCreditCommitTime = G.D.RealTime;
        }

        // Smoothly animate the display every frame by projecting forward from the last commit
        // using the current income rate, without touching saved XP/credits state.
        Decimal512 xpForNextLevel = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
        double projectedElapsed = G.D.RealTime - _lastCreditCommitTime;
        Decimal512 displayedXp = SaveGame.Members.MonsterCreditsXp_09_08_2025 + creditXpPerSecond * projectedElapsed;

        double t = displayedXp.ToDouble() / xpForNextLevel.ToDouble();
        if (t < 0) t = 0;
        if (t > 1) t = 1;
        ProgressBarImage.fillAmount = (float)t;

        double pct = t * 100.0;
        if (pct > 100) pct = 100;
        TextCredits.text = $"Credits: {SaveGame.Members.MonsterCredits_09_08_2025}\n<size=-3><color=#cccccc>Next: {pct:#0}%";

        const double MaxSeconds = 60 * 60 * 12;
        // When the speed limit is what holds the next credit back, show that wait instead of the (shorter) XP wait.
        double limitWait = (1.0 - _creditAllowance) * minSecondsPerCredit - projectedElapsed;
        // Decimal512 throws below 0, and the projected XP can pass the cost while the limit holds the credit back.
        double xpWait = displayedXp >= xpForNextLevel ? 0.0
            : creditXpPerSecond > 0.01 ? ((xpForNextLevel - displayedXp) / creditXpPerSecond).ToDouble()
            : double.MaxValue;
        if (limitWait > 0 && limitWait > xpWait)
            TextTimeLeft.text = Assets.Script.Misc.Format.FormatTimeShort(TimeSpan.FromSeconds(Math.Ceiling(limitWait)));
        else
            TextTimeLeft.text = UpgradeManagerHelper.FormatTimeLeft(xpForNextLevel, displayedXp, MaxSeconds, creditXpPerSecond);
    }
}
