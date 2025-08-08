using System;
using System.Text;

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
        byte[] bytes = Convert.FromBase64String(encoded);
        string decoded = Encoding.UTF8.GetString(bytes);
        if (!decoded.StartsWith(Marker))
            throw new Exception("does not look a valid save game for this version");

        decoded = decoded.Substring(Marker.Length);
        return decoded;
    }
}
