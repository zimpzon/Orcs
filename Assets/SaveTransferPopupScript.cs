using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Save game copy/paste (backup or move a save between browsers/computers). Opened with LeftShift+S, a player-facing
// shortcut handled in GameManager since this popup starts inactive.
public class SaveTransferPopupScript : MonoBehaviour
{
    public TMP_InputField SaveText;

    static SaveTransferPopupScript _instance;

    static SaveTransferPopupScript Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<SaveTransferPopupScript>(FindObjectsInactive.Include);
            return _instance;
        }
    }

    public static void CheckHotkey()
    {
        if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKeyDown(KeyCode.S))
            return;

        // Typing a capital S somewhere must not open/close it.
        var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null && selected.GetComponent<TMP_InputField>() != null)
            return;

        var popup = Instance;
        if (popup == null)
            return;

        if (popup.gameObject.activeSelf)
            popup.OnClose();
        else if (!GlobalPopupManager.Instance.HasOpenPopup)
            popup.Show();
    }

    public void Show()
    {
        SaveText.text = SaveGame.GetObfuscatedSaveGame();
        gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);

        // Focus the field; it selects all on focus, so Ctrl+C copies the save right away.
        SaveText.Select();
        SaveText.ActivateInputField();
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    public void OnImportClick()
    {
        if (SaveGame.ImportObfuscatedSaveGame(SaveText.text))
        {
            OnClose();
            GameCanvasScript.Instance.ShowPopup("Save game was imported");
        }
    }
}
