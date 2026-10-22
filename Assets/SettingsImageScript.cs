using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsImageScript : MonoBehaviour
{
    public GameObject Popup;
    public Button ButtonDeleteSave;
    public TextMeshProUGUI TextButtonDeleteSave;
    public TMP_InputField TextDiscordLink;
    public Toggle ShowFloatingDamageToggle;
    public Toggle ShowDetailsOnHoverToggle;

    public Toggle CombatSoundToggle;
    public Toggle MusicToggle;
    public Toggle SoundInBackgroundToggle;
    public Toggle HideVictoryBannerToggle; // disabled until Scary Earl is defeated (his victory banner exists from then)
    public Toggle ShowHealthBarsToggle;
    public Toggle FullscreenToggle; // always shown; only clickable in standalone builds on a 16:9 / 16:10 monitor
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SfxVolumeSlider;
    public CreditsPopupScript CreditsPopup;
    public MusicPopupScript MusicPopup;
    public Button MusicListButton; // ♪ under the Music checkbox: only usable while music is on

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

        if (ShowHealthBarsToggle != null)
            ShowHealthBarsToggle.onValueChanged.AddListener(on =>
            {
                SaveGame.Members.ShowHealthBars = on;
                SaveGame.Save();
                AudioManager.Instance.PlayMenuClick();
            });

        if (HideVictoryBannerToggle != null)
            HideVictoryBannerToggle.onValueChanged.AddListener(on =>
            {
                SaveGame.Members.HideVictoryBanner = on;
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
            UpdateMusicListButton();
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
        ShowDetailsOnHoverToggle.SetIsOnWithoutNotify(SaveGame.Members.ShowDetailsOnHover);
        CombatSoundToggle.SetIsOnWithoutNotify(SaveGame.Members.CombatSoundEnabled);
        // No music on the web: the checkbox is shown off and disabled (faded like Fullscreen).
        bool musicSupported = MusicManagerScript.Supported;
        MusicToggle.SetIsOnWithoutNotify(musicSupported && SaveGame.Members.MusicEnabled);
        MusicToggle.interactable = musicSupported;
        if (!MusicToggle.TryGetComponent<CanvasGroup>(out var musicGroup))
            musicGroup = MusicToggle.gameObject.AddComponent<CanvasGroup>();
        musicGroup.alpha = musicSupported ? 1f : DisabledAlpha;
        if (!musicSupported)
            MarkMusicSteamOnly();
        UpdateMusicListButton();
        SoundInBackgroundToggle.SetIsOnWithoutNotify(SaveGame.Members.SoundInBackground);
        if (ShowHealthBarsToggle != null)
            ShowHealthBarsToggle.SetIsOnWithoutNotify(SaveGame.Members.ShowHealthBars);
        if (HideVictoryBannerToggle != null)
        {
            // Shown always, but disabled (faded like Music on the web) until Scary Earl is defeated.
            bool defeated = SaveGame.Members.ScaryEarlDefeated;
            HideVictoryBannerToggle.gameObject.SetActive(true);
            HideVictoryBannerToggle.interactable = defeated;
            if (!HideVictoryBannerToggle.TryGetComponent<CanvasGroup>(out var bannerGroup))
                bannerGroup = HideVictoryBannerToggle.gameObject.AddComponent<CanvasGroup>();
            bannerGroup.alpha = defeated ? 1f : DisabledAlpha;
            HideVictoryBannerToggle.SetIsOnWithoutNotify(SaveGame.Members.HideVictoryBanner);
        }

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

    public void ShowDetailsOnHover(bool show)
    {
        SaveGame.Members.ShowDetailsOnHover = ShowDetailsOnHoverToggle.isOn;
        SaveGame.Save();
        AudioManager.Instance.PlayMenuClick();
    }

    // Credits button on the settings page: settings closes, the credits dialog opens on its own (closing it goes back
    // to the game, not to settings).
    public void OnCreditsClick()
    {
        SetPopupEnabled(false);
        CreditsPopup.Show();
    }

    // Web: the music files aren't in the build (StripMusicForWebGL), so say why the checkbox is off. The label is the
    // "Music ..." text in the toggle (or next to it).
    void MarkMusicSteamOnly()
    {
        var label = FindMusicLabel(MusicToggle.transform) ?? FindMusicLabel(MusicToggle.transform.parent);
        if (label != null)
            label.text = "Music <color=#BBBBBB>(Steam only)</color>";
    }

    static TMP_Text FindMusicLabel(Transform root)
    {
        if (root == null)
            return null;
        foreach (var text in root.GetComponentsInChildren<TMP_Text>(true))
            if (text.text.StartsWith("Music"))
                return text;
        return null;
    }

    // ♪ button under the Music checkbox: settings closes, the music dialog (track list) opens on its own.
    void UpdateMusicListButton()
    {
        if (MusicListButton != null)
            MusicListButton.interactable = MusicManagerScript.Supported && SaveGame.Members.MusicEnabled;
    }

    public void OnMusicListClick()
    {
        SetPopupEnabled(false);
        MusicPopup.Show(this);
    }

    // The music dialog closed (OK, Escape or a click outside): back to settings, a frame later so the popup manager
    // is done closing the music dialog first.
    public void ReopenAfterMusic()
    {
        if (isActiveAndEnabled)
            StartCoroutine(ReopenNextFrame());
    }

    System.Collections.IEnumerator ReopenNextFrame()
    {
        yield return null;
        if (!Popup.activeSelf && !GlobalPopupManager.AnyDialogOpen)
            SetPopupEnabled(true);
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

            GameCanvasScript.Instance.ShowPopup($"<color={TextColors.Yellow}>Your save game was deleted</color>\n\nWelcome to a new beginning!");

            FloatingTextSpawner.Instance.Spawn(
            GameManager.Instance.ArenaCenter,
            "Save game deleted",
            Color.cyan,
            speed: 0.01f,
            timeToLive: 10.0f,
            fontStyle: FontStyles.Italic);
        }
    }

    // The settings Button (a child next to the popup, so clicks inside the popup don't reach it) calls this.
    public void OnButtonClick()
    {
        SetPopupEnabled(true);
    }
}
