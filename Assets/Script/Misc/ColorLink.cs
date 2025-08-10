using Assets.Script.Misc;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
