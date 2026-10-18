using System;
using TMPro;
using UnityEngine;

public class SkinsPopupScript : MonoBehaviour
{
    public SkinsPopupScript Instance;
    public TextMeshProUGUI TextBonusHeader; // "You get bonus income for each unlocked skin." (scene text)
    public TextMeshProUGUI TextBonus;       // scene text with a [BONUS] placeholder; always shown

    // "unlocked / total" under the "Skins" header.
    TextMeshProUGUI _progressText;
    float _nextProgressUpdate;
    string _bonusTemplate;
    string _headerTemplate; // scene text, shown once Skin Collector is bought

    // Bonus line before the card is bought: shown, but faded (same alpha as the disabled Fullscreen checkbox).
    const float DisabledAlpha = 0.4f;

    private void Awake()
    {
        Instance = this;
        _progressText = PopupHeaderProgress.Create(transform, "TextSkins");
        if (TextBonus != null)
            _bonusTemplate = TextBonus.text;
        if (TextBonusHeader != null)
            _headerTemplate = TextBonusHeader.text;
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

        // Skin Collector card: header + bonus line (both authored in the scene, laid out like the bestiary's) are always
        // shown; the bonus only applies once the card is bought (GameManager.UpdateSkinBonus).
        bool bought = SaveGame.Members.BoughtSkinCollector;
        // Not bought: "Unlock with Skin Collector" and a faded bonus line (no green). Bought: the scene header, green number.
        if (TextBonusHeader != null)
            TextBonusHeader.text = bought ? _headerTemplate : "Unlock bonus with Skin Collector";
        if (TextBonus != null)
        {
            long pct = (long)Math.Round(unlocked * Assets.Script.Upgrades.UpgradeProgression.SkinCollectorPerSkin * 100);
            string strPct = $"<color={TextColors.GreenSmallest}>{pct}</color>";
            string text = _bonusTemplate.Replace("[BONUS]", strPct);
            if (!bought)
                text = System.Text.RegularExpressions.Regex.Replace(text, "</?color[^>]*>", "");
            TextBonus.text = text;
            TextBonus.alpha = bought ? 1f : DisabledAlpha;
        }
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
