using System.Collections.Generic;
using UnityEngine;

class PopupBaseValues
{
    public Vector3 Position;
    public CanvasGroup CanvasGroup;
}

public class GlobalPopupManager : MonoBehaviour
{
    public static GlobalPopupManager Instance { get; private set; }

    private GameObject CurrentPopup;
    private Dictionary<int, PopupBaseValues> _gameObjects = new();

    private void Awake()
    {
        Instance = this;
    }

    void CancelTweens(GameObject go)
    {
        // get base values and add CanvasGroup if not present.
        if (!_gameObjects.TryGetValue(go.GetInstanceID(), out PopupBaseValues popupBaseValues))
            return;

        go.transform.position = popupBaseValues.Position;
        popupBaseValues.CanvasGroup.alpha = 1f;
    }

    public void AfterShowPopup(GameObject popup)
    {
        if (popup == CurrentPopup)
        {
            Debug.Log("AfterShowPopup: already open, closing: " + CurrentPopup.name);
            // Close if trying to show again, ex second click on button.
            popup.SetActive(false);
            CancelTweens(popup);
            CurrentPopup = null;
            return;
        }

        // Open popup not currently open. Close previous if any.
        if (CurrentPopup != null)
        {
            Debug.Log("AfterShowPopup: closing existing popup: " + CurrentPopup.name);
            CurrentPopup.SetActive(false);
            CancelTweens(CurrentPopup);
        }

        // get base values and add CanvasGroup if not present.
        if (!_gameObjects.TryGetValue(popup.GetInstanceID(), out PopupBaseValues popupBaseValues))
        {
            popupBaseValues = new PopupBaseValues
            {
                Position = popup.transform.position,
            };

            if (!popup.TryGetComponent<CanvasGroup>(out var popupCanvasGroup))
            {
                popupCanvasGroup = popup.AddComponent<CanvasGroup>();
            }
            popupBaseValues.CanvasGroup = popupCanvasGroup;
            _gameObjects.Add(popup.GetInstanceID(), popupBaseValues);
        }

        Debug.Log("AfterShowPopup: Setting current pop up: " + popup.name);
        CurrentPopup = popup;

        // Reset starting state
        //popupBaseValues.CanvasGroup.alpha = 0f;
        popup.transform.position = popupBaseValues.Position + new Vector3(0, -20f, 0); // start slightly lower

        LeanTween.moveY(popup, popupBaseValues.Position.y, 0.2f).setEaseOutCubic();
    }

    public void AfterHidePopup()
    {
        CurrentPopup = null;
    }
}
