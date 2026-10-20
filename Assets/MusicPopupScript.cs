using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Music dialog, opened from the ♪ button in settings: the playlist's track names; click one to play it (turns music on
// if it was off). The playing track is highlighted. Rows are copies of RowTemplate (a button in the scene, inactive)
// under TrackList (a vertical layout). Escape / OK closes it.
public class MusicPopupScript : MonoBehaviour
{
    public static MusicPopupScript Instance;

    public RectTransform TrackList;
    public Button RowTemplate;

    static readonly string PlayingHex = TextColors.Gold;

    readonly List<TextMeshProUGUI> _rowTexts = new();
    SettingsImageScript _returnTo; // settings, reopened when this closes
    int _shownTrack = -2;
    bool _shownOn;

    void Awake()
    {
        Instance = this;
    }

    public void Show(SettingsImageScript returnTo = null)
    {
        _returnTo = returnTo;
        BuildRows();
        gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
        _shownTrack = -2; // refresh the highlight
    }

    public void OnClose()
    {
        gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    // However it closes (OK, Escape, click outside): back to where it was opened from.
    void OnDisable()
    {
        var back = _returnTo;
        _returnTo = null;
        if (back != null)
            back.ReopenAfterMusic();
    }

    void BuildRows()
    {
        var music = MusicManagerScript.Instance;
        if (_rowTexts.Count > 0 || music == null || RowTemplate == null || TrackList == null)
            return;

        RowTemplate.gameObject.SetActive(false);
        for (int i = 0; i < music.TrackCount; ++i)
        {
            int index = i;
            var row = Instantiate(RowTemplate, TrackList);
            row.name = "Track " + (i + 1);
            row.gameObject.SetActive(true);
            row.onClick.RemoveAllListeners();
            row.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayMenuClick();
                MusicManagerScript.Instance.PlayFromList(index);
            });
            _rowTexts.Add(row.GetComponentInChildren<TextMeshProUGUI>(true));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClose();
            return;
        }

        var music = MusicManagerScript.Instance;
        if (music == null)
            return;
        bool on = SaveGame.Members.MusicEnabled;
        if (music.CurrentTrack == _shownTrack && on == _shownOn)
            return;
        _shownTrack = music.CurrentTrack;
        _shownOn = on;

        for (int i = 0; i < _rowTexts.Count; ++i)
        {
            if (_rowTexts[i] == null)
                continue;
            string name = music.TrackName(i);
            _rowTexts[i].text = i == _shownTrack && on
                ? $"<color={PlayingHex}>{name}</color>"
                : $"{name}";
        }
    }
}
