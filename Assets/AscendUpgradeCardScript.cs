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
    public Color CanAffordColor = Color.white;
    public Color CannotAffordColor = Color.gray;
    public Color OwnedColor = Color.black;
    public Image ButtonOverlay;
    public Image CardOverlay;
    public Button ButtonBuy;
    public TextMeshProUGUI ButtonBuyText;
    public TextMeshProUGUI OwnedText;
    public AscendUpgradeCardId CardId = AscendUpgradeCardId.NotSet;

    Image _background;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    public void Update()
    {
        // Just for testing owned
        bool isOwned = CardId == AscendUpgradeCardId.NotSet;
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
            bool canAfford = SaveGame.Members.DiamondCount >= Cost;
            _background.color = canAfford ? CanAffordColor : CannotAffordColor;
            ButtonOverlay.enabled = !canAfford;
            ButtonBuy.interactable = canAfford;
        }
        ButtonBuyText.text = $"{Cost} <sprite=0>";
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
