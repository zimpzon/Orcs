using UnityEngine;

// Lightning for zaps. Each zap gets its own pooled bolt (so every jump of a chain is visible at once, instead of one
// shared line being overwritten), drawn as a bright core over a wide soft glow. The shape is a midpoint-displacement
// bolt tapered at both ends, it re-crackles a few times while it fades, and sometimes throws a short fork.
// Line renderers are cloned from ArenaBoundsScript's LineRenderer, so material and sorting match the old look.
public class LightningBolts : MonoBehaviour
{
    const int PoolSize = 12;
    const int MaxForks = 1;
    const float Life = 0.45f;
    const float CrackleInterval = 0.8f;   // re-shape the bolt this often while it lives
    const float Displacement = 0.25f;      // initial sideways displacement, relative to bolt length
    const float GlowWidthMul = 4.5f;
    const float GlowAlpha = 0.4f;
    const float ForkChance = 0.1f;
    const int ImpactParticles = 10;

    class Bolt
    {
        public LineRenderer Core, Glow;
        public LineRenderer[] Forks;
        public Vector2 From, To;
        public float Age = Life;
        public float NextCrackle;
        public int ForkCount;
    }

    static LightningBolts _instance;
    readonly Bolt[] _bolts = new Bolt[PoolSize];
    int _next;
    float _baseWidth;
    Color _coreStart, _coreEnd;

    // Reused buffers (no per-zap allocations).
    static readonly Vector3[] _points = new Vector3[65];
    static readonly Vector3[] _forkPoints = new Vector3[17];

    public static void Zap(Vector2 from, Vector2 to)
    {
        if (_instance == null)
        {
            var template = ArenaBoundsScript.Instance.LineRenderer;
            var go = new GameObject("LightningBolts");
            _instance = go.AddComponent<LightningBolts>();
            _instance.Init(template, ArenaBoundsScript.Instance.LineRendererBaseWidth);
        }

        _instance.Spawn(from, to);
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

    void Spawn(Vector2 from, Vector2 to)
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
        bolt.ForkCount = Random.value < ForkChance ? Random.Range(1, MaxForks + 1) : 0;
        Shape(bolt, emitParticles: true);

        // Impact sparks at the target.
        var trail = Particles.I.ClickTrail;
        trail.transform.position = to;
        trail.Emit(ImpactParticles);

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
            int count = BuildBolt(bolt.From, bolt.To, _points, 6, Displacement);
            bolt.Core.positionCount = count;
            bolt.Core.SetPositions(_points);
            bolt.Glow.positionCount = count;
            bolt.Glow.SetPositions(_points);

            if (emitParticles)
            {
                var trail = Particles.I.ClickTrail;
                for (int i = 0; i < count; ++i)
                {
                    if (Random.value < 0.6f)
                    {
                        trail.transform.position = _points[i];
                        trail.Emit(1);
                    }
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
        float t = Mathf.Clamp01(bolt.Age / Life);
        // Bright and full-width at first, quickly thinning (ease-out), plus a little flicker.
        float k = (1f - t) * (1f - t);
        float flicker = 0.75f + 0.25f * Random.value;
        float width = _baseWidth * k * flicker;

        bolt.Core.startWidth = bolt.Core.endWidth = width;
        bolt.Core.startColor = _coreStart;
        bolt.Core.endColor = _coreEnd;

        bolt.Glow.startWidth = bolt.Glow.endWidth = width * GlowWidthMul;
        bolt.Glow.startColor = WithAlpha(_coreStart, _coreStart.a * GlowAlpha * k);
        bolt.Glow.endColor = WithAlpha(_coreEnd, _coreEnd.a * GlowAlpha * k);

        for (int f = 0; f < bolt.ForkCount && f < MaxForks; ++f)
        {
            var fork = bolt.Forks[f];
            fork.startWidth = width * 0.6f;
            fork.endWidth = 0f;
        }
    }

    static Color WithAlpha(Color c, float a) => new Color(c.r, c.g, c.b, a);

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
            if (bolt.Age >= Life)
                continue;

            bolt.Age += dt;
            if (bolt.Age >= Life)
            {
                Hide(bolt);
                continue;
            }

            if (bolt.Age >= bolt.NextCrackle)
            {
                bolt.NextCrackle = bolt.Age + CrackleInterval;
                Shape(bolt, emitParticles: false);
            }

            Apply(bolt);
        }
    }
}
