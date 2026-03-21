using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateTreasureMasks
{
    [MenuItem("Tools/Generate Treasure1 Masks")]
public static void Generate()
    {
        GenerateForMap("treasure1");
        GenerateForMap("treasure2");
        AssetDatabase.Refresh();
        Debug.Log("Treasure masks generated!");
    }

static void GenerateForMap(string mapName)
    {
        string folder = $"Assets/Images/treasure/{mapName}";
        int size = 512;
        int half = size / 2;
        int overlap = 2;

        var regions = new (int x, int y, int w, int h)[]
        {
            (0,              half - overlap, half + overlap, half + overlap), // mask1 左上
            (half - overlap, half - overlap, half + overlap, half + overlap), // mask2 右上
            (0,              0,              half + overlap, half + overlap), // mask3 左下
            (half - overlap, 0,              half + overlap, half + overlap), // mask4 右下
        };

        for (int i = 0; i < regions.Length; i++)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];
            for (int p = 0; p < pixels.Length; p++)
                pixels[p] = Color.clear;

            var r = regions[i];
            int xMax = Mathf.Min(r.x + r.w, size);
            int yMax = Mathf.Min(r.y + r.h, size);
            for (int py = r.y; py < yMax; py++)
                for (int px = r.x; px < xMax; px++)
                    pixels[py * size + px] = Color.black;

            tex.SetPixels(pixels);
            tex.Apply();

            string path = $"{folder}/{mapName}mask{i + 1}.png";
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }

}
