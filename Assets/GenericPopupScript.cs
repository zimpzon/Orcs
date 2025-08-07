using UnityEngine;

public class GenericPopupScript : MonoBehaviour
{
    public void OnClose()
    {
        Destroy(gameObject);
    }
}
