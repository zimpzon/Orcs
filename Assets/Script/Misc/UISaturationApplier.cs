using UnityEngine;
using UnityEngine.UI;

// Gives UI Images and legacy Texts the "UI/Saturated" shader so G.UISaturationBoost affects buttons and panels
// (including the sprite pixels, not just their tint). TMP text gets the same effect from the patched TMP_SDF shaders.
// Only graphics still on Unity's default UI material are switched; custom materials are left alone.
// Re-scans periodically so UI spawned later (upgrade rows, cards, popups) is covered too.
public class UISaturationApplier : MonoBehaviour
{
    const float ScanInterval = 0.5f;

    Material _saturatedMaterial;
    float _nextScan;

    void Awake()
    {
        var shader = Resources.Load<Shader>("UI-Saturated");
        if (shader == null)
        {
            Debug.LogError("UISaturationApplier: Resources/UI-Saturated.shader not found, UI saturation disabled.");
            enabled = false;
            return;
        }

        _saturatedMaterial = new Material(shader) { name = "UISaturated (runtime)" };
    }

    void Update()
    {
        if (Time.realtimeSinceStartup < _nextScan)
            return;

        _nextScan = Time.realtimeSinceStartup + ScanInterval;

        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!canvas.isRootCanvas)
                continue;

            foreach (var graphic in canvas.GetComponentsInChildren<Graphic>(true))
            {
                // Images and legacy Text only: TMP has its own shaders, and RawImage often shows render
                // textures (game views) that shouldn't be saturated.
                bool isCandidate = graphic is Image || graphic is Text;
                if (isCandidate && graphic.material == graphic.defaultMaterial)
                    graphic.material = _saturatedMaterial;
            }
        }
    }
}
