using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Developer cheat sheet: RightShift+H toggles an overlay listing every cheat and shortcut (GameManager cheats section
// and a few other keys). Built in code on first use, on top of everything. Close it with RightShift+H, Escape or a
// click. Keep Entries in sync when adding or changing a cheat.
public class CheatSheet : MonoBehaviour
{
    static readonly (string Keys, string What)[] Entries =
    {
        ("RightShift+H", "Show / hide this cheat sheet"),
        ("", ""),
        ("RightCtrl+M", "+45 octillion money"),
        ("RightCtrl+RightShift+M", "Money to 0"),
        ("RightCtrl+V", "+100 diamonds, +10 credits"),
        ("RightCtrl+C", "Diamonds to 0, +1 credit, credit XP almost full"),
        ("RightCtrl+X", "+100 lifetime credits"),
        ("RightCtrl+A", "Arena +500"),
        ("RightCtrl+S", "Arena -25"),
        ("RightCtrl+Z", "Set a mid-game upgrade loadout + arena +5000"),
        ("RightCtrl+T", "Toggle cheat speed (x10 time, arena jumps x10)"),
        ("RightCtrl+Right / Left", "Game time scale +0.1 / -0.1"),
        ("", ""),
        ("RightCtrl+Q", "Mystery question mark ready now"),
        ("RightShift+Q", "Show a chest now"),
        ("RightCtrl+K", "Unlock the White Earl skin"),
        ("RightCtrl+R", "Preview the X2 rank-up text at the cursor"),
        ("RightShift+G", "Toggle 100% victory effects preview (crown pulse, victory dialog)"),
        ("RightShift+L", "Show save game load info"),
        ("", ""),
        ("F", "Toggle fullscreen"),
        ("I", "Toggle FPS counter"),
        ("LeftShift+S", "Save game import/export dialog (player shortcut)"),
        ("Escape", "Close the open dialog / collapse the upgrade list"),
    };

    const string KeyHex = "#FFD54A";
    const string TextHex = "#DDDDDD";
    const string HeaderHex = "#8DBE4C";
    const int SortingOrder = 130; // above dialogs (100) and messages (110)

    static CheatSheet _instance;
    public static bool IsOpen => _instance != null && _instance.gameObject.activeSelf;

    public static void Toggle(Canvas rootCanvas)
    {
        if (_instance == null)
            _instance = Build(rootCanvas);
        else
            _instance.gameObject.SetActive(!_instance.gameObject.activeSelf);
    }

    static CheatSheet Build(Canvas rootCanvas)
    {
        // Full-screen dark backdrop on its own top canvas.
        var root = new GameObject("CheatSheet", typeof(RectTransform));
        root.transform.SetParent(rootCanvas.transform, false);
        var canvas = root.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = SortingOrder;
        root.AddComponent<GraphicRaycaster>();
        var rt = (RectTransform)root.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var backdrop = root.AddComponent<Image>();
        backdrop.color = new Color(0f, 0f, 0f, 0.85f); // also catches the click that closes it

        // Text.
        var textGo = new GameObject("Text", typeof(RectTransform));
        textGo.transform.SetParent(root.transform, false);
        var text = textGo.AddComponent<TextMeshProUGUI>();
        var fontSource = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault(t => t.font != null && t.font.name.Contains("Lato"));
        if (fontSource != null)
            text.font = fontSource.font;
        text.fontSize = 14;
        text.raycastTarget = false;
        text.alignment = TextAlignmentOptions.TopLeft;
        var trt = text.rectTransform;
        trt.anchorMin = new Vector2(0.15f, 0.06f);
        trt.anchorMax = new Vector2(0.85f, 0.94f);
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        var sb = new StringBuilder();
        sb.Append($"<align=center><size=150%><b><color={HeaderHex}>CHEATS & SHORTCUTS</color></b></size></align>\n");
        sb.Append("<align=center><size=80%><color=#AAAAAA>Click the Game view first so it gets the keys. Click or press Escape to close.</color></size></align>\n\n");
        foreach (var (keys, what) in Entries)
        {
            if (keys.Length == 0)
            {
                sb.Append("<size=50%> </size>\n");
                continue;
            }
            sb.Append($"<b><color={KeyHex}>{keys}</color></b><pos=34%><color={TextHex}>{what}</color>\n");
        }
        text.text = sb.ToString();

        return root.AddComponent<CheatSheet>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
            gameObject.SetActive(false);
    }
}
