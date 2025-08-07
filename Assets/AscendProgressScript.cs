using Assets.Script.Upgrades;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AscendProgressScript : MonoBehaviour
{
    public TextMeshProUGUI TextCredits;
    public Image ProgressBarImage;
    public GameObject AscendRoot;
    private float _nextCreditUpdate;

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
        // Additive
        float passiveAdditiveMultiplier = SaveGame.Members.BoughtPassiveX2_1 ? 2 : 1;
        passiveAdditiveMultiplier += SaveGame.Members.BoughtPassiveX2_2 ? 2 : 0;
        passiveAdditiveMultiplier += SaveGame.Members.BoughtPassiveX4_1 ? 4 : 0;
        PlayerUpgrades.Data.PassiveIncomeAscendMultiplier = passiveAdditiveMultiplier;

        // 10% per diamond
        PlayerUpgrades.Data.PassiveIncomeDiamondMultiplier = SaveGame.Members.DiamondCount * 0.1f;
    }

    void Update()
    {
        // Apply ascend bonuses
        ApplyAscendPermanentBonuses();

        if (G.D.GameTime < _nextCreditUpdate)
            return;

        // Credits are updated every CreditUpdateRate, not every frame. Currently it MUST be
        // once per second. Or scale TotalPassiveIncome to fit delta time.
        const float CreditUpdateRate = 1.0f;
        _nextCreditUpdate = G.D.GameTime + CreditUpdateRate;

        SaveGame.Members.MonsterCreditsXp += GameManager.Instance.TotalPassiveIncome;
        Decimal256 xpForNextLevel = UpgradeProgression.MonsterCreditXpForNextLevel(SaveGame.Members.MonsterCreditsLifetimeTemp);
        if (SaveGame.Members.MonsterCreditsXp > xpForNextLevel)
        {
            SaveGame.Members.MonsterCreditsXp -= xpForNextLevel;
            SaveGame.Members.MonsterCreditsLifetimeTemp++;
            SaveGame.Members.MonsterCreditsTemp++;
        }

        float t = (float)(SaveGame.Members.MonsterCreditsXp.ToDouble() / xpForNextLevel.ToDouble());
        ProgressBarImage.fillAmount = t;

        float pct = t * 100.0f;
        TextCredits.text = $"Monster credits: {SaveGame.Members.MonsterCreditsTemp}\n<size=-3>Next: {pct:#0.0}%";
    }
}
