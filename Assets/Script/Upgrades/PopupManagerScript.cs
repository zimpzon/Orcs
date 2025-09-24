using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManagerScript : MonoBehaviour
{
    public static PopupManagerScript Instance;
    public Image PopupRoot;
    public TextMeshProUGUI Label;
    private float _hideTime;
    private bool _hidePending;

    public void PlaceNextToTarget(RectTransform hoveredRect)
    {
        // Compute world corners and convert to screen space for placement anchors
        Vector3[] corners = new Vector3[4];
        hoveredRect.GetWorldCorners(corners);
        Vector2 topLeftScreen = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
        Vector2 topRightScreen = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        // Determine where the hovered rect sits relative to the screen center
        Vector2 hoveredCenterScreen = RectTransformUtility.WorldToScreenPoint(
            null,
            hoveredRect.TransformPoint(hoveredRect.rect.center)
        );

        // Get the popup dimensions in screen space
        RectTransform popupRectTransform = PopupRoot.GetComponent<RectTransform>();
        Canvas canvas = PopupRoot.GetComponentInParent<Canvas>();
        float scaleFactor = canvas ? canvas.scaleFactor : 1f;

        float popupWidth = popupRectTransform.rect.width * scaleFactor;
        float hoveredHeight = hoveredRect.rect.height * scaleFactor;

        // Place popup on the side opposite the screen edge to keep it visible
        Vector2 popupPos;
        if (hoveredCenterScreen.x < Screen.width * 0.5f)
        {
            popupPos = topRightScreen;
            popupPos.x += popupWidth * 0.5f;
        }
        else
        {
            popupPos = topLeftScreen;
            popupPos.x -= popupWidth * 0.5f;
        }

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
        _hidePending = false;
    }

    public void Hide()
    {
        _hidePending = true;
        _hideTime = G.D.GameTime + 0.1f;
    }

    private void Update()
    {
        if (_hidePending && G.D.GameTime > _hideTime)
        {
            PopupRoot.transform.position = Vector2.left * 1000;
            PopupRoot.gameObject.SetActive(false);
            _hidePending = false;
        }
    }

    private void Awake()
    {
        Instance = this;
        Hide();
    }
}
