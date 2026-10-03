using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Shows "+X% income!" just above the cursor when buying an X2 reaches the next X2 rank (every 5 X2s): fades in,
// drifts slowly upward, stays a while and fades out. Built entirely from code on the upgrade list's root canvas, so
// no scene wiring is needed. Animates on unscaled time.
public class X2RankUpEffect : MonoBehaviour
{
    const string GreenHex = "#8DBE4C";

    const float StartAboveCursor = 30f;
    const float FadeInTime = 0.3f;
    const float HoldTime = 2.0f;
    const float FadeOutTime = 0.8f;
    const float Life = FadeInTime + HoldTime + FadeOutTime;
    const float Rise = 25f;

    class Item
    {
        public TextMeshProUGUI Text;
        public Vector2 Start;
        public float Age;
    }

    static X2RankUpEffect _instance;
    readonly List<Item> _items = new();
    RectTransform _rect;
    TMP_FontAsset _font;

    public static void Spawn(Component anyUiElement, Vector2 screenPos, double pctPerRank, long rank)
    {
        if (_instance == null)
        {
            var canvas = anyUiElement.GetComponentInParent<Canvas>();
            if (canvas == null)
                return;

            var go = new GameObject("X2RankUpEffect", typeof(RectTransform));
            go.transform.SetParent(canvas.rootCanvas.transform, false);
            _instance = go.AddComponent<X2RankUpEffect>();
            _instance._rect = (RectTransform)go.transform;
            _instance._rect.anchorMin = Vector2.zero;
            _instance._rect.anchorMax = Vector2.one;
            _instance._rect.offsetMin = Vector2.zero;
            _instance._rect.offsetMax = Vector2.zero;
            var anyText = anyUiElement.GetComponentInChildren<TextMeshProUGUI>(true);
            _instance._font = anyText != null ? anyText.font : null;
        }

        _instance.DoSpawn(screenPos, pctPerRank, rank);
    }

    void DoSpawn(Vector2 screenPos, double pctPerRank, long rank)
    {
        transform.SetAsLastSibling();

        var canvas = GetComponentInParent<Canvas>().rootCanvas;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, screenPos, cam, out Vector2 local);

        var text = CreateText($"<color={GreenHex}>+{pctPerRank:0}% income!</color>\n<size=60%><color=#cccccc>X2 rank {rank}</color></size>", 20f);
        _items.Add(new Item { Text = text, Start = local + new Vector2(0, StartAboveCursor) });

        Update();
    }

    TextMeshProUGUI CreateText(string content, float fontSize)
    {
        var go = new GameObject("Fx", typeof(RectTransform));
        go.transform.SetParent(transform, false);
        var text = go.AddComponent<TextMeshProUGUI>();
        if (_font != null)
            text.font = _font;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        text.outlineWidth = 0.25f;
        text.outlineColor = new Color32(20, 20, 20, 255);
        text.rectTransform.sizeDelta = new Vector2(300, 60);
        return text;
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        for (int i = _items.Count - 1; i >= 0; --i)
        {
            var item = _items[i];
            item.Age += dt;
            if (item.Age >= Life)
            {
                Destroy(item.Text.gameObject);
                _items.RemoveAt(i);
                continue;
            }

            // Slow, even drift upward over the whole life; smooth fade in and out.
            float t = item.Age / Life;
            item.Text.rectTransform.anchoredPosition = item.Start + new Vector2(0, Rise * Mathf.SmoothStep(0f, 1f, t));

            float alpha = item.Age < FadeInTime
                ? item.Age / FadeInTime
                : item.Age > FadeInTime + HoldTime ? 1f - (item.Age - FadeInTime - HoldTime) / FadeOutTime : 1f;
            item.Text.alpha = Mathf.SmoothStep(0f, 1f, alpha);
        }
    }
}
