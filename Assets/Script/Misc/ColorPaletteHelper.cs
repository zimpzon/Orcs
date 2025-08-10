using UnityEngine;

namespace Assets.Script.Misc
{
    public enum ColorType
    {
        PanelDefault,
        BackgroundDefault,
        PanelSecondary,
        PanelCanAfford,
        PanelCannotAfford,
        TextMoney,
        TextPerSec,
        TextHeaderDefault,
        TextMainDefault,
        TextMainDisabled,
        TextSecondaryDefault,
        TextGameInfo,
        ButtonDefault,
        ButtonDefaultPressed,
        ButtonDefaultDisabled,
        ButtonTextDisabled,
        ArenaBackground,
        ArenaHpBar,
    }

    public static class ColorFromEnum
    {
        public static Color Get(ColorPalette palette, ColorType colorType)
        {
            return colorType switch
            {
                ColorType.PanelDefault => palette.PanelDefault,
                ColorType.BackgroundDefault => palette.BackgroundDefault,
                ColorType.PanelSecondary => palette.PanelSecondary,
                ColorType.PanelCanAfford => palette.PanelCanAfford,
                ColorType.PanelCannotAfford => palette.PanelCannotAfford,
                ColorType.TextMoney => palette.TextMoney,
                ColorType.TextPerSec => palette.TextPerSec,
                ColorType.TextHeaderDefault => palette.TextHeaderDefault,
                ColorType.TextMainDefault => palette.TextMainDefault,
                ColorType.TextMainDisabled => palette.ButtonTextDisabled,
                ColorType.TextSecondaryDefault => palette.TextSecondaryDefault,
                ColorType.TextGameInfo => palette.TextGameInfo,
                ColorType.ButtonDefault => palette.ButtonDefault,
                ColorType.ButtonDefaultPressed => palette.ButtonDefaultPressed,
                ColorType.ButtonDefaultDisabled => palette.ButtonDefaultDisabled,
                ColorType.ArenaBackground => palette.ArenaBackground,
                ColorType.ArenaHpBar => palette.ArenaHpBar,
                _ => Color.magenta
            };
        }
    }
}
