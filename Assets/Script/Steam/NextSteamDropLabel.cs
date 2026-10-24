using TMPro;
using UnityEngine;

// TextNextSteamDrop (bottom stats): countdown to the next Steam item drop (SteamItemDrops.SecondsToNextDrop).
// Empty without Steam.
[RequireComponent(typeof(TextMeshProUGUI))]
public class NextSteamDropLabel : MonoBehaviour
{
    TextMeshProUGUI _text;
    int _shownSeconds = -2;

    void Awake() => _text = GetComponent<TextMeshProUGUI>();

    void Update()
    {
        float? left = SteamItemDrops.SecondsToNextDrop;
        int seconds = left.HasValue ? Mathf.CeilToInt(left.Value) : -1;
        if (seconds == _shownSeconds)
            return;
        _shownSeconds = seconds;

        if (seconds < 0)
            _text.text = "";
        else if (seconds == 0)
            _text.text = "Next Steam drop: <color={TextColors.GreenSmallest}>any moment</color>";
        else
            _text.text = $"Next Steam drop: <color={TextColors.GreenSmallest}>{seconds / 3600:00}</color>:<color={TextColors.GreenSmallest}>{seconds / 60 % 60:00}</color>:<color={TextColors.GreenSmallest}>{seconds % 60:00}</color>";
    }
}
