using UnityEngine;
using UnityEngine.UI;

// The upgrade list's content has a fixed, very tall height (ScrollPanel), so with only a few tiers visible the player
// could scroll them all out of view into empty space. This caps the scroll at the last visible item: it can come up to
// the bottom of the view, no further. Reads the items' positions only, so it doesn't fight UpgradeResizer's sizing.
[RequireComponent(typeof(ScrollRect))]
[DefaultExecutionOrder(100)] // after the ScrollRect moved the content this frame
public class ScrollClampToItems : MonoBehaviour
{
    [Tooltip("Extra room below the last item (canvas units).")]
    public float BottomPadding = 10f;

    ScrollRect _scroll;

    void Awake()
    {
        _scroll = GetComponent<ScrollRect>();
    }

    void LateUpdate()
    {
        var content = _scroll.content;
        if (content == null)
            return;
        var viewport = _scroll.viewport != null ? _scroll.viewport : (RectTransform)transform;

        // Lowest bottom edge of the active items, measured down from the content's top.
        float bottom = 0f;
        for (int i = 0; i < content.childCount; ++i)
        {
            var child = content.GetChild(i) as RectTransform;
            if (child == null || !child.gameObject.activeSelf)
                continue;
            float childBottom = content.rect.yMax - (child.localPosition.y + child.rect.yMin);
            bottom = Mathf.Max(bottom, childBottom);
        }

        float maxScroll = Mathf.Max(0f, bottom + BottomPadding - viewport.rect.height);
        var pos = content.anchoredPosition;
        if (pos.y > maxScroll)
        {
            pos.y = maxScroll;
            content.anchoredPosition = pos;
            _scroll.velocity = Vector2.zero;
        }
    }
}
