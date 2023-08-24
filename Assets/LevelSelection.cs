using UnityEngine;

public class LevelSelection : MonoBehaviour
{
    public Transform Indicator;
    public float ItemSize;
    public UpgradeChoiceScript[] Choices;

    Vector3 indicatorBase;
    int selectionIdx;
    const int SelectionCount = 4;

    private void OnEnable()
    {
        selectionIdx = 0;
        SetIndicatorPosition();
    }

    void Start()
    {
        indicatorBase = Indicator.GetComponent<RectTransform>().anchoredPosition;
        Debug.Log(indicatorBase);
    }

    void SetIndicatorPosition()
    {
        Debug.Log(selectionIdx);
        Indicator.GetComponent<RectTransform>().anchoredPosition = indicatorBase + Vector3.down * selectionIdx * ItemSize;
    }

    void Update()
    {
        if (G.MoveUpTap() && selectionIdx > 0)
        {
            selectionIdx--;
            SetIndicatorPosition();
        }

        if (G.MoveDownTap() && selectionIdx < SelectionCount - 1)
        {
            selectionIdx++;
            SetIndicatorPosition();
        }

        if (G.SelectionTap())
        {
            Choices[selectionIdx].OnClick();
        }
    }
}
