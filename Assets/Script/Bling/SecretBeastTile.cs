using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Secret last tile in the bestiary, only after 100% completion (or the RightShift+F / RightShift+G preview): a frame
// full of thick drifting white/grey fog, no silhouette. "Touch fog" (BeastiaryPopupScript.OnTouchFog) clears it and Scary Earl appears,
// idle-animated. Not a beast: no counter, bonus or completion. Built from code on an extra BeastPrefab tile; the fog
// blobs are soft circles from a small runtime texture. Unscaled time.
public class SecretBeastTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    const int BlobCount = 12;
    const float BlobSizeMul = 1.1f;      // relative to the tile's shorter side
    const float BlobDrift = 0.12f;       // relative to the tile size
    const float BlobBreath = 0.15f;
    const float RevealTime = 1.0f;
    const float RevealBlobGrow = 0.6f;   // extra scale while the fog clears
    const float FoggedIconAlpha = 0.12f;   // Scary Earl behind the fog: only a faint hint
    const float RevealIconPop = 0.1f;      // Scary Earl bumps up this much while the fog clears
    const float FrameTime = 0.15f;       // same as the skin preview
    const float IconInset = 0f;          // like the final enemy's icon (BeastiaryPopupScript)
    const float IconRaise = 3f;
    static readonly Color FogColor = new Color(0.86f, 0.86f, 0.88f, 1f); // white/grey, heavy

    const string DefaultHovertext = "Hover over a beast to see details";
    const string FogHovertext = "???\n<size=80%>Something hides in the fog</size>";
    const string RevealedHovertext = "Scary Earl, the MASTER MIND";

    public static SecretBeastTile Instance;
    public static bool Unlocked => SaveGame.Members.Achieved.Contains(Achieved.Completion100) || CrownPulse.CheatForceShow;

    class Blob
    {
        public RectTransform Rect;
        public Image Image;
        public Vector2 Home;   // 0..1 inside the tile
        public float Phase;
        public float Speed;
        public float Alpha;
    }

    TextMeshProUGUI _hovertext;
    Image _icon;
    Sprite[] _frames;
    int _frame;
    float _nextFrame;
    Blob[] _blobs;
    float _reveal = -1f;   // -1: not revealing, 0..1 while the fog clears
    static Sprite _blobSprite;

    public void Init(Image icon, TextMeshProUGUI hovertext)
    {
        Instance = this;
        _icon = icon;
        _hovertext = hovertext;
        gameObject.AddComponent<RectMask2D>(); // the fog stays inside the frame

        var rect = _icon.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(IconInset, IconInset + IconRaise);
        rect.offsetMax = new Vector2(-IconInset, -IconInset + IconRaise);
        _icon.preserveAspect = true;

        // The skin list asset is referenced by the (possibly never opened) skins popup: look it up directly.
        var skins = Resources.FindObjectsOfTypeAll<SkinsAnimationList>().FirstOrDefault();
        _frames = skins?.SkinAnimations.FirstOrDefault(s => s.Animation == SkinAnimation.ScaryEarl)?.IdleSprites;
        if (_frames != null && _frames.Length > 0)
            _icon.sprite = _frames[0];
        else
            Debug.LogWarning("SecretBeastTile: no Scary Earl idle sprites found");

        _blobs = new Blob[BlobCount];
        for (int i = 0; i < BlobCount; ++i)
        {
            var go = new GameObject("Fog", typeof(RectTransform));
            go.transform.SetParent(transform, false); // after the icon: drawn on top of it
            var image = go.AddComponent<Image>();
            image.sprite = BlobSprite();
            image.raycastTarget = false;
            _blobs[i] = new Blob
            {
                Rect = image.rectTransform,
                Image = image,
                Home = new Vector2(UnityEngine.Random.Range(0.1f, 0.9f), UnityEngine.Random.Range(0.1f, 0.9f)),
                Phase = UnityEngine.Random.value * Mathf.PI * 2f,
                Speed = UnityEngine.Random.Range(0.4f, 0.8f),
                Alpha = UnityEngine.Random.Range(0.65f, 0.9f),
            };
        }

        SyncState();
    }

    // Shows the saved state right away: fog, or Scary Earl when the fog was already touched (also after the cheat).
    public void SyncState()
    {
        _reveal = -1f;
        bool revealed = SaveGame.Members.FogTouched;
        SetIcon(revealed ? 1f : FoggedIconAlpha, 1f); // always there, almost invisible behind the fog until it clears
        foreach (var blob in _blobs)
            blob.Image.enabled = !revealed;
    }

    public void Reveal()
    {
        _reveal = 0f;
        if (_hovertext != null && _hovertext.text == FogHovertext)
            HoverTextFx.Of(_hovertext).Hover(this, RevealedHovertext);
    }

    void OnEnable()
    {
        if (_blobs != null)
            SyncState();
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime;

        if (_frames != null && _frames.Length > 1 && Time.unscaledTime >= _nextFrame)
        {
            _nextFrame = Time.unscaledTime + FrameTime;
            _frame = (_frame + 1) % _frames.Length;
            _icon.sprite = _frames[_frame];
        }

        float clear = 0f;
        if (_reveal >= 0f)
        {
            _reveal = Mathf.Min(1f, _reveal + dt / RevealTime);
            clear = _reveal;
            float ease = 1f - (1f - _reveal) * (1f - _reveal);
            SetIcon(Mathf.Lerp(FoggedIconAlpha, 1f, ease), 1f + RevealIconPop * Mathf.Sin(ease * Mathf.PI));
            if (_reveal >= 1f)
            {
                _reveal = -1f;
                foreach (var blob in _blobs)
                    blob.Image.enabled = false;
                return;
            }
        }

        Vector2 size = ((RectTransform)transform).rect.size;
        float blobSize = Mathf.Min(size.x, size.y) * BlobSizeMul;
        float t = Time.unscaledTime;
        foreach (var blob in _blobs)
        {
            if (!blob.Image.enabled)
                continue;
            float a = t * blob.Speed + blob.Phase;
            var drift = new Vector2(Mathf.Sin(a), Mathf.Cos(a * 0.7f)) * BlobDrift;
            blob.Rect.anchoredPosition = Vector2.Scale(blob.Home + drift - new Vector2(0.5f, 0.5f), size);
            float scale = (1f + BlobBreath * Mathf.Sin(a * 1.3f)) * (1f + RevealBlobGrow * clear);
            blob.Rect.sizeDelta = new Vector2(blobSize, blobSize) * scale;
            var c = FogColor;
            c.a = blob.Alpha * (1f - clear);
            blob.Image.color = c;
        }
    }

    void SetIcon(float alpha, float scale)
    {
        var c = _icon.color;
        c.a = alpha;
        _icon.color = c;
        _icon.rectTransform.localScale = Vector3.one * scale;
    }

    string CurrentHovertext => SaveGame.Members.FogTouched ? RevealedHovertext : FogHovertext;

    // Same as the beasts: mouse hover, or a tap that pins the text (TouchPin).
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TouchPin.IsTouch(eventData))
            TouchPin.Tap(this, ShowDetails, HideDetails);
        else
            ShowDetails();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TouchPin.IsTouch(eventData))
            HideDetails();
    }

    void ShowDetails() => HoverTextFx.Of(_hovertext).Hover(this, CurrentHovertext);

    void HideDetails() => HoverTextFx.Of(_hovertext).Unhover(this, DefaultHovertext);

    // Soft white circle (alpha falls off toward the edge), shared by all blobs; tinted per blob.
    static Sprite BlobSprite()
    {
        if (_blobSprite != null)
            return _blobSprite;

        const int Size = 32;
        var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        var pixels = new Color32[Size * Size];
        for (int y = 0; y < Size; ++y)
        {
            for (int x = 0; x < Size; ++x)
            {
                float dx = (x + 0.5f) / Size * 2f - 1f;
                float dy = (y + 0.5f) / Size * 2f - 1f;
                float d = Mathf.Clamp01(Mathf.Sqrt(dx * dx + dy * dy));
                float a = 1f - d;
                pixels[y * Size + x] = new Color32(255, 255, 255, (byte)(a * a * 255));
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        _blobSprite = Sprite.Create(tex, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f));
        return _blobSprite;
    }
}
