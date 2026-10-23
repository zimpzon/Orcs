using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class Beast
{
    public Image Icon;
    public ActorBase Actor;
    public BeastiaryBeastScript BeastiaryBeastScript;
    // Slaying goal reached (BeastGoals): gold tile + crown badge.
    public Image Tile;
    public Color TileColor;
    public GameObject Badge;
    // While decimating (after Scary Earl, before the goal): tiny "10/50" over the portrait.
    public TextMeshProUGUI Progress;
    public long ShownKills = -1;
}

public class BeastiaryPopupScript : MonoBehaviour
{
    public EnemyPrefabs EnemyPrefabs;
    public GameObject BeastPrefab;
    public GameObject Layout;
    public TextMeshProUGUI Hovertext;
    public Sprite MasteredBadgeSprite; // small crown in the corner of a mastered beast's tile (BeastGoals)
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
            var tile = beast.GetComponent<Image>();
            _beasts.Add(new Beast
            {
                Actor = actor, Icon = image, BeastiaryBeastScript = beastScript,
                Tile = tile, TileColor = tile != null ? tile.color : Color.white, Badge = AddMasteredBadge(beast.transform),
                Progress = AddProgressText(beast.transform),
            });
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
        if (SaveGame.Members.ScaryEarlDefeated)
            SaveGame.Members.ViewedBestiaryAfterScaryEarl = true; // seen the decimation goals: stops the button pulse
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }

    // Slaying goal reached (BeastGoals): the tile turns warm gold and a small crown sits in its top-right corner.
    static readonly Color MasteredTileColor = new Color32(0xB8, 0x92, 0x3E, 255);
    const float BadgeSize = 12f;

    // Tiny gold decimation progress over the portrait's head: at the top of the tile, a little below the frame edge.
    const float ProgressFontSize = 6f;
    static readonly Color ProgressColor = new Color32(0xFF, 0xD5, 0x4A, 230); // gold
    const float ProgressTopGap = 3f; // below the tile's top edge

    TextMeshProUGUI AddProgressText(Transform tile)
    {
        if (Hovertext == null)
            return null;
        var go = new GameObject("DecimateProgress", typeof(RectTransform));
        go.transform.SetParent(tile, false);
        var text = go.AddComponent<TextMeshProUGUI>();
        text.font = Hovertext.font;
        text.fontSharedMaterial = Hovertext.fontSharedMaterial;
        text.fontSize = ProgressFontSize;
        text.color = ProgressColor;
        text.alignment = TextAlignmentOptions.Top;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.raycastTarget = false;
        var rt = text.rectTransform;
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -ProgressTopGap);
        rt.sizeDelta = new Vector2(0f, ProgressFontSize + 2f);
        go.SetActive(false);
        return text;
    }

    static void UpdateProgressText(Beast beast, bool show)
    {
        if (beast.Progress == null)
            return;
        if (beast.Progress.gameObject.activeSelf != show)
            beast.Progress.gameObject.SetActive(show);
        if (!show)
            return;
        long kills = SaveGame.Members.GetBeastKills(beast.Actor.ActorType);
        if (kills == beast.ShownKills)
            return;
        beast.ShownKills = kills;
        beast.Progress.text = $"{kills}/{BeastGoals.Goal}";
    }

    GameObject AddMasteredBadge(Transform tile)
    {
        if (MasteredBadgeSprite == null)
            return null;
        var go = new GameObject("MasteredBadge", typeof(RectTransform));
        go.transform.SetParent(tile, false);
        var image = go.AddComponent<Image>();
        image.sprite = MasteredBadgeSprite;
        image.preserveAspect = true;
        image.raycastTarget = false;
        var rt = image.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(-3f, -3f); // straddling the corner a little
        rt.sizeDelta = new Vector2(BadgeSize, BadgeSize);
        rt.localRotation = Quaternion.Euler(0, 0, -15f);
        go.SetActive(false);
        return go;
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
            bool mastered = isUnlocked && BeastGoals.IsMastered(beast.Actor.ActorType);
            if (beast.Badge != null && beast.Badge.activeSelf != mastered)
                beast.Badge.SetActive(mastered);
            if (beast.Tile != null)
                beast.Tile.color = mastered ? MasteredTileColor : beast.TileColor;
            UpdateProgressText(beast, isUnlocked && BeastGoals.Active && !mastered);
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
