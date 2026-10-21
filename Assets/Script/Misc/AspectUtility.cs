using UnityEngine;

// Letterboxes / pillarboxes the world camera to 16:9 (black bars). The overlay UI does the same via UiAspectFrame.
public class AspectUtility : MonoBehaviour
{
    public static float WantedAspectRatio = 16.0f / 9.0f;
    static float wantedAspectRatio;
    int _lastW, _lastH;
    static Camera cam;
    static Camera backgroundCam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (!cam)
        {
            cam = Camera.main;
        }

        if (!cam)
        {
            Debug.LogError("No camera available");
            return;
        }

        wantedAspectRatio = WantedAspectRatio;

        Apply();
    }

    // Any window size change (not only aspect): re-letterbox, so the arena always lines up with the UI (UiAspectFrame).
    void Update()
    {
        if (cam != null && (Screen.width != _lastW || Screen.height != _lastH))
            Apply();
    }

    void Apply()
    {
        _lastW = Screen.width;
        _lastH = Screen.height;
        SetCamera();
        Debug.Log($"AspectUtility: screen {Screen.width}x{Screen.height}, camera rect {cam.rect}");
    }

    public static void SetCamera()
    {
        if (Screen.height <= 0)
            return;
        float currentAspectRatio = (float)Screen.width / Screen.height;
        // Full viewport only when (practically) exactly 16:9. This used to compare two decimals (1.77..1.78 counted as
        // 16:9), which stretched the arena while the UI (UiAspectFrame) letterboxed exactly: after a resize to a
        // nearly-16:9 window the arena sat a little off from the UI.
        if (Mathf.Abs(currentAspectRatio - wantedAspectRatio) < 0.0005f)
        {
            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            if (backgroundCam)
            {
                Destroy(backgroundCam.gameObject);
            }
            return;
        }

        // Pillarbox
        if (currentAspectRatio > wantedAspectRatio)
        {
            float inset = 1.0f - wantedAspectRatio / currentAspectRatio;
            cam.rect = new Rect(inset / 2, 0.0f, 1.0f - inset, 1.0f);
        }
        // Letterbox
        else
        {
            float inset = 1.0f - currentAspectRatio / wantedAspectRatio;
            cam.rect = new Rect(0.0f, inset / 2, 1.0f, 1.0f - inset);
        }

        if (!backgroundCam)
        {
            // Make a new camera behind the normal camera which displays black; otherwise the unused space is undefined
            backgroundCam = new GameObject("BackgroundCam", typeof(Camera)).GetComponent<Camera>();
            backgroundCam.depth = int.MinValue;
            backgroundCam.clearFlags = CameraClearFlags.SolidColor;
            backgroundCam.backgroundColor = Color.black;
            backgroundCam.cullingMask = 0;
        }
    }

    public static int screenHeight
    {
        get
        {
            return (int)(Screen.height * cam.rect.height);
        }
    }

    public static int screenWidth
    {
        get
        {
            return (int)(Screen.width * cam.rect.width);
        }
    }

    public static int xOffset
    {
        get
        {
            return (int)(Screen.width * cam.rect.x);
        }
    }

    public static int yOffset
    {
        get
        {
            return (int)(Screen.height * cam.rect.y);
        }
    }

    public static Rect screenRelativeRect
    {
        get
        {
            float orthoH = cam.orthographicSize;
            return new Rect(-orthoH * wantedAspectRatio, -orthoH, 2 * orthoH * wantedAspectRatio, 2 * orthoH);
        }
    }

    public static Rect screenRect
    {
        get
        {
            return new Rect(cam.rect.x * Screen.width, cam.rect.y * Screen.height, cam.rect.width * Screen.width, cam.rect.height * Screen.height);
        }
    }

    //public static Vector3 mousePosition
    //{
    //    get
    //    {
    //        Vector3 mousePos = Input.mousePosition;
    //        mousePos.y -= (int)(cam.rect.y * Screen.height);
    //        mousePos.x -= (int)(cam.rect.x * Screen.width);
    //        return mousePos;
    //    }
    //}

    public static Vector2 GuiPosition(Vector3 pos)
    {
            pos.y = Mathf.Clamp(pos.y, cam.rect.y * Screen.height, cam.rect.y * Screen.height + cam.rect.height * Screen.height);
            pos.x = Mathf.Clamp(pos.x, cam.rect.x * Screen.width, cam.rect.x * Screen.width + cam.rect.width * Screen.width);
            return pos;
    }

    public static Vector2 guiMousePosition
    {
        get
        {
            Vector2 mousePos = Event.current.mousePosition;
            return GuiPosition(mousePos);
        }
    }
}