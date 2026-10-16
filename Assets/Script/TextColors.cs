using UnityEngine;

// Rich-text colors used across UI text built in code (upgrade tooltips, popups). Change them here.
public static class TextColors
{
    public const string Green = "#8ECA3F";   // headers, requirement met
    public const string Orange = "#EE873F";  // requirement not met
    public const string GreenBright = "#8ECA3F"; // brighter green for lighter panels (rebirth dialog)
    public static readonly Color GreenColor = new(0.553f, 0.745f, 0.298f); // Green as a Color
}
