using UnityEngine;

// Small health bar over an enemy's head, only while it's damaged and alive: dark background, red fill (gold for the
// final enemy) and a short white chunk trailing behind the fill so hits read clearly. Not on Scary Earl (he has the
// big bar at the top). Added in code to every enemy (ActorBase.Awake). The bar is its own root object, positioned in
// world space each frame, so the enemy's flipping / death rotation never touches it. Settings "Show healthbars" (or RightShift+V) turns them off.
public class EnemyHealthBar : MonoBehaviour
{
    // Settings checkbox "Show healthbars" (saved; RightShift+V flips it too).
    static bool Enabled => SaveGame.Members.ShowHealthBars;

    const float WidthFactor = 0.7f;   // x the enemy sprite's width
    const float MinWidth = 0.5f, MaxWidth = 2.0f;
    const float Height = 0.12f;
    const float Gap = -0.1f;         // above the sprite's top (negative: the sprites have empty rows above the head)
    const float Border = 0.03f;       // background showing around the fill
    const float TrailSpeed = 3f;      // trail catches up this fraction of the bar per second

    static readonly Color BackColor = new(0f, 0f, 0f, 0.6f);
    static readonly Color FillColor = new Color32(0xD9, 0x43, 0x3F, 255);
    static readonly Color FinalEnemyFillColor = new Color32(0xF5, 0xB8, 0x2E, 255);
    static readonly Color TrailColor = new(1f, 1f, 1f, 0.85f);

    static Sprite _center, _left;

    ActorBase _actor;
    SpriteRenderer _body;
    GameObject _root;
    SpriteRenderer _back, _trail, _fill;
    float _trailValue = 1f;

    public static void Attach(ActorBase actor)
    {
        if (actor.ActorType == ActorTypeEnum.None || actor.GetComponent<EnemyHealthBar>() != null)
            return;
        actor.gameObject.AddComponent<EnemyHealthBar>();
    }

    void Awake()
    {
        _actor = GetComponent<ActorBase>();
        _body = GetComponent<SpriteRenderer>();
        if (_center == null)
            MakeSprites();

        _root = new GameObject("EnemyHealthBar") { hideFlags = HideFlags.HideInHierarchy }; // one per pooled enemy
        _back = NewPart("Back", _center, BackColor);
        _trail = NewPart("Trail", _left, TrailColor);
        _fill = NewPart("Fill", _left, FillColor);
        _root.SetActive(false);
    }

    SpriteRenderer NewPart(string name, Sprite sprite, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(_root.transform, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = color;
        return sr;
    }

    // 1x1 white, 1 unit in size: one centered (background), one pivoted on its left edge (fill and trail shrink from
    // the right with their x scale).
    static void MakeSprites()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        _center = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        _left = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0f, 0.5f), 1f);
    }

    void LateUpdate()
    {
        bool show = Enabled && _actor != null && !_actor.IsDead && !_actor.IsScaryEarl
                    && _actor.BaseHp > 0 && _actor.Hp < _actor.BaseHp && _body != null && _body.enabled;
        if (!show)
        {
            if (_root.activeSelf)
                _root.SetActive(false);
            _trailValue = 1f;
            return;
        }

        if (!_root.activeSelf)
            _root.SetActive(true);

        float value = Mathf.Clamp01((float)((double)_actor.Hp / _actor.BaseHp));
        _trailValue = Mathf.Max(value, _trailValue - TrailSpeed * Time.deltaTime);

        var bounds = _body.bounds;
        float width = Mathf.Clamp(bounds.size.x * WidthFactor, MinWidth, MaxWidth);
        float inner = width - Border * 2;
        var center = new Vector3(bounds.center.x, bounds.max.y + Gap + Height * 0.5f, 0f);
        _root.transform.position = center;

        _back.transform.localScale = new Vector3(width, Height, 1f);
        var left = new Vector3(-inner * 0.5f, 0f, 0f);
        _trail.transform.localPosition = left;
        _trail.transform.localScale = new Vector3(inner * _trailValue, Height - Border * 2, 1f);
        _fill.transform.localPosition = left;
        _fill.transform.localScale = new Vector3(inner * value, Height - Border * 2, 1f);
        _fill.color = _actor.ActorType == ActorTypeEnum.FromTheVoid ? FinalEnemyFillColor : FillColor;

        // Same layer as the body, just above it: an enemy standing in front still covers the bar.
        int layer = _body.sortingLayerID, order = _body.sortingOrder;
        _back.sortingLayerID = _trail.sortingLayerID = _fill.sortingLayerID = layer;
        _back.sortingOrder = order + 1;
        _trail.sortingOrder = order + 2;
        _fill.sortingOrder = order + 3;
    }

    // Pooled enemies are deactivated: hide the bar with them (it's not a child, so it wouldn't hide by itself).
    void OnDisable()
    {
        if (_root != null)
            _root.SetActive(false);
        _trailValue = 1f;
    }

    void OnDestroy()
    {
        if (_root != null)
            Destroy(_root);
    }
}
