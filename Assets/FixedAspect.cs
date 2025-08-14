using UnityEngine;

[ExecuteAlways] // works in edit and play mode
[RequireComponent(typeof(Camera))]
public class FixedAspect : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    Camera cam;
    float lastAspect;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        ApplyAspect();
    }

    void Update()
    {
        // Detect aspect changes (editor Game view resizing)
        float windowAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(windowAspect - lastAspect) > 0.001f)
        {
            ApplyAspect();
        }
    }

    void ApplyAspect()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        lastAspect = windowAspect;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1f)
        {
            // Letterbox
            Rect rect = cam.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0f;
            rect.y = (1f - scaleHeight) / 2f;
            cam.rect = rect;
        }
        else
        {
            // Pillarbox
            float scaleWidth = 1f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0f;
            cam.rect = rect;
        }
    }
}
