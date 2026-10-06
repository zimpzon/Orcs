using System.Linq;
using System.Numerics;
using System.Text;
using TMPro;
using UnityEngine;

// Tips popup. The layout lives in the scene (PopupTips): TextNumbers (left column), Divider, TextProgress (right
// column, the free-text tips you write). This script only fills TextNumbers with the big number table (million ..
// duodecillion, from Format512's suffix list) - also in the Editor, so the layout can be adjusted with real content.
[ExecuteAlways]
public class TipsPopupScript : MonoBehaviour
{
    public TextMeshProUGUI ProgressText; // the tips (authored in the scene)
    public TextMeshProUGUI NumbersText;  // filled by this script

    const string HeaderHex = "#8DBE4C"; // the game's standard green header color (TIPS uses it too, in the scene text)
    const int MinPower = 6;   // million
    const int MaxPower = 45;  // quattuordecillion (powers come in steps of 3)

    void OnEnable() => FillNumbers();
    void OnValidate() => FillNumbers();

    void FillNumbers()
    {
        if (NumbersText == null)
            return;

        string table = BuildNumbersTable();
        if (NumbersText.text != table)
            NumbersText.text = table;
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

    // "10" with a readable raised exponent (TMP's <sup> is too small to read at this size).
    static string Power(int power) => $"10<voffset=0.45em><size=80%>{power}</size></voffset>";

    static string Row(string shortName, string name, string power, string color)
        => $"\n<pos=2%><color={color}>{shortName}</color><pos=20%><color={color}>{name}</color><pos=74%><color={color}>{power}</color>";

    static string BuildNumbersTable()
    {
        var sb = new StringBuilder();
        sb.Append($"<align=left><size=100%><b><color={HeaderHex}>Big Numbers</color></b></size>\n<size=90%>");
        sb.Append(Row("Short", "Name", "Value", "#AAAAAA"));

        var rows = Format512.SuffixList
            .Select(s => (s.Short, Name: s.Long.Trim(), Power: (int)System.Math.Round(BigInteger.Log10(s.Threshold))))
            .Where(s => s.Power >= MinPower && s.Power <= MaxPower)
            .OrderBy(s => s.Power);
        foreach (var (shortName, name, power) in rows)
            sb.Append(Row(shortName, name, Power(power), "#dddddd"));
        sb.Append("</size>");
        return sb.ToString();
    }
}
