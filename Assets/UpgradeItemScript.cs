using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeItemScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage BuyButtonOverlay;
    public Button BuyButton;
    public TextMeshProUGUI PriceLabel;
    public TextMeshProUGUI LevelLabel;

    long _latestPrice = -1;
    long _latestLevel = -1;

    public void SetCanAfford(bool canAfford, long priceNext, long currentLevel)
    {
        BuyButtonOverlay.enabled = !canAfford;
        BuyButton.interactable = canAfford;
        SetPrice(priceNext);
        SetLevel(currentLevel);
    }

    void SetPrice(long price)
    {
        if (price == _latestPrice)
            return;

        PriceLabel.text = $"${price}";
        _latestPrice = price;
    }

    void SetLevel(long level, long maxLevel = -1)
    {
        if (level == _latestLevel)
            return;

        if (level == 0)
        {
            LevelLabel.text = "";
        }
        else if (maxLevel == -1)
        {
            LevelLabel.text = $"{level}";
        }
        else
        {
            LevelLabel.text = $"{level}/{maxLevel}";
        }
        _latestPrice = level;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PopupManagerScript.Instance.SetText(UpgradeManager.Instance.GetText(this));
        PopupManagerScript.Instance.PlaceLeftOfTarget(GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopupManagerScript.Instance.Hide();
    }
}
