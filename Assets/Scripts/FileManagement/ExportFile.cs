using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SFB;

public class ExportFile : MonoBehaviour
{
    const int GLYPH_SIZE = 512;
    const int SOURCE_W = 1024;
    const int SOURCE_H = 512;

    [System.Serializable]
    public class GlyphMeta
    {
        public int codepoint;
        public int x, y, w, h;
        public int xOffset, yOffset;
        public int xAdvance;
    }

    [System.Serializable]
    public class FontMeta
    {
        public int atlasWidth;
        public int atlasHeight;
        public int lineHeight;
        public List<GlyphMeta> glyphs = new();
    }

    public void ExportFontAtlas()
    {
        string folder = StandaloneFileBrowser.OpenFolderPanel("Export Font Atlas", "", false)[0];
        if (string.IsNullOrEmpty(folder)) return;

        var ps = ProjectSettingsManager.Instance.currentProjectSettings;
        if (ps == null || ps.characters == null || ps.characters.Count == 0) return;

        int count = ps.characters.Count;
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)cols);

        Texture2D atlas = new Texture2D(cols * GLYPH_SIZE, rows * GLYPH_SIZE, TextureFormat.RGBA32, false);

        Clear(atlas);

        FontMeta meta = new FontMeta
        {
            atlasWidth = atlas.width,
            atlasHeight = atlas.height,
            lineHeight = GLYPH_SIZE
        };

        int index = 0;
        foreach (var ch in ps.characters)
        {
            Texture2D glyph = RenderCharacter(ch);
            RectInt bounds = ComputeBounds(glyph);

            int x = (index % cols) * GLYPH_SIZE;
            int y = (rows - 1 - index / cols) * GLYPH_SIZE;

            atlas.SetPixels(x, y, GLYPH_SIZE, GLYPH_SIZE, glyph.GetPixels());

            GlyphMeta g = new GlyphMeta
            {
                codepoint = ch.codepoint,
                x = x + bounds.x,
                y = y + bounds.y,
                w = bounds.width,
                h = bounds.height,
                xOffset = bounds.x - GLYPH_SIZE / 2,
                yOffset = bounds.y - GLYPH_SIZE / 2,
                xAdvance = bounds.width + 10
            };

            meta.glyphs.Add(g);
            index++;
        }

        atlas.Apply();

        File.WriteAllBytes(Path.Combine(folder, ps.projectName + "_Atlas.png"), atlas.EncodeToPNG());

        string json = JsonUtility.ToJson(meta, true);
        File.WriteAllText(Path.Combine(folder, ps.projectName + "_Font.json"), json);

        Debug.Log("Atlas and metadata exported");
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
        Vector2 a = Normalize(s.Start);
        Vector2 b = Normalize(s.End);

        float dist = Vector2.Distance(a, b);
        float step = Mathf.Max(1f, s.brushSize * s.spacing);
        int count = Mathf.CeilToInt(dist / step);

        for (int i = 0; i <= count; i++)
        {
            float t = i / (float)count;
            Vector2 p = Vector2.Lerp(a, b, t);
            DrawBrushStamp(tex, p, s);
        }
    }

    void DrawBrushStamp(Texture2D tex, Vector2 pos, Stroke s)
    {
        int cx = (int)pos.x;
        int cy = (int)pos.y;
        int r = s.brushSize;

        for (int x = -r; x <= r; x++)
        for (int y = -r; y <= r; y++)
        {
            int px = cx + x;
            int py = cy + y;
            if (px < 0 || py < 0 || px >= tex.width || py >= tex.height)
                continue;

            float d = s.shape switch
            {
                BrushShape.Circle => Mathf.Sqrt(x * x + y * y) / r,
                BrushShape.Square => Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) / r,
                BrushShape.Diamond => (Mathf.Abs(x) + Mathf.Abs(y)) / r,
                BrushShape.Calligraphy => CalligraphyDistance(x, y, r, s),
                _ => 1f
            };

            if (d > 1f) continue;

            float falloff = Mathf.Pow(1f - d, s.hardness * 4f);
            float a = falloff * s.opacity;

            if (s.isEraser)
            {
                // Apply eraser: remove existing pixels
                Color dst = tex.GetPixel(px, py);
                if (dst.a > 0f)
                    tex.SetPixel(px, py, Color.clear);
            }
            else
            {
                Color dst = tex.GetPixel(px, py);
                Color outCol = Color.Lerp(dst, s.Color, a);
                tex.SetPixel(px, py, outCol);
            }
        }
    }

    float CalligraphyDistance(int x, int y, int r, Stroke s)
    {
        float rad = s.angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        float rx = (x * cos - y * sin) / r;
        float ry = (x * sin + y * cos) / (r * s.aspectRatio);

        return Mathf.Sqrt(rx * rx + ry * ry);
    }

    RectInt ComputeBounds(Texture2D tex)
    {
        int minX = tex.width, minY = tex.height;
        int maxX = 0, maxY = 0;
        bool found = false;

        var pixels = tex.GetPixels();
        for (int y = 0; y < tex.height; y++)
        for (int x = 0; x < tex.width; x++)
        {
            if (pixels[y * tex.width + x].a > 0f)
            {
                found = true;
                minX = Mathf.Min(minX, x);
                minY = Mathf.Min(minY, y);
                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);
            }
        }

        if (!found) return new RectInt(0, 0, 0, 0);

        return new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    Vector2 Normalize(Vector2 p)
    {
        return new Vector2(p.x / SOURCE_W * GLYPH_SIZE, p.y / SOURCE_H * GLYPH_SIZE);
    }

    void Clear(Texture2D tex)
    {
        Color[] c = new Color[tex.width * tex.height];
        for (int i = 0; i < c.Length; i++) c[i] = Color.clear;
        tex.SetPixels(c);
    }
}
