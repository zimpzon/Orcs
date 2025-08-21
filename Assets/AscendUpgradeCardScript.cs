using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AscendUpgradeCardId
{
    NotSet,
    PassiveIncomeX2_1,
    PassiveIncomeX2_2,
    PassiveIncomeX4_1,
    FasterMystery,
};

public class AscendUpgradeCardScript : MonoBehaviour
{
    public long Cost = 1;
    public Color CanAffordColor = Color.white;
    public Color CannotAffordColor = Color.gray;
    public Color OwnedColor = Color.black;
    public Image ButtonOverlay;
    public Image CardOverlay;
    public Button ButtonBuy;
    public TextMeshProUGUI ButtonBuyText;
    public TextMeshProUGUI OwnedText;
    public AscendUpgradeCardId CardId = AscendUpgradeCardId.NotSet;
    //public AscendUpgradeCardScript UpgradeCardPassiveX2_1;
    //public AscendUpgradeCardScript UpgradeCardPassiveX2_2;
    //public AscendUpgradeCardScript UpgradeCardPassiveX4_1;

    Image _background;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    void Update()
    {
        UpdateUi();
    }

    private void UpdateUi()
    {
        // Diamonds etc.
        AscendDecisionScript.Instance.UpdateUi();

        bool isOwned = CardId switch
        {
            AscendUpgradeCardId.PassiveIncomeX2_1 => SaveGame.Members.BoughtPassiveX2_1,
            AscendUpgradeCardId.PassiveIncomeX2_2 => SaveGame.Members.BoughtPassiveX2_2,
            AscendUpgradeCardId.PassiveIncomeX4_1 => SaveGame.Members.BoughtPassiveX4_1,
            AscendUpgradeCardId.FasterMystery => SaveGame.Members.BoughtFasterMystery,
            _ => throw new NotImplementedException()
        };

        UpdateUiForSingleCard(isOwned);
    }

    private void UpdateUiForSingleCard(bool isOwned)
    {
        OwnedText.enabled = isOwned;
        CardOverlay.enabled = isOwned;

        if (isOwned)
        {
            _background.color = OwnedColor;
            ButtonOverlay.enabled = false;
            ButtonBuy.interactable = false;
        }
        else
        {
            bool canAfford = SaveGame.Members.DiamondCount_09_08_2025 >= Cost;
            _background.color = canAfford ? CanAffordColor : CannotAffordColor;
            ButtonOverlay.enabled = !canAfford;
            ButtonBuy.interactable = canAfford;
        }
        ButtonBuyText.text = $"{Cost} <sprite=0>";
    }

    public void OnBuy()
    {
        Debug.Log("Bought " + CardId);
        if (CardId == AscendUpgradeCardId.PassiveIncomeX2_1)
        {
            SaveGame.Members.BoughtPassiveX2_1 = true;
        }
        else if (CardId == AscendUpgradeCardId.PassiveIncomeX2_2)
        {
            SaveGame.Members.BoughtPassiveX2_2 = true;
        }
        else if (CardId == AscendUpgradeCardId.PassiveIncomeX4_1)
        {
            SaveGame.Members.BoughtPassiveX4_1 = true;
        }
        else if (CardId == AscendUpgradeCardId.FasterMystery)
        {
            SaveGame.Members.BoughtFasterMystery = true;
            SaveGame.Members.QuestionMarkRealTimeLeft *= 0.5f;        }
        else
            throw new NotImplementedException();

        SaveGame.Members.DiamondCount_09_08_2025 -= Cost;
        SaveGame.Save();
        AscendDecisionScript.Instance.UpdateUi();
    }
}
