using UnityEngine;
using UnityEngine.UI;

// Victory button crown that pulses once the game is 100% complete (Completion100 achievement): a larger,
// semi-transparent crown pops up over the icon, then shrinks toward the icon size while fading, and repeats.
// Never fully opaque and never size zero, so the real icon always shows and it stays a gentle hint.
// Also used as a button pulse: on the rebirth (diamond) button while the player has credits but never rebirthed
// (big until the dialog was opened, small after), and on the Victory button after 100% until the victory dialog has
// been opened once.
[RequireComponent(typeof(Image))]
public class CrownPulse : MonoBehaviour
{
    public enum PulseCondition { Completion100, FirstRebirthReady, VictoryUnseen, FirstRebirthReadySeen }
    public PulseCondition Condition = PulseCondition.Completion100;

    public float Period = 1.6f;      // seconds per pop + shrink/fade
    public float StartScale = 1.8f;  // x icon size, right after the pop
    public float EndScale = 1.05f;   // x icon size at the end of the cycle
    [Range(0, 1)] public float StartAlpha = 0.55f;
    [Range(0, 1)] public float EndAlpha = 0.05f;
    public bool ForceShow;           // testing: pulse even before 100%
    public static bool CheatForceShow; // toggled with RightShift+G (GameManager cheats)

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
            // Big pulse until the rebirth dialog has been opened with credits, then a small one until the first rebirth:
            // the player may keep collecting credits, so the big one would get annoying.
            PulseCondition.FirstRebirthReady => m.TimesAscended_09_08_2025 == 0 && m.MonsterCredits_09_08_2025 > 0 && !m.ViewedRebirthDialog,
            PulseCondition.FirstRebirthReadySeen => m.TimesAscended_09_08_2025 == 0 && m.MonsterCredits_09_08_2025 > 0 && m.ViewedRebirthDialog,
            PulseCondition.VictoryUnseen => m.Achieved.Contains(Achieved.Completion100) && !m.ViewedVictoryDialog,
            _ => m.Achieved.Contains(Achieved.Completion100),
        };
        bool show = ForceShow || CheatForceShow || conditionMet;
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
