using System;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeChoiceScript : MonoBehaviour
{
    public Text TextHeader;
    public Text TextDescription;

    Choice choice_;
    bool wasSelected_;

    public Action<Choice> SelectionCallback;

    public void OnClick()
    {
        wasSelected_ = true;
        SelectionCallback(choice_);
    }

    private void OnEnable()
    {
        wasSelected_ = false;
        choice_ = UpgradeChoices.GetRandomChoice();
        TextHeader.text = choice_.Title;
        TextDescription.text = choice_.Description;
    }

    private void OnDisable()
    {
        UpgradeChoices.ReturnChoice(wasSelected_ ? choice_.NextLevel : choice_);
    }
}
