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
    public GameObject ButtonTouchFog; // "Touch fog": after 100%, until Scary Earl is revealed (SecretBeastTile)
    private List<Beast> _beasts = new List<Beast>();
    SecretBeastTile _secretTile;
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

        CreateSecretTile();
        CreateProgressText();
    }

    // One more tile after the beasts, shown after 100% only: fog, then Scary Earl (not counted as a beast).
    void CreateSecretTile()
    {
        var tile = Instantiate(BeastPrefab);
        tile.name = "SecretBeast";
        tile.transform.SetParent(Layout.transform, worldPositionStays: false);
        if (tile.TryGetComponent<BeastiaryBeastScript>(out var beastScript))
            Destroy(beastScript);
        _secretTile = tile.AddComponent<SecretBeastTile>();
        _secretTile.Init(tile.transform.Find("Icon").GetComponent<Image>(), Hovertext);
        tile.SetActive(SecretBeastTile.Unlocked);
    }

    public void OnTouchFog()
    {
        if (SaveGame.Members.FogTouched)
            return;
        SaveGame.Members.FogTouched = true;
        SaveGame.Save();
        AudioManager.Instance.PlayMenuClick();
        if (ButtonTouchFog != null)
            ButtonTouchFog.SetActive(false);
        _secretTile.Reveal();
        GameManager.Instance.RestartRound(); // the Scary Earl fight starts now, not after the current normal round
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

        bool secret = SecretBeastTile.Unlocked;
        if (_secretTile != null && _secretTile.gameObject.activeSelf != secret)
            _secretTile.gameObject.SetActive(secret);
        bool showFogButton = secret && !SaveGame.Members.FogTouched;
        if (ButtonTouchFog != null && ButtonTouchFog.activeSelf != showFogButton)
            ButtonTouchFog.SetActive(showFogButton);
    }

    // "(unlocked/total)" under the "Bestiary" header. The room for it is in the scene (the bonus lines and grid sit lower
    // and the popup is taller): it used to be made here at runtime, so the editor layout didn't match the game and
    // anything placed below the grid ended up somewhere else when playing.
    TextMeshProUGUI _progressText;

    void CreateProgressText()
    {
        _progressText = PopupHeaderProgress.Create(transform, "TextBestiary");
    }
}
