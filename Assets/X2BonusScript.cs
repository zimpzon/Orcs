using TMPro;
using UnityEngine;

public class X2BonusScript : MonoBehaviour
{
    public TextMeshProUGUI BonusText;

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        long bought = PlayerUpgrades.Data.NumberOfX2Bought;
        long bonuses = bought / 5;
        long next = (bonuses + 1) * 5;
        BonusText.text = $"Current: <color=#8DBE4C>{bought}</color>, next: <color=#8DBE4C>{next}</color>\r\nBonus: <color=#8DBE4C>{PlayerUpgrades.Data.PassiveIncomeX2Multiplier * 100:0}</color>%";
    }
}
