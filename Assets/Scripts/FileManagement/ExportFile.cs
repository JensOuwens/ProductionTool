using UnityEngine;
using System.IO;
using SFB;

/// <summary>
/// add metadata for all the furthest points from the center, plus the size of every letter. how many letters and probably where every letter is
/// </summary>
public class ExportFile : MonoBehaviour
{
    const int GLYPH_SIZE = 512;
    const int SOURCE_W = 1024;
    const int SOURCE_H = 512;

    public void ExportFontAtlas()
    {
        string folder = StandaloneFileBrowser.OpenFolderPanel("Export Font Atlas", "", false)[0];
        if (string.IsNullOrEmpty(folder)) return;

        var ps = ProjectSettingsManager.Instance.currentProjectSettings;
        if (ps == null || ps.characters == null || ps.characters.Count == 0) return;

        int count = ps.characters.Count;
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)cols);

        Texture2D atlas = new Texture2D(
            cols * GLYPH_SIZE,
            rows * GLYPH_SIZE,
            TextureFormat.RGBA32,
            false
        );

        Clear(atlas);

        int index = 0;
        foreach (var ch in ps.characters)
        {
            Texture2D glyph = RenderCharacter(ch);

            int x = (index % cols) * GLYPH_SIZE;
            int y = (rows - 1 - index / cols) * GLYPH_SIZE;

            atlas.SetPixels(x, y, GLYPH_SIZE, GLYPH_SIZE, glyph.GetPixels());
            index++;
        }

        atlas.Apply();

        File.WriteAllBytes(
            Path.Combine(folder, ps.projectName + "_Atlas.png"),
            atlas.EncodeToPNG()
        );

        Debug.Log("Atlas exported");
    }

    Texture2D RenderCharacter(CharacterData ch)
    {
        Texture2D tex = new Texture2D(GLYPH_SIZE, GLYPH_SIZE, TextureFormat.RGBA32, false);
        Clear(tex);

        foreach (var s in ch.strokes)
        {
            DrawStroke(tex, s);
        }

        tex.Apply();
        return tex;
    }

    void DrawStroke(Texture2D tex, Stroke s)
    {
        Vector2 a = Normalize(s.GetStart());
        Vector2 b = Normalize(s.GetEnd());

        int steps = Mathf.CeilToInt(Vector2.Distance(a, b));
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 p = Vector2.Lerp(a, b, t);
            DrawCircle(tex, (int)p.x, (int)p.y, s.brushSize, s.GetColor());
        }
    }

    Vector2 Normalize(Vector2 p)
    {
        return new Vector2(
            p.x / SOURCE_W * GLYPH_SIZE,
            p.y / SOURCE_H * GLYPH_SIZE
        );
    }

    void DrawCircle(Texture2D tex, int cx, int cy, int r, Color col)
    {
        for (int x = -r; x <= r; x++)
        for (int y = -r; y <= r; y++)
        {
            if (x * x + y * y > r * r) continue;
            int px = cx + x;
            int py = cy + y;
            if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                tex.SetPixel(px, py, col);
        }
    }

    void Clear(Texture2D tex)
    {
        Color[] c = new Color[tex.width * tex.height];
        for (int i = 0; i < c.Length; i++) c[i] = Color.clear;
        tex.SetPixels(c);
    }
}
