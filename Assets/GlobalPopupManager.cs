using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // Darkens everything behind the open popup, same look as the rebirth popup's AscendDarkenBackground.
    // Purely visual (no raycasts), so popup buttons, toggle-close and click-outside closing work as before.
    static readonly Color DarkenColor = new Color(0, 0, 0, 0.843f);
    const float DarkenSize = 10000f;
    GameObject _darken;

    private void Awake()
    {
        Instance = this;
    }

    void ShowDarken(GameObject popup)
    {
        if (_darken == null)
        {
            _darken = new GameObject("PopupDarkenBackground", typeof(RectTransform), typeof(Image));
            var image = _darken.GetComponent<Image>();
            image.color = DarkenColor;
            image.raycastTarget = false;
        }

        var rt = (RectTransform)_darken.transform;
        rt.SetParent(popup.transform.parent, worldPositionStays: false);
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(DarkenSize, DarkenSize);
        rt.localScale = Vector3.one;

        // Center it on the screen (root canvas center), whatever size/position the popup's parent has.
        var canvas = popup.GetComponentInParent<Canvas>();
        if (canvas != null)
            rt.position = canvas.rootCanvas.transform.position;

        // Directly behind the popup: moving the overlay to the popup's index pushes the popup one step up.
        rt.SetSiblingIndex(popup.transform.GetSiblingIndex());
        if (rt.GetSiblingIndex() > popup.transform.GetSiblingIndex())
            rt.SetSiblingIndex(popup.transform.GetSiblingIndex());

        _darken.SetActive(true);
    }

    void HideDarken()
    {
        if (_darken != null)
            _darken.SetActive(false);
    }

    private void Update()
    {
        // Safety net: popup closed some other way.
        if (CurrentPopup != null && !CurrentPopup.activeInHierarchy)
            CurrentPopup = null;

        if (CurrentPopup == null && _darken != null && _darken.activeSelf)
            HideDarken();
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
            HideDarken();
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
        ShowDarken(popup);

        // Reset starting state
        //popupBaseValues.CanvasGroup.alpha = 0f;
        popup.transform.position = popupBaseValues.Position + new Vector3(0, -20f, 0); // start slightly lower

        LeanTween.moveY(popup, popupBaseValues.Position.y, 0.2f).setEaseOutCubic();
    }

    public void AfterHidePopup()
    {
        CurrentPopup = null;
        HideDarken();
    }
}
