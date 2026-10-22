using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericPopupScript : MonoBehaviour
{
    private float _autoCloseTime;

    // Number of open message popups. While one is open, the dialogs below ignore click-outside (ModalDialogCloser) and
    // Escape (ButtonBackScript), so closing the popup doesn't also close e.g. the rebirth screen behind it. Still
    // counted during the frame the popup is closed (Destroy happens at the end of the frame).
    public static int OpenCount;

    // Re-enabled (e.g. the item drop popup coming back after its "What is this?" popup): no input in that frame,
    // so the click / Escape that closed the other popup doesn't close this one too.
    private void OnEnable()
    {
        OpenCount++;
        _openedFrame = Time.frameCount;
    }

    private void OnDisable() => OpenCount--;

    private int _openedFrame;

    private void Awake()
    {
        _autoCloseTime = Time.realtimeSinceStartup + 60 * 5;
        _openedFrame = Time.frameCount;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.frameCount != _openedFrame)
            OnClose();

        if (Time.realtimeSinceStartup > _autoCloseTime)
            OnClose();

        // Click outside closes it, like the dialogs (ModalDialogCloser). Not in the frame it opened, so the click that
        // caused it can't close it again.
        if (Input.GetMouseButtonDown(0) && Time.frameCount != _openedFrame && !PointerOverThisPopup())
            OnClose();
    }

    static readonly List<RaycastResult> _raycastHits = new();

    bool PointerOverThisPopup()
    {
        if (EventSystem.current == null)
            return true;

        var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _raycastHits.Clear();
        EventSystem.current.RaycastAll(pointer, _raycastHits);
        foreach (var hit in _raycastHits)
        {
            if (hit.gameObject.transform.IsChildOf(transform))
                return true;
        }
        return false;
    }

    bool _closed;

    public void OnClose()
    {
        if (!_closed)
        {
            _closed = true;
            AudioManager.Instance.PlayDialogClose();
        }
        Destroy(gameObject);
    }
}
