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
    public TextMeshProUGUI TextBonus;
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
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        int incomeBonus = 0;
        int idx = 0;
        foreach(var beast in _beasts)
        {
            bool isUnlocked = SaveGame.Members.BeastsSeen.Contains(beast.Actor.ActorType);
            beast.Icon.color = isUnlocked ? Color.white : Color.black;
            incomeBonus += isUnlocked ? idx + 1 : 0;
            idx++;
        }
        TextBonus.text = $"Bonus: +{incomeBonus}% passive income";

        PlayerUpgrades.Data.PassiveIncomeBestiaryBonuses = incomeBonus * 0.01f;
    }
}
