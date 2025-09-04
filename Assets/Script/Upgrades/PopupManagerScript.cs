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
        // corners[2] is top-right
        Vector3 topRightWorld = corners[2];

        // Convert the top-right world position to screen coordinates
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, topRightWorld);

        // Get the width of the popup in screen space
        RectTransform popupRectTransform = PopupRoot.GetComponent<RectTransform>();

        // Get canvas scale factor (to convert from local size to screen space)
        Canvas canvas = PopupRoot.GetComponentInParent<Canvas>();
        float scaleFactor = canvas ? canvas.scaleFactor : 1f;

        float popupWidth = popupRectTransform.rect.width * scaleFactor;
        float hoveredHeight = hoveredRect.rect.height * scaleFactor;

        Vector2 popupPos = screenPoint;
        popupPos.x += popupWidth * 0.5f; // Changed to place popup to the right
        popupPos.y -= hoveredHeight * 0.5f;

        Show(popupPos);
    }

    public void SetText(string text)
    {
        Label.text = text;
    }

    public void Show(Vector2 position)
    {
        // Activate popup first to ensure it's in the hierarchy for calculations
        PopupRoot.gameObject.SetActive(true);

        // Force canvas to update layouts immediately so we get accurate dimensions
        Canvas.ForceUpdateCanvases();

        // Get popup dimensions in screen space
        RectTransform popupRectTransform = PopupRoot.GetComponent<RectTransform>();
        Canvas canvas = PopupRoot.GetComponentInParent<Canvas>();
        float scaleFactor = canvas ? canvas.scaleFactor : 1f;

        float popupWidth = popupRectTransform.rect.width * scaleFactor;
        float popupHeight = popupRectTransform.rect.height * scaleFactor;

        // Get screen dimensions
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate popup bounds (assuming center pivot)
        float halfWidth = popupWidth * 0.5f;
        float halfHeight = popupHeight * 0.5f;

        // Constrain position to screen bounds
        Vector2 constrainedPosition = position;

        // Horizontal constraints
        constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, halfWidth, screenWidth - halfWidth);

        // Vertical constraints  
        constrainedPosition.y = Mathf.Clamp(constrainedPosition.y, halfHeight, screenHeight - halfHeight);

        PopupRoot.transform.position = constrainedPosition;
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