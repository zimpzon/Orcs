using UnityEngine;
using UnityEngine.EventSystems;

public class ModalDialogCloser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject dialog; // reference to the dialog root
    private bool pointerInside = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
    }

    void Update()
    {
        // A message popup on top (e.g. "Welcome back" after a rebirth): its clicks are not "outside this dialog".
        if (GenericPopupScript.OpenCount > 0)
            return;

        if (!pointerInside && Input.GetMouseButtonDown(0))
        {
            dialog.SetActive(false);
            GlobalPopupManager.Instance.AfterHidePopup();
        }
    }
}
