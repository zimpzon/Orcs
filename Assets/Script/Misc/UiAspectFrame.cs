using UnityEngine;
using UnityEngine.UI;

// Keeps an overlay canvas' UI at the 16:9 layout it was built for, on any screen aspect (Steam Deck is 16:10).
// Overlay canvases ignore the camera's letterboxed viewport (AspectUtility), so on 16:10 the UI used to lose ~10% of
// its width. On Awake every child of the canvas moves into a centered 16:9 "AspectFrame" (anchors stay relative, so
// the layout is unchanged), and the CanvasScaler matches width on narrower screens / height on wider ones, so the
// frame always fits: empty bands over the camera's black bars instead of cut-off UI. Exactly 16:9 is unchanged.
// Things created later at the canvas root (popups, darken, cheat sheet) stay full-screen.
[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public class UiAspectFrame : MonoBehaviour
{
    const float LayoutHeight = 500f;                 // CanvasScaler reference height the layout was built with
    const float LayoutAspect = 16f / 9f;
    static readonly Vector2 FrameSize = new(LayoutHeight * LayoutAspect, LayoutHeight);

    CanvasScaler _scaler;
    float _lastAspect = -1f;

    void Awake()
    {
        _scaler = GetComponent<CanvasScaler>();

        var frameGo = new GameObject("AspectFrame", typeof(RectTransform));
        var frame = (RectTransform)frameGo.transform;
        frame.SetParent(transform, worldPositionStays: false);
        frame.anchorMin = frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.anchoredPosition = Vector2.zero;
        frame.sizeDelta = FrameSize;
        frame.localScale = Vector3.one;

        // Move everything else (in order) into the frame.
        var children = new Transform[transform.childCount];
        for (int i = 0; i < children.Length; ++i)
            children[i] = transform.GetChild(i);
        foreach (var child in children)
        {
            if (child != frame)
                child.SetParent(frame, worldPositionStays: false);
        }

        Apply();
    }

    void Update()
    {
        Apply();
    }

    void Apply()
    {
        float aspect = Screen.height > 0 ? (float)Screen.width / Screen.height : LayoutAspect;
        if (Mathf.Approximately(aspect, _lastAspect))
            return;
        _lastAspect = aspect;

        _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _scaler.referenceResolution = FrameSize;
        _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        _scaler.matchWidthOrHeight = aspect < LayoutAspect ? 0f : 1f; // narrower: fit width, wider: fit height
    }
}
