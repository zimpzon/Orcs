using System.Linq;
using TMPro;
using UnityEngine;

public class ProgressPopupScript : MonoBehaviour
{
    public TextMeshProUGUI ProgressText;

    const string GreenHex = "#8DBE4C";
    const string GoldHex = "#FFD54A";

    // The popup is authored narrow; widen it (staying centered where it was) and grow it downward to fit the text.
    // Top-left pivot, text top-anchored, OK button bottom-center anchored, so both adjustments are safe.
    const float ExtraWidth = 80f;
    RectTransform _popupRect;
    float _popupBaseHeight;
    float _textBaseHeight;
    string _lastText;

    void Awake()
    {
        _popupRect = (RectTransform)transform;
        _popupRect.sizeDelta += new Vector2(ExtraWidth, 0);
        _popupRect.anchoredPosition -= new Vector2(ExtraWidth * 0.5f, 0);

        var textRect = ProgressText.rectTransform;
        textRect.sizeDelta += new Vector2(ExtraWidth, 0);

        _popupBaseHeight = _popupRect.sizeDelta.y;
        _textBaseHeight = textRect.sizeDelta.y;
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
        => $"<pos=4%><color=#cccccc>{label}</color> <color=#aaaaaa>({have}/{total})</color><pos={PctColumn}><color={GreenHex}>{pct:0}%</color>";

    // Percentage column, just past the longest row ("Upgrade tiers bought (23/23)").
    const string PctColumn = "62%";

    void Update()
    {
        (int enemiesUnlocked, int enemiesTotal) = EnemySpawner.GetTierUnlockProgress();
        (int skinsUnlocked, int skinsTotal) = SkinScript.GetUnlockProgress();

        // A tier counts as bought once at least one level of it has been purchased. Uses the shared tier list, so new
        // tiers are counted automatically.
        var tiers = UpgradeTierList.Tiers;
        int tiersBought = tiers.Count(t => t.Level() > 0);
        int tiersTotal = tiers.Length;

        double enemyPct = enemiesTotal > 0 ? (double)enemiesUnlocked / enemiesTotal * 100 : 0;
        double skinPct = skinsTotal > 0 ? (double)skinsUnlocked / skinsTotal * 100 : 0;
        double tierPct = tiersTotal > 0 ? (double)tiersBought / tiersTotal * 100 : 0;
        double totalPct = (enemyPct + skinPct + tierPct) / 3.0;
        bool won = totalPct >= 99.999;

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
            Row("Upgrade tiers bought", tierPct, tiersBought, tiersTotal) + "\r\n" +
            "\r\n" +
            $"<pos=4%><b>Total completion</b><pos={PctColumn}><b><color={GreenHex}>{totalPct:0}%</color></b>";

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
    }
}
