using UnityEngine;

public class ArenaBoundsScript : MonoBehaviour
{
    public static ArenaBoundsScript Instance;

    public LineRenderer LineRenderer;
    public float LineRendererBaseWidth;

    private void Awake()
    {
        Instance = this;

        LineRendererBaseWidth = LineRenderer.startWidth;
        LineRenderer.startWidth = 0;
        LineRenderer.endWidth = 0;
    }

    void Update()
    {
    }
}
