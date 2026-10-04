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
    GameObject _darken;

    // Popups are nested at different depths (e.g. the settings popup is inside SettingsImage, so later siblings like
    // the credits box drew on top of it). Instead of relying on hierarchy order, the open popup gets its own canvas
    // with a sorting override, and the darken layer sits just below it - both above all regular UI.
    const int PopupSortingOrder = 100;

    private void Awake()
    {
        Instance = this;
    }

    void BringPopupToFront(GameObject popup)
    {
        if (!popup.TryGetComponent<Canvas>(out var popupCanvas))
            popupCanvas = popup.AddComponent<Canvas>();
        popupCanvas.overrideSorting = true;
        popupCanvas.sortingOrder = PopupSortingOrder;

        // A nested canvas needs its own raycaster, or the popup's buttons stop receiving clicks.
        if (!popup.TryGetComponent<GraphicRaycaster>(out _))
            popup.AddComponent<GraphicRaycaster>();
    }

    void ShowDarken(GameObject popup)
    {
        var rootCanvas = popup.GetComponentInParent<Canvas>()?.rootCanvas;
        if (rootCanvas == null)
            return;

        if (_darken == null)
        {
            _darken = new GameObject("PopupDarkenBackground", typeof(RectTransform), typeof(Canvas), typeof(Image));
            var image = _darken.GetComponent<Image>();
            image.color = DarkenColor;
            image.raycastTarget = false;
        }

        // Fill the root canvas, drawn just below the popup's own canvas.
        var rt = (RectTransform)_darken.transform;
        rt.SetParent(rootCanvas.transform, worldPositionStays: false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        rt.localScale = Vector3.one;

        _darken.SetActive(true);
        var darkenCanvas = _darken.GetComponent<Canvas>();
        darkenCanvas.overrideSorting = true;
        darkenCanvas.sortingOrder = PopupSortingOrder - 1;

        BringPopupToFront(popup);
    }

    void HideDarken()
    {
        if (_darken != null)
            _darken.SetActive(false);
    }

    // Click outside the open popup closes it. Remembered briefly so that if that same click lands on the popup's own
    // open button (whose onClick fires on mouse up and would reopen it), it acts as the usual toggle-close instead.
    GameObject _closedByOutsideClick;
    float _closedByOutsideClickTime;
    const float OutsideClickToggleWindow = 0.5f;

    bool IsPointerOver(GameObject popup)
    {
        var rt = popup.transform as RectTransform;
        if (rt == null)
            return true; // can't tell - don't close

        var canvas = popup.GetComponentInParent<Canvas>();
        Camera cam = canvas == null || canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.rootCanvas.worldCamera;
        return RectTransformUtility.RectangleContainsScreenPoint(rt, Input.mousePosition, cam);
    }

    private void Update()
    {
        // Safety net: popup closed some other way.
        if (CurrentPopup != null && !CurrentPopup.activeInHierarchy)
            CurrentPopup = null;

        if (CurrentPopup != null && Input.GetMouseButtonDown(0) && !IsPointerOver(CurrentPopup))
        {
            var popup = CurrentPopup;
            popup.SetActive(false);
            CancelTweens(popup);
            CurrentPopup = null;
            _closedByOutsideClick = popup;
            _closedByOutsideClickTime = Time.unscaledTime;
        }

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
        // This popup was just closed by the outside click that is now hitting its own open button: keep it closed,
        // like clicking the open button of an already open popup always did.
        if (popup == _closedByOutsideClick && Time.unscaledTime - _closedByOutsideClickTime < OutsideClickToggleWindow)
        {
            _closedByOutsideClick = null;
            popup.SetActive(false);
            CancelTweens(popup);
            HideDarken();
            return;
        }
        _closedByOutsideClick = null;

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

        CenterOnScreen(popup);

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

        CurrentPopup = popup;
        ShowDarken(popup);

        // Reset starting state
        //popupBaseValues.CanvasGroup.alpha = 0f;
        popup.transform.position = popupBaseValues.Position + new Vector3(0, -20f, 0); // start slightly lower

        LeanTween.moveY(popup, popupBaseValues.Position.y, 0.2f).setEaseOutCubic();
    }

    // Every popup opens centered on the screen, wherever it was placed in the scene (some are nested, e.g. settings
    // inside its icon). Pivot goes to the center too, so popups that grow to fit their text after opening (victory,
    // X2, credits) grow evenly up and down and stay centered.
    void CenterOnScreen(GameObject popup)
    {
        var rt = popup.transform as RectTransform;
        var rootCanvas = popup.GetComponentInParent<Canvas>()?.rootCanvas;
        if (rt == null || rootCanvas == null)
            return;

        var canvasRt = (RectTransform)rootCanvas.transform;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.position = canvasRt.TransformPoint(canvasRt.rect.center);
    }

    public void AfterHidePopup()
    {
        CurrentPopup = null;
        HideDarken();
    }
}
