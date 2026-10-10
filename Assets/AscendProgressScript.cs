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

    [NonSerialized] public bool RebirthEnabled = true;

    private void Awake()
    {
        Instance = this;
        _lastCreditCommitTime = G.D.RealTime;
    }

    public void OnShowClick()
    {
        AscendRoot.SetActive(true);
        if (SaveGame.Members.MonsterCredits_09_08_2025 > 0)
            SaveGame.Members.ViewedRebirthDialog = true; // big rebirth button pulse -> small (CrownPulse)
    }

    public void OnCloseClick()
    {
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

        // Passive income, but never below the small credit speed floor (UpgradeProgression.CreditXpPerSecond).
        // Times the rebirth credit bonus (x1.0 after a rebirth, falling to x0.5; UpgradeProgression.CreditBonusMultiplier).
        Decimal512 creditXpPerSecond = UpgradeProgression.CreditXpPerSecond() * UpgradeProgression.CreditBonusMultiplier();

        const float CreditUpdateRate = 1.0f;
        const float MaxRealtimeDelta = 60 * 5; // 5 minutes

        // Commit actual XP/credit gain once per second (the display below updates every frame). Hard cap: at most one
        // credit per second (per second of delta after a stall); XP beyond that is dropped, not banked.
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

            int maxCredits = Math.Max(1, (int)Math.Floor(delta));
            for (int i = 0; i < maxCredits; ++i)
            {
                Decimal512 xpForNextLevelCommit = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
                if (SaveGame.Members.MonsterCreditsXp_09_08_2025 < xpForNextLevelCommit)
                    break;

                SaveGame.Members.MonsterCreditsXp_09_08_2025 -= xpForNextLevelCommit;
                SaveGame.Members.MonsterCreditsLifetime_09_08_2025++;
                SaveGame.Members.MonsterCredits_09_08_2025++;
            }

            // Capped: keep the bar just short of full instead of banking a backlog.
            Decimal512 xpCap = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1) * 0.999;
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
        TextTimeLeft.text = UpgradeManagerHelper.FormatTimeLeft(xpForNextLevel, displayedXp, MaxSeconds, creditXpPerSecond);
    }
}
