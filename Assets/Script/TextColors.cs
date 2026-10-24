using UnityEngine;

// Rich-text colors used across UI text built in code (upgrade tooltips, popups). Change them here.
public static class TextColors
{
    public const string Green = "#8EBE4D";   // headers, requirement met
    public const string Orange = "#DF874A";  // requirement not met

    // Brighter versions for small body text (~10-12 px): anti-aliasing and the drop shadow make thin glyphs read much
    // darker than the hex value, so small text uses these. Green/Orange stay for big text (headers, titles).
    // (currently small use same as default, while smallest uses brighter green/orange)
    public const string GreenSmall = "#8EBE4D";
    public const string OrangeSmall = "#DF874A";

    public const string GreenSmallest = "#AADC64";
    public const string OrangeSmallest = "#F5A05F";

    public const string Yellow = "#F4C430";
    public const string Gold = "#FFD54A";

    public const string Grey = "#C8C8C8";
    public const string GreyDark = "#9A9A9A";
    public const string GreyLight = "#F0F0F0";
}
