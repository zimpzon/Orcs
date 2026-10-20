using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// WebGL has no music (too big to download): empties MusicManagerScript.Tracks in the scenes going into a WebGL build,
// so nothing references the mp3s and Unity leaves them out. Only the build's copy of the scene changes, not the file.
public class StripMusicForWebGL : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report == null || report.summary.platform != BuildTarget.WebGL)
            return; // null = entering Play mode in the Editor

        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var music in root.GetComponentsInChildren<MusicManagerScript>(true))
            {
                int count = music.Tracks?.Length ?? 0;
                music.Tracks = new AudioClip[0];
                if (music.TryGetComponent<AudioSource>(out var source))
                    source.clip = null;
                Debug.Log($"StripMusicForWebGL: stripped {count} music tracks from '{scene.name}' for the WebGL build");
            }
        }
    }
}
