using Assets.Script;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeedUI : MonoBehaviour {

    public TextMeshProUGUI DeedName;
    public TextMeshProUGUI DeedReq;
    public TextMeshProUGUI DeedDescription;
    public Button StartButton;
    public RawImage Checkmark;

    public void UpdateFromDeed(DeedData deed, Action<DeedData> onButtonGo)
    {
        bool isComplete = SaveGame.Members.GetCounter(deed.CompletionCounter) > 0;
        DeedName.text = GameEvents.WrapInColor(deed.Title, isComplete);
        DeedReq.text = string.Format(deed.Req, deed.BaseKillReq);
        DeedDescription.text = deed.Description;
        Checkmark.color = isComplete ? Color.green : new Color(0.2f, 0.1f, 0.1f);

        StartButton.onClick.RemoveAllListeners();
        StartButton.onClick.AddListener(() =>
        {
            onButtonGo(deed);
        });
    }
}
