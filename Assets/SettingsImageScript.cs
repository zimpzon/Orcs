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
    public TMP_InputField TextDiscordLink;
    public Toggle ShowFloatingDamageToggle;
    public Toggle ShowFloatingGoldToggle;
    public Toggle UseScientificNotationToggle;
    public Toggle ShowDetailsOnHoverToggle;

    private int _clickCount;

    void SetPopupEnabled(bool enabled)
    {
        _clickCount = 0;
        TextButtonDeleteSave.text = "DELETE SAVE GAME";

        // Save game export text
        TextImportInputField.text = SaveGame.GetObfuscatedSaveGame();

        Popup.SetActive(enabled);
        if (enabled)
        {
            GlobalPopupManager.Instance.AfterShowPopup(Popup);
        }
        else
        {
            GlobalPopupManager.Instance.AfterHidePopup();
        }

        ShowFloatingDamageToggle.isOn = SaveGame.Members.ShowFloatingDamageNumbers;
        ShowFloatingGoldToggle.isOn = SaveGame.Members.ShowFloatingGoldNumbers;
        UseScientificNotationToggle.isOn = SaveGame.Members.UseScientificNotation;
        ShowDetailsOnHoverToggle.isOn = SaveGame.Members.ShowDetailsOnHover;
    }

    //public void OnExportSaveClick()
    //{
    //    string fileName = JsMappings.ExportSave();
    //    GameCanvasScript.Instance.ShowPopup("Your save game was exported as: " + fileName);
    //}

    public void OnOpenDiscordLink()
    {
        Application.OpenURL(TextDiscordLink.text);
    }

    public void OnImportSaveClick()
    {
        if (SaveGame.ImportObfuscatedSaveGame(TextImportInputField.text))
            GameCanvasScript.Instance.ShowPopup("Save game was imported");
    }

    public void ShowFloatingDamageNumbers(bool show)
    {
        SaveGame.Members.ShowFloatingDamageNumbers = ShowFloatingDamageToggle.isOn;
        SaveGame.Save();
    }

    public void ShowFloatingGoldNumbers(bool show)
    {
        SaveGame.Members.ShowFloatingGoldNumbers = ShowFloatingGoldToggle.isOn;
        SaveGame.Save();
    }

    public void UseScientificNotation(bool show)
    {
        SaveGame.Members.UseScientificNotation = UseScientificNotationToggle.isOn;
        SaveGame.Save();
    }

    public void ShowDetailsOnHover(bool show)
    {
        SaveGame.Members.ShowDetailsOnHover = ShowDetailsOnHoverToggle.isOn;
        SaveGame.Save();
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

            GameCanvasScript.Instance.ShowPopup($"<color=yellow>Your save game was deleted</color>\n\nWelcome to a new beginning!");

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
