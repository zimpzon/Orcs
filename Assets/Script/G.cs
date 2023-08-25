using System;
using UnityEngine;

public class G : MonoBehaviour
{
    public static G D;

    public static void Dbg(object key, object value)
        => GameManager.SetDebugOutput(key.ToString(), value);

    public int CosmeticArmorKills = 10000;
    public int CosmeticHeartsSaves = 100;

    public Color UpgradePositiveColor;
    public Color UpgradeNegativeColor;
    public Color UpgradeNeutralColor;
    public Color DeflectedProjectileColor;
    public Color DefaultButtonBackgroundColor;
    public Color DisabledButtonBackgroundColor;
    public Color BurstOfFrostColor;
    [NonSerialized] public string UpgradePositiveColorHex;
    [NonSerialized] public string UpgradeNegativeColorHex;
    [NonSerialized] public string UpgradeNeutralColorHex;

    public PlayerScript PlayerScript;
    public Transform PlayerTrans;

    // Dynamic
    [NonSerialized] public Vector3 PlayerPos;
    public Vector3 PlayerCenterOffset;

    public float GameTime => GameManager.Instance.GameTime;
    public float GameDeltaTime => GameManager.Instance.GameDeltaTime;

    public static string ColorTimeNeutral(TimeSpan time)
        => $"{D.UpgradeNeutralColorHex}{time.Minutes:00}</color><color=#f0f0f0>:</color>{D.UpgradeNeutralColorHex}{time.Seconds:00}</color>";

    public static string ColorTimePositive(TimeSpan time)
        => $"{D.UpgradePositiveColorHex}{time.Minutes:00}</color><color=#f0f0f0>:</color>{D.UpgradePositiveColorHex}{time.Seconds:00}</color>";

    string ColorToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255.0f);
        int g = Mathf.RoundToInt(color.g * 255.0f);
        int b = Mathf.RoundToInt(color.b * 255.0f);
        int a = Mathf.RoundToInt(color.a * 255.0f);
        return $"<color=#{r:X2}{g:X2}{b:X2}{a:X2}>";
    }

    public static bool MoveDownTap()
        => Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

    public static bool MoveUpTap()
        => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);

    public static bool MoveLeftTap()
        => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);

    public static bool MoveRightTap()
        => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

    public static bool SelectionTap()
        => Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);

    public static bool GetCheatKeyDown(KeyCode code)
    {
        return Input.GetKeyDown(code);
    }

    public static bool GetCheatKey(KeyCode code)
    {
        return Input.GetKey(code);
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
    }
}
