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
