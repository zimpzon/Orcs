using UnityEngine;
using UnityEngine.UI;

// Very discrete violet sparks drifting up from a UI icon: the bestiary's final enemy (The Night Creeper), same colors as
// his hit sparks in the arena (BossLook). Small square "pixels" built from code, a few alive at a time, unscaled time.
// Only while Active (BeastiaryPopupScript turns it on for good, locked silhouette included).
public class UiVoidSparks : MonoBehaviour
{
    const int MaxSparks = 6;
    const float SpawnInterval = 0.35f;
    const float Life = 1.4f;
    const float SparkSize = 2f;
    const float Rise = 12f;
    const float Drift = 3f;
    const float MaxAlpha = 0.7f;
    static readonly Color SparkA = new Color(0.75f, 0.35f, 1f, 1f);
    static readonly Color SparkB = new Color(1f, 0.55f, 1f, 1f);

    public bool Active;

    class Spark
    {
        public Image Image;
        public Vector2 Start;
        public float Age;
        public float Sway;
    }

    readonly Spark[] _sparks = new Spark[MaxSparks];
    RectTransform _area;
    float _nextSpawn;

    void Awake()
    {
        _area = (RectTransform)transform;
        for (int i = 0; i < MaxSparks; ++i)
        {
            var go = new GameObject("VoidSpark", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var image = go.AddComponent<Image>();
            image.raycastTarget = false;
            image.rectTransform.sizeDelta = new Vector2(SparkSize, SparkSize);
            go.SetActive(false);
            _sparks[i] = new Spark { Image = image, Age = Life };
        }
    }

    void OnDisable()
    {
        foreach (var spark in _sparks)
        {
            spark.Age = Life;
            spark.Image.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        if (Active && Time.unscaledTime >= _nextSpawn)
        {
            _nextSpawn = Time.unscaledTime + SpawnInterval * Random.Range(0.6f, 1.4f);
            SpawnOne();
        }

        foreach (var spark in _sparks)
        {
            if (spark.Age >= Life)
                continue;

            spark.Age += dt;
            if (spark.Age >= Life)
            {
                spark.Image.gameObject.SetActive(false);
                continue;
            }

            float t = spark.Age / Life;
            spark.Image.rectTransform.anchoredPosition = spark.Start
                + new Vector2(Mathf.Sin(spark.Age * 3f + spark.Sway) * Drift, Rise * t);
            var c = spark.Image.color;
            c.a = MaxAlpha * Mathf.Sin(t * Mathf.PI); // fade in and out
            spark.Image.color = c;
        }
    }

    void SpawnOne()
    {
        foreach (var spark in _sparks)
        {
            if (spark.Age < Life)
                continue;

            // Somewhere over the body: the middle half horizontally, 15-70% of the height (they rise from there).
            Rect r = _area.rect;
            float x = Random.Range(r.xMin + r.width * 0.25f, r.xMax - r.width * 0.25f);
            float y = Random.Range(r.yMin + r.height * 0.05f, r.yMin + r.height * 0.3f);
            spark.Start = new Vector2(x, y) - r.center; // sparks are center-anchored
            spark.Age = 0f;
            spark.Sway = Random.value * Mathf.PI * 2f;
            var c = Color.Lerp(SparkA, SparkB, Random.value);
            c.a = 0f;
            spark.Image.color = c;
            spark.Image.rectTransform.anchoredPosition = spark.Start;
            spark.Image.gameObject.SetActive(true);
            return;
        }
    }
}
