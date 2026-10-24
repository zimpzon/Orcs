using TMPro;
using UnityEngine;

// "More tips" dialog (Tips > More tips, PopupMoreTips in the scene - made once by Tools > UI > Create More Tips Dialog,
// edit it like any other dialog): a header and the text, written in the scene. Closing (X / Escape) goes back to the
// tips.
public class MoreTipsPopup : MonoBehaviour
{
    public TextMeshProUGUI Text; // the tips, authored in the scene

    TipsPopupScript _returnTo;

    public void Show(TipsPopupScript returnTo)
    {
        _returnTo = returnTo;
        gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject); // closes the tips
    }

    public void Close()
    {
        gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
        // Back to the tips (not on Escape: the tips' own X would catch the same key press and close them again).
        if (_returnTo != null && !Input.GetKeyDown(KeyCode.Escape))
            _returnTo.Show();
    }
}
