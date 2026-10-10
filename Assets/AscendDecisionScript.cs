using Assets.Script.Upgrades;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscendDecisionScript : MonoBehaviour
{
    public static AscendDecisionScript Instance;

    public AscendProgressScript AscendProgressScript;
    public Button ButtonAscend;
    public TextMeshProUGUI TextCurrentDiamonds;
    public TextMeshProUGUI TextCurrentMonsterCredits;
    public TextMeshProUGUI TextAscendNowGain;
    public TextMeshProUGUI TextButtonRebirth;
    public TextMeshProUGUI TextWhatYouLose;
    public TextMeshProUGUI TextDiamondEach; // scene text with a [PCT] placeholder

    [NonSerialized] public long MonsterCreditsAtStart;
    [NonSerialized] public long DiamondsGainedAtRebirth;

    private string _whatYouLoseTemplate;
    private string _diamondEachTemplate;
    private int _clickCount;

    // Header: "◆ Rebirth ◆" with the two diamonds gently bobbing. Built from the scene's header text (was
    // "Rebirth<sprite=0>"), so no scene wiring is needed.
    const float DiamondGap = 24f;  // glyph edge of "Rebirth" to diamond center
    const float DiamondScale = 0.75f;  // relative to the header font size
    const float BobAmount = 3f;
    const float BobSpeed = 2.2f;
    RectTransform[] _headerDiamonds;
    Vector3[] _headerDiamondBase;

    private void Awake()
    {
        Instance = this;
        _whatYouLoseTemplate = TextWhatYouLose.text;
        if (TextDiamondEach != null)
            _diamondEachTemplate = TextDiamondEach.text;
        SetupHeaderDiamonds();
    }

    void SetupHeaderDiamonds()
    {
        Transform headerTransform = null;
        for (var t = transform.parent; t != null && headerTransform == null; t = t.parent)
            headerTransform = t.Find("HeaderSection/TextHeader");
        if (headerTransform == null)
            return;

        var header = headerTransform.GetComponent<TextMeshProUGUI>();
        header.text = "Rebirth";
        // Drawn glyph bounds in the header's local space, so placement doesn't depend on the rect's pivot/anchors.
        Bounds bounds = DrawnGlyphBounds(header);

        _headerDiamonds = new RectTransform[2];
        _headerDiamondBase = new Vector3[2];
        for (int i = 0; i < 2; ++i)
        {
            var diamond = Instantiate(header, header.rectTransform, false);
            diamond.name = i == 0 ? "HeaderDiamondLeft" : "HeaderDiamondRight";
            // The copy also copied the header's children (the left diamond, when making the right one): drop them.
            foreach (Transform child in diamond.transform)
                Destroy(child.gameObject);
            diamond.text = "<sprite=0>";
            diamond.raycastTarget = false;

            diamond.fontSize = header.fontSize * DiamondScale;

            var rect = diamond.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(40f, header.rectTransform.rect.height);

            // The sprite glyph isn't centered in its own rect (sprite bearing), which pushed both diamonds right.
            // Measure it and center the glyph itself on the target spot.
            Vector3 glyphCenter = DrawnGlyphBounds(diamond).center;

            float x = i == 0 ? bounds.min.x - DiamondGap : bounds.max.x + DiamondGap;
            _headerDiamondBase[i] = new Vector3(x - glyphCenter.x, bounds.center.y - glyphCenter.y, 0);
            rect.localPosition = _headerDiamondBase[i];
            _headerDiamonds[i] = rect;
        }
    }

    // Bounds of the visible glyph quads only. TMP's textBounds includes the last character's trailing advance, which
    // made the right side measure wider than it looks.
    static Bounds DrawnGlyphBounds(TextMeshProUGUI text)
    {
        text.ForceMeshUpdate();
        var info = text.textInfo;
        bool any = false;
        var bounds = new Bounds();
        for (int i = 0; i < info.characterCount; ++i)
        {
            var c = info.characterInfo[i];
            if (!c.isVisible)
                continue;

            var min = c.vertex_BL.position;
            var max = c.vertex_TR.position;
            if (!any)
            {
                bounds = new Bounds((min + max) * 0.5f, max - min);
                any = true;
            }
            else
            {
                bounds.Encapsulate(min);
                bounds.Encapsulate(max);
            }
        }

        return any ? bounds : text.textBounds;
    }

    private void Update()
    {
        if (_headerDiamonds == null)
            return;

        for (int i = 0; i < _headerDiamonds.Length; ++i)
        {
            // Up and down only, slightly out of step with each other.
            float wave = Mathf.Sin(Time.unscaledTime * BobSpeed + i * 1.0f);
            _headerDiamonds[i].localPosition = _headerDiamondBase[i] + new Vector3(0, wave * BobAmount, 0);
        }
    }

    public void OnAscendClick()
    {
        if (_clickCount == 0)
        {
            _clickCount = 1;
            TextButtonRebirth.text = "YOU SURE?";
        }
        else if (_clickCount == 1)
        {
            if (AscendProgressScript.RebirthEnabled)
            {
                SaveGame.Members.MonsterCredits_09_08_2025 -= MonsterCreditsAtStart;
                SaveGame.Members.DiamondCount_09_08_2025 += MonsterCreditsAtStart;
                SaveGame.Members.TimesAscended_09_08_2025++;
                SaveGame.Members.TimeSinceLastAscend = 0;

                GameManager.Instance.ResetAllProgress(ascend: true);

                string diamondGainTxt = DiamondsGainedAtRebirth == 1 ? "DIAMOND" : "DIAMONDS";
                string msg = $"<color=yellow>REBIRTH</color>\n\nWelcome back!\n\nYOU GAINED {DiamondsGainedAtRebirth} {diamondGainTxt}";
                msg += "\n\nSpend your diamonds on the cards!";
                GameCanvasScript.Instance.ShowPopup(msg);

                // Stay on the rebirth screen so the new diamonds can be spent right away (0 credits now, so the
                // Rebirth button disables itself).
                ResetDialogState();
            }
            else
            {
                GameCanvasScript.Instance.ShowPopup("Nothing happened. This feature is not implemented yet.");
                AscendProgressScript.OnCloseClick();
            }
        }
    }

    public void UpdateUi()
    {
        //Color gainTextColor = DiamondsGainedAtRebirth == 0 ? new Color(0.9f, 0.2f, 0.1f) : Color.yellow;
        string gainTextColorStr = DiamondsGainedAtRebirth == 0 ?  "E35125" : "9DE05C";// ColorUtility.ToHtmlStringRGBA(gainTextColor);

        string diamondTxt = SaveGame.Members.DiamondCount_09_08_2025 == 1 ? "diamond" : "diamonds";
        string diamondGainTxt = DiamondsGainedAtRebirth == 1 ? "diamond" : "diamonds";
        string creditTxt = MonsterCreditsAtStart == 1 ? "credit" : "credits";

        long diamondIncomePct = (long)Math.Round(UpgradeProgression.DiamondIncomeBonus() * 100);
        TextCurrentDiamonds.text = $"You have <color=#9DE05C>{Assets.Script.Misc.Format64.Format(SaveGame.Members.DiamondCount_09_08_2025)}</color> {diamondTxt} <sprite=0> (+<color=#9DE05C>{diamondIncomePct}</color>% income)";

        // Scene text with a [PCT] placeholder: income bonus per diamond held (base + Shiny Diamonds cards).
        if (TextDiamondEach != null)
            TextDiamondEach.text = _diamondEachTemplate.Replace("[PCT]", Math.Round(UpgradeProgression.DiamondBonusPerDiamond() * 100).ToString());
        TextCurrentMonsterCredits.text = $"You have <color=#{gainTextColorStr}>{MonsterCreditsAtStart}</color> {creditTxt}";
        TextAscendNowGain.text = $"Rebirth now to gain +<color=#{gainTextColorStr}>{DiamondsGainedAtRebirth}</color> {diamondGainTxt}<sprite=0>";

        TextWhatYouLose.text = _whatYouLoseTemplate;

        ButtonAscend.interactable = SaveGame.Members.MonsterCredits_09_08_2025 > 0;
    }

    private void OnEnable() => ResetDialogState();

    // Fresh numbers and button state; also used right after a rebirth, since the screen stays open then.
    void ResetDialogState()
    {
        MonsterCreditsAtStart = SaveGame.Members.MonsterCredits_09_08_2025;
        DiamondsGainedAtRebirth = UpgradeProgression.DiamondsForMonsterCredits(MonsterCreditsAtStart);
        UpdateUi();

        _clickCount = 0;
        TextButtonRebirth.text = "REBIRTH!";
    }
}
