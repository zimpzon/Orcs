using UnityEngine;
using UnityEngine.EventSystems;

// Opens the credits popup on click: the heart icon below the settings icon, and the Idle Earl logo text.
public class CreditsImageScript : MonoBehaviour, IPointerClickHandler
{
    public CreditsPopupScript CreditsPopup;

    public void OnPointerClick(PointerEventData eventData)
    {
        CreditsPopup.Show();
    }
}
