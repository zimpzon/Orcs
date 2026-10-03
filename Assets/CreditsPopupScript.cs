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
    const float EarlHeightMul = 2.2f;  // relative to the header's drawn glyph height
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

        // Header line bounds from the drawn glyphs of line 0, in the text's local space.
        ProgressText.ForceMeshUpdate();
        var info = ProgressText.textInfo;
        if (info.lineCount == 0)
        {
            Debug.LogWarning("CreditsPopupScript: header text has no lines, Earls not placed");
            return;
        }
        var line = info.lineInfo[0];
        bool any = false;
        Vector3 min = Vector3.zero, max = Vector3.zero;
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
        if (!any)
            return;

        float size = (max.y - min.y) * EarlHeightMul;
        float centerY = (min.y + max.y) * 0.5f;
        _earls = new Image[2];
        for (int i = 0; i < 2; ++i)
        {
            var go = new GameObject(i == 0 ? "EarlLeft" : "EarlRight", typeof(RectTransform));
            go.transform.SetParent(ProgressText.rectTransform, false);
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
