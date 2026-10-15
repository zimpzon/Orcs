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

    public Toggle CombatSoundToggle;
    public Toggle MusicToggle;
    public Toggle SoundInBackgroundToggle;
    public Toggle FullscreenToggle; // always shown; only clickable in standalone builds on a 16:9 / 16:10 monitor
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SfxVolumeSlider;

    const float DisabledAlpha = 0.4f;

    private int _clickCount;

    // Listeners in code, so the scene needs no persistent calls for them.
    void Awake()
    {
        if (FullscreenToggle != null)
            FullscreenToggle.onValueChanged.AddListener(on =>
            {
                AudioManager.Instance.PlayMenuClick();
                WindowedAspect.SetFullscreen(on);
            });

        CombatSoundToggle.onValueChanged.AddListener(on =>
        {
            SaveGame.Members.CombatSoundEnabled = on;
            AudioManager.Instance.ApplySoundSettings();
            SaveGame.Save();
            AudioManager.Instance.PlayMenuClick();
        });

        SoundInBackgroundToggle.onValueChanged.AddListener(on =>
        {
            SaveGame.Members.SoundInBackground = on;
            AudioManager.Instance.ApplyBackgroundPause();
            SaveGame.Save();
            AudioManager.Instance.PlayMenuClick();
        });

        MusicToggle.onValueChanged.AddListener(on =>
        {
            SaveGame.Members.MusicEnabled = on;
            MusicManagerScript.Instance.ApplyMusicEnabled();
            SaveGame.Save();
            AudioManager.Instance.PlayMenuClick();
        });

        // Volumes: saved by the regular auto-save (every 5 s), not on every drag step.
        MasterVolumeSlider.onValueChanged.AddListener(value =>
        {
            SaveGame.Members.VolumeMaster = value;
            GameManager.Instance.ApplyVolume();
        });
        MusicVolumeSlider.onValueChanged.AddListener(value =>
        {
            SaveGame.Members.VolumeMusic = value;
            GameManager.Instance.ApplyVolume();
        });
        SfxVolumeSlider.onValueChanged.AddListener(value =>
        {
            SaveGame.Members.VolumeSfx = value;
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

        // Without notify: setting them must not run the handlers (they click and save).
        ShowFloatingDamageToggle.SetIsOnWithoutNotify(SaveGame.Members.ShowFloatingDamageNumbers);
        ShowFloatingGoldToggle.SetIsOnWithoutNotify(SaveGame.Members.ShowFloatingGoldNumbers);
        UseScientificNotationToggle.SetIsOnWithoutNotify(SaveGame.Members.UseScientificNotation);
        ShowDetailsOnHoverToggle.SetIsOnWithoutNotify(SaveGame.Members.ShowDetailsOnHover);
        CombatSoundToggle.SetIsOnWithoutNotify(SaveGame.Members.CombatSoundEnabled);
        MusicToggle.SetIsOnWithoutNotify(SaveGame.Members.MusicEnabled);
        SoundInBackgroundToggle.SetIsOnWithoutNotify(SaveGame.Members.SoundInBackground);

        // Fullscreen works only where WindowedAspect runs (standalone builds; not WebGL / Editor / Steam Deck, which
        // is fullscreen already) and on a 16:9 / 16:10 monitor. Elsewhere it's shown disabled.
        if (FullscreenToggle != null)
        {
            bool canFullscreen = WindowedAspect.IsActive && WindowedAspect.MonitorSupportsFullscreen;
            FullscreenToggle.interactable = canFullscreen;
            FullscreenToggle.SetIsOnWithoutNotify(WindowedAspect.IsFullscreen);
            // Faded when disabled (the toggle itself has no disabled look). CanvasGroup is on the toggle in the scene.
            if (FullscreenToggle.TryGetComponent<CanvasGroup>(out var group))
                group.alpha = canFullscreen ? 1f : DisabledAlpha;
        }
        MasterVolumeSlider.SetValueWithoutNotify(SaveGame.Members.VolumeMaster);
        MusicVolumeSlider.SetValueWithoutNotify(SaveGame.Members.VolumeMusic);
        SfxVolumeSlider.SetValueWithoutNotify(SaveGame.Members.VolumeSfx);
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
        AudioManager.Instance.PlayMenuClick();
    }

    public void ShowFloatingGoldNumbers(bool show)
    {
        SaveGame.Members.ShowFloatingGoldNumbers = ShowFloatingGoldToggle.isOn;
        SaveGame.Save();
        AudioManager.Instance.PlayMenuClick();
    }

    public void UseScientificNotation(bool show)
    {
        SaveGame.Members.UseScientificNotation = UseScientificNotationToggle.isOn;
        SaveGame.Save();
        AudioManager.Instance.PlayMenuClick();
    }

    public void ShowDetailsOnHover(bool show)
    {
        SaveGame.Members.ShowDetailsOnHover = ShowDetailsOnHoverToggle.isOn;
        SaveGame.Save();
        AudioManager.Instance.PlayMenuClick();
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
        // The popup is a child of this icon, so clicks inside it bubble up here - they'd toggle it closed.
        var clicked = eventData.rawPointerPress != null ? eventData.rawPointerPress : eventData.pointerCurrentRaycast.gameObject;
        if (clicked != null && clicked.transform.IsChildOf(Popup.transform))
            return;

        SetPopupEnabled(true);
    }
}
