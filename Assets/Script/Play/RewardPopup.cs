using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Reward message in the style of the item drop / welcome back popups: the reward popup (gold rays behind a yellow
// header, chime), the reward big and gold on a darkened band (PopupBuilder) - money counts up from 0 - and a small
// grey line with the details. Chests and mystery bonuses.
public static class RewardPopup
{
    const string Grey = "#C8C8C8";

    // Text on the band (e.g. "X10 INCOME").
    public static void Show(string header, string bandText, string details)
        => Build(header, Big(bandText), details, null);

    // Money on the band, counting up to `amount`.
    public static void ShowMoney(string header, Decimal512 amount, string details)
        => Build(header, MoneyText(amount), details, amount);

    static void Build(string header, string band, string details, Decimal512? countUp)
    {
        var canvas = GameCanvasScript.Instance;
        if (canvas == null)
            return;

        var popup = canvas.CreatePopup($"<color={TextColors.Yellow}><size=130%>{header}</size></color>", celebrate: true);
        var headerText = popup.GetComponentInChildren<TextMeshProUGUI>();
        if (headerText == null)
            return;

        var bandText = PopupBuilder.AddBand(headerText, band, "RewardBand");
        if (countUp.HasValue)
        {
            // Sized for the final amount up front, so the popup doesn't grow while the number counts up.
            var size = bandText.GetPreferredValues();
            if (!bandText.TryGetComponent<LayoutElement>(out var layout)) // the copied prefab text usually has one
                layout = bandText.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = size.x;
            layout.minHeight = size.y;
            bandText.gameObject.AddComponent<CountUpMoney>().Init(countUp.Value, MoneyText);
        }

        var note = PopupBuilder.CloneText(headerText, PopupBuilder.AfterBand(bandText), $"<color={Grey}>{details}</color>");
        PopupBuilder.FixMargins(headerText, note);
    }

    static string Big(string text) => $"<color={TextColors.Gold}><size=170%>{text}</size></color>";

    static string MoneyText(Decimal512 value) => Big($"+${Format512.Format(value)}");
}
