using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasScript : MonoBehaviour
{
    public static GameCanvasScript Instance;

    public GameObject GenericPopupPrefab;
    public Transform GenericPopupUiParent;
    // Steam item drop icons (Texture/ItemDrops, named SteamItemDefs.Def.IconName). Emptied in WebGL builds
    // (StripItemIconsForWebGL): no Steam there.
    public Sprite[] ItemDropIcons;

    public Sprite FindItemDropIcon(string iconName)
    {
        if (ItemDropIcons == null)
            return null;
        foreach (var s in ItemDropIcons)
            if (s != null && s.name == iconName)
                return s;
        return null;
    }

    private void Awake()
    {
        Instance = this;
    }

    const int MessageSortingOrder = 110; // above GlobalPopupManager's dialogs (100) and their dark overlay (99)

    // celebrate: reward effects (RewardPopupFx) for chest/mystery pickups.
    public void ShowPopup(string message, bool celebrate = false) => CreatePopup(message, celebrate);

    // Same, returning the popup so callers can add to it (ItemDropPopup).
    public GameObject CreatePopup(string message, bool celebrate = false)
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

        if (celebrate)
        {
            popup.AddComponent<RewardPopupFx>();
            AudioManager.Instance.PlayClipForReal(AudioManager.Instance.AudioData.Chime, minInterval: 0.3f, fixedPitch: true); // chest / mystery reward
        }
        else
        {
            AudioManager.Instance.PlayDialogShow();
        }

        // Optionally force layout update (usually not needed unless immediate measurement required)
        //LayoutRebuilder.ForceRebuildLayoutImmediate(popup.GetComponent<RectTransform>());
        return popup;
    }
}
