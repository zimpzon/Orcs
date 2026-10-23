using System;
using TMPro;
using UnityEngine;

// Counts a money text up from 0 to its amount (fast at first, easing in), then gives it a small pop. Unscaled time.
// The text itself comes from a formatter, so any markup around the number stays. (WelcomeBackPopup)
public class CountUpMoney : MonoBehaviour
{
    const float Delay = 0.3f;      // let the popup spring in first
    const float Duration = 1.1f;
    const float PopScale = 1.18f;
    const float PopDuration = 0.25f;

    TextMeshProUGUI _text;
    Decimal512 _amount;
    Func<Decimal512, string> _format;
    float _start;
    bool _done;

    public void Init(Decimal512 amount, Func<Decimal512, string> format)
    {
        _text = GetComponent<TextMeshProUGUI>();
        _amount = amount;
        _format = format;
        _start = Time.unscaledTime + Delay;
        _text.text = format(0);
    }

    void Update()
    {
        if (_text == null || _format == null)
            return;

        float t = (Time.unscaledTime - _start) / Duration;
        if (t < 0f)
            return;

        if (!_done)
        {
            if (t >= 1f)
            {
                _done = true;
                _text.text = _format(_amount);
                _start = Time.unscaledTime; // reuse as the pop start
                return;
            }
            float ease = 1f - Mathf.Pow(1f - t, 3f);
            _text.text = _format(_amount * (double)ease);
            return;
        }

        // Pop: quick swell and settle.
        float p = (Time.unscaledTime - _start) / PopDuration;
        if (p >= 1f)
        {
            transform.localScale = Vector3.one;
            enabled = false;
            return;
        }
        transform.localScale = Vector3.one * Mathf.Lerp(PopScale, 1f, p * p);
    }
}
