using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableLinks : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(
            textComponent,
            eventData.position,
            eventData.pressEventCamera);

        if (linkIndex != -1)
        {
            string url = textComponent.textInfo.linkInfo[linkIndex].GetLinkID();
            Application.OpenURL(url);
        }
    }
}
