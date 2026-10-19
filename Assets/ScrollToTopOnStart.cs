using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ScrollToTopOnStart : MonoBehaviour
{
    public static ScrollToTopOnStart Instance;

    public ScrollRectEx scrollRect;

    private void Awake()
    {
        Instance = this;
    }

    public void ScrollToTopNow()
    {
        StartCoroutine(ScrollToTopNextFrame());
    }

    public float VerticalPosition => scrollRect.verticalNormalizedPosition; // 1 = top

    // Right now, in this frame: rebuild the layout first so the new content size is known, then jump there (1 = top).
    // Used when the expanded tier view collapses (a next-frame scroll showed one frame at the old position - a flash).
    public void ScrollToImmediate(float verticalNormalized)
    {
        Canvas.ForceUpdateCanvases();
        if (scrollRect.content != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(verticalNormalized);
    }

    void Start()
    {
        // Defer the change until after UI has been fully built
        ScrollToTopNow();
    }

    private System.Collections.IEnumerator ScrollToTopNextFrame()
    {
        yield return null; // Wait for end of frame
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
