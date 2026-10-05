using System;
using TMPro;
using UnityEngine;

public class X2BonusScript : MonoBehaviour
{
    public TextMeshProUGUI BonusText;

    // "You get X% bonus income for every 5 of X2 bought." - was static text in the scene, but X2 Mastery cards
    // change the percentage, so it's filled in from code. Found by name to avoid a scene wiring change.
    TextMeshProUGUI _descriptionText;
    const string GreenHex = "#8DBE4C";

    // X2s bought per tier, listed for unlocked tiers only (level > 0) so the full roster stays a surprise.
    // Tier list is shared with the progress popup (UpgradeTierList).
    static (string Name, Func<long> Level, Func<long> X2)[] Tiers => UpgradeTierList.Tiers;

    // The popup grows downward (top-left pivot, children top-anchored, OK button bottom-anchored) to fit the list.
    RectTransform _popupRect;
    float _popupBaseHeight;
    float _bonusTextBaseHeight;
    string _lastText;
    readonly System.Text.StringBuilder _sb = new();
    readonly System.Collections.Generic.List<(string Name, Func<long> Level, Func<long> X2)> _unlocked = new();

    // Up to this many unlocked tiers are listed in one centered column, more in two columns.
    const int SingleColumnMax = 6;

    void AppendHeader(string namePos, string countPos)
    {
        _sb.Append(namePos);
        _sb.Append("<size=85%><color=#999999>Tier</color></size>");
        _sb.Append(countPos);
        _sb.Append("<size=85%><color=#999999>X2</color></size>");
    }

    void AppendEntry((string Name, Func<long> Level, Func<long> X2) tier, string namePos, string countPos)
    {
        _sb.Append(namePos);
        _sb.Append($"<color=#cccccc>{tier.Name}</color>");
        _sb.Append(countPos);
        _sb.Append($"<color={GreenHex}>{tier.X2()}</color>");
    }

    void Awake()
    {
        var description = transform.Find("TextBonusDescription");
        if (description != null)
            _descriptionText = description.GetComponent<TextMeshProUGUI>();

        _popupRect = (RectTransform)transform;
        _popupBaseHeight = _popupRect.sizeDelta.y;
        _bonusTextBaseHeight = BonusText.rectTransform.sizeDelta.y;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    // Horizontal divider between the summary lines and the tier table, on the spacer line. It's a scene object (child
    // of BonusText) so its color/thickness can be set in the Inspector; this script only sets its position and width.
    public RectTransform Divider;
    const int SpacerLine = 2;
    const float DividerSideMargin = 0.04f; // fraction of the text width left free on each side

    void PlaceDivider()
    {
        if (Divider == null)
        {
            Divider = UiDivider.Create(BonusText.rectTransform, "Divider");
            Divider.sizeDelta = new Vector2(0, UiDivider.Thickness);
        }

        BonusText.ForceMeshUpdate();
        var info = BonusText.textInfo;
        bool show = _unlocked.Count > 0 && info.lineCount > SpacerLine;
        Divider.gameObject.SetActive(show);
        if (!show)
            return;

        var line = info.lineInfo[SpacerLine];
        var rect = BonusText.rectTransform.rect;
        float y = (line.ascender + line.descender) * 0.5f;
        Divider.localPosition = new Vector3(rect.center.x, y, 0);
        Divider.sizeDelta = new Vector2(rect.width * (1f - 2f * DividerSideMargin), Divider.sizeDelta.y); // keep the authored thickness
    }

    void Update()
    {
        long bought = PlayerUpgrades.Data.NumberOfX2Bought;
        long bonuses = bought / 5;
        long next = (bonuses + 1) * 5;

        _sb.Clear();
        _sb.Append($"Current: <color={GreenHex}>{bought}</color>, next: <color={GreenHex}>{next}</color>\r\nBonus: <color={GreenHex}>{PlayerUpgrades.Data.PassiveIncomeX2Multiplier * 100:0}</color>%");

        // Unlocked tiers: one centered column when there are few, else two columns filled top-to-bottom first (like
        // the upgrade list in the game view), then the right column.
        _unlocked.Clear();
        foreach (var tier in Tiers)
        {
            if (tier.Level() > 0)
                _unlocked.Add(tier);
        }

        if (_unlocked.Count > 0)
        {
            // Left-align just the list; the lines above keep the text's own alignment. A short spacer line (line 2)
            // makes room for the divider between the summary and the table.
            _sb.Append("\r\n<align=left><size=130%> </size>");

            if (_unlocked.Count <= SingleColumnMax)
            {
                // Few tiers: one centered column, so a short list still reads as a table.
                AppendHeader("\r\n<pos=24%>", "<pos=70%>");
                foreach (var tier in _unlocked)
                    AppendEntry(tier, "\r\n<pos=24%>", "<pos=70%>");
            }
            else
            {
                AppendHeader("\r\n<pos=4%>", "<pos=38%>");
                AppendHeader("<pos=53%>", "<pos=87%>");
                int rows = (_unlocked.Count + 1) / 2;
                for (int r = 0; r < rows; ++r)
                {
                    // Name at the column start, X2 count at a fixed spot so the numbers line up.
                    AppendEntry(_unlocked[r], "\r\n<pos=4%>", "<pos=38%>");
                    if (r + rows < _unlocked.Count)
                        AppendEntry(_unlocked[r + rows], "<pos=53%>", "<pos=87%>");
                }
            }
        }

        string text = _sb.ToString();
        if (text != _lastText)
        {
            _lastText = text;
            BonusText.text = text;

            // Resize the text box and the popup to fit (only grows from the scene-authored size).
            var textRect = BonusText.rectTransform;
            float needed = BonusText.GetPreferredValues(text, textRect.rect.width, 0).y;
            float textHeight = Mathf.Max(_bonusTextBaseHeight, needed);
            textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textHeight);
            _popupRect.sizeDelta = new Vector2(_popupRect.sizeDelta.x, _popupBaseHeight + (textHeight - _bonusTextBaseHeight));

            PlaceDivider();
        }

        if (_descriptionText != null)
        {
            double pctPerRank = UpgradeManager.X2BonusPerRank() * 100;
            _descriptionText.text = $"You get <color={GreenHex}>{pctPerRank:0}%</color> bonus income for every <color={GreenHex}>5</color> of X2 bought.";
        }
    }
}
