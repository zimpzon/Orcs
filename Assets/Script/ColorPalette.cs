using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Game/Color Palette")]
public class ColorPalette : ScriptableObject
{
    public Color PanelDefault = Color.white;
    public Color BackgroundDefault = Color.gray;
    public Color PanelSecondary = Color.green;
    public Color PanelCanAfford = Color.green;
    public Color PanelCannotAfford = Color.green;
    public Color TextMoney = Color.green;
    public Color TextPerSec = Color.green;
    public Color TextHeaderDefault = Color.green;
    public Color TextMainDefault = Color.green;
    public Color TextSecondaryDefault = Color.green;
    public Color TextGameInfo = Color.green;
    public Color ButtonDefault = Color.green;
    public Color ButtonDefaultPressed = Color.green;
    public Color ButtonDefaultDisabled = Color.green;
    public Color ButtonTextDisabled = Color.green;
    public Color ArenaBackground = Color.green;
    public Color ArenaHpBar = Color.green;
    // All panel-3 panels and dialogs (ColorLink with ColorType.DialogPanel).
    public Color DialogPanel = new Color(0.4509804f, 0.4117647f, 0.3764706f, 1f);
    // Buttons (ColorLink ButtonMain / ButtonIcon): normal color; pressed and disabled are darker versions of it.
    // ButtonIcon = the top icon buttons (skins, bestiary, X2, victory...), ButtonMain = everything else.
    // Not used by the blue tier buy / X2 buttons.
    public Color ButtonMain = new Color(0.7372549f, 0.67058825f, 0.6156863f, 1f);
    // Panels inside a dialog (e.g. the rebirth dialog sections), a bit darker than DialogPanel so they show.
    public Color InnerPanel = new Color(0.41f, 0.354f, 0.304f, 1f);
    public Color ButtonIcon = new Color(0.7372549f, 0.67058825f, 0.6156863f, 1f);

    public event Action OnColorsChanged;

    private void OnValidate()
    {
        OnColorsChanged?.Invoke();
    }
}
