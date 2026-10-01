using TMPro;
using UnityEngine;

public class X2BonusScript : MonoBehaviour
{
    public TextMeshProUGUI BonusText;

    // "You get X% bonus income for every 5 of X2 bought." - was static text in the scene, but X2 Mastery cards
    // change the percentage, so it's filled in from code. Found by name to avoid a scene wiring change.
    TextMeshProUGUI _descriptionText;
    const string GreenHex = "#8DBE4C";

    void Awake()
    {
        var description = transform.Find("TextBonusDescription");
        if (description != null)
            _descriptionText = description.GetComponent<TextMeshProUGUI>();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    void Update()
    {
        long bought = PlayerUpgrades.Data.NumberOfX2Bought;
        long bonuses = bought / 5;
        long next = (bonuses + 1) * 5;
        BonusText.text = $"Current: <color={GreenHex}>{bought}</color>, next: <color={GreenHex}>{next}</color>\r\nBonus: <color={GreenHex}>{PlayerUpgrades.Data.PassiveIncomeX2Multiplier * 100:0}</color>%";

        if (_descriptionText != null)
        {
            double pctPerRank = UpgradeManager.X2BonusPerRank() * 100;
            _descriptionText.text = $"You get <color={GreenHex}>{pctPerRank:0}%</color> bonus income for every <color={GreenHex}>5</color> of X2 bought.";
        }
    }
}
