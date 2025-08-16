using UnityEngine;

public class SkinsPopupScript : MonoBehaviour
{
    public SkinsPopupScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
    }
}
