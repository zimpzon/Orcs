using UnityEngine;
using UnityEngine.UI;

// Escape with nothing else open shows this menu (for now just Quit game); Escape again or a click outside closes it.
// Lives on an always-active object with the menu popup as child, like SettingsImageScript. Runs before every other
// script, so it sees the state before Escape closes anything this frame (a dialog's back button, a message popup,
// the expanded upgrade panel, the cheat sheet): if anything is open, Escape is theirs.
[DefaultExecutionOrder(-500)]
public class EscapeMenuScript : MonoBehaviour
{
    public GameObject Popup;
    public Button QuitButton;
    public GameObject RebirthDialog; // AscendProgressScript.AscendRoot (not a GlobalPopupManager popup)

    bool IsOpen => Popup.activeSelf;

    bool AnythingElseOpen()
        => GlobalPopupManager.Instance.HasOpenPopup
        || GenericPopupScript.OpenCount > 0
        || CheatSheet.IsOpen
        || UpgradeResizer.ExpandedCount > 0
        || (RebirthDialog != null && RebirthDialog.activeInHierarchy);

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (IsOpen)
            Close();
        else if (!AnythingElseOpen())
            Open();
    }

    void Open()
    {
        // Quitting only makes sense in a desktop build (not the Editor, not WebGL); shown disabled elsewhere.
        QuitButton.interactable = Application.platform is RuntimePlatform.WindowsPlayer or RuntimePlatform.LinuxPlayer or RuntimePlatform.OSXPlayer;
        _openedFrame = Time.frameCount;
        Popup.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(Popup);
    }

    void Close()
    {
        Popup.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    // Close button (ButtonBack prefab, top right like the other dialogs). Its ButtonBackScript also clicks it on
    // Escape: ignored in the frame the menu opened, so the Escape that opened it can't close it again right away.
    public void OnCloseClick()
    {
        if (Time.frameCount == _openedFrame || !IsOpen)
            return;
        Close();
    }

    int _openedFrame = -1;

    public void OnQuitClick()
    {
        SaveGame.Save();
        Application.Quit();
    }
}
