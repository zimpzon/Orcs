using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class Beast
{
    public Image Icon;
    public ActorBase Actor;
    public BeastiaryBeastScript BeastiaryBeastScript;
}

public class BeastiaryPopupScript : MonoBehaviour
{
    public EnemyPrefabs EnemyPrefabs;
    public GameObject BeastPrefab;
    public GameObject Layout;
    public TextMeshProUGUI Hovertext;
    private List<Beast> _beasts = new List<Beast>();
    const float FinalBeastInset = 0f; // gap between the final enemy's icon and its frame edge (negative = past it)
    const float FinalBeastRaise = 3f; // moves it up: the sprite's feet sit low, the frame's bottom edge is thicker

    private void Awake()
    {
        int idx = 0;
        foreach (var enemy in EnemyPrefabs.Enemies)
        {
            var beast = Instantiate(BeastPrefab);
            var image = beast.transform.Find("Icon").GetComponent<Image>();
            image.sprite = enemy.GetComponentInChildren<SpriteRenderer>().sprite;
            beast.transform.SetParent(Layout.transform, worldPositionStays: false);
            var actor = enemy.GetComponent<ActorBase>();
            var beastScript = beast.GetComponent<BeastiaryBeastScript>();
            beastScript.Index = idx++;
            beastScript.ActorType = actor.ActorType;
            beastScript.Hovertext = Hovertext;
            _beasts.Add(new Beast { Actor = actor, Icon = image, BeastiaryBeastScript = beastScript });
        }

        // The final enemy (last in the list) fills its frame instead of the fixed 20x30 icon: stretched to the tile
        // minus a small inset, aspect kept, so it's as big as fits.
        if (_beasts.Count > 0)
        {
            var finalIcon = _beasts[^1].Icon;
            var rect = finalIcon.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(FinalBeastInset, FinalBeastInset + FinalBeastRaise);
            rect.offsetMax = new Vector2(-FinalBeastInset, -FinalBeastInset + FinalBeastRaise);
            finalIcon.preserveAspect = true;
            finalIcon.gameObject.AddComponent<UiVoidSparks>().Active = true; // also on the locked silhouette
        }

        CreateProgressText();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    private void Update()
    {
        int unlocked = 0;
        foreach(var beast in _beasts)
        {
            bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(beast.Actor.ActorType);
            beast.Icon.color = isUnlocked ? Color.white : Color.black;
            if (isUnlocked)
                unlocked++;
        }

        if (_progressText != null)
            _progressText.text = $"({unlocked}/{_beasts.Count})";
    }

    // "(unlocked/total)" under the "Bestiary" header. Everything below the header (bonus lines and the beast grid) is
    // moved down to make room, and the popup grows a bit taller (top-left pivot, so it grows downward; the hover text
    // and OK button are bottom-anchored and follow).
    TextMeshProUGUI _progressText;
    const float ProgressLineRoom = 14f;
    const float ExtraPopupHeight = 20f;

    void CreateProgressText()
    {
        _progressText = PopupHeaderProgress.Create(transform, "TextBestiary");
        if (_progressText == null)
            return;

        foreach (var childName in new[] { "TextBonusHeader", "TextBonus", "Layout" })
        {
            if (transform.Find(childName) is RectTransform rt)
                rt.anchoredPosition -= new Vector2(0, ProgressLineRoom);
        }

        ((RectTransform)transform).sizeDelta += new Vector2(0, ExtraPopupHeight);
    }
}
