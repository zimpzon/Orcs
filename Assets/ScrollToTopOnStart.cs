using UnityEngine;
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
