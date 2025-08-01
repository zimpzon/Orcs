using UnityEngine;
using UnityEngine.EventSystems;

public class QuestionmarkScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Popup;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Popup.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Popup.SetActive(false);
    }
}
