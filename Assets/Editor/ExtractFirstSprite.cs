using UnityEditor;
using UnityEngine;
using System.IO;

public class ExtractFirstSprite : MonoBehaviour
{
    [MenuItem("Tools/Sprites/Extract First Sprite From Selected Textures")]
    private static void ExtractFirstSpriteFromSelected()
    {
        // Get all selected Texture2D assets in the Project window
        Object[] selectedObjects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.Assets);

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No textures selected! Please select multi-sprite textures in the Project window.");
            return;
        }

        string outputDir = Path.Combine(@"c:\IdleEarl\", "ExtractedSprites");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        int extractedCount = 0;

        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);

            // Load all sub-assets (including Sprite objects) associated with the texture
            Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            Sprite targetSprite = null;

            // Find the first actual Sprite sub-asset
            foreach (Object subAsset in subAssets)
            {
                if (subAsset is Sprite sprite)
                {
                    targetSprite = sprite;
                    break;
                }
            }

            if (targetSprite == null)
            {
                Debug.LogWarning($"No sub-sprites found in texture: {path}");
                continue;
            }

            // Read texture pixels for the sub-sprite rect
            Texture2D sourceTex = (Texture2D)obj;
            Rect rect = targetSprite.rect;

            // Ensure source texture is readable
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            bool originallyReadable = importer.isReadable;

            if (!originallyReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            // Extract sub-sprite pixels
            Color[] pixels = sourceTex.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
            newTex.SetPixels(pixels);
            newTex.Apply();

            // Save to PNG file
            byte[] bytes = newTex.EncodeToPNG();
            string newPath = Path.Combine(outputDir, $"{targetSprite.name}.png");
            File.WriteAllBytes(newPath, bytes);

            // Revert texture read/write setting if needed
            if (!originallyReadable)
            {
                importer.isReadable = false;
                importer.SaveAndReimport();
            }

            DestroyImmediate(newTex);
            extractedCount++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"Successfully extracted {extractedCount} sprite(s) to Assets/ExtractedSprites/");
    }
}
