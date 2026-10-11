using System.Collections.Generic;
using UnityEngine;

// Crown on Earl's head once the game is 100% complete (Achieved.Completion100; RightShift+G previews it, like CrownPulse).
// The head top differs per skin and per animation frame, so it is found from the sprite's own pixels: the topmost
// opaque row and the head's width there, cached per sprite. The crown then sits on the head through the idle bob and
// running and follows flipX; it has the same size on every skin (CrownWidthPx). Added by PlayerScript.Awake.
public class PlayerCrown : MonoBehaviour
{
    const float AlphaThreshold = 0.5f;
    const int HeadRows = 3;        // opaque rows from the top used to find the head's center
    const float CrownWidthPx = 10f; // same size on every skin, in the player sprites' pixels
    const float SinkPx = 3f;       // crown rests this many pixels into the head so it doesn't float

    SpriteRenderer _player;
    SpriteRenderer _crown;
    Transform _crownTrans;

    struct Head
    {
        public bool Valid;
        public float TopPx, CenterXPx, WidthPx; // in sprite pixels, relative to the sprite's pivot
    }

    static readonly Dictionary<Sprite, Head> _heads = new();
    static readonly Dictionary<Texture2D, Color32[]> _pixels = new();

    public void Init(SpriteRenderer player, Sprite crownSprite)
    {
        _player = player;
        if (crownSprite == null)
        {
            enabled = false;
            return;
        }

        var go = new GameObject("Crown");
        go.layer = gameObject.layer;
        _crownTrans = go.transform;
        _crownTrans.SetParent(transform, false);
        _crown = go.AddComponent<SpriteRenderer>();
        _crown.sprite = crownSprite;
        _crown.sortingLayerID = player.sortingLayerID;
        _crown.enabled = false;
    }

    void LateUpdate()
    {
        if (_crown == null)
            return;

        bool show = CrownPulse.CheatForceShow || SaveGame.Members.Achieved.Contains(Achieved.Completion100);
        var sprite = _player.sprite;
        var head = show && sprite != null ? GetHead(sprite) : default;
        bool visible = show && head.Valid && _player.enabled;
        if (_crown.enabled != visible)
            _crown.enabled = visible;
        if (!visible)
            return;

        float ppu = sprite.pixelsPerUnit;
        var crownSprite = _crown.sprite;
        float crownWidthUnits = crownSprite.rect.width / crownSprite.pixelsPerUnit;
        float wantedWidthUnits = CrownWidthPx / ppu;
        float scale = wantedWidthUnits / crownWidthUnits;
        _crownTrans.localScale = new Vector3(scale, scale, 1f);

        // Crown sprite pivot is its center: put its bottom edge on the head top (sunk by SinkPx).
        float crownHalfHeightUnits = crownSprite.rect.height / crownSprite.pixelsPerUnit * scale * 0.5f;
        float x = head.CenterXPx / ppu;
        if (_player.flipX)
            x = -x;
        float y = (head.TopPx - SinkPx) / ppu + crownHalfHeightUnits;
        _crownTrans.localPosition = new Vector3(x, y, 0f);

        _crown.sortingOrder = _player.sortingOrder + 1;
    }

    static Head GetHead(Sprite sprite)
    {
        if (_heads.TryGetValue(sprite, out var head))
            return head;

        head = MeasureHead(sprite);
        _heads[sprite] = head;
        return head;
    }

    static Head MeasureHead(Sprite sprite)
    {
        var tex = sprite.texture;
        var px = ReadPixels(tex);
        if (px == null)
            return default;

        var r = sprite.textureRect;
        int x0 = Mathf.RoundToInt(r.x), y0 = Mathf.RoundToInt(r.y);
        int w = Mathf.RoundToInt(r.width), h = Mathf.RoundToInt(r.height);
        byte threshold = (byte)(AlphaThreshold * 255);

        int topRow = -1, minX = int.MaxValue, maxX = int.MinValue, rowsSeen = 0;
        for (int y = h - 1; y >= 0 && rowsSeen < HeadRows; --y)
        {
            bool any = false;
            for (int x = 0; x < w; ++x)
            {
                if (px[(y0 + y) * tex.width + x0 + x].a < threshold)
                    continue;
                any = true;
                minX = Mathf.Min(minX, x);
                maxX = Mathf.Max(maxX, x);
            }

            if (!any)
            {
                if (topRow >= 0)
                    rowsSeen++; // a gap inside the head rows still counts as a row
                continue;
            }

            if (topRow < 0)
                topRow = y;
            rowsSeen++;
        }

        if (topRow < 0)
            return default;

        var pivot = sprite.pivot; // in pixels, relative to the sprite rect
        return new Head
        {
            Valid = true,
            TopPx = topRow + 1 - pivot.y,
            CenterXPx = (minX + maxX + 1) * 0.5f - pivot.x,
            WidthPx = maxX - minX + 1,
        };
    }

    // The sprite sheets aren't readable: copy once through a RenderTexture (works on WebGL too). Cached per texture.
    static Color32[] ReadPixels(Texture2D tex)
    {
        if (tex == null)
            return null;
        if (_pixels.TryGetValue(tex, out var cached))
            return cached;

        var rt = RenderTexture.GetTemporary(tex.width, tex.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        var prev = RenderTexture.active;
        Graphics.Blit(tex, rt);
        RenderTexture.active = rt;
        var copy = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
        copy.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        copy.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        var px = copy.GetPixels32();
        Destroy(copy);
        _pixels[tex] = px;
        return px;
    }
}
