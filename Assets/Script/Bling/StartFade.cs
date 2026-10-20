using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Full-screen cover in the boot colour on top of everything (survives the scene load), faded out once the Arena has
// drawn its first frames, so the game appears smoothly instead of popping in. Made by BootLoader; destroys itself.
public class StartFade : MonoBehaviour
{
    Image _cover;
    float _seconds;
    string _sceneName;

    public static void Create(Color color, float seconds, string sceneName)
    {
        var go = new GameObject("StartFade", typeof(RectTransform));
        DontDestroyOnLoad(go);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        var coverGo = new GameObject("Cover", typeof(RectTransform));
        coverGo.transform.SetParent(go.transform, false);
        var img = coverGo.AddComponent<Image>();
        img.color = new Color(color.r, color.g, color.b, 1f);
        img.raycastTarget = false;
        var rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var fade = go.AddComponent<StartFade>();
        fade._cover = img;
        fade._seconds = seconds;
        fade._sceneName = sceneName;
    }

    IEnumerator Start()
    {
        while (SceneManager.GetActiveScene().name != _sceneName)
            yield return null;

        // A couple of frames under the cover: the Arena's first-frame layout and load hitch stay hidden.
        yield return null;
        yield return null;

        var c = _cover.color;
        float t = 0f;
        while (t < _seconds)
        {
            t += Mathf.Min(Time.unscaledDeltaTime, 1f / 30f); // a hitch must not skip the fade
            c.a = 1f - Mathf.SmoothStep(0f, 1f, t / _seconds);
            _cover.color = c;
            yield return null;
        }

        Destroy(gameObject);
    }
}
