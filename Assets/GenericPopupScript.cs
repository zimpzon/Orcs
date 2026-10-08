using UnityEngine;

public class GenericPopupScript : MonoBehaviour
{
    private float _autoCloseTime;

    // Number of open message popups. While one is open, the dialogs below ignore click-outside (ModalDialogCloser) and
    // Escape (ButtonBackScript), so closing the popup doesn't also close e.g. the rebirth screen behind it. Still
    // counted during the frame the popup is closed (Destroy happens at the end of the frame).
    public static int OpenCount;

    private void OnEnable() => OpenCount++;
    private void OnDisable() => OpenCount--;

    private void Awake()
    {
        _autoCloseTime = Time.realtimeSinceStartup + 60 * 5;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnClose();

        if (Time.realtimeSinceStartup > _autoCloseTime)
            OnClose();
    }

    public void OnClose()
    {
        Destroy(gameObject);
    }
}
