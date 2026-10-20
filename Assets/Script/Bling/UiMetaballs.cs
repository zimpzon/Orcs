using UnityEngine;
using UnityEngine.UI;

// Faint moving metaballs behind a dialog's content (Custom/UIMetaballs). Takes the color from the dialog panel it sits
// in (the parent Image, ColorLink palette), so the blobs are just a lighter shade of the same color. Its own material
// instance, unscaled time. Put it on a stretched child Image as the first child of the dialog.
// All settings below apply live (also while playing), per dialog.
[RequireComponent(typeof(Image))]
public class UiMetaballs : MonoBehaviour
{
    [Header("Motion")]
    [Tooltip("Animation speed. Low = calm background texture.")]
    [Range(0f, 2f)] public float Speed = 0.15f;

    [Header("Blobs")]
    [Tooltip("Blob size (relative to the panel height).")]
    [Range(0.05f, 0.6f)] public float BallSize = 0.22f;
    [Tooltip("Lower = blobs merge more and look bigger, higher = smaller, more separate blobs.")]
    [Range(0.3f, 3f)] public float Threshold = 1.0f;
    [Tooltip("How much lighter than the panel color the blobs are (0 = same color, negative = darker).")]
    [Range(-0.5f, 0.5f)] public float BallLift = 0.1f;
    [Tooltip("Blob opacity.")]
    [Range(0f, 1f)] public float BallAlpha = 0.35f;

    [Header("Rim")]
    [Tooltip("How much lighter than the panel color the rim around the blobs is.")]
    [Range(-0.5f, 0.5f)] public float RimLift = 0.2f;
    [Tooltip("Rim opacity (0 = no rim).")]
    [Range(0f, 1f)] public float RimAlpha = 0.3f;

    [Header("Color")]
    [Tooltip("Follow the parent panel's color. Off: use this Image's own color.")]
    public bool UsePanelColor = true;

    static readonly int PhaseId = Shader.PropertyToID("_Phase");
    static readonly int AspectId = Shader.PropertyToID("_Aspect");
    static readonly int BallSizeId = Shader.PropertyToID("_BallSize");
    static readonly int ThresholdId = Shader.PropertyToID("_Threshold");
    static readonly int BallLiftId = Shader.PropertyToID("_BallLift");
    static readonly int BallAlphaId = Shader.PropertyToID("_BallAlpha");
    static readonly int RimLiftId = Shader.PropertyToID("_RimLift");
    static readonly int RimAlphaId = Shader.PropertyToID("_RimAlpha");

    Image _image;
    Image _panel;
    Material _mat;
    float _phase;

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.raycastTarget = false;
        _mat = new Material(_image.material);
        _image.material = _mat;
        _panel = transform.parent != null ? transform.parent.GetComponent<Image>() : null;
        _phase = Random.value * 100f;
    }

    void Update()
    {
        _phase += Time.unscaledDeltaTime * Speed;
        _mat.SetFloat(PhaseId, _phase);

        var size = ((RectTransform)transform).rect.size;
        if (size.y > 0f)
            _mat.SetFloat(AspectId, size.x / size.y);

        _mat.SetFloat(BallSizeId, BallSize);
        _mat.SetFloat(ThresholdId, Threshold);
        _mat.SetFloat(BallLiftId, BallLift);
        _mat.SetFloat(BallAlphaId, BallAlpha);
        _mat.SetFloat(RimLiftId, RimLift);
        _mat.SetFloat(RimAlphaId, RimAlpha);

        // The panel's color (it can change with the palette); our own alpha stays as authored.
        if (UsePanelColor && _panel != null)
        {
            var c = _panel.color;
            c.a = _image.color.a;
            if (_image.color != c)
                _image.color = c;
        }
    }

    void OnDestroy()
    {
        if (_mat != null)
            Destroy(_mat);
    }
}
