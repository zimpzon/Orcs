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
    bool isAnimating = false;

    const float AnimationDuration = 0.3f;

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
        if (isAnimating) return;

        if (isExpanded)
            Collapse();
        else
            Expand();
    }

    public void Collapse()
    {
        if (isAnimating) return;

        isExpanded = false;
        isAnimating = false;

        // Cancel any running animations
        if (ScrollPanel != null)
            LeanTween.cancel(ScrollPanel.gameObject);
        if (UpgradeItemPanel != null)
            LeanTween.cancel(UpgradeItemPanel.gameObject);

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
        if (isAnimating) return;

        isExpanded = true;
        isAnimating = true;

        if (GridLayoutGroup != null)
        {
            GridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            GridLayoutGroup.constraintCount = ExpandedColumnCount;
        }

        int animationsCompleted = 0;
        int totalAnimations = 0;

        if (ScrollPanel != null)
        {
            totalAnimations += 2; // position and size

            LeanTween.cancel(ScrollPanel.gameObject);

            LeanTween.move(ScrollPanel, ScrollExpandedPos, AnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(() =>
                {
                    animationsCompleted++;
                    if (animationsCompleted >= totalAnimations)
                        isAnimating = false;
                });

            LeanTween.size(ScrollPanel, ScrollExpandedSize, AnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(() =>
                {
                    animationsCompleted++;
                    if (animationsCompleted >= totalAnimations)
                        isAnimating = false;
                });
        }

        if (UpgradeItemPanel != null)
        {
            totalAnimations += 2; // position and size

            LeanTween.cancel(UpgradeItemPanel.gameObject);

            LeanTween.move(UpgradeItemPanel, ItemPanelPos, AnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(() =>
                {
                    animationsCompleted++;
                    if (animationsCompleted >= totalAnimations)
                        isAnimating = false;
                });

            LeanTween.size(UpgradeItemPanel, ItemPanelExpandedSize, AnimationDuration)
                .setEaseOutQuad()
                .setOnComplete(() =>
                {
                    animationsCompleted++;
                    if (animationsCompleted >= totalAnimations)
                        isAnimating = false;
                });
        }

        // Fallback to clear animation flag if no panels to animate
        if (totalAnimations == 0)
        {
            isAnimating = false;
        }
    }

    void Update()
    {
        if (isExpanded && !isAnimating && Input.GetKeyDown(KeyCode.Escape))
        {
            Collapse();
        }
    }
}
