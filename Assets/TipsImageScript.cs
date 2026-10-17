using UnityEngine;

// "?" button below the settings button; opens the tips popup. The "?" itself is a text child set up in the scene.
// A real Button in the scene (like Skins / Bestiary) calls OnClick.
public class TipsImageScript : MonoBehaviour
{
    public TipsPopupScript TipsPopup;

    public void OnClick()
    {
        TipsPopup.Show();
    }
}
