using System;
using TMPro;
using UnityEngine;

public class SkinsPopupScript : MonoBehaviour
{
    public SkinsPopupScript Instance;
    public TextMeshProUGUI TextBonusHeader; // "You get bonus income for each unlocked skin." (scene text)
    public TextMeshProUGUI TextBonus;       // scene text with a [BONUS] placeholder; both shown once Skin Collector is bought

    // "unlocked / total" under the "Skins" header.
    TextMeshProUGUI _progressText;
    float _nextProgressUpdate;
    string _bonusTemplate;

    private void Awake()
    {
        Instance = this;
        _progressText = PopupHeaderProgress.Create(transform, "TextSkins");
        if (TextBonus != null)
            _bonusTemplate = TextBonus.text;
    }

    private void Update()
    {
        // GetUnlockProgress scans the skin objects, so only refresh a couple of times per second.
        if (Time.unscaledTime < _nextProgressUpdate)
            return;

        _nextProgressUpdate = Time.unscaledTime + 0.5f;
        (int unlocked, int total) = SkinScript.GetUnlockProgress();
        if (_progressText != null)
            _progressText.text = $"({unlocked}/{total})";

        // Skin Collector card: header + bonus line (both authored in the scene, laid out like the bestiary's), only
        // shown once the card is bought.
        bool bought = SaveGame.Members.BoughtSkinCollector;
        SetShown(TextBonusHeader, bought);
        SetShown(TextBonus, bought);
        if (bought && TextBonus != null)
        {
            long pct = (long)Math.Round(unlocked * Assets.Script.Upgrades.UpgradeProgression.SkinCollectorPerSkin * 100);
            TextBonus.text = _bonusTemplate.Replace("[BONUS]", pct.ToString());
        }
    }

    static void SetShown(TextMeshProUGUI text, bool shown)
    {
        if (text == null)
            return;
        text.enabled = shown;
        if (text.gameObject.activeSelf != shown)
            text.gameObject.SetActive(shown);
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
