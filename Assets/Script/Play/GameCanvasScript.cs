using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    public static GameCanvasScript Instance;

    public GameObject GenericPopupPrefab;
    public Transform GenericPopupUiParent;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPopup(string message)
    {
        var popup = Instantiate(GenericPopupPrefab, GenericPopupUiParent);
        var text = popup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        // Optionally force layout update (usually not needed unless immediate measurement required)
        //LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
    }
}
