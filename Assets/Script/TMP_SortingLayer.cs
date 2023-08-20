using UnityEngine;

[ExecuteInEditMode]  // This attribute allows the changes to be seen immediately in the editor
public class TMP_SortingLayer : MonoBehaviour
{
    public string SortingLayerName = "Default";
    public int SortingOrder = 0;

    void Awake()
    {
        SetSortingLayer();
    }

    void Update()
    {
        // This ensures that changes made in the editor are reflected immediately
        SetSortingLayer();
    }

    void SetSortingLayer()
    {
        MeshRenderer meshRenderer = this.GetComponent<MeshRenderer>();

        if (meshRenderer)
        {
            meshRenderer.sortingLayerName = SortingLayerName;
            meshRenderer.sortingOrder = SortingOrder;
        }
    }
}
