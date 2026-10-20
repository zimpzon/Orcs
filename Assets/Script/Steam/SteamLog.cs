using System;
using System.IO;
using UnityEngine;

// Everything Steam-related goes to steam.log next to the save (persistentDataPath, see SaveGame.SaveFilePath), fresh
// each run; the previous run is kept as steam-prev.log. Flushed per line so it can be read while the game runs.
public static class SteamLog
{
    static StreamWriter _writer;

    public static string FilePath => Path.Combine(Application.persistentDataPath, "steam.log");

    public static void Open()
    {
        if (_writer != null)
            return;
        try
        {
            string path = FilePath;
            string prev = Path.Combine(Application.persistentDataPath, "steam-prev.log");
            if (File.Exists(path))
            {
                if (File.Exists(prev))
                    File.Delete(prev);
                File.Move(path, prev);
            }
            _writer = new StreamWriter(path, false) { AutoFlush = true };
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Steam] could not open {FilePath}: {e.Message}");
        }
    }

    public static void Write(string message)
    {
        Debug.Log($"[Steam] {message}");
        try
        {
            _writer?.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {message}");
        }
        catch (Exception)
        {
            // Logging must never break the game.
        }
    }

    public static void Close()
    {
        try
        {
            _writer?.Dispose();
        }
        catch (Exception)
        {
        }
        _writer = null;
    }
}
