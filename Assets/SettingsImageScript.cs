using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsImageScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject Popup;
    public Button ButtonDeleteSave;
    public TextMeshProUGUI TextButtonDeleteSave;
    public TMP_InputField TextImportInputField;
    private int _clickCount;

    void SetPopupEnabled(bool enabled)
    {
        _clickCount = 0;
        TextButtonDeleteSave.text = "DELETE SAVE GAME";

        // Save game export text
        TextImportInputField.text = SaveGame.GetObfuscatedSaveGame();

        Popup.SetActive(enabled);
    }

    //public void OnExportSaveClick()
    //{
    //    string fileName = JsMappings.ExportSave();
    //    GameCanvasScript.Instance.ShowPopup("Your save game was exported as: " + fileName);
    //}

    public void OnImportSaveClick()
    {
        if (SaveGame.ImportObfuscatedSaveGame(TextImportInputField.text))
            GameCanvasScript.Instance.ShowPopup("Save game was imported");
    }

    public void OnBackClick()
    {
        SetPopupEnabled(false);
    }

    public void OnDeleteSaveClick()
    {
        if (_clickCount == 0)
        {
            _clickCount = 1;
            TextButtonDeleteSave.text = "YOU SURE?";
        }
        else if (_clickCount == 1)
        {
            GameManager.Instance.ResetAllProgress();
            SetPopupEnabled(false);

            GameCanvasScript.Instance.ShowPopup($"<color=yellow>Your savegame was deleted</color>\n\nWelcome to a new beginning!");

            FloatingTextSpawner.Instance.Spawn(
            GameManager.Instance.ArenaCenter,
            "Save game deleted",
            Color.cyan,
            speed: 0.01f,
            timeToLive: 10.0f,
            fontStyle: FontStyles.Italic);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetPopupEnabled(true);
    }
}
