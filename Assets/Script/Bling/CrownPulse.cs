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
    public enum PulseCondition { Completion100, FirstRebirthReady, VictoryUnseen, FirstRebirthReadySeen, TipsUnseen, ExpandTiersUnseen, CreditBonusLow, FogUnseen }
    public PulseCondition Condition = PulseCondition.Completion100;

    public float Period = 1.6f;      // seconds per pop + shrink/fade
    public float StartScale = 1.8f;  // x icon size, right after the pop
    public float EndScale = 1.05f;   // x icon size at the end of the cycle
    [Range(0, 1)] public float StartAlpha = 0.55f;
    [Range(0, 1)] public float EndAlpha = 0.05f;
    public bool ForceShow;           // testing: pulse even before 100%
    public static bool CheatForceShow; // RightShift+G (GameManager cheats): previews the 100% pulses only

    Image _image;

    // Progress reminder on the crown (VictoryUnseen): players forget to check their completion late in the game. From
    // ReminderFromPct on, every ReminderStepPct reached since the crown dialog was last opened makes it pulse, and so
    // does a big moment (final enemy defeated, SaveGame.ProgressNewsUnseen). Opening the dialog stops it (ProgressSeen).
    const double ReminderFromPct = 101; // 101 = disabled
    const double ReminderStepPct = 5;
    const float ReminderCheckInterval = 2f; // GetProgress scans the skins: not every frame
    static float _nextReminderCheck;
    static int _currentStep = -1;

    static int CompletionStep(double pct) => pct >= ReminderFromPct ? (int)System.Math.Floor(pct / ReminderStepPct) : -1;

    static bool ProgressReminderDue()
    {
        var m = SaveGame.Members;
        if (m.ProgressNewsUnseen)
            return true;
        if (m.Achieved.Contains(Achieved.Completion100))
            return false; // 100%: the victory pulse takes over
        if (Time.unscaledTime >= _nextReminderCheck)
        {
            _nextReminderCheck = Time.unscaledTime + ReminderCheckInterval;
            _currentStep = CompletionStep(GameCompletion.GetProgress().TotalPct);
        }
        return _currentStep >= 0 && _currentStep > m.ProgressSeenStep;
    }

    public static void ProgressSeen()
    {
        var m = SaveGame.Members;
        m.ProgressNewsUnseen = false;
        _currentStep = CompletionStep(GameCompletion.GetProgress().TotalPct);
        m.ProgressSeenStep = System.Math.Max(m.ProgressSeenStep, _currentStep);
    }

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
            // Bestiary button: the secret tile's fog is there (after 100%) and "Touch fog" hasn't been clicked yet - but
            // only once the victory dialog has been opened (at 100% the crown pulses first, one thing at a time). And
            // again once Scary Earl is beaten, until the bestiary has been opened (the decimation goals are in there).
            PulseCondition.FogUnseen => SecretBeastTile.Unlocked && !m.FogTouched
                                            && (m.ViewedVictoryDialog || ProgressPopupScript.OpenedDuringPreview)
                                        || m.ScaryEarlDefeated && !m.ViewedBestiaryAfterScaryEarl,
            // Crown (progress / victory) button: once the tips have been seen (their pulse stopped) until it's been
            // opened once; and after 100% until the victory dialog has been opened.
            PulseCondition.VictoryUnseen => m.ViewedTipsDialog && !m.ViewedProgressDialog
                                            || m.Achieved.Contains(Achieved.Completion100) && !m.ViewedVictoryDialog
                                            || ProgressReminderDue(),
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
