using UnityEditor;
using UnityEngine;

// Tools > Clear Sound Settings: deletes only the audio settings (SoundSettings) from PlayerPrefs, so the next Play
// starts with their defaults. Unlike Edit > Clear All PlayerPrefs it keeps the save, the drop countdown etc.
public static class ClearSoundSettings
{
    [MenuItem("Tools/Clear Sound Settings")]
    static void Clear()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Clear Sound Settings", "Stop Play mode first: the running game already holds the current settings.", "OK");
            return;
        }

        SoundSettings.ResetToDefaults();
        Debug.Log("Sound settings cleared - defaults on the next Play");
    }
}
