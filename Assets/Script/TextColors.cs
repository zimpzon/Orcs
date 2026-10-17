using UnityEngine;

// Rich-text colors used across UI text built in code (upgrade tooltips, popups). Change them here.
public static class TextColors
{
    public const string Green = "#8EBE4D";   // headers, requirement met
    public const string Orange = "#DF874A";  // requirement not met

    // Brighter versions for small body text (~10-12 px): anti-aliasing and the drop shadow make thin glyphs read much
    // darker than the hex value, so small text uses these. Green/Orange stay for big text (headers, titles).
    public const string GreenSmall = "#AADC64";
    public const string OrangeSmall = "#F5A05F";
    public const string Yellow = "#F4C430";  // random yellow
    
    public static readonly Color GreenColor = new(0.553f, 0.745f, 0.298f); // Green as a Color
}
