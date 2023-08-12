using System;
using UnityEngine;

public class G : MonoBehaviour
{
    public static G D;
    public Color UpgradePositiveColor;
    public Color UpgradeNegativeColor;
    public Color UpgradeNeutralColor;
    public Color DeflectedProjectileColor;
    public Color DefaultButtonBackgroundColor;
    public Color DisabledButtonBackgroundColor;
    [NonSerialized] public string UpgradePositiveColorHex;
    [NonSerialized] public string UpgradeNegativeColorHex;
    [NonSerialized] public string UpgradeNeutralColorHex;

    public PlayerScript PlayerScript;
    public Transform PlayerTrans;

    // Dynamic
    [NonSerialized] public float GameTime;
    [NonSerialized] public float GameDeltaTime;
    [NonSerialized] public Vector3 PlayerPos;

    string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        return $"<color=#{r:X2}{g:X2}{b:X2}{a:X2}>";
    }

    private void Awake()
    {
        D = this;

        UpgradePositiveColorHex = ColorToHex(UpgradePositiveColor);
        UpgradeNegativeColorHex = ColorToHex(UpgradeNegativeColor);
        UpgradeNeutralColorHex = ColorToHex(UpgradeNeutralColor);
    }

    private void Update()
    {
        D.PlayerPos = PlayerTrans.position;
        GameTime = GameManager.Instance.GameTime;
        GameDeltaTime = GameManager.Instance.GameDeltaTime;
    }
}
