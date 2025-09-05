using UnityEngine;

public class GlobalPopupManager : MonoBehaviour
{
    public static GlobalPopupManager Instance { get; private set; }

    private GameObject CurrentPopup;

    private void Awake()
    {
        Instance = this;
    }

    public void AfterShowPopup(GameObject popup)
    {
        if (popup == CurrentPopup)
        {
            Debug.Log("AfterShowPopup: already open, closing: " + CurrentPopup.name);
            // Close if trying to show again, ex second click on button.
            popup.SetActive(false);
            CurrentPopup = null;
            return;
        }


        // Open popup not currently open. Close previous if any.
        if (CurrentPopup != null)
        {
            Debug.Log("AfterShowPopup: closing existing popup: " + CurrentPopup.name);
            CurrentPopup.SetActive(false);
        }

        Debug.Log("AfterShowPopup: Setting current pop up: " + popup.name);
        CurrentPopup = popup;
    }

    public void AfterHidePopup()
    {
        CurrentPopup = null;
    }
}
