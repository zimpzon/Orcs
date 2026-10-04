using UnityEngine;

// Standalone builds (Windows/Linux) must render at exactly 16:9 - parts of the UI can't scale to other aspect ratios.
// The game always runs in a resizable window that is kept at 16:9:
// - Start: the window size from the previous run if it's 16:9 and fits, else (first run) 60% of the usable desktop
//   width (taskbar excluded) at 16:9.
// - Resizing / maximizing: once the size has settled, it snaps back to 16:9, following the dimension the player
//   changed most, clamped to what fits on the monitor.
// Editor and WebGL are untouched. Player settings disable Alt+Enter (no fullscreen).
public class WindowedAspect : MonoBehaviour
{
    const float StartWidthFraction = 0.6f; // first-run window width, as a fraction of the usable desktop width
    const string SizeChosenKey = "WindowedAspect.SizeChosen"; // set once we've picked a size, so later runs keep the player's
    const int TitleBarMargin = 60;         // title bar + borders, so a window of the max size still fits
    const int SideMargin = 20;
    const int MinHeight = 360;             // 640x360
    const float SettleTime = 0.25f;        // snap only after the size stopped changing (don't fight a drag)

#if UNITY_STANDALONE && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        var go = new GameObject("WindowedAspect");
        go.hideFlags = HideFlags.HideInHierarchy;
        DontDestroyOnLoad(go);
        go.AddComponent<WindowedAspect>().ApplyStartSize();
    }
#endif

    int _appliedW, _appliedH;   // last size we set (or accepted)
    int _seenW, _seenH;         // last size observed, for the settle timer
    float _sizeChangedAt;

    static bool Is16x9(int w, int h) => h > 0 && Mathf.Abs(w * 9 - h * 16) <= 16;

    // Usable desktop area of the window's monitor, minus room for the window frame.
    static (int w, int h) Available()
    {
        var display = Screen.mainWindowDisplayInfo;
        int fullW = display.width > 0 ? display.width : Screen.currentResolution.width;
        int fullH = display.height > 0 ? display.height : Screen.currentResolution.height;
        var work = display.workArea; // may be empty on some Linux setups
        int w = (work.width > 0 ? work.width : fullW) - SideMargin;
        int h = (work.height > 0 ? work.height : fullH) - TitleBarMargin;
        return (Mathf.Max(w, 640), Mathf.Max(h, MinHeight));
    }

    // Largest exact 16:9 height (multiple of 9) that fits in the given area.
    static int MaxHeight(int availW, int availH)
        => Mathf.Max(MinHeight, Mathf.Min(availH, availW * 9 / 16) / 9 * 9);

    void SetSize(int h)
    {
        int w = h / 9 * 16;
        h = h / 9 * 9;
        _appliedW = _seenW = w;
        _appliedH = _seenH = h;
        PlayerPrefs.SetInt(SizeChosenKey, 1);
        if (Screen.fullScreenMode != FullScreenMode.Windowed || Screen.width != w || Screen.height != h)
            Screen.SetResolution(w, h, FullScreenMode.Windowed);
    }

    void ApplyStartSize()
    {
        var (availW, availH) = Available();
        int maxH = MaxHeight(availW, availH);

        // Keep the size the player left it at last time (Unity remembers it), if it's still valid on this monitor.
        // On the very first run Unity starts at the Player Settings default (1280x720), which is 16:9 too - so only
        // trust it once we've chosen a size ourselves.
        if (PlayerPrefs.GetInt(SizeChosenKey, 0) == 1 && Screen.fullScreenMode == FullScreenMode.Windowed && Is16x9(Screen.width, Screen.height)
            && Screen.height >= MinHeight && Screen.height <= maxH)
        {
            SetSize(Screen.height);
            return;
        }

        int startH = (int)(availW * StartWidthFraction) * 9 / 16;
        SetSize(Mathf.Clamp(startH, MinHeight, maxH));
    }

    void Update()
    {
        int w = Screen.width, h = Screen.height;
        if (w != _seenW || h != _seenH)
        {
            _seenW = w;
            _seenH = h;
            _sizeChangedAt = Time.unscaledTime;
            return;
        }

        bool wrongMode = Screen.fullScreenMode != FullScreenMode.Windowed;
        if (!wrongMode && (w == _appliedW && h == _appliedH || Time.unscaledTime - _sizeChangedAt < SettleTime))
            return;

        var (availW, availH) = Available();
        int maxH = MaxHeight(availW, availH);
        if (!wrongMode && Is16x9(w, h) && h >= MinHeight && h <= maxH)
        {
            // Player resized to a valid 16:9 size (or it already was): accept it.
            _appliedW = w;
            _appliedH = h;
            return;
        }

        // Follow the dimension the player changed most.
        float dw = _appliedW > 0 ? Mathf.Abs(w - _appliedW) / (float)_appliedW : 1f;
        float dh = _appliedH > 0 ? Mathf.Abs(h - _appliedH) / (float)_appliedH : 0f;
        int targetH = dw >= dh ? Mathf.RoundToInt(w * 9f / 16f) : h;
        targetH = Mathf.RoundToInt(targetH / 9f) * 9;
        SetSize(Mathf.Clamp(targetH, MinHeight, maxH));
    }
}
