using Assets.Script.Upgrades;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscendProgressScript : MonoBehaviour
{
    public AscendProgressScript Instance;
    public TextMeshProUGUI TextCredits;
    public Image ProgressBarImage;
    public GameObject AscendRoot;
    private float _nextCreditUpdate;

    [NonSerialized] public bool RebirthEnabled = true;

    private void Awake()
    {
        Instance = this;
    }

    public void OnShowClick()
    {
        AscendRoot.SetActive(true);
    }

    public void OnCloseClick()
    {
        AscendRoot.SetActive(false);
    }

    static void ApplyAscendPermanentBonuses()
    {
        // Multiplicative
        float passiveMultiplier = 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX2_1 ? 2 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX2_2 ? 2 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX4_1 ? 4 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX4_2 ? 4 : 1;
        passiveMultiplier *= SaveGame.Members.BoughtPassiveX4_3 ? 4 : 1;
        PlayerUpgrades.Data.PassiveIncomeAscendMultiplier = passiveMultiplier;

        // 10% per diamond
        PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier = SaveGame.Members.DiamondCount_09_08_2025 * 0.1f;
    }

    void Update()
    {
        // Apply ascend bonuses
        ApplyAscendPermanentBonuses();

        if (G.D.RealTime < _nextCreditUpdate)
            return;

        const float CreditUpdateRate = 1.0f;
        const float MaxRealtimeDelta = 60 * 5; // 5 minutes

        // Calculate how much time actually passed since last update
        float delta = G.D.RealTime - _nextCreditUpdate + CreditUpdateRate;

        // Clamp it so we don't get a huge spike after sleep
        if (delta > MaxRealtimeDelta)
            delta = MaxRealtimeDelta;

        // Schedule next update
        _nextCreditUpdate = G.D.RealTime + CreditUpdateRate;

        // Add scaled income
        SaveGame.Members.MonsterCreditsXp_09_08_2025 +=
            GameManager.Instance.TotalPassiveIncome * delta;

        Decimal512 xpForNextLevel = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetime_09_08_2025 + 1);
        if (SaveGame.Members.MonsterCreditsXp_09_08_2025 > xpForNextLevel)
        {
            SaveGame.Members.MonsterCreditsXp_09_08_2025 -= xpForNextLevel;
            SaveGame.Members.MonsterCreditsLifetime_09_08_2025++;
            SaveGame.Members.MonsterCredits_09_08_2025++;
            SaveGame.Members.MaxCredits = Math.Max(SaveGame.Members.MaxCredits, SaveGame.Members.MonsterCredits_09_08_2025);
        }

        float t = (float)(SaveGame.Members.MonsterCreditsXp_09_08_2025.ToDouble() / xpForNextLevel.ToDouble());
        ProgressBarImage.fillAmount = t;

        float pct = t * 100.0f;
        TextCredits.text = $"Credits: {SaveGame.Members.MonsterCredits_09_08_2025}\n<size=-3>Next: {pct:#0.000}%";
    }
}
