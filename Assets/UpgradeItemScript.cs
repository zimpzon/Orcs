using MoreMountains.Tools;
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

    public void UpdateUi(bool canAfford, long priceNext, long currentLevel, long maxLevel = -1)
    {
        BuyButtonOverlay.enabled = !canAfford;
        BuyButton.interactable = canAfford;
        SetPrice(priceNext);
        SetLevel(currentLevel, maxLevel);
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
        _latestLevel = level;
    }

    public void SetPopupText()
    {
        PopupManagerScript.Instance.SetText(UpgradeManager.Instance.GetText(this));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetPopupText();
        PopupManagerScript.Instance.PlaceLeftOfTarget(GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PopupManagerScript.Instance.Hide();
    }
}
