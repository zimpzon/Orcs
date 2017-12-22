using UnityEngine;
using UnityEngine.UI;

public class TextBlinkScript : MonoBehaviour
{
    public float BlinkTime = 0.5f;

    public bool IsActive;

    float endTime_ = float.MaxValue;
    float nextToggle_;
    public Text Text;

    public void Clear()
    {
        Text.enabled = false;
        endTime_ = 0.0f;
        IsActive = false;
    }

    public void SetText(string text, float time)
    {
        IsActive = true;
        Text.enabled = true;
        Text.text = text;
        nextToggle_ = Time.realtimeSinceStartup + BlinkTime;
        endTime_ = Time.realtimeSinceStartup + time;
    }

    void Awake()
    {
        Text = GetComponent<Text>();
        Text.enabled = false;
        nextToggle_ = 0.0f;
	}
	
	void Update()
    {
        if (Time.realtimeSinceStartup > endTime_)
        {
            Text.enabled = false;
            IsActive = false;
            return;
        }

        if (Time.realtimeSinceStartup > nextToggle_)
        {
            if (BlinkTime != 0.0f)
                Text.enabled = Time.realtimeSinceStartup < endTime_ ? !Text.enabled : false;

            nextToggle_ = Time.realtimeSinceStartup + BlinkTime;
        }
    }
}
