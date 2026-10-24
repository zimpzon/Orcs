using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Player-facing history of notable events ("Earl defeated Scary Earl!"), newest last. Not shown anywhere yet - for now
// every entry also goes to the console. Kept separately from the save game (a big list would make every auto-save
// bigger): desktop eventlog.txt next to the save (persistentDataPath), WebGL PlayerPrefs. Up to MaxEntries, the oldest
// dropped. Cleared when the save is deleted (not on rebirth). Written when an entry is added.
public static class EventLog
{
    public const int MaxEntries = 5000;
    const int TrimSlack = 200;

    public struct Entry
    {
        public DateTime TimeUtc;
        public string Text;
    }

    static List<Entry> _entries;

    public static IReadOnlyList<Entry> Entries
    {
        get
        {
            EnsureLoaded();
            return _entries;
        }
    }

    public static void Add(string text)
    {
        EnsureLoaded();
        var entry = new Entry { TimeUtc = DateTime.UtcNow, Text = text };
        _entries.Add(entry);
        Debug.Log($"[EventLog] {text}");

        // Trimmed in batches (the whole file is rewritten then), not on every entry once full.
        if (_entries.Count > MaxEntries + TrimSlack)
        {
            _entries.RemoveRange(0, _entries.Count - MaxEntries);
            SaveAll();
        }
        else
        {
            Append(entry);
        }
    }

    static bool _hooked;

    static void EnsureLoaded()
    {
        if (!_hooked)
        {
            _hooked = true;
            GameEvents.OnSaveWiped += reason =>
            {
                if (reason == GameEvents.SaveWipeReason.UserWipe)
                    Clear();
            };
        }
        if (_entries != null)
            return;
        _entries = new List<Entry>();
        try
        {
            foreach (var line in ReadAll().Split('\n'))
            {
                int tab = line.IndexOf('\t');
                if (tab <= 0)
                    continue;
                if (DateTime.TryParse(line.Substring(0, tab), System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.RoundtripKind, out var t))
                    _entries.Add(new Entry { TimeUtc = t.ToUniversalTime(), Text = line.Substring(tab + 1).TrimEnd('\r') });
            }
            if (_entries.Count > MaxEntries)
                _entries.RemoveRange(0, _entries.Count - MaxEntries);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"EventLog: could not read it, starting empty: {e.Message}");
        }
    }

    public static void Clear()
    {
        _entries = new List<Entry>();
        SaveAll();
    }

    // ---- Storage: one "time<TAB>text" line per entry ----

    static string Line(Entry e) => e.TimeUtc.ToString("o", System.Globalization.CultureInfo.InvariantCulture) + "\t"
                                   + e.Text.Replace('\n', ' ').Replace('\t', ' ') + "\n";

#if UNITY_EDITOR
    const string FileName = "eventlog-editor.txt"; // like the save: the Editor has its own
#else
    const string FileName = "eventlog.txt";
#endif
    const string PrefsKey = "EventLog";

    static bool UseFile => Application.platform != RuntimePlatform.WebGLPlayer;
    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    static string ReadAll()
    {
        if (UseFile)
            return File.Exists(FilePath) ? File.ReadAllText(FilePath) : "";
        return PlayerPrefs.GetString(PrefsKey, "");
    }

    static void Append(Entry e)
    {
        try
        {
            if (UseFile)
            {
                File.AppendAllText(FilePath, Line(e));
            }
            else
            {
                PlayerPrefs.SetString(PrefsKey, PlayerPrefs.GetString(PrefsKey, "") + Line(e));
                PlayerPrefs.Save();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"EventLog: could not write: {ex.Message}");
        }
    }

    static void SaveAll()
    {
        try
        {
            var sb = new StringBuilder();
            foreach (var e in _entries)
                sb.Append(Line(e));
            if (UseFile)
            {
                File.WriteAllText(FilePath, sb.ToString());
            }
            else
            {
                PlayerPrefs.SetString(PrefsKey, sb.ToString());
                PlayerPrefs.Save();
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"EventLog: could not write: {ex.Message}");
        }
    }
}
