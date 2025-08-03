using Assets.Script.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AscendProgressScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject NotImplemented;
    public TextMeshProUGUI TextCredits;
    public Image ProgressBarImage;
    private float _nextUpdate;

    public void OnPointerEnter(PointerEventData eventData)
    {
        NotImplemented.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        NotImplemented.SetActive(false);
    }

    void Update()
    {
        if (G.D.GameTime < _nextUpdate)
            return;

        _nextUpdate = G.D.GameTime + 1.0f;

        SaveGame.Members.AscendXp += GameManager.Instance.TotalPassiveIncome;
        Decimal256 xpForNextLevel = UpgradeProgression.AscendXpForNextLevel(SaveGame.Members.AscendLevelTemp);
        if (SaveGame.Members.AscendXp > xpForNextLevel)
        {
            SaveGame.Members.AscendXp -= xpForNextLevel;
            SaveGame.Members.AscendLevelTemp++;
        }

        float t = (float)(SaveGame.Members.AscendXp.ToDouble() / xpForNextLevel.ToDouble());
        ProgressBarImage.fillAmount = t;

        float pct = t * 100.0f;
        TextCredits.text = $"Monster credits: {SaveGame.Members.AscendLevelTemp}\n<size=-3>Next: {pct:00.00}%";
    }
}
