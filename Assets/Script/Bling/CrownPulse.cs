using UnityEngine;
using UnityEngine.UI;

// Victory button crown that pulses once the game is 100% complete (Completion100 achievement): a larger,
// semi-transparent crown pops up over the icon, then shrinks toward the icon size while fading, and repeats.
// Never fully opaque and never size zero, so the real icon always shows and it stays a gentle hint.
// Also used as a button pulse: on the rebirth (diamond) button while the player has credits but never rebirthed
// (the same big pulse as the other buttons, until the first rebirth), and on the Victory button after 100% until the
// victory dialog has been opened once.
[RequireComponent(typeof(Image))]
public class CrownPulse : MonoBehaviour
{
    public enum PulseCondition { Completion100, FirstRebirthReady, VictoryUnseen, FirstRebirthReadySeen, TipsUnseen, ExpandTiersUnseen, CreditBonusLow }
    public PulseCondition Condition = PulseCondition.Completion100;

    public float Period = 1.6f;      // seconds per pop + shrink/fade
    public float StartScale = 1.8f;  // x icon size, right after the pop
    public float EndScale = 1.05f;   // x icon size at the end of the cycle
    [Range(0, 1)] public float StartAlpha = 0.55f;
    [Range(0, 1)] public float EndAlpha = 0.05f;
    public bool ForceShow;           // testing: pulse even before 100%
    public static bool CheatForceShow; // RightShift+G (GameManager cheats): previews the 100% pulses only

    Image _image;

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.raycastTarget = false;
    }

    void Update()
    {
        var m = SaveGame.Members;
        bool conditionMet = Condition switch
        {
            // Rebirth button: the big pulse from the first credit until the first rebirth, whether or not the dialog was
            // opened. (FirstRebirthReadySeen was a smaller pulse after opening it - no longer used.)
            PulseCondition.FirstRebirthReady => m.TimesAscended_09_08_2025 == 0 && m.MonsterCredits_09_08_2025 >= 1,
            PulseCondition.FirstRebirthReadySeen => false,
            PulseCondition.TipsUnseen => !m.ViewedTipsDialog,
            // Expand-tiers button: from the first Wizard bought until the expanded tier view has been opened.
            PulseCondition.ExpandTiersUnseen => m.LevelWizard > 0 && !m.ViewedExpandedTiers,
            // Highlight behind the rebirth dialog's credit bonus line once it's low (same threshold as its orange text).
            PulseCondition.CreditBonusLow => Assets.Script.Upgrades.UpgradeProgression.CreditBonusDisplayPct() <= 100,
            PulseCondition.VictoryUnseen => m.Achieved.Contains(Achieved.Completion100) && !m.ViewedVictoryDialog,
            // The crown on the Victory button too: only until the victory dialog has been opened.
            _ => m.Achieved.Contains(Achieved.Completion100) && !m.ViewedVictoryDialog,
        };
        // The preview cheat only fakes 100%: rebirth/tips pulses keep following their own conditions.
        // Both 100% pulses still stop once the victory dialog has been opened.
        bool cheat = CheatForceShow && (Condition == PulseCondition.Completion100 || Condition == PulseCondition.VictoryUnseen)
            && !m.ViewedVictoryDialog && !ProgressPopupScript.OpenedDuringPreview;
        bool show = ForceShow || cheat || conditionMet;
        if (_image.enabled != show)
            _image.enabled = show;
        if (!show)
            return;

        float t = Mathf.Repeat(Time.unscaledTime, Period) / Period;
        float ease = 1f - (1f - t) * (1f - t); // fast shrink right after the pop, then settling
        transform.localScale = Vector3.one * Mathf.Lerp(StartScale, EndScale, ease);

        var c = _image.color;
        c.a = Mathf.Lerp(StartAlpha, EndAlpha, ease);
        _image.color = c;
    }
}
