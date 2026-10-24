using UnityEngine;

// Audio settings belong to the machine, not the save game (PlayerPrefs, not SaveGame): a save synced to another PC
// (Steam Cloud, import) or a save wipe / rebirth doesn't change how loud the game is here. Read once, written on change.
public static class SoundSettings
{
    const string Prefix = "Sound.";

    public static float VolumeMaster { get => GetFloat("VolumeMaster", 0.5f); set => SetFloat("VolumeMaster", value); }
    public static float VolumeMusic { get => GetFloat("VolumeMusic", 0.6f); set => SetFloat("VolumeMusic", value); }
    public static float VolumeSfx { get => GetFloat("VolumeSfx", 0.4f); set => SetFloat("VolumeSfx", value); }

    // Master switch: off = no sound at all, music included (AudioManager: AudioListener.volume).
    public static bool SoundEnabled { get => GetBool("SoundEnabled", true); set => SetBool("SoundEnabled", value); }
    // Music on/off (MusicManagerScript), separate from the music volume.
    public static bool MusicEnabled { get => GetBool("MusicEnabled", true); set => SetBool("MusicEnabled", value); }
    // All game sounds except UI ones (menu click, chime, dialogs) and music. Coin pickups count as game sounds.
    public static bool CombatSoundEnabled { get => GetBool("CombatSoundEnabled", true); set => SetBool("CombatSoundEnabled", value); }
    // Keep sound and music playing while minimized / in another tab.
    public static bool SoundInBackground { get => GetBool("SoundInBackground", true); set => SetBool("SoundInBackground", value); }

    // Cached: some are read every frame. A volume slider drag writes often, so only switches save PlayerPrefs to disk
    // right away (volumes are saved by Unity on quit, and by the next switch change).
    static readonly System.Collections.Generic.Dictionary<string, float> _floats = new();
    static readonly System.Collections.Generic.Dictionary<string, bool> _bools = new();

    static float GetFloat(string key, float def)
    {
        if (!_floats.TryGetValue(key, out var v))
            _floats[key] = v = PlayerPrefs.GetFloat(Prefix + key, def);
        return v;
    }

    static void SetFloat(string key, float value)
    {
        _floats[key] = value;
        PlayerPrefs.SetFloat(Prefix + key, value);
    }

    // Every key above (keep in sync): ResetToDefaults deletes them.
    static readonly string[] Keys =
    {
        "VolumeMaster", "VolumeMusic", "VolumeSfx",
        "SoundEnabled", "MusicEnabled", "CombatSoundEnabled", "SoundInBackground",
    };

    // Back to the defaults (Tools > Clear Sound Settings, for testing them); nothing else in PlayerPrefs is touched.
    public static void ResetToDefaults()
    {
        foreach (var key in Keys)
            PlayerPrefs.DeleteKey(Prefix + key);
        PlayerPrefs.Save();
        _floats.Clear();
        _bools.Clear();
    }

    static bool GetBool(string key, bool def)
    {
        if (!_bools.TryGetValue(key, out var v))
            _bools[key] = v = PlayerPrefs.GetInt(Prefix + key, def ? 1 : 0) == 1;
        return v;
    }

    static void SetBool(string key, bool value)
    {
        _bools[key] = value;
        PlayerPrefs.SetInt(Prefix + key, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
