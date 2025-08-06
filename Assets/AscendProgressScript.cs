using Assets.Script.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscendProgressScript : MonoBehaviour
{
    public TextMeshProUGUI TextCredits;
    public Image ProgressBarImage;
    public GameObject AscendRoot;
    private float _nextUpdate;

    public void OnShowClick()
    {
        AscendRoot.SetActive(true);
    }

    public void OnCloseClick()
    {
        AscendRoot.SetActive(false);
    }

    void Update()
    {
        if (G.D.GameTime < _nextUpdate)
            return;

        _nextUpdate = G.D.GameTime + 1.0f;

        // 
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
