using System;
using Assets.Script.Misc;
using TMPro;
using UnityEngine;

// "WELCOME BACK!" for offline income (GameManager.PayOfflineIncome): the reward popup (gold rays behind the header,
// chime), the time away in soft grey, then the amount big and gold on a darkened band (PopupBuilder), counting up
// from 0 and popping when it lands, and a small note on what it is.
public static class WelcomeBackPopup
{
    public static void Show(TimeSpan away, Decimal512 amount, double sharePercent)
    {
        var canvas = GameCanvasScript.Instance;
        if (canvas == null)
            return;

        var popup = canvas.CreatePopup($"<color={TextColors.Yellow}><size=130%>WELCOME BACK!</size></color>", celebrate: true);
        var header = popup.GetComponentInChildren<TextMeshProUGUI>();
        if (header == null)
            return;

        var awayText = PopupBuilder.CloneText(header, header.transform.GetSiblingIndex() + 1,
            $"\n<color={TextColors.Grey}>You were away for\n</color><color={TextColors.GreyLight}>{FormatTime.Format(away.Days, away.Hours, away.Minutes)}\n</color>");

        // Sized for the final amount up front, so the popup doesn't grow while the number counts up.
        var amountText = PopupBuilder.AddBand(awayText, AmountText(amount), "OfflineIncomeBand",
            gapAbove: 14f, gapBelow: 14f); // more air than the default 8 around the big amount
        var size = amountText.GetPreferredValues();
        // (the copied prefab text already has a LayoutElement)
        if (!amountText.TryGetComponent<UnityEngine.UI.LayoutElement>(out var layout))
            layout = amountText.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        layout.minWidth = size.x;
        layout.minHeight = size.y;
        amountText.gameObject.AddComponent<CountUpMoney>().Init(amount, AmountText);

        var note = PopupBuilder.CloneText(header, PopupBuilder.AfterBand(amountText),
            $"<size=85%><color={TextColors.Grey}>Offline income: <color={TextColors.GreyLight}>{sharePercent:0}</color>% of your income</color></size>");
        PopupBuilder.FixMargins(header, note);
    }

    static string AmountText(Decimal512 value)
        => $"<color={TextColors.Gold}><size=170%>+${Format512.Format(value)}</size></color>";
}
