using UnityEngine;

// Slowly turning god rays behind an item drop icon (ItemDropPopup), clipped to the icon's square. Unscaled time.
public class ItemIconRays : MonoBehaviour
{
    public float Speed = 15f; // degrees per second

    void Update()
    {
        float t = Time.unscaledTime;
        transform.localRotation = Quaternion.Euler(0, 0, -t * Speed);
        transform.localScale = Vector3.one * (1f + 0.05f * Mathf.Sin(t * 1.3f));
    }
}
