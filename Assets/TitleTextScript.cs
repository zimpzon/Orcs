using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class TitleTextScript : MonoBehaviour
{
    public static TitleTextScript Instance;

    public TextMeshProUGUI Text;

    [NonSerialized] public string EarlName = "Good ol' Earl";
    [NonSerialized] public string Link = "nemesis of";

    public void SetName(string name)
    {
        EarlName = name;
        UpdateTitle();
    }

    public void UpdateTitle()
    {
        ActorTypeEnum maxBeastSeen = SaveGame.Members.BeastsSeen.Max();
        if (!ActorBase.Names.TryGetValue(maxBeastSeen, out string beastName))
            beastName = "Nothing";
        Text.text = $"{EarlName}, {Link} {beastName}";
    }

    private void Awake()
    {
        Instance = this;
        UpdateTitle();
    }
}
