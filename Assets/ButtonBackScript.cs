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
        if (Input.GetKeyDown( KeyCode.Escape))
        {
            _myButton.onClick.Invoke();
        }
    }
}
