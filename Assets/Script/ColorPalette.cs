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

    public event Action OnColorsChanged;

    private void OnValidate()
    {
        OnColorsChanged?.Invoke();
    }
}
