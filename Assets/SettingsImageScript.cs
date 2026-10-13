using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsImageScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject Popup;
    public Button ButtonDeleteSave;
    public TextMeshProUGUI TextButtonDeleteSave;
    public TMP_InputField TextDiscordLink;
    public Toggle ShowFloatingDamageToggle;
    public Toggle ShowFloatingGoldToggle;
    public Toggle UseScientificNotationToggle;
    public Toggle ShowDetailsOnHoverToggle;

    public Toggle SoundToggle;
    public Toggle MenuSoundToggle;
    public Slider VolumeSlider;

    private int _clickCount;

    // Listeners in code, so the scene needs no persistent calls for them.
    void Awake()
    {
        SoundToggle.onValueChanged.AddListener(on =>
        {
            SaveGame.Members.SoundEnabled = on;
            AudioManager.Instance.ApplySoundSettings();
            SaveGame.Save();
        });

        MenuSoundToggle.onValueChanged.AddListener(on =>
        {
            SaveGame.Members.MenuSoundEnabled = on;
            SaveGame.Save();
            if (on)
                AudioManager.Instance.PlayMenuClick(); // little confirmation
        });

        // Saved by the regular auto-save (every 5 s), not on every drag step.
        VolumeSlider.onValueChanged.AddListener(value =>
        {
            SaveGame.Members.VolumeMaster = value;
            GameManager.Instance.ApplyVolume();
        });
    }

    void SetPopupEnabled(bool enabled)
    {
        _clickCount = 0;
        TextButtonDeleteSave.text = "DELETE SAVE GAME";

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
        SoundToggle.SetIsOnWithoutNotify(SaveGame.Members.SoundEnabled);
        MenuSoundToggle.SetIsOnWithoutNotify(SaveGame.Members.MenuSoundEnabled);
        VolumeSlider.SetValueWithoutNotify(SaveGame.Members.VolumeMaster);
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
