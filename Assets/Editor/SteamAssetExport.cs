using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Tools > Steam: writes Steam icons + preliminary JSON to c:\IdleEarlExtra. Fully deterministic (no randomness, fixed
// ordering, ids from enums), so running it again gives the same files.
//
// Achievements: one icon pair per Achieved value, made from the skin that achievement unlocks (found by asking
// SkinScript.GetUnlockStatus with only that achievement earned). Colored = achieved, black = not yet achieved.
// Item drops: every bestiary beast (EnemyPrefabs order = arena order, later = rarer) plus Earl and Scary Earl
// (legendary), in a rarity-colored frame. Item ids: Earl 1, Scary Earl 2, beasts 100 + ActorTypeEnum value.
public static class SteamAssetExport
{
    const string OutRoot = @"c:\IdleEarlExtra";
    const string EnemyPrefabsPath = "Assets/ScriptableObject/EnemyPrefabs.asset";
    const string SkinsPath = "Assets/ScriptableObject/SkinsAnimationList.asset";
    const int Size = 256;

    // Placeholder until the game has a Steam app and somewhere to host item icons (Steam needs icon URLs).
    const int AppId = 480;
    const string IconBaseUrl = "https://example.com/idle-earl/steam-items/";

    // ---------------- Achievements ----------------

    [MenuItem("Tools/Steam/Export Achievement Icons + JSON")]
    static void ExportAchievements()
    {
        string dir = PrepareDir("SteamAchievements");
        var skins = LoadSkins();
        var json = new StringBuilder();
        json.Append("{\n  \"note\": \"Preliminary. Steam achievements are entered on the Steamworks site (Stats & Achievements); API name must equal the Achieved enum name.\",\n");
        json.Append($"  \"appid\": {AppId},\n  \"achievements\": [\n");

        var all = (Achieved[])Enum.GetValues(typeof(Achieved));
        for (int i = 0; i < all.Length; ++i)
        {
            var a = all[i];
            var (skin, displayName, description) = FindSkinFor(a);
            var sprite = SkinSprite(skins, skin) ?? SkinSprite(skins, SkinAnimation.Default);

            string iconFile = $"{a}.png";
            string lockedFile = $"{a}_locked.png";
            var art = ReadSprite(sprite);
            WritePng(Path.Combine(dir, iconFile), AchievementIcon(art, locked: false));
            WritePng(Path.Combine(dir, lockedFile), AchievementIcon(art, locked: true));

            json.Append("    {\n");
            json.Append($"      \"apiName\": {Q(a.ToString())},\n");
            json.Append($"      \"enum\": {Q("Achieved." + a)},\n");
            json.Append($"      \"enumValue\": {(int)a},\n");
            json.Append($"      \"displayName\": {Q(displayName)},\n");
            json.Append($"      \"description\": {Q(description)},\n");
            json.Append($"      \"hidden\": false,\n");
            json.Append($"      \"unlocksSkin\": {Q(skin == SkinAnimation.NotSet ? "" : skin.ToString())},\n");
            json.Append($"      \"icon\": {Q(iconFile)},\n");
            json.Append($"      \"iconLocked\": {Q(lockedFile)}\n");
            json.Append(i < all.Length - 1 ? "    },\n" : "    }\n");
        }
        json.Append("  ]\n}\n");
        File.WriteAllText(Path.Combine(dir, "achievements.json"), json.ToString());
        Debug.Log($"Steam achievements: {all.Length} icon pairs + achievements.json written to {dir}");
    }

    // The skin this achievement unlocks: the one that turns unlocked with only this achievement in the save.
    static (SkinAnimation skin, string name, string description) FindSkinFor(Achieved a)
    {
        var saved = SaveGame.Members;
        try
        {
            SaveGame.Members = new SaveGameMembers();
            SaveGame.Members.Achieved.Add(a);
            foreach (SkinAnimation skin in Enum.GetValues(typeof(SkinAnimation)))
            {
                if (skin == SkinAnimation.NotSet || skin == SkinAnimation.Default)
                    continue;
                (bool unlocked, string hover) status;
                try { status = SkinScript.GetUnlockStatus(skin); }
                catch (ArgumentException) { continue; } // no skin rule (GhostEarl)
                if (!status.unlocked)
                    continue;
                int colon = status.hover.IndexOf(':');
                string name = colon > 0 ? status.hover.Substring(0, colon).Trim() : status.hover;
                string desc = colon > 0 ? status.hover.Substring(colon + 1).Trim() : "";
                return (skin, name, desc);
            }
        }
        finally
        {
            SaveGame.Members = saved;
        }
        return (SkinAnimation.NotSet, a.ToString(), "TODO: no skin for this achievement - write a description");
    }

    static Color32[] AchievementIcon(Art art, bool locked)
    {
        var canvas = new Color32[Size * Size];
        var body = locked ? new Color32(0, 0, 0, 255) : (Color32?)null;
        var outline = locked ? new Color32(96, 96, 96, 255) : new Color32(16, 16, 16, 255);
        DrawArt(canvas, art, fit: 212, outlineRadius: 5, outline, body);
        return canvas;
    }

    // ---------------- Item drops ----------------

    enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

    static readonly Color32[] RarityColors =
    {
        new Color32(157, 165, 176, 255), // Common: steel grey
        new Color32(79, 191, 90, 255),   // Uncommon: green
        new Color32(61, 139, 255, 255),  // Rare: blue
        new Color32(168, 85, 247, 255),  // Epic: purple
        new Color32(245, 184, 46, 255),  // Legendary: gold
    };

    // Drop weight per item of that rarity (relative). Legendary = Earl and Scary Earl only.
    static readonly int[] RarityWeights = { 100, 35, 12, 4, 1 };

    public const int PlaytimeGeneratorId = 1000; // same as SteamItemDefs.PlaytimeGenerator

    class ItemDef
    {
        public int Id;
        public string Name;
        public string Description;
        public Rarity Rarity;
        public Sprite Sprite;
        public string File;
    }

    [MenuItem("Tools/Steam/Export Item Drop Icons + itemdefs JSON")]
    static void ExportItems()
    {
        string dir = PrepareDir("SteamItems");
        var items = BuildItemList();

        foreach (var item in items)
            WritePng(Path.Combine(dir, item.File), ItemIcon(ReadSprite(item.Sprite), item.Rarity));

        File.WriteAllText(Path.Combine(dir, "itemdefs.json"), ItemDefsJson(items));
        Debug.Log($"Steam items: {items.Count} icons + itemdefs.json written to {dir}");
    }

    static List<ItemDef> BuildItemList()
    {
        var skins = LoadSkins();
        var items = new List<ItemDef>
        {
            new ItemDef { Id = 1, Name = "Earl", Description = "Good ol' Earl. The hero himself - the rarest find of all.",
                Rarity = Rarity.Legendary, Sprite = SkinSprite(skins, SkinAnimation.Default) },
            new ItemDef { Id = 2, Name = "Scary Earl", Description = "What hides behind the fog. Almost nobody finds this one.",
                Rarity = Rarity.Legendary, Sprite = SkinSprite(skins, SkinAnimation.ScaryEarl) },
        };

        var enemies = AssetDatabase.LoadAssetAtPath<EnemyPrefabs>(EnemyPrefabsPath);
        if (enemies == null)
            throw new InvalidOperationException($"Not found: {EnemyPrefabsPath}");

        // Beasts in arena order; split into rarity bands: first 40% common, 30% uncommon, 20% rare, last 10% epic.
        var beasts = new List<(ActorTypeEnum type, Sprite sprite)>();
        var seen = new HashSet<ActorTypeEnum>();
        foreach (var prefab in enemies.Enemies)
        {
            if (prefab == null)
                continue;
            var actor = prefab.GetComponentInChildren<ActorBase>(true);
            var renderer = prefab.GetComponentInChildren<SpriteRenderer>(true);
            if (actor == null || renderer == null || renderer.sprite == null || !seen.Add(actor.ActorType))
                continue;
            beasts.Add((actor.ActorType, renderer.sprite));
        }

        for (int i = 0; i < beasts.Count; ++i)
        {
            float t = (i + 0.5f) / beasts.Count;
            var rarity = t < 0.4f ? Rarity.Common : t < 0.7f ? Rarity.Uncommon : t < 0.9f ? Rarity.Rare : Rarity.Epic;
            var (type, sprite) = beasts[i];
            string name = ActorBase.Names.TryGetValue(type, out var n) ? n : type.ToString();
            items.Add(new ItemDef
            {
                Id = 100 + (int)type,
                Name = name,
                Description = $"{name}, a beast from the arena.",
                Rarity = rarity,
                Sprite = sprite,
            });
        }

        foreach (var item in items)
            item.File = $"item_{item.Id:000}_{FileSafe(item.Name)}.png";
        return items;
    }

    static string ItemDefsJson(List<ItemDef> items)
    {
        var sb = new StringBuilder();
        sb.Append("{\n");
        sb.Append($"  \"appid\": {AppId},\n");
        sb.Append("  \"items\": [\n");
        foreach (var item in items)
        {
            var c = RarityColors[(int)item.Rarity];
            string rarity = item.Rarity.ToString().ToLowerInvariant();
            sb.Append("    {\n");
            sb.Append($"      \"itemdefid\": {item.Id},\n");
            sb.Append("      \"type\": \"item\",\n");
            sb.Append($"      \"name\": {Q(item.Name)},\n");
            sb.Append($"      \"description\": {Q(item.Description)},\n");
            sb.Append($"      \"display_type\": {Q(item.Rarity + " Beast")},\n");
            sb.Append($"      \"icon_url\": {Q(IconBaseUrl + item.File)},\n");
            sb.Append($"      \"icon_url_large\": {Q(IconBaseUrl + item.File)},\n");
            sb.Append($"      \"name_color\": \"{c.r:X2}{c.g:X2}{c.b:X2}\",\n");
            sb.Append("      \"background_color\": \"1B1A1E\",\n");
            sb.Append($"      \"tags\": \"rarity:{rarity}\",\n");
            sb.Append("      \"tradable\": true,\n");
            sb.Append("      \"marketable\": true\n");
            sb.Append("    },\n");
        }

        // The drop table: Steam grants one of these per drop_interval minutes of play, weighted.
        string bundle = string.Join(";", items.Select(i => $"{i.Id}x{RarityWeights[(int)i.Rarity]}"));
        sb.Append("    {\n");
        sb.Append($"      \"itemdefid\": {PlaytimeGeneratorId},\n");
        sb.Append("      \"type\": \"playtimegenerator\",\n");
        sb.Append("      \"name\": \"Beast drops\",\n");
        sb.Append($"      \"bundle\": {Q(bundle)},\n");
        sb.Append("      \"drop_interval\": 30,\n");
        sb.Append("      \"use_drop_window\": true,\n");
        sb.Append("      \"drop_window\": 1440,\n");
        sb.Append("      \"drop_max_per_window\": 6,\n");
        sb.Append("      \"use_drop_limit\": false\n");
        sb.Append("    }\n");
        sb.Append("  ]\n}\n");
        return sb.ToString();
    }

    static Color32[] ItemIcon(Art art, Rarity rarity)
    {
        var canvas = new Color32[Size * Size];
        var rc = RarityColors[(int)rarity];
        const int Frame = 14;
        const float Center = (Size - 1) * 0.5f;
        const float GlowRadius = Size * 0.5f - Frame;
        int glowAlpha = rarity switch { Rarity.Common => 34, Rarity.Uncommon => 44, Rarity.Rare => 54, Rarity.Epic => 66, _ => 80 };

        for (int y = 0; y < Size; ++y)
        for (int x = 0; x < Size; ++x)
        {
            int i = y * Size + x;
            int edge = Math.Min(Math.Min(x, y), Math.Min(Size - 1 - x, Size - 1 - y));

            if (edge < Frame)
            {
                // Bevelled frame all the way into the corners: dark rim, highlight, body (lighter at the top), dark inner line.
                float shade = edge < 2 ? 0.45f
                    : edge < 5 ? 1.35f
                    : edge >= Frame - 3 ? 0.4f
                    : Mathf.Lerp(0.78f, 1.15f, y / (float)(Size - 1));
                canvas[i] = Shade(rc, shade, 255);
                continue;
            }

            // Inside: transparent, with a soft glow of the rarity color behind the beast.
            float dx = x - Center, dy = y - Center;
            float r = Mathf.Sqrt(dx * dx + dy * dy) / GlowRadius;
            float glow = Mathf.Clamp01(1f - r);
            float a = glowAlpha * glow * glow;
            if (rarity == Rarity.Legendary)
            {
                // Twelve soft rays.
                float angle = Mathf.Atan2(dy, dx);
                float ray = Mathf.Pow(Mathf.Abs(Mathf.Cos(angle * 6f)), 10f);
                a += 70f * ray * Mathf.Clamp01(1f - r) * Mathf.Clamp01(r * 3f);
            }
            if (a >= 1f)
                canvas[i] = Shade(rc, 1.1f, (byte)Mathf.Min(255f, a));
        }

        DrawCornerGems(canvas, rc, Frame);
        DrawArt(canvas, art, fit: 172, outlineRadius: 5, new Color32(16, 16, 16, 255), null);
        return canvas;
    }

    // Diamond gems on the four corners, over the frame.
    static void DrawCornerGems(Color32[] canvas, Color32 rc, int frame)
    {
        const int R = 22;
        int c0 = frame / 2, c1 = Size - 1 - frame / 2;
        foreach (var (cx, cy) in new[] { (c0, c0), (c1, c0), (c0, c1), (c1, c1) })
        {
            for (int y = cy - R; y <= cy + R; ++y)
            for (int x = cx - R; x <= cx + R; ++x)
            {
                if (x < 0 || y < 0 || x >= Size || y >= Size)
                    continue;
                int d = Math.Abs(x - cx) + Math.Abs(y - cy);
                if (d > R)
                    continue;
                Color32 col;
                if (d > R - 3)
                    col = Shade(rc, 0.35f, 255); // dark edge
                else if (y - cy > 0 && x - cx < 0 && d < R / 2)
                    col = Shade(rc, 1.7f, 255);  // top-left facet: highlight
                else if (y - cy > 0)
                    col = Shade(rc, 1.3f, 255);
                else
                    col = Shade(rc, 0.85f, 255);
                canvas[y * Size + x] = col;
            }
        }
    }

    // ---------------- Pixel helpers ----------------

    struct Art
    {
        public Color32[] Pixels;
        public int W, H;
    }

    // Reads a sprite's pixels through a RenderTexture, so the texture needn't be readable or uncompressed (no
    // import settings are touched). Trimmed to the opaque pixels.
    static Art ReadSprite(Sprite sprite)
    {
        if (sprite == null)
            throw new InvalidOperationException("Missing sprite");
        var tex = sprite.texture;
        var rect = sprite.rect;
        int w = Mathf.RoundToInt(rect.width), h = Mathf.RoundToInt(rect.height);

        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        rt.filterMode = FilterMode.Point;
        var oldFilter = tex.filterMode;
        tex.filterMode = FilterMode.Point;
        var scale = new Vector2(rect.width / tex.width, rect.height / tex.height);
        var offset = new Vector2(rect.x / tex.width, rect.y / tex.height);
        Graphics.Blit(tex, rt, scale, offset);
        tex.filterMode = oldFilter;

        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var read = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
        read.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        read.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        var px = read.GetPixels32();
        UnityEngine.Object.DestroyImmediate(read);

        int minX = w, minY = h, maxX = -1, maxY = -1;
        for (int y = 0; y < h; ++y)
        for (int x = 0; x < w; ++x)
        {
            if (px[y * w + x].a < 8)
                continue;
            minX = Math.Min(minX, x); maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y); maxY = Math.Max(maxY, y);
        }
        if (maxX < 0)
            return new Art { Pixels = px, W = w, H = h };

        int tw = maxX - minX + 1, th = maxY - minY + 1;
        var trimmed = new Color32[tw * th];
        for (int y = 0; y < th; ++y)
        for (int x = 0; x < tw; ++x)
            trimmed[y * tw + x] = px[(y + minY) * w + x + minX];
        return new Art { Pixels = trimmed, W = tw, H = th };
    }

    // Art scaled up by a whole number (crisp pixels) to fit `fit` px, centered, with an outline around it.
    // body != null: draw the silhouette in that color instead (locked achievement icon).
    static void DrawArt(Color32[] canvas, Art art, int fit, int outlineRadius, Color32 outline, Color32? body)
    {
        int scale = Math.Max(1, fit / Math.Max(art.W, art.H));
        int w = art.W * scale, h = art.H * scale;
        int ox = (Size - w) / 2, oy = (Size - h) / 2;

        var layer = new Color32[Size * Size];
        var solid = new bool[Size * Size];
        for (int y = 0; y < h; ++y)
        for (int x = 0; x < w; ++x)
        {
            int cx = ox + x, cy = oy + y;
            if (cx < 0 || cy < 0 || cx >= Size || cy >= Size)
                continue;
            var p = art.Pixels[(y / scale) * art.W + x / scale];
            if (p.a < 8)
                continue;
            int i = cy * Size + cx;
            layer[i] = body.HasValue ? new Color32(body.Value.r, body.Value.g, body.Value.b, p.a) : p;
            solid[i] = true;
        }

        // Outline: round dilation of the silhouette.
        int r2 = outlineRadius * outlineRadius;
        var outlined = new bool[Size * Size];
        for (int y = 0; y < Size; ++y)
        for (int x = 0; x < Size; ++x)
        {
            if (!solid[y * Size + x])
                continue;
            for (int dy = -outlineRadius; dy <= outlineRadius; ++dy)
            for (int dx = -outlineRadius; dx <= outlineRadius; ++dx)
            {
                if (dx * dx + dy * dy > r2)
                    continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && ny >= 0 && nx < Size && ny < Size)
                    outlined[ny * Size + nx] = true;
            }
        }

        for (int i = 0; i < canvas.Length; ++i)
        {
            if (outlined[i] && !solid[i])
                canvas[i] = Over(canvas[i], outline);
            if (solid[i])
                canvas[i] = Over(Over(canvas[i], outline), layer[i]);
        }
    }

    static Color32 Over(Color32 dst, Color32 src)
    {
        float sa = src.a / 255f, da = dst.a / 255f;
        float oa = sa + da * (1f - sa);
        if (oa <= 0f)
            return new Color32(0, 0, 0, 0);
        byte Mix(byte s, byte d) => (byte)Mathf.Clamp(Mathf.RoundToInt((s * sa + d * da * (1f - sa)) / oa), 0, 255);
        return new Color32(Mix(src.r, dst.r), Mix(src.g, dst.g), Mix(src.b, dst.b), (byte)Mathf.RoundToInt(oa * 255f));
    }

    static Color32 Shade(Color32 c, float f, byte a) =>
        new Color32((byte)Mathf.Clamp(Mathf.RoundToInt(c.r * f), 0, 255),
                    (byte)Mathf.Clamp(Mathf.RoundToInt(c.g * f), 0, 255),
                    (byte)Mathf.Clamp(Mathf.RoundToInt(c.b * f), 0, 255), a);

    static void WritePng(string path, Color32[] pixels)
    {
        var tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false, false);
        tex.SetPixels32(pixels);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
    }

    // ---------------- Misc ----------------

    static SkinsAnimationList LoadSkins()
    {
        var skins = AssetDatabase.LoadAssetAtPath<SkinsAnimationList>(SkinsPath);
        if (skins == null)
            throw new InvalidOperationException($"Not found: {SkinsPath}");
        return skins;
    }

    static Sprite SkinSprite(SkinsAnimationList skins, SkinAnimation skin)
    {
        var anim = skins.SkinAnimations.FirstOrDefault(s => s.Animation == skin);
        return anim?.IdleSprites != null && anim.IdleSprites.Length > 0 ? anim.IdleSprites[0] : null;
    }

    // Fresh folder each run: old PNGs (e.g. a renamed item) don't linger.
    static string PrepareDir(string name)
    {
        string dir = Path.Combine(OutRoot, name);
        Directory.CreateDirectory(dir);
        foreach (var f in Directory.GetFiles(dir, "*.png"))
            File.Delete(f);
        return dir;
    }

    static string FileSafe(string s) => new string(s.Where(char.IsLetterOrDigit).ToArray());

    static string Q(string s)
    {
        var sb = new StringBuilder("\"");
        foreach (char c in s ?? "")
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.Append('"').ToString();
    }
}
