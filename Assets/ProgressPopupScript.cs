using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPopupScript : MonoBehaviour
{
    public TextMeshProUGUI ProgressText;
    public Sprite CrownSprite;

    const string GreenHex = "#8DBE4C";
    const string GoldHex = "#FFD54A";

    // The popup is authored narrow; widen it (staying centered where it was) and grow it downward to fit the text.
    // Top-left pivot, text top-anchored, OK button bottom-center anchored, so both adjustments are safe.
    const float ExtraWidth = 80f;
    RectTransform _popupRect;
    float _popupBaseHeight;
    float _textBaseHeight;
    string _lastText;

    // A crown on each side of the "GLORIOUS VICTORY" header line, gently bobbing up and down.
    const float CrownHeightMul = 0.7f;  // crown size relative to the header's drawn glyph height
    const float CrownGap = 8f;          // glyph edge of the header to the crown's edge
    const float BobAmount = 3f;
    const float BobSpeed = 2.2f;
    RectTransform[] _crowns;
    Vector3[] _crownBase;

    void Awake()
    {
        _popupRect = (RectTransform)transform;
        _popupRect.sizeDelta += new Vector2(ExtraWidth, 0);
        _popupRect.anchoredPosition -= new Vector2(ExtraWidth * 0.5f, 0);

        var textRect = ProgressText.rectTransform;
        textRect.sizeDelta += new Vector2(ExtraWidth, 0);

        _popupBaseHeight = _popupRect.sizeDelta.y;
        _textBaseHeight = textRect.sizeDelta.y;

        if (CrownSprite != null)
        {
            _crowns = new RectTransform[2];
            _crownBase = new Vector3[2];
            for (int i = 0; i < 2; ++i)
            {
                var go = new GameObject(i == 0 ? "CrownLeft" : "CrownRight", typeof(RectTransform));
                go.transform.SetParent(textRect, false);
                var image = go.AddComponent<Image>();
                image.sprite = CrownSprite;
                image.preserveAspect = true;
                image.raycastTarget = false;
                var rect = (RectTransform)go.transform;
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
                _crowns[i] = rect;
            }
        }
    }

    // Place the crowns from the drawn glyphs of the first (header) line, in the text's local space.
    void PlaceCrowns()
    {
        if (_crowns == null)
            return;

        ProgressText.ForceMeshUpdate();
        var info = ProgressText.textInfo;
        if (info.lineCount == 0)
            return;

        var line = info.lineInfo[0];
        bool any = false;
        Vector3 min = Vector3.zero, max = Vector3.zero;
        for (int c = line.firstCharacterIndex; c <= line.lastCharacterIndex && c < info.characterCount; ++c)
        {
            var ch = info.characterInfo[c];
            if (!ch.isVisible)
                continue;

            Vector3 bl = ch.vertex_BL.position, tr = ch.vertex_TR.position;
            min = any ? Vector3.Min(min, bl) : bl;
            max = any ? Vector3.Max(max, tr) : tr;
            any = true;
        }

        if (!any)
            return;

        float size = (max.y - min.y) * CrownHeightMul;
        float centerY = (min.y + max.y) * 0.5f;
        for (int i = 0; i < 2; ++i)
        {
            _crowns[i].sizeDelta = new Vector2(size, size);
            float x = i == 0 ? min.x - CrownGap - size * 0.5f : max.x + CrownGap + size * 0.5f;
            _crownBase[i] = new Vector3(x, centerY, 0);
        }
    }

    void BobCrowns()
    {
        if (_crowns == null)
            return;

        for (int i = 0; i < _crowns.Length; ++i)
        {
            // Up and down only, slightly out of step with each other.
            float wave = Mathf.Sin(Time.unscaledTime * BobSpeed + i * 1.0f);
            _crowns[i].localPosition = _crownBase[i] + new Vector3(0, wave * BobAmount, 0);
        }
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

    // One aligned row: label with its count right after it (grey), percentage in a fixed column.
    static string Row(string label, double pct, int have, int total)
        => $"<pos={LabelColumn}><color=#cccccc>{label}</color> <color=#999999>({have}/{total})</color><pos={PctColumn}><color={GreenHex}>{pct:0}%</color>";

    // Percentage column, just past the longest row ("Upgrade tiers bought (23/23)").
    // Shifted right so the table sits centered in the dialog (it spans about 62% of the width).
    const string LabelColumn = "16%";
    const string PctColumn = "74%";

    void Update()
    {
        BobCrowns();

        // Shared with the achievement check and the bottom bar; tiers count the most ever bought, so a rebirth
        // doesn't undo victory.
        var p = GameCompletion.GetProgress();
        int enemiesUnlocked = p.EnemiesUnlocked, enemiesTotal = p.EnemiesTotal;
        int skinsUnlocked = p.SkinsUnlocked, skinsTotal = p.SkinsTotal;
        int tiersBought = p.TiersBought, tiersTotal = p.TiersTotal;
        double enemyPct = p.EnemyPct, skinPct = p.SkinPct, tierPct = p.TierPct, totalPct = p.TotalPct;
        bool won = totalPct >= 99.999 || SaveGame.Members.Achieved.Contains(Achieved.Completion100);

        string subtitle = won
            ? $"<color={GoldHex}>is yours, champion!</color>"
            : "<color=#cccccc>awaits those who reach 100%</color>";

        string text =
            $"<align=center><size=170%><b><color={GoldHex}>GLORIOUS VICTORY</color></b></size>\r\n" +
            $"<size=90%>{subtitle}</size>\r\n" +
            "\r\n" +
            "<align=left>" +
            Row("Enemies unlocked", enemyPct, enemiesUnlocked, enemiesTotal) + "\r\n" +
            Row("Skins unlocked", skinPct, skinsUnlocked, skinsTotal) + "\r\n" +
            Row("Upgrade tiers reached", tierPct, tiersBought, tiersTotal) + "\r\n" +
            "\r\n" +
            $"<pos={LabelColumn}><b>Total completion</b><pos={PctColumn}><b><color={GreenHex}>{totalPct:0}%</color></b>";

        if (text == _lastText)
            return;

        _lastText = text;
        ProgressText.text = text;

        // Grow the text box and the popup downward to fit (never smaller than authored).
        var textRect = ProgressText.rectTransform;
        float needed = ProgressText.GetPreferredValues(text, textRect.rect.width, 0).y;
        float textHeight = Mathf.Max(_textBaseHeight, needed);
        textRect.sizeDelta = new Vector2(textRect.sizeDelta.x, textHeight);
        _popupRect.sizeDelta = new Vector2(_popupRect.sizeDelta.x, _popupBaseHeight + (textHeight - _textBaseHeight));

        PlaceCrowns();
        BobCrowns();
    }
}
