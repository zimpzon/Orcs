using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AscendUpgradeCardId {
    NotSet,
    PlusTwoDaggers,
    PassiveIncomeX2_1,
    PassiveIncomeX2_2,
    GoldX2_1,
    GoldX2_2,
};

public class AscendUpgradeCardScript : MonoBehaviour
{
    public long Cost = 1;
    public Button ButtonBuy;
    public TextMeshProUGUI ButtonBuyText;
    public AscendUpgradeCardId CardId = AscendUpgradeCardId.NotSet;

    private Color _textBaseColor;

    private void Awake()
    {
        _textBaseColor = ButtonBuyText.color;
    }

    public void Start()
    {
        bool canAfford = SaveGame.Members.DiamondCount >= Cost;
        ButtonBuy.interactable = canAfford; 
        ButtonBuyText.text = $"{Cost} <sprite=0>";
        //ButtonBuyText.color = canAfford ? _textBaseColor : Color.gray;
    }

    public void OnBuy()
    {
        Debug.Log("Bought " + CardId);
        switch (CardId)
        {
            default:
                break;
        };
    }
}
