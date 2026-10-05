using UnityEngine;
using UnityEngine.EventSystems;

// "?" icon below the credits heart; opens the tips popup. The "?" itself is a text child set up in the scene.
public class TipsImageScript : MonoBehaviour, IPointerClickHandler
{
    public TipsPopupScript TipsPopup;

    public void OnPointerClick(PointerEventData eventData)
    {
        TipsPopup.Show();
    }
}
