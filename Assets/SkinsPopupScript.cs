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
        GlobalPopupManager.Instance.AfterShowPopup(gameObject);
    }

    public void OnClose()
    {
        this.gameObject.SetActive(false);
        GlobalPopupManager.Instance.AfterHidePopup();
    }
}
