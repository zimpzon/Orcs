using Assets.Script.Misc;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class ColorLink : MonoBehaviour
{
    public ColorPalette palette;
    public ColorType colorType;

    // Optional override for dynamic gameplay states
    public bool useOverrideColor = false;
    public Color overrideColor;

    private Image uiImage;
    private SpriteRenderer spriteRenderer;
    private TextMeshProUGUI tmpText;
    private Button uiButton;

    private bool lastInteractableState = true;

    private void OnEnable()
    {
        if (palette != null)
            palette.OnColorsChanged += ApplyColor;

        ApplyColor();
    }

    private void OnDisable()
    {
        if (palette != null)
            palette.OnColorsChanged -= ApplyColor;
    }

    private void OnValidate()
    {
        ApplyColor();
    }

    private void Update()
    {
        // For buttons, track interactable state and update text color live
        if (uiButton == null) uiButton = GetComponent<Button>();
        if (uiButton == null) return;
        // Tint-only types: the button's text is owned by its own script.
        if (colorType == ColorType.DialogPanel || colorType == ColorType.ButtonMain || colorType == ColorType.ButtonIcon) return;

        if (tmpText == null) tmpText = GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText == null) return;

        if (uiButton.interactable != lastInteractableState)
        {
            lastInteractableState = uiButton.interactable;
            UpdateButtonTextColorBasedOnInteractable();
        }
    }

    private void UpdateButtonTextColorBasedOnInteractable()
    {
        if (useOverrideColor)
        {
            // If override active, keep overrideColor always
            tmpText.color = overrideColor;
            return;
        }

        if (uiButton.interactable)
        {
            tmpText.color = ColorFromEnum.Get(palette, ColorType.ButtonDefault);
        }
        else
        {
            tmpText.color = ColorFromEnum.Get(palette, ColorType.ButtonTextDisabled);
        }
    }

    public void ApplyColor()
    {
        if (palette == null) return;

        if (uiButton == null) uiButton = GetComponent<Button>();

        if (uiButton != null)
        {
            ApplyButtonColors(uiButton);
            return;
        }

        ApplyStandardColors();
    }

    private void ApplyStandardColors()
    {
        Color colorToUse = useOverrideColor ? overrideColor : ColorFromEnum.Get(palette, colorType);

        if (uiImage == null) uiImage = GetComponent<Image>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (tmpText == null) tmpText = GetComponent<TextMeshProUGUI>();

        if (uiImage != null) uiImage.color = colorToUse;
        if (spriteRenderer != null) spriteRenderer.color = colorToUse;
        if (tmpText != null) tmpText.color = colorToUse;
    }

    const float PressedFactor = 0.857f;
    const float DisabledFactor = 0.7f;

    static Color Darker(Color c, float factor) => new Color(c.r * factor, c.g * factor, c.b * factor, c.a);

    private void ApplyButtonColors(Button button)
    {
        if (tmpText == null) tmpText = GetComponentInChildren<TextMeshProUGUI>();

        switch (colorType)
        {
            case ColorType.ButtonDefault:
            case ColorType.ButtonDefaultPressed:
            case ColorType.ButtonDefaultDisabled:
                if (button.targetGraphic is Graphic graphic)
                {
                    Color bgColor = useOverrideColor ? overrideColor : ColorFromEnum.Get(palette, colorType);
                    graphic.color = bgColor;
                }
                break;

            case ColorType.ButtonTextDisabled:
                UpdateButtonTextColorBasedOnInteractable();
                break;

            case ColorType.ButtonMain:
            case ColorType.ButtonIcon:
                // Image stays white, the button tint gives the color: palette color normally, darker when pressed
                // and much darker when disabled (same ratios the buttons were authored with).
                {
                    Color normal = useOverrideColor ? overrideColor : ColorFromEnum.Get(palette, colorType);
                    var colors = button.colors;
                    colors.normalColor = colors.highlightedColor = colors.selectedColor = normal;
                    colors.pressedColor = Darker(normal, PressedFactor);
                    colors.disabledColor = Darker(normal, DisabledFactor);
                    button.colors = colors;
                }
                break;

            case ColorType.DialogPanel:
                // A panel that is also a button (e.g. the QuestionMark): its Image stays white and the button's
                // color tint provides the panel color, the same in every state.
                {
                    Color panelColor = useOverrideColor ? overrideColor : ColorFromEnum.Get(palette, colorType);
                    var colors = button.colors;
                    colors.normalColor = colors.highlightedColor = colors.pressedColor =
                        colors.selectedColor = colors.disabledColor = panelColor;
                    button.colors = colors;
                }
                break;

            default:
                if (button.targetGraphic is Graphic defaultGraphic)
                {
                    Color defColor = useOverrideColor ? overrideColor : palette.ButtonDefault;
                    defaultGraphic.color = defColor;
                }
                break;
        }
    }
}
