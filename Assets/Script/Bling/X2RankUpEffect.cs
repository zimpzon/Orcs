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
    const float Rise = 45f;
    const float PopTime = 0.35f;
    const float SwaySpeed = 4.5f;
    const float SwayAmount = 4f;
    const float TiltDegrees = 3f;
    const float EdgeMargin = 8f; // min distance from the screen edge (also covers the tilt and the pop overshoot)

    // Animation preset: the big rank-up / unlock text, or the small "+N" buy feedback.
    class Style
    {
        public float FontSize, StartAbove, FadeIn, Hold, FadeOut, Rise, Tilt;
        public bool Pop;
        public Color Color;
        public float Life => FadeIn + Hold + FadeOut;
    }

    static readonly Style Big = new()
    {
        FontSize = 16f, StartAbove = StartAboveCursor, FadeIn = FadeInTime, Hold = HoldTime, FadeOut = FadeOutTime,
        Rise = X2RankUpEffect.Rise, Tilt = TiltDegrees, Pop = true, Color = new Color(1.0f, 0.835f, 0.29f),
    };

    // Small, quick and discreet: "+10" when buying levels, so the selected buy amount is easy to see.
    static readonly Style Small = new()
    {
        FontSize = 11f, StartAbove = 18f, FadeIn = 0.1f, Hold = 0.35f, FadeOut = 0.35f,
        Rise = 22f, Tilt = 0f, Pop = false, Color = new Color(1f, 1f, 1f, 0.85f),
    };

    class Item
    {
        public TextMeshProUGUI Text;
        public Vector2 Start;
        public float Age;
        public Style Style;
    }

    static X2RankUpEffect _instance;
    readonly List<Item> _items = new();
    RectTransform _rect;
    TMP_FontAsset _font;
    Material _fontMaterial;

    public static void Spawn(Component anyUiElement, Vector2 screenPos, double pct, long rank)
        => SpawnText(anyUiElement, screenPos, $"+{pct:0}% income!\n<size=80%>X2 rank {rank}</size>");

    // Several ranks in one text (UpgradeManager coalesces everything after the first two ranks of a buy).
    public static void Spawn(Component anyUiElement, Vector2 screenPos, double pct, long firstRank, long lastRank)
    {
        if (firstRank == lastRank)
            Spawn(anyUiElement, screenPos, pct, firstRank);
        else
            SpawnText(anyUiElement, screenPos, $"+{pct:0}% income!\n<size=80%>X2 rank {firstRank}-{lastRank}</size>");
    }

    // Same floating text with any content (also used for "New skin unlocked!"). Big texts chime, small ones don't.
    // Big texts are queued, at least BigTextGap apart, so several unlocks at once show one after another.
    public static void SpawnText(Component anyUiElement, Vector2 screenPos, string content)
        => Spawn(anyUiElement, screenPos, content, Big);

    const float BigTextGap = 1f;
    readonly Queue<(Vector2 pos, string content)> _bigQueue = new();
    float _nextBigTime;

    void ShowQueuedBig()
    {
        if (_bigQueue.Count == 0 || Time.unscaledTime < _nextBigTime)
            return;

        var (pos, content) = _bigQueue.Dequeue();
        _nextBigTime = Time.unscaledTime + BigTextGap;
        AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.Chime, minInterval: 0.3f, fixedPitch: true);
        DoSpawn(pos, content, Big);
    }

    // Small, short "+N" at the cursor after a buy (UpgradeManager.BuyLevels, PercentageBonusScript).
    public static void SpawnSmall(Component anyUiElement, Vector2 screenPos, string content)
        => Spawn(anyUiElement, screenPos, content, Small);

    static void Spawn(Component anyUiElement, Vector2 screenPos, string content, Style style)
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
            // Above the dialogs (GlobalPopupManager draws them at sorting order 100), e.g. "+5 diamonds" on a refund in
            // the rebirth dialog. Only text: no raycaster, clicks go through.
            var overlay = go.AddComponent<Canvas>();
            overlay.overrideSorting = true;
            overlay.sortingOrder = 120;
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

        if (style == Big)
        {
            _instance._bigQueue.Enqueue((screenPos, content));
            _instance.ShowQueuedBig(); // right away if nothing was shown in the last second
            return;
        }

        _instance.DoSpawn(screenPos, content, style);
    }

    void DoSpawn(Vector2 screenPos, string content, Style style)
    {
        transform.SetAsLastSibling();

        var canvas = GetComponentInParent<Canvas>().rootCanvas;
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, screenPos, cam, out Vector2 local);

        // Yellow with drop shadow - green/orange would clash with the upgrade buttons it floats over.
        var text = CreateText(content, style.FontSize, style.Color);

        // Keep the whole text on screen for its entire animation: measure it, then push the start point inward from
        // the edges (room for the sway sideways and for the rise at the top).
        Vector2 size = text.GetPreferredValues(content);
        text.rectTransform.sizeDelta = size;
        Rect area = _rect.rect;
        Vector2 start = local + new Vector2(0, style.StartAbove);
        float halfW = size.x * 0.5f + EdgeMargin + SwayAmount;
        float halfH = size.y * 0.5f + EdgeMargin;
        start.x = area.width > 2 * halfW ? Mathf.Clamp(start.x, area.xMin + halfW, area.xMax - halfW) : area.center.x;
        start.y = area.height > 2 * halfH + style.Rise ? Mathf.Clamp(start.y, area.yMin + halfH, area.yMax - halfH - style.Rise) : area.center.y;
        _items.Add(new Item { Text = text, Start = start, Style = style });

        Update();
    }

    TextMeshProUGUI CreateText(string content, float fontSize, Color color)
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
        text.color = color;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        text.raycastTarget = false;
        text.rectTransform.sizeDelta = new Vector2(300, 60);
        return text;
    }

    void Update()
    {
        ShowQueuedBig();

        float dt = Time.unscaledDeltaTime;
        for (int i = _items.Count - 1; i >= 0; --i)
        {
            var item = _items[i];
            var s = item.Style;
            item.Age += dt;
            if (item.Age >= s.Life)
            {
                Destroy(item.Text.gameObject);
                _items.RemoveAt(i);
                continue;
            }

            // Soft pop in (slight overshoot), then rise with ease-out while swaying and tilting gently side to side.
            float t = item.Age / s.Life;
            var rt = item.Text.rectTransform;
            float rise = 1f - (1f - t) * (1f - t) * (1f - t);
            float sway = Mathf.Sin(item.Age * SwaySpeed) * SwayAmount;
            rt.anchoredPosition = item.Start + new Vector2(sway, s.Rise * rise);
            rt.localRotation = Quaternion.Euler(0, 0, -Mathf.Sin(item.Age * SwaySpeed + 0.6f) * s.Tilt);

            float scale = 1f;
            if (s.Pop)
            {
                float pop = Mathf.Clamp01(item.Age / PopTime);
                float c = 1.70158f;
                scale = 0.6f + 0.4f * (1f + (c + 1f) * Mathf.Pow(pop - 1f, 3) + c * Mathf.Pow(pop - 1f, 2));
            }
            rt.localScale = Vector3.one * scale;

            float alpha = item.Age < s.FadeIn
                ? item.Age / s.FadeIn
                : item.Age > s.FadeIn + s.Hold ? 1f - (item.Age - s.FadeIn - s.Hold) / s.FadeOut : 1f;
            item.Text.alpha = Mathf.SmoothStep(0f, 1f, alpha) * s.Color.a;
        }
    }
}
