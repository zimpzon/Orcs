using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

// Touchscreen version of hover details (bestiary, skins, rebirth cards, tier upgrade tooltips): there is no hover, so a
// tap pins the item's details. Tapping the same item again closes them, tapping another item switches, tapping anything
// else closes. Mouse users keep normal hover (callers only route touch input here).
// Items call Tap from OnPointerEnter with a touch pointer: a touch press enters every object under the finger (clicks
// only reach the first handler, e.g. a skin's own Button, and disabled buttons send none), in the same frame the touch
// begins - so a touch that began this frame without any Tap closes the pinned details.
public class TouchPin : MonoBehaviour
{
    static TouchPin _instance;
    static Object _owner;
    static Action _hide;
    static int _claimFrame = -1;

    // This item's details are pinned by a tap right now.
    public static bool IsPinned(Object owner) => owner != null && owner == _owner;

    // A touch pointer (mouse ids are negative).
    public static bool IsTouch(PointerEventData eventData) => eventData != null && eventData.pointerId >= 0;

    // The touch landed on a Button inside the item (buy / X2): that's a purchase, not a request for details.
    public static bool HitChildButton(PointerEventData eventData, Transform item)
    {
        var hit = eventData.pointerCurrentRaycast.gameObject;
        var button = hit != null ? hit.GetComponentInParent<UnityEngine.UI.Selectable>() : null;
        return button != null && button.transform != item && button.transform.IsChildOf(item);
    }

    // An item was tapped: same item again closes its details, otherwise the previous one closes and this one shows.
    public static void Tap(Object owner, Action show, Action hide)
    {
        EnsureInstance();
        _claimFrame = Time.frameCount;
        if (owner == _owner)
        {
            Close();
            return;
        }

        Close();
        _owner = owner;
        _hide = hide;
        show();
    }

    public static void Close()
    {
        var hide = _hide;
        _owner = null;
        _hide = null;
        hide?.Invoke();
    }

    static void EnsureInstance()
    {
        if (_instance != null)
            return;
        var go = new GameObject("TouchPin");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<TouchPin>();
    }

    // After the event system: a touch that began this frame without hitting an item (Tap) closes the pinned details.
    void LateUpdate()
    {
        if (_owner == null || _claimFrame == Time.frameCount)
            return;

        for (int i = 0; i < Input.touchCount; ++i)
        {
            var phase = Input.GetTouch(i).phase;
            if (phase == TouchPhase.Began)
            {
                Close();
                return;
            }
        }
    }
}
