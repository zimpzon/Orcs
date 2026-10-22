using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Clicks on <link="id"> tags in a TMP text, handled in code (ClickableLinks opens them as URLs instead).
public class TmpLinkClick : MonoBehaviour, IPointerClickHandler
{
    public Action<string> OnLink;

    public void OnPointerClick(PointerEventData eventData)
    {
        var text = GetComponent<TextMeshProUGUI>();
        if (text == null)
            return;
        int index = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, eventData.pressEventCamera);
        if (index >= 0)
            OnLink?.Invoke(text.textInfo.linkInfo[index].GetLinkID());
    }
}
