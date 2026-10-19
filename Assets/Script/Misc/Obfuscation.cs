using System;
using System.Text;

// Save game encoding (save files, WebGL storage via SaveGame, export/import dialog).
// v2: base64 of [magic "IE2"][JSON bytes XOR-ed with a pseudo-random stream][4-byte checksum of JSON + secret]. Not
// encryption - it only means editing a save takes reverse engineering the game instead of a base64 decoder and a text
// editor; an edited save fails the checksum and is rejected.
// Older formats are not supported.
public static class Obfuscation
{
    static readonly byte[] MagicV2 = { (byte)'I', (byte)'E', (byte)'2' };
    const uint StreamSeed = 0x5EA11E57u;
    const string ChecksumSalt = "earl-knows-what-you-did:7f3a";

    public static string EncodeForSave(string json)
    {
        byte[] data = Encoding.UTF8.GetBytes(json);
        uint checksum = Checksum(data);
        Scramble(data);

        var bytes = new byte[MagicV2.Length + data.Length + 4];
        Buffer.BlockCopy(MagicV2, 0, bytes, 0, MagicV2.Length);
        Buffer.BlockCopy(data, 0, bytes, MagicV2.Length, data.Length);
        int c = MagicV2.Length + data.Length;
        bytes[c] = (byte)checksum;
        bytes[c + 1] = (byte)(checksum >> 8);
        bytes[c + 2] = (byte)(checksum >> 16);
        bytes[c + 3] = (byte)(checksum >> 24);
        return Convert.ToBase64String(bytes);
    }

    public static string Decode(string encoded)
    {
        byte[] bytes = Convert.FromBase64String(encoded.Trim());

        if (bytes.Length >= MagicV2.Length + 4 && bytes[0] == MagicV2[0] && bytes[1] == MagicV2[1] && bytes[2] == MagicV2[2])
        {
            int len = bytes.Length - MagicV2.Length - 4;
            var data = new byte[len];
            Buffer.BlockCopy(bytes, MagicV2.Length, data, 0, len);
            Scramble(data); // XOR again = unscramble
            int c = MagicV2.Length + len;
            uint stored = bytes[c] | (uint)bytes[c + 1] << 8 | (uint)bytes[c + 2] << 16 | (uint)bytes[c + 3] << 24;
            if (stored != Checksum(data))
                throw new Exception("save game is damaged or was edited");
            return Encoding.UTF8.GetString(data);
        }

        throw new Exception("does not look a valid save game for this version");
    }

    // XOR with an xorshift32 byte stream from a fixed seed (same stream both ways).
    static void Scramble(byte[] data)
    {
        uint x = StreamSeed;
        for (int i = 0; i < data.Length; ++i)
        {
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            data[i] ^= (byte)(x >> 11);
        }
    }

    // FNV-1a over the salt and the JSON bytes.
    static uint Checksum(byte[] data)
    {
        uint h = 2166136261u;
        foreach (char ch in ChecksumSalt)
        {
            h ^= (byte)ch;
            h *= 16777619u;
        }
        foreach (byte b in data)
        {
            h ^= b;
            h *= 16777619u;
        }
        return h;
    }
}
