using Assets.Script.Misc;
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
    public Color HighlightColor;

    Image _background;
    Color _baseColor;
    Color _colorTarget;
    long _latestPrice = -1;
    long _latestLevel = -1;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _baseColor = _background.color;
    }


    public void UpdateUi(bool canAfford, long priceNext, long currentLevel)
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

        PriceLabel.text = $"${Format64.Format(price)}";
        _latestPrice = price;
    }

    void SetLevel(long level)
    {
        if (level == _latestLevel)
            return;

        if (level == 0)
        {
            LevelLabel.text = "";
        }
        else
        {
            LevelLabel.text = $"{level}";
        }
        _latestLevel = level;
    }

    public void SetPopupText()
    {
        PopupManagerScript.Instance.SetText(UpgradeManager.Instance.GetText(this));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _background.color = HighlightColor;
        SetPopupText();
        PopupManagerScript.Instance.PlaceLeftOfTarget(GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _background.color = _baseColor;
        PopupManagerScript.Instance.Hide();
    }
}
