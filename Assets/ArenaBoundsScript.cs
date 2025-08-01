using UnityEngine;
using UnityEngine.EventSystems;

public class ArenaBoundsScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static ArenaBoundsScript Instance;

    public GameObject GameVisibleWarning;
    public LineRenderer LineRenderer;
    public float LineRendererBaseWidth;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameVisibleWarning.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameVisibleWarning.SetActive(false);
    }

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
