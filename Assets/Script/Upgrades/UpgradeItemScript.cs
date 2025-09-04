using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeItemScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage BuyButtonOverlay;
    public Button BuyButton;
    public RawImage X2ButtonOverlay;
    public Button X2Button;
    public TextMeshProUGUI PriceLabel;
    public TextMeshProUGUI LevelLabel;
    public Color CanAffordColor;
    public Color CanAffordfHighlightColor;
    public Color CannotAffordColor;
    public Color CannotAffordHighlightColor;

    Image _background;
    bool _isHovering;
    Decimal512 _latestPrice = 99999;
    long _latestLevel = 99999;
    bool _canAfford;

    private void Awake()
    {
        _background = GetComponent<Image>();
    }

    public void UpdateUi(bool canAfford, bool enableBtnX2, Decimal512 priceNext, long currentLevel)
    {
        BuyButtonOverlay.enabled = !canAfford;
        BuyButton.interactable = canAfford;

        X2ButtonOverlay.enabled = !enableBtnX2;
        X2Button.interactable = enableBtnX2;

        SetPrice(priceNext);
        SetLevel(currentLevel);

        _canAfford = canAfford;
    }

    void SetPrice(Decimal512 price)
    {
        if (price == _latestPrice)
            return;

        PriceLabel.text = $"${Format512.Format(price)}";
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
        _isHovering = true;
        SetPopupText();
        PopupManagerScript.Instance.PlaceLeftOfTarget(GetComponent<RectTransform>());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovering = false;
        PopupManagerScript.Instance.Hide();
    }

    float _nextUpdate;

    void Update()
    {
        if (_isHovering)
        {
            _background.color = _canAfford ? CanAffordfHighlightColor : CannotAffordHighlightColor;
        }
        else
        {
            _background.color = _canAfford ? CanAffordColor : CannotAffordColor;
        }

        if (_isHovering && G.D.GameTime > _nextUpdate)
        {
            SetPopupText();
            _nextUpdate = G.D.GameTime + 0.25f;
        }
    }
}
