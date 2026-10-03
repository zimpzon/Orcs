using TMPro;
using UnityEngine;

public class SkinsPopupScript : MonoBehaviour
{
    public SkinsPopupScript Instance;

    // "unlocked / total" under the "Skins" header.
    TextMeshProUGUI _progressText;
    float _nextProgressUpdate;

    private void Awake()
    {
        Instance = this;
        _progressText = PopupHeaderProgress.Create(transform, "TextSkins");
    }

    private void Update()
    {
        // GetUnlockProgress scans the skin objects, so only refresh a couple of times per second.
        if (_progressText == null || Time.unscaledTime < _nextProgressUpdate)
            return;

        _nextProgressUpdate = Time.unscaledTime + 0.5f;
        (int unlocked, int total) = SkinScript.GetUnlockProgress();
        _progressText.text = $"({unlocked}/{total})";
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }
}
