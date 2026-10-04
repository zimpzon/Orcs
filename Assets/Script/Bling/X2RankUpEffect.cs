using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Shows "+X% income!" just above the cursor when buying an X2 reaches the next X2 rank (every 5 X2s): fades in,
// pops in, floats upward with a gentle sway, stays a while and fades out. Built entirely from code on the upgrade list's root canvas, so
// no scene wiring is needed. Animates on unscaled time.
public class X2RankUpEffect : MonoBehaviour
{
    const float StartAboveCursor = 30f;
    const float FadeInTime = 0.3f;
    const float HoldTime = 2.0f;
    const float FadeOutTime = 0.8f;
    const float Life = FadeInTime + HoldTime + FadeOutTime;
    const float Rise = 45f;
    const float PopTime = 0.35f;
    const float SwaySpeed = 4.5f;
    const float SwayAmount = 4f;
    const float TiltDegrees = 3f;

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
    Material _fontMaterial;

    public static void Spawn(Component anyUiElement, Vector2 screenPos, double pctPerRank, long rank)
        => SpawnText(anyUiElement, screenPos, $"+{pctPerRank:0}% income!\n<size=80%>X2 rank {rank}</size>");

    // Same floating text with any content (also used for "New skin unlocked!").
    public static void SpawnText(Component anyUiElement, Vector2 screenPos, string content)
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
            // Use the game's Lato Black text style (its material has the drop shadow most UI text uses).
            TextMeshProUGUI source = null;
            foreach (var t in anyUiElement.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                source ??= t;
                if (t.font != null && t.font.name.Contains("Lato") && t.fontSharedMaterial == t.font.material)
                {
                    source = t;
                    break;
                }
            }
            _instance._font = source != null ? source.font : null;
            _instance._fontMaterial = source != null ? source.fontSharedMaterial : null;
        }

        _instance.DoSpawn(screenPos, content);
    }

    void DoSpawn(Vector2 screenPos, string content)
    {
        transform.SetAsLastSibling();

        var canvas = GetComponentInParent<Canvas>().rootCanvas;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, screenPos, cam, out Vector2 local);

        // Yellow with drop shadow - green/orange would clash with the upgrade buttons it floats over.
        var text = CreateText(content, 16f);
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
        if (_fontMaterial != null)
            text.fontSharedMaterial = _fontMaterial;
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.color = new Color(1.0f, 0.835f, 0.29f);
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
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

            // Soft pop in (slight overshoot), then rise with ease-out while swaying and tilting gently side to side.
            float t = item.Age / Life;
            var rt = item.Text.rectTransform;
            float rise = 1f - (1f - t) * (1f - t) * (1f - t);
            float sway = Mathf.Sin(item.Age * SwaySpeed) * SwayAmount;
            rt.anchoredPosition = item.Start + new Vector2(sway, Rise * rise);
            rt.localRotation = Quaternion.Euler(0, 0, -Mathf.Sin(item.Age * SwaySpeed + 0.6f) * TiltDegrees);

            float pop = Mathf.Clamp01(item.Age / PopTime);
            float c = 1.70158f;
            float scale = 0.6f + 0.4f * (1f + (c + 1f) * Mathf.Pow(pop - 1f, 3) + c * Mathf.Pow(pop - 1f, 2));
            rt.localScale = Vector3.one * scale;

            float alpha = item.Age < FadeInTime
                ? item.Age / FadeInTime
                : item.Age > FadeInTime + HoldTime ? 1f - (item.Age - FadeInTime - HoldTime) / FadeOutTime : 1f;
            item.Text.alpha = Mathf.SmoothStep(0f, 1f, alpha);
        }
    }
}
