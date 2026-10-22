using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

// WebGL has no Steam, so no item drops: empties GameCanvasScript.ItemDropIcons in the scenes going into a WebGL build,
// so the item icons stay out of the download. Only the build's copy of the scene changes (like StripMusicForWebGL).
public class StripItemIconsForWebGL : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report == null || report.summary.platform != BuildTarget.WebGL)
            return; // null = entering Play mode in the Editor

        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var canvas in root.GetComponentsInChildren<GameCanvasScript>(true))
            {
                int count = canvas.ItemDropIcons?.Length ?? 0;
                canvas.ItemDropIcons = new Sprite[0];
                Debug.Log($"StripItemIconsForWebGL: stripped {count} item icons from '{scene.name}' for the WebGL build");
            }
        }
    }
}
