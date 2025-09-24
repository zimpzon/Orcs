using UnityEngine;
using UnityEngine.UI;

public class UpgradeResizer : MonoBehaviour
{
    public GridLayoutGroup GridLayoutGroup;
    public RectTransform UpgradeItemPanel;
    public RectTransform ScrollPanel;

    const int ExpandedColumnCount = 6;
    int collapsedColumnCount;

    static readonly Vector2 ScrollExpandedSize = new(1450, 762);
    static readonly Vector2 ScrollExpandedPos = Vector2.zero;
    Vector2 scrollCollapsedSize;
    Vector2 scrollCollapsedPos;

    static readonly Vector2 ItemPanelPos = new(-115, -61);
    static readonly Vector2 ItemPanelExpandedSize = new(760, 396);
    Vector2 itemPanelCollapsedSize;
    bool isExpanded = false;

    void Start()
    {
        CaptureCurrentValues();
    }

    void CaptureCurrentValues()
    {
        if (GridLayoutGroup != null)
        {
            collapsedColumnCount = GridLayoutGroup.constraintCount;
        }

        if (ScrollPanel != null)
        {
            scrollCollapsedSize = ScrollPanel.sizeDelta;
            scrollCollapsedPos = ScrollPanel.anchoredPosition;
        }

        if (UpgradeItemPanel != null)
        {
            itemPanelCollapsedSize = UpgradeItemPanel.sizeDelta;
        }
    }

    public void Toggle()
    {
        if (isExpanded)
            Collapse();
        else
            Expand();
    }

    public void Collapse()
    {
        isExpanded = false;
        if (GridLayoutGroup != null)
        {
            GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GridLayoutGroup.constraintCount = collapsedColumnCount;
        }

        if (ScrollPanel != null)
        {
            ScrollPanel.anchoredPosition = scrollCollapsedPos;
            ScrollPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scrollCollapsedSize.x);
            ScrollPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scrollCollapsedSize.y);
        }

        if (UpgradeItemPanel != null)
        {
            UpgradeItemPanel.anchoredPosition = ItemPanelPos;
            UpgradeItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemPanelCollapsedSize.x);
            UpgradeItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemPanelCollapsedSize.y);
        }
    }

    void Expand()
    {
        isExpanded = true;
        if (GridLayoutGroup != null)
        {
            GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            GridLayoutGroup.constraintCount = ExpandedColumnCount;
        }

        if (ScrollPanel != null)
        {
            ScrollPanel.anchoredPosition = ScrollExpandedPos;
            ScrollPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ScrollExpandedSize.x);
            ScrollPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScrollExpandedSize.y);
        }

        if (UpgradeItemPanel != null)
        {
            UpgradeItemPanel.anchoredPosition = ItemPanelPos;
            UpgradeItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, ItemPanelExpandedSize.x);
            UpgradeItemPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ItemPanelExpandedSize.y);
        }
    }
}
