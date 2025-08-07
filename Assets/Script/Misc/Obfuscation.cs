using System;
using System.Text;
using UnityEngine;

public static class Obfuscation
{
    const string Marker = "idle-earl-save-game-v1";

    public static string EncodeForSave(string json)
    {
        json = Marker + json;
        byte[] bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes);
    }

    public static string Decode(string encoded)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(encoded);
            string decoded = Encoding.UTF8.GetString(bytes);
            if (!decoded.StartsWith(Marker))
                throw new Exception("does not look a valid save game for this version");

            decoded = decoded.Substring(Marker.Length);
            return decoded;
        }
        catch (Exception e)
        {

            Debug.LogError("Failed to decode save data: " + e.Message);
            GameCanvasScript.Instance.ShowPopup("Could not import savegame, error: " + e.Message);
            return ""; // Return empty string if decoding fails
        }
    }
}
