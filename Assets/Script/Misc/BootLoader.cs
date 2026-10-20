using UnityEngine;
using UnityEngine.SceneManagement;

// First scene in the build (Boot.unity): just a dark camera, so the window goes dark right away instead of staying
// Windows-white while the big Arena scene loads. Loads the Arena and leaves a StartFade cover behind that fades it in.
public class BootLoader : MonoBehaviour
{
    public string SceneName = "Arena";
    public float FadeSeconds = 0.4f;

    void Start()
    {
        var cam = Camera.main;
        StartFade.Create(cam != null ? cam.backgroundColor : Color.black, FadeSeconds, SceneName);
        SceneManager.LoadSceneAsync(SceneName);
    }
}
