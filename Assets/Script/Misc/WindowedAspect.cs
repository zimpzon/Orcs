using UnityEngine;

// Standalone builds (Windows/Linux) run in a resizable window kept at 16:9 or 16:10 (other aspects are letterboxed
// anyway by AspectUtility + UiAspectFrame, but these two look best):
// - Start: the window size from the previous run if it's 16:9 and fits, else (first run) 70% of the usable desktop
//   width (taskbar excluded) at 16:9.
// - Resizing / maximizing: once the size has settled, it snaps back to 16:9 (16:10 sizes are kept), following the
//   dimension the player changed most, clamped to what fits on the monitor.
// - Fullscreen (settings checkbox / F key, SetFullscreen): only on a 16:9 or 16:10 monitor, borderless at the
//   monitor's resolution; no snapping while in it. Remembered in PlayerPrefs.
// Steam Deck: skipped - the game runs fullscreen at the native 1280x800 (16:10) and letterboxes itself.
// Editor and WebGL are untouched. Player settings disable Alt+Enter (fullscreen only through SetFullscreen).
public class WindowedAspect : MonoBehaviour
{
    const float StartWidthFraction = 0.7f; // first-run window width, as a fraction of the usable desktop width
    const string SizeChosenKey = "WindowedAspect.SizeChosen"; // set once we've picked a size, so later runs keep the player's
    const int TitleBarMargin = 60;         // title bar + borders, so a window of the max size still fits
    const int SideMargin = 20;
    const int MinHeight = 360;             // 640x360
    const float SettleTime = 0.6f;         // snap only after the size stopped changing (don't fight a drag; was 0.25 s,
                                           // short enough to fire during a pause mid-drag)
    const int MaxSnapTries = 3;            // a size the OS / window manager won't give us (maximized, DPI rounding): stop
                                           // after this many tries and keep it - letterboxing handles any aspect
    const string FullscreenKey = "WindowedAspect.Fullscreen";

    static WindowedAspect _instance;

#if UNITY_STANDALONE && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (System.Environment.GetEnvironmentVariable("SteamDeck") == "1")
            return;

        var go = new GameObject("WindowedAspect");
        go.hideFlags = HideFlags.HideInHierarchy;
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<WindowedAspect>();
        _instance.ApplyStartSize();
    }
#endif

    // Settings checkbox / F key. Without an instance (Editor, WebGL, Steam Deck) these do nothing / report false.
    public static bool IsActive => _instance != null;

    public static bool MonitorSupportsFullscreen
    {
        get
        {
            var (w, h) = MonitorSize();
            return IsSupported(w, h);
        }
    }

    public static bool IsFullscreen => _instance != null && Screen.fullScreenMode != FullScreenMode.Windowed;

    public static void SetFullscreen(bool on)
    {
        if (_instance == null)
            return;
        if (on && !MonitorSupportsFullscreen)
            return;

        PlayerPrefs.SetInt(FullscreenKey, on ? 1 : 0);
        PlayerPrefs.Save();
        if (on)
            _instance.GoFullscreen();
        else
            _instance.RestoreWindow();
    }

    static (int w, int h) MonitorSize()
    {
        var display = Screen.mainWindowDisplayInfo;
        int w = display.width > 0 ? display.width : Screen.currentResolution.width;
        int h = display.height > 0 ? display.height : Screen.currentResolution.height;
        return (w, h);
    }

    static bool WantsFullscreen => PlayerPrefs.GetInt(FullscreenKey, 0) == 1;

    void GoFullscreen()
    {
        var (w, h) = MonitorSize();
        Screen.SetResolution(w, h, FullScreenMode.FullScreenWindow);
    }

    // Back to the last windowed size we set or accepted.
    void RestoreWindow()
    {
        int w = _appliedW > 0 ? _appliedW : 1280, h = _appliedH > 0 ? _appliedH : 720;
        _seenW = w;
        _seenH = h;
        Screen.SetResolution(w, h, FullScreenMode.Windowed);
    }

    int _appliedW, _appliedH;   // last size we set (or accepted)
    int _seenW, _seenH;         // last size observed, for the settle timer
    float _sizeChangedAt;
    int _snapTries;             // snaps in a row that didn't stick (reset when the player changes the size)
    int _snapFromW, _snapFromH; // the size we last tried to snap away from

    // Within ~0.5% of the aspect: DPI scaling and window frames round sizes by a few pixels, and a near miss shouldn't
    // count as "wrong" (that kept re-snapping forever).
    static bool NearAspect(int w, int h, float aspect) => h > 0 && Mathf.Abs(w / (float)h - aspect) <= aspect * 0.005f;
    static bool Is16x9(int w, int h) => NearAspect(w, h, 16f / 9f);
    static bool Is16x10(int w, int h) => NearAspect(w, h, 16f / 10f);
    static bool IsSupported(int w, int h) => Is16x9(w, h) || Is16x10(w, h);

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
        if (PlayerPrefs.GetInt(SizeChosenKey, 0) == 1 && !WantsFullscreen && Screen.fullScreenMode == FullScreenMode.Windowed && IsSupported(Screen.width, Screen.height)
            && Screen.height >= MinHeight && Screen.height <= maxH)
        {
            if (Is16x10(Screen.width, Screen.height))
            {
                // Keep a 16:10 window as it is (SetSize always makes 16:9).
                _appliedW = _seenW = Screen.width;
                _appliedH = _seenH = Screen.height;
            }
            else
            {
                SetSize(Screen.height);
            }
            return;
        }

        int startH = (int)(availW * StartWidthFraction) * 9 / 16;
        SetSize(Mathf.Clamp(startH, MinHeight, maxH));
    }

    void Start()
    {
        // After the windowed size is known (ApplyStartSize), so un-fullscreening returns to it.
        if (WantsFullscreen && MonitorSupportsFullscreen)
            GoFullscreen();
    }

    void Update()
    {
        if (WantsFullscreen)
            return; // fullscreen: no snapping

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
        // Still a mouse button down: probably still dragging the frame (where Unity sees it) - wait.
        if (!wrongMode && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
            return;

        var (availW, availH) = Available();
        int maxH = MaxHeight(availW, availH);
        if (!wrongMode && IsSupported(w, h) && h >= MinHeight && h <= maxH)
        {
            // Player resized to a valid 16:9 / 16:10 size (or it already was): accept it.
            _appliedW = w;
            _appliedH = h;
            return;
        }

        // The same unwanted size again after our snap: it didn't stick. A few tries, then accept it as it is (the camera
        // and UI letterbox to 16:9 anyway) instead of calling SetResolution forever, which made the window jump.
        if (w == _snapFromW && h == _snapFromH)
        {
            if (++_snapTries >= MaxSnapTries)
            {
                _appliedW = w;
                _appliedH = h;
                return;
            }
        }
        else
        {
            _snapFromW = w;
            _snapFromH = h;
            _snapTries = 1;
        }

        // Follow the dimension the player changed most.
        float dw = _appliedW > 0 ? Mathf.Abs(w - _appliedW) / (float)_appliedW : 1f;
        float dh = _appliedH > 0 ? Mathf.Abs(h - _appliedH) / (float)_appliedH : 0f;
        int targetH = dw >= dh ? Mathf.RoundToInt(w * 9f / 16f) : h;
        targetH = Mathf.RoundToInt(targetH / 9f) * 9;
        SetSize(Mathf.Clamp(targetH, MinHeight, maxH));
    }
}
