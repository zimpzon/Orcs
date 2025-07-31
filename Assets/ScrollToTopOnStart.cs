using UnityEngine;
using UnityEngine.UI.Extensions;

public class ScrollToTopOnStart : MonoBehaviour
{
    public ScrollRectEx scrollRect;

    void Start()
    {
        // Defer the change until after UI has been fully built
        StartCoroutine(ScrollToTopNextFrame());
    }

    private System.Collections.IEnumerator ScrollToTopNextFrame()
    {
        yield return null; // Wait for end of frame
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
