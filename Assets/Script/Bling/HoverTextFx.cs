using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Hover description texts (bestiary, skins, rebirth cards): a new item hovered (or tapped) pops the text in (small
// scale + fade) with a soft tick. Added on demand (Of), so the scene needs no wiring. Unscaled time.
// Touchscreens: a tap is enter + exit right away, so callers ignore touch exits (IsTouch) - the tapped item's text
// stays until another item is tapped.
public class HoverTextFx : MonoBehaviour
{
    const float PopTime = 0.15f;
    const float PopScale = 1.08f;
    const float PopStartAlpha = 0.4f;
    const float TickVolume = 0.15f;
    const float TickMinInterval = 0.08f;

    TextMeshProUGUI _text;
    Color _baseColor;
    Vector3 _baseScale;
    Object _owner;
    float _popAge = PopTime;

    public static HoverTextFx Of(TextMeshProUGUI text)
    {
        if (text == null)
            return null;
        if (!text.TryGetComponent<HoverTextFx>(out var fx))
            fx = text.gameObject.AddComponent<HoverTextFx>();
        return fx;
    }

    // A touch pointer (mouse ids are negative). Its exit comes as soon as the finger lifts, so it shouldn't clear the text.
    public static bool IsTouch(PointerEventData eventData) => eventData != null && eventData.pointerId >= 0;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _baseColor = _text.color;
        _baseScale = transform.localScale;
    }

    // Shows text for this owner (a beast, skin or card). A new owner pops and ticks; the same owner just updates the
    // text (the skins popup refreshes it while hovered, and a click on the hovered item shows it again).
    public void Hover(Object owner, string text)
    {
        _text.text = text;
        if (owner == _owner)
            return;

        _owner = owner;
        _popAge = 0f;
        var audio = AudioManager.Instance;
        if (audio != null && audio.AudioData != null)
            audio.PlayClipForReal(audio.AudioData.Menu, volumeScale: TickVolume, minInterval: TickMinInterval, fixedPitch: true);
        Apply();
    }

    // Back to the default text, only if this owner is still the hovered one (enter on the next item can come first).
    public void Unhover(Object owner, string defaultText)
    {
        if (owner != _owner)
            return;
        _owner = null;
        _text.text = defaultText;
    }

    void OnDisable()
    {
        _owner = null;
        _popAge = PopTime;
        if (_text != null)
            Apply();
    }

    void Update()
    {
        if (_popAge >= PopTime)
            return;
        _popAge = Mathf.Min(PopTime, _popAge + Time.unscaledDeltaTime);
        Apply();
    }

    void Apply()
    {
        float p = _popAge / PopTime;
        float ease = 1f - (1f - p) * (1f - p);
        transform.localScale = _baseScale * Mathf.Lerp(PopScale, 1f, ease);
        var c = _baseColor;
        c.a = _baseColor.a * Mathf.Lerp(PopStartAlpha, 1f, ease);
        _text.color = c;
    }
}
