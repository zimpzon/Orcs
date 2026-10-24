using System.Text;
using Assets.Script.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// "Events" dialog (Tips > Events, PopupEvents in the scene - made once by Tools > UI > Create Events Dialog, edit it
// like any other dialog): the event log (EventLog) newest first, each entry with its local date and time and how long
// ago, in a scroll view. Closing (X / Escape) goes back to the tips.
public class EventLogPopup : MonoBehaviour
{
    public ScrollRect Scroll;
    public RectTransform Content;           // the entries go in here (vertical layout)
    public TextMeshProUGUI EntryTemplate;   // inactive: the look of the entries

    const int EntriesPerBlock = 50; // entries per text object: thousands of separate texts would be slow to lay out
    const string TimeColor = TextColors.GreyDark;

    TipsPopupScript _returnTo;

    public void Show(TipsPopupScript returnTo)
    {
        _returnTo = returnTo;
        Fill();
        gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject); // closes the tips
        if (Scroll != null)
            Scroll.verticalNormalizedPosition = 1f; // newest at the top
    }

    void Fill()
    {
        if (Content == null || EntryTemplate == null)
            return;
        for (int i = Content.childCount - 1; i >= 0; --i)
        {
            var child = Content.GetChild(i);
            if (child != EntryTemplate.transform)
                Destroy(child.gameObject);
        }

        var entries = EventLog.Entries;
        if (entries.Count == 0)
        {
            AddBlock("<color=" + TimeColor + ">Nothing has happened yet.</color>");
            return;
        }

        var now = System.DateTime.UtcNow;
        var sb = new StringBuilder();
        int inBlock = 0;
        for (int i = entries.Count - 1; i >= 0; --i)
        {
            var e = entries[i];
            string when = e.TimeUtc.ToLocalTime().ToString("g", DisplayNumberFormat.DateCulture);
            if (inBlock > 0)
                sb.Append('\n');
            var (bullet, textColor) = Style(e.Text);
            sb.Append($"<color={TimeColor}>{when}</color>  <color={bullet}>•</color> <color={textColor}>{e.Text}</color>" +
                      $"  <color={TimeColor}>({Ago(now - e.TimeUtc)})</color>");
            if (++inBlock == EntriesPerBlock)
            {
                AddBlock(sb.ToString());
                sb.Clear();
                inBlock = 0;
            }
        }
        if (inBlock > 0)
            AddBlock(sb.ToString());
    }

    // A little color by kind of event: the big moments gold, rebirths and Steam items light blue, finds (beasts, skins,
    // chests, mystery) green bullets, and the routine "entered / left the arena" faded. (Bullet color, text color.)
    const string Blue = "#7FC4F0";

    static (string bullet, string text) Style(string text)
    {
        if (text.Contains("Scary Earl") || text.Contains("decimated") || text.Contains("100%") || text.Contains("Night Creeper"))
            return (TextColors.Gold, TextColors.Gold);
        if (text.Contains("reborn") || text.Contains("rebirth card") || text.StartsWith("Item drop"))
            return (Blue, TextColors.GreyLight);
        if (text.Contains("the arena."))
            return (TextColors.GreyDark, TextColors.GreyDark);
        return (TextColors.GreenSmallest, TextColors.GreyLight);
    }

    // "2d, 4h, 4m ago" - only the units that aren't 0, no seconds; under a minute "just now".
    static string Ago(System.TimeSpan span)
    {
        if (span.TotalMinutes < 1)
            return "just now";
        var parts = new System.Collections.Generic.List<string>(3);
        if (span.Days > 0) parts.Add($"{span.Days}d");
        if (span.Hours > 0) parts.Add($"{span.Hours}h");
        if (span.Minutes > 0) parts.Add($"{span.Minutes}m");
        return string.Join(", ", parts) + " ago";
    }

    void AddBlock(string text)
    {
        var block = Instantiate(EntryTemplate, Content);
        block.gameObject.SetActive(true);
        block.name = "Entries";
        block.text = text;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
        // Back to the tips, like the music dialog goes back to settings. Not on Escape: the tips' own X would catch the
        // same key press this frame and close them again - Escape just closes.
        if (_returnTo != null && !Input.GetKeyDown(KeyCode.Escape))
            _returnTo.Show();
    }
}
