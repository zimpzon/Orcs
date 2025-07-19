using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManagerScript : MonoBehaviour
{
    public static PopupManagerScript Instance;

    public Image PopupRoot;
    public TextMeshProUGUI Label;

    public void PlaceLeftOfTarget(RectTransform hoveredRect)
    {
        // Get the world corners of the hovered rect
        Vector3[] corners = new Vector3[4];
        hoveredRect.GetWorldCorners(corners);

        // corners[1] is top-left
        Vector3 topLeftWorld = corners[1];

        // Convert the top-left world position to screen coordinates
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, topLeftWorld);

        // Get the width of the popup in screen space
        RectTransform popupRectTransform = PopupRoot.GetComponent<RectTransform>();

        // Get canvas scale factor (to convert from local size to screen space)
        Canvas canvas = PopupRoot.GetComponentInParent<Canvas>();
        float scaleFactor = canvas ? canvas.scaleFactor : 1f;

        float popupWidth = popupRectTransform.rect.width * scaleFactor;
        float hoveredHeight = hoveredRect.rect.height * scaleFactor;

        Vector2 popupPos = screenPoint;
        popupPos.x -= popupWidth * 0.5f;
        popupPos.y -= hoveredHeight * 0.5f;

        Show(popupPos);
    }

    public void SetText(string text)
    {
        Label.text = text;
    }

    public void Show(Vector2 position)
    {
        PopupRoot.transform.position = position;
        PopupRoot.gameObject.SetActive(true);
    }

    public void Hide()
    {
        PopupRoot.transform.position = Vector2.left * 1000;
        PopupRoot.gameObject.SetActive(false);
    }

    private void Awake()
    {
        Instance = this;
        Hide();
    }
}
