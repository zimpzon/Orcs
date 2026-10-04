using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsPopupScript : MonoBehaviour
{
    public TextMeshProUGUI ProgressText;

    static readonly string[] Thanks =
    {
        "Name One",
        "Name Two",
    };

    const string GoldHex = "#FFD54A";

    // Same layout as the victory popup it was cloned from: widen it (staying centered) and grow it downward to fit.
    const float ExtraWidth = 80f;
    RectTransform _popupRect;
    float _popupBaseHeight;
    float _textBaseHeight;

    void Awake()
    {
        _popupRect = (RectTransform)transform;
        _popupRect.sizeDelta += new Vector2(ExtraWidth, 0);
        _popupRect.anchoredPosition -= new Vector2(ExtraWidth * 0.5f, 0);

        var textRect = ProgressText.rectTransform;
        textRect.sizeDelta += new Vector2(ExtraWidth, 0);

        _popupBaseHeight = _popupRect.sizeDelta.y;
        _textBaseHeight = textRect.sizeDelta.y;

        SetupEarls();
    }

    // An idle-animated Earl on each side of the header line, both facing the text.
    const float EarlHeightMul = 1.8f;  // relative to one header line's drawn glyph height (fallback)
    const float EarlHeightMulHeader = 0.5f;
    const float EarlFeetRefMul = 0.9f;  // feet line: where the bottom of a 0.9x-block-height Earl would be // relative to the whole header block (TextGameName, may be 2 lines)
    const float EarlGap = 6f;          // header glyph edge to the Earl's edge
    const float EarlFrameTime = 0.15f; // same as the skin preview
    Image[] _earls;
    Sprite[] _earlFrames;
    int _earlFrame;
    float _nextEarlFrame;

    void SetupEarls()
    {
        // The skin list asset is referenced by the (possibly never activated) Skins popup, so look it up directly
        // rather than through SelectedSkinScript.Instance, which is null until that popup has been opened.
        var skins = Resources.FindObjectsOfTypeAll<SkinsAnimationList>().FirstOrDefault();
        var earl = skins != null ? skins.SkinAnimations.FirstOrDefault(s => s.Animation == SkinAnimation.Default) : null;
        _earlFrames = earl?.IdleSprites;
        if (_earlFrames == null || _earlFrames.Length == 0)
        {
            var player = FindAnyObjectByType<PlayerScript>();
            _earlFrames = player != null ? player.IdleSprites : null;
        }
        if (_earlFrames == null || _earlFrames.Length == 0)
        {
            Debug.LogWarning("CreditsPopupScript: no Earl idle sprites found");
            return;
        }

        // The "Idle Earl" header is its own text object (TextGameName...). Use all its drawn glyphs (it may be two
        // lines). Fallback: the first line of the main text that has visible characters.
        var header = GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name.StartsWith("TextGameName"));
        TextMeshProUGUI anchorText = header != null ? header : ProgressText;
        if (!GlyphBounds(anchorText, wholeText: header != null, out Vector3 min, out Vector3 max))
        {
            Debug.LogWarning("CreditsPopupScript: no visible header text, Earls not placed");
            return;
        }

        float size = (max.y - min.y) * (header != null ? EarlHeightMulHeader : EarlHeightMul);
        float centerY = (min.y + max.y) * 0.5f;
        // Next to the header block, keep the Earls' feet where the old 0.9-size Earls stood (bottom-aligned)
        // instead of centering, so shrinking them moves them down rather than shrinking toward the middle.
        if (header != null)
        {
            float blockHeight = max.y - min.y;
            float feetY = centerY - blockHeight * EarlFeetRefMul * 0.5f;
            centerY = feetY + size * 0.5f;
        }
        _earls = new Image[2];
        for (int i = 0; i < 2; ++i)
        {
            var go = new GameObject(i == 0 ? "EarlLeft" : "EarlRight", typeof(RectTransform));
            go.transform.SetParent(anchorText.rectTransform, false);
            var image = go.AddComponent<Image>();
            image.sprite = _earlFrames[0];
            image.preserveAspect = true;
            image.raycastTarget = false;

            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            float x = i == 0 ? min.x - EarlGap - size * 0.5f : max.x + EarlGap + size * 0.5f;
            rect.localPosition = new Vector3(x, centerY, 0);
            // Earl's sprites face right: the left one already looks at the text, mirror the right one.
            rect.localScale = new Vector3(i == 0 ? 1f : -1f, 1f, 1f);
            _earls[i] = image;
        }
    }

    // Drawn glyph bounds (text-local space) of all visible characters, or of the first line that has any.
    static bool GlyphBounds(TextMeshProUGUI text, bool wholeText, out Vector3 min, out Vector3 max)
    {
        min = max = Vector3.zero;
        bool any = false;
        text.ForceMeshUpdate();
        var info = text.textInfo;
        for (int l = 0; l < info.lineCount; ++l)
        {
            var line = info.lineInfo[l];
            for (int c = line.firstCharacterIndex; c <= line.lastCharacterIndex && c < info.characterCount; ++c)
            {
                var ch = info.characterInfo[c];
                if (!ch.isVisible)
                    continue;
                Vector3 bl = ch.vertex_BL.position, tr = ch.vertex_TR.position;
                min = any ? Vector3.Min(min, bl) : bl;
                max = any ? Vector3.Max(max, tr) : tr;
                any = true;
            }
            if (!wholeText && any)
                return true;
        }
        return any;
    }

    void Update()
    {
        if (_earls == null || Time.unscaledTime < _nextEarlFrame)
            return;

        _nextEarlFrame = Time.unscaledTime + EarlFrameTime;
        _earlFrame = (_earlFrame + 1) % _earlFrames.Length;
        foreach (var earl in _earls)
            earl.sprite = _earlFrames[_earlFrame];
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }
}
