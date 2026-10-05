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

    const int MessageSortingOrder = 110; // above GlobalPopupManager's dialogs (100) and their dark overlay (99)

    public void ShowPopup(string message)
    {
        var popup = Instantiate(GenericPopupPrefab, GenericPopupUiParent);
        var text = popup.GetComponentInChildren<TextMeshProUGUI>();
        text.text = message;

        // Messages can be raised from inside an open dialog (e.g. Settings > Import), which GlobalPopupManager draws
        // on its own canvas at sorting order 100 with a dark overlay below it. Put the message above that, or it
        // would open hidden behind the dialog. (Needs its own raycaster to stay clickable.)
        if (!popup.TryGetComponent<Canvas>(out var canvas))
            canvas = popup.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = MessageSortingOrder;
        if (!popup.TryGetComponent<GraphicRaycaster>(out _))
            popup.AddComponent<GraphicRaycaster>();

        // Optionally force layout update (usually not needed unless immediate measurement required)
        //LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
    }
}
