using UnityEngine;

public class GenericPopupScript : MonoBehaviour
{
    private float _autoCloseTime;

    private void Awake()
    {
        _autoCloseTime = Time.realtimeSinceStartup + 60 * 5;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnClose();

        if (Time.realtimeSinceStartup > _autoCloseTime)
            OnClose();
    }

    public void OnClose()
    {
        Destroy(gameObject);
    }
}
