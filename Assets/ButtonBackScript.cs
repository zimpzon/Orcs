using UnityEngine;
using UnityEngine.UI;

public class ButtonBackScript : MonoBehaviour
{
    private Button _myButton;

    private void Awake()
    {
        _myButton = GetComponent<Button>();
    }

    void Update()
    {
        // Escape closes a message popup on top first (GenericPopupScript), not the dialog behind it.
        if (Input.GetKeyDown( KeyCode.Escape) && GenericPopupScript.OpenCount == 0)
        {
            _myButton.onClick.Invoke();
        }
    }
}
