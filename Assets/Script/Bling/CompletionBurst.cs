using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 100% completion celebration at the cursor (AchievementChecks.AnnounceCompletion): confetti fountains in the victory
// dialog's style (gold / green / white flecks tumbling as they fall, VictoryCelebration) plus gold spark rings. A big
// burst and two smaller follow-ups. Built from code on the root canvas, ignores clicks, unscaled time, destroys itself.
public class CompletionBurst : MonoBehaviour
{
    const float Gravity = -420f;           // canvas units / s^2
    const float Drag = 0.9f;               // velocity kept per second (air resistance, so it floats down)
    const float ConfettiLife = 2.6f;
    const float SparkLife = 0.8f;
    static readonly Color Gold = Hex(TextColors.Gold);
    static readonly Color[] ConfettiColors = { Hex(TextColors.Gold), Hex(TextColors.Green), Color.white };

    // Bursts: delay, offset from the cursor, confetti count, launch speed, spark count.
    static readonly (float delay, Vector2 offset, int confetti, float speed, int sparks)[] Bursts =
    {
        (0.00f, Vector2.zero, 70, 330f, 22),
        (0.40f, new Vector2(-45f, 15f), 30, 250f, 14),
        (0.80f, new Vector2(50f, 25f), 30, 250f, 14),
    };

    class Piece
    {
        public RectTransform Rt;
        public Image Img;
        public Vector2 Pos, Vel;
        public float Age, Life, Spin, Phase;
        public bool IsSpark;
        public Color Color;
    }

    readonly List<Piece> _pieces = new();
    RectTransform _rect;
    Vector2 _origin;
    float _age;
    int _nextBurst;

    static Color Hex(string html)
    {
        ColorUtility.TryParseHtmlString(html, out var c);
        return c;
    }

    // anyUiElement: anything on the main canvas (to find it). screenPos: where to burst from.
    public static void Play(Component anyUiElement, Vector2 screenPos)
    {
        var canvas = anyUiElement != null ? anyUiElement.GetComponentInParent<Canvas>() : null;
        if (canvas == null)
            return;
        var root = canvas.rootCanvas;

        var go = new GameObject("CompletionBurst", typeof(RectTransform));
        go.transform.SetParent(root.transform, false);
        go.transform.SetAsLastSibling();
        var burst = go.AddComponent<CompletionBurst>();
        var rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        burst._rect = rect;

        Camera cam = root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPos, cam, out burst._origin);
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;
        _age += dt;

        while (_nextBurst < Bursts.Length && _age >= Bursts[_nextBurst].delay)
            Spawn(Bursts[_nextBurst++]);

        float drag = Mathf.Pow(Drag, dt);
        for (int i = _pieces.Count - 1; i >= 0; --i)
        {
            var p = _pieces[i];
            p.Age += dt;
            if (p.Age >= p.Life)
            {
                Destroy(p.Rt.gameObject);
                _pieces.RemoveAt(i);
                continue;
            }

            float t = p.Age / p.Life;
            if (p.IsSpark)
            {
                p.Vel *= Mathf.Pow(0.2f, dt); // sparks slow down fast, no gravity
                p.Pos += p.Vel * dt;
                p.Rt.anchoredPosition = p.Pos;
                var c = p.Color;
                c.a = 1f - t;
                p.Img.color = c;
                continue;
            }

            p.Vel.y += Gravity * dt;
            p.Vel *= drag;
            // Falling confetti sways and caps its fall speed, like paper.
            if (p.Vel.y < -140f)
                p.Vel.y = -140f;
            p.Pos += p.Vel * dt + new Vector2(Mathf.Sin(p.Age * 4f + p.Phase) * 25f * dt, 0f);
            p.Rt.anchoredPosition = p.Pos;
            p.Rt.localRotation *= Quaternion.Euler(0, 0, p.Spin * dt);
            // Flip-like tumble, as in the victory dialog: squash the width with the spin.
            float flip = Mathf.Abs(Mathf.Cos(p.Age * 6f + p.Phase));
            p.Rt.localScale = new Vector3(0.35f + 0.65f * flip, 1f, 1f);
            var col = p.Color;
            col.a = t < 0.75f ? 1f : 1f - (t - 0.75f) / 0.25f;
            p.Img.color = col;
        }

        if (_nextBurst >= Bursts.Length && _pieces.Count == 0)
            Destroy(gameObject);
    }

    void Spawn((float delay, Vector2 offset, int confetti, float speed, int sparks) b)
    {
        Vector2 at = _origin + b.offset;

        for (int i = 0; i < b.confetti; ++i)
        {
            // A fountain: mostly upward, spread +-55 degrees.
            float angle = (90f + Random.Range(-55f, 55f)) * Mathf.Deg2Rad;
            float speed = b.speed * Random.Range(0.55f, 1.1f);
            var color = ConfettiColors[Random.Range(0, ConfettiColors.Length)];
            Add(at, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed, new Vector2(4f, 7f), color,
                ConfettiLife * Random.Range(0.8f, 1.1f), spark: false);
        }

        for (int i = 0; i < b.sparks; ++i)
        {
            float angle = i / (float)b.sparks * Mathf.PI * 2f + Random.Range(-0.1f, 0.1f);
            Add(at, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(160f, 230f), new Vector2(3f, 3f), Gold,
                SparkLife * Random.Range(0.8f, 1.1f), spark: true);
        }
    }

    void Add(Vector2 pos, Vector2 vel, Vector2 size, Color color, float life, bool spark)
    {
        var go = new GameObject(spark ? "Spark" : "Confetto", typeof(RectTransform));
        go.transform.SetParent(transform, false);
        var img = go.AddComponent<Image>();
        img.raycastTarget = false;
        img.color = color;
        var rt = img.rectTransform;
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        rt.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        _pieces.Add(new Piece
        {
            Rt = rt, Img = img, Pos = pos, Vel = vel, Life = life, IsSpark = spark, Color = color,
            Spin = Random.Range(-360f, 360f), Phase = Random.value * Mathf.PI * 2f,
        });
    }
}
