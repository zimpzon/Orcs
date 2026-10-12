using UnityEngine;

// Lightning for zaps. Each zap gets its own pooled bolt (so every jump of a chain is visible at once, instead of one
// shared line being overwritten), drawn as a bright core over a wide soft glow. The shape is a midpoint-displacement
// bolt tapered at both ends, it re-crackles a few times while it fades, and sometimes throws a short fork.
// Line renderers are cloned from ArenaBoundsScript's LineRenderer, so material and sorting match the old look.
public class LightningBolts : MonoBehaviour
{
    const int PoolSize = 12;
    const int MaxForks = 2;
    const float Life = 0.45f;
    const float StormLife = 0.7f;
    const float StormCrackleInterval = 0.12f;
    const float StormWidthMul = 1.4f;
    const float StormDetourMin = 0.18f;   // sideways push of each zig-zag waypoint, relative to bolt length
    const float StormDetourMax = 0.1f;
    const float StormScreenMargin = 0.4f; // world units kept free inside the screen edge for the zig-zag's jaggies
    const float CrackleInterval = 0.8f;   // re-shape the bolt this often while it lives
    const float Displacement = 0.15f;      // initial sideways displacement, relative to bolt length
    const float GlowWidthMul = 4.5f;
    const float GlowAlpha = 0.4f;
    const float ForkChance = 0.1f;
    const int ImpactParticles = 10;

    class Bolt
    {
        public LineRenderer Core, Glow;
        public LineRenderer[] Forks;
        public Vector2 From, To;
        public float Age = LightningBolts.Life;
        public float NextCrackle;
        public int ForkCount;
        public Color Start, End;
        public bool Storm;
        public float Life = LightningBolts.Life;
    }

    static LightningBolts _instance;
    readonly Bolt[] _bolts = new Bolt[PoolSize];
    int _next;
    float _baseWidth;
    Color _coreStart, _coreEnd;

    // Storm Lord's bolts are yellow so they read apart from the regular zaps.
    static readonly Color StormStart = new Color(1f, 0.92f, 0.45f, 1f);
    static readonly Color StormEnd = new Color(1f, 0.8f, 0.25f, 1f);

    // Reused buffers (no per-zap allocations).
    static readonly Vector3[] _points = new Vector3[65];
    static readonly Vector3[] _forkPoints = new Vector3[17];
    static readonly Vector3[] _segPoints = new Vector3[17];

    public static void Zap(Vector2 from, Vector2 to, bool storm = false)
    {
        if (_instance == null)
        {
            var template = ArenaBoundsScript.Instance.LineRenderer;
            var go = new GameObject("LightningBolts");
            _instance = go.AddComponent<LightningBolts>();
            _instance.Init(template, ArenaBoundsScript.Instance.LineRendererBaseWidth);
        }

        _instance.Spawn(from, to, storm);
    }

    void Init(LineRenderer template, float baseWidth)
    {
        _baseWidth = baseWidth;
        _coreStart = template.startColor;
        _coreEnd = template.endColor;

        for (int i = 0; i < PoolSize; ++i)
        {
            var bolt = new Bolt
            {
                Glow = CloneLine(template, "Glow", -1),
                Core = CloneLine(template, "Core", 0),
                Forks = new LineRenderer[MaxForks],
            };
            for (int f = 0; f < MaxForks; ++f)
                bolt.Forks[f] = CloneLine(template, "Fork", 0);
            _bolts[i] = bolt;
        }
    }

    LineRenderer CloneLine(LineRenderer template, string name, int sortingOffset)
    {
        var line = Instantiate(template, transform);
        line.name = name;
        line.positionCount = 0;
        line.startWidth = line.endWidth = 0;
        line.sortingOrder = template.sortingOrder + sortingOffset;
        line.useWorldSpace = true;
        return line;
    }

    void Spawn(Vector2 from, Vector2 to, bool storm)
    {
        // Same safety limit as the old jagged trail.
        if ((to - from).sqrMagnitude > 100f)
            return;

        var bolt = _bolts[_next];
        _next = (_next + 1) % PoolSize;

        bolt.From = from;
        bolt.To = to;
        bolt.Age = 0f;
        bolt.NextCrackle = 0f;
        bolt.Storm = storm;
        if (storm)
            bolt.From = ClampToScreen(from);
        bolt.Life = storm ? StormLife : Life;
        bolt.Start = storm ? WithAlpha(StormStart, _coreStart.a) : _coreStart;
        bolt.End = storm ? WithAlpha(StormEnd, _coreEnd.a) : _coreEnd;
        bolt.ForkCount = storm ? MaxForks : (Random.value < ForkChance ? 1 : 0);
        Shape(bolt, emitParticles: true);

        // Impact sparks at the target.
        EmitSparks(to, ImpactParticles, storm);

        Apply(bolt);
    }

    // Midpoint displacement between from and to, tapered so both ends stay pinned. Writes count points into buffer.
    static int BuildBolt(Vector2 from, Vector2 to, Vector3[] buffer, int maxLevels, float displacement)
    {
        float length = Vector2.Distance(from, to);
        int levels = Mathf.Clamp(Mathf.CeilToInt(Mathf.Log(Mathf.Max(length, 0.01f) / 0.12f, 2f)), 2, maxLevels);
        int count = (1 << levels) + 1;

        buffer[0] = from;
        buffer[count - 1] = to;
        Vector2 dir = length > 0.0001f ? (to - from) / length : Vector2.right;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        float amp = length * displacement;
        for (int step = count - 1; step > 1; step /= 2)
        {
            int half = step / 2;
            for (int i = half; i < count - 1; i += step)
            {
                Vector2 mid = (buffer[i - half] + buffer[i + half]) * 0.5f;
                float t = (float)i / (count - 1);
                float taper = Mathf.Sin(t * Mathf.PI);
                buffer[i] = mid + perp * (Random.Range(-1f, 1f) * amp * (0.35f + 0.65f * taper));
            }
            amp *= 0.55f;
        }

        return count;
    }

    void Shape(Bolt bolt, bool emitParticles)
    {
        try
        {
            int count = bolt.Storm
                ? BuildStormBolt(bolt.From, bolt.To)
                : BuildBolt(bolt.From, bolt.To, _points, 6, Displacement);
            bolt.Core.positionCount = count;
            bolt.Core.SetPositions(_points);
            bolt.Glow.positionCount = count;
            bolt.Glow.SetPositions(_points);

            if (emitParticles)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (Random.value < 0.6f)
                        EmitSparks(_points[i], 1, bolt.Storm);
                }
            }

            Vector2 dir = (bolt.To - bolt.From).normalized;
            float length = Vector2.Distance(bolt.From, bolt.To);
            for (int f = 0; f < MaxForks; ++f)
            {
                var fork = bolt.Forks[f];
                if (f >= bolt.ForkCount)
                {
                    fork.positionCount = 0;
                    continue;
                }

                // Branch off somewhere in the middle part of the bolt, angled away from the main direction.
                Vector2 start = _points[Random.Range(count / 4, count * 3 / 4)];
                float angle = Random.Range(25f, 50f) * (Random.value < 0.5f ? -1f : 1f);
                Vector2 forkDir = Quaternion.Euler(0, 0, angle) * dir;
                Vector2 end = start + forkDir * length * Random.Range(0.2f, 0.35f);
                int forkCount = BuildBolt(start, end, _forkPoints, 4, Displacement * 1.2f);
                fork.positionCount = forkCount;
                fork.SetPositions(_forkPoints);
            }
        }
        catch (System.Exception)
        {
            // WebGL has thrown 'memory access out of bounds' from SetPositions before (see Trails.DrawJaggedTrail).
        }
    }

    void Apply(Bolt bolt)
    {
        float t = Mathf.Clamp01(bolt.Age / bolt.Life);
        // Bright and full-width at first, quickly thinning (ease-out), plus a little flicker.
        float k = (1f - t) * (1f - t);
        float flicker = 0.75f + 0.25f * Random.value;
        float width = _baseWidth * k * flicker * (bolt.Storm ? StormWidthMul : 1f);

        bolt.Core.startWidth = bolt.Core.endWidth = width;
        bolt.Core.startColor = bolt.Start;
        bolt.Core.endColor = bolt.End;

        bolt.Glow.startWidth = bolt.Glow.endWidth = width * GlowWidthMul;
        bolt.Glow.startColor = WithAlpha(bolt.Start, bolt.Start.a * GlowAlpha * k);
        bolt.Glow.endColor = WithAlpha(bolt.End, bolt.End.a * GlowAlpha * k);

        for (int f = 0; f < bolt.ForkCount && f < MaxForks; ++f)
        {
            var fork = bolt.Forks[f];
            fork.startWidth = width * 0.6f;
            fork.endWidth = 0f;
            fork.startColor = bolt.Start;
            fork.endColor = bolt.End;
        }
    }

    static Color WithAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);

    // ClickTrail's own start color is blue, so storm sparks override it (Color over Lifetime is off, so it sticks).
    static void EmitSparks(Vector2 pos, int count, bool storm)
    {
        var trail = Particles.I.ClickTrail;
        trail.transform.position = pos;
        if (!storm)
        {
            trail.Emit(count);
            return;
        }

        var emit = new ParticleSystem.EmitParams { startColor = Color.Lerp(StormStart, StormEnd, Random.value) };
        trail.Emit(emit, count);
    }

    static Vector2 ClampToScreen(Vector2 p)
    {
        var cam = Camera.main;
        if (cam == null)
            return p;

        Vector2 min = cam.ViewportToWorldPoint(Vector3.zero);
        Vector2 max = cam.ViewportToWorldPoint(Vector3.one);
        return new Vector2(
            Mathf.Clamp(p.x, min.x + StormScreenMargin, max.x - StormScreenMargin),
            Mathf.Clamp(p.y, min.y + StormScreenMargin, max.y - StormScreenMargin));
    }

    // Storm bolts zig-zag: 3 waypoints pushed sideways on alternating sides, each of the 4 legs its own small bolt.
    static int BuildStormBolt(Vector2 from, Vector2 to)
    {
        const int Legs = 4;
        float length = Vector2.Distance(from, to);
        Vector2 dir = length > 0.0001f ? (to - from) / length : Vector2.right;
        Vector2 perp = new Vector2(-dir.y, dir.x);
        float side = Random.value < 0.5f ? -1f : 1f;

        int count = 0;
        Vector2 legStart = from;
        for (int leg = 0; leg < Legs; ++leg)
        {
            Vector2 legEnd = to;
            if (leg < Legs - 1)
            {
                float along = (leg + 1f) / Legs + Random.Range(-0.05f, 0.05f);
                legEnd = ClampToScreen(from + dir * (length * along) + perp * (side * length * Random.Range(StormDetourMin, StormDetourMax)));
                side = -side;
            }

            int legCount = BuildBolt(legStart, legEnd, _segPoints, 4, Displacement);
            // Skip each leg's first point after the first leg, it's the previous leg's last point.
            for (int i = leg == 0 ? 0 : 1; i < legCount; ++i)
                _points[count++] = _segPoints[i];
            legStart = legEnd;
        }

        return count;
    }

    void Hide(Bolt bolt)
    {
        bolt.Core.positionCount = 0;
        bolt.Glow.positionCount = 0;
        foreach (var fork in bolt.Forks)
            fork.positionCount = 0;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        foreach (var bolt in _bolts)
        {
            if (bolt.Age >= bolt.Life)
                continue;

            bolt.Age += dt;
            if (bolt.Age >= bolt.Life)
            {
                Hide(bolt);
                continue;
            }

            if (bolt.Age >= bolt.NextCrackle)
            {
                bolt.NextCrackle = bolt.Age + (bolt.Storm ? StormCrackleInterval : CrackleInterval);
                Shape(bolt, emitParticles: false);
            }

            Apply(bolt);
        }
    }
}
