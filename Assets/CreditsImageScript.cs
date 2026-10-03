using UnityEngine;
using UnityEngine.EventSystems;

// Heart icon below the settings icon; opens the credits popup.
public class CreditsImageScript : MonoBehaviour, IPointerClickHandler
{
    public CreditsPopupScript CreditsPopup;

    public void OnPointerClick(PointerEventData eventData)
    {
        CreditsPopup.Show();
    }
}
