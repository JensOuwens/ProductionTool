using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Stroke
{
    public Vector2 start;
    public Vector2 end;
    public int brushSize;
    public Color color;

    public Stroke(Vector2 s, Vector2 e, int size, Color c)
    {
        start = s;
        end = e;
        brushSize = size;
        color = c;
    }
}

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private RawImage drawImage;
    [SerializeField] private int totalPixelsX = 1024;
    [SerializeField] private int totalPixelsY = 512;
    [SerializeField] private int brushSize = 6;
    [SerializeField] private Color brushColor = Color.black;

    private Texture2D generatedTexture;
    private RectTransform rectTransform;

    private Vector2 lastPos;
    private bool hasLast = false;

    private List<Stroke> strokes = new List<Stroke>();

    void Start()
    {
        rectTransform = drawImage.rectTransform;

        generatedTexture = new Texture2D(totalPixelsX, totalPixelsY, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;

        ClearCanvas();
        drawImage.texture = generatedTexture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            DrawFromMouse();
        else
            hasLast = false;
    }

    void DrawFromMouse()
    {
        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPos))
            return;

        Vector2 size = rectTransform.rect.size;
        float x = (localPos.x + size.x * 0.5f) / size.x * totalPixelsX;
        float y = (localPos.y + size.y * 0.5f) / size.y * totalPixelsY;

        if (x < 0 || y < 0 || x >= totalPixelsX || y >= totalPixelsY) return;

        Vector2 curPos = new Vector2(x, y);

        Stroke stroke;
        if (!hasLast)
        {
            // Single point stroke
            stroke = new Stroke(curPos, curPos, brushSize, brushColor);
            hasLast = true;
        }
        else
        {
            stroke = new Stroke(lastPos, curPos, brushSize, brushColor);
        }

        strokes.Add(stroke);
        lastPos = curPos;

        // Draw only the new stroke
        DrawLinePixels(stroke.start, stroke.end, stroke.brushSize, stroke.color);
        generatedTexture.Apply();
    }

    void DrawLinePixels(Vector2 start, Vector2 end, int size, Color color)
    {
        int steps = Mathf.Max(1, (int)Vector2.Distance(start, end));
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(start, end, t);
            DrawCirclePixels(point, size, color);
        }
    }

    void DrawCirclePixels(Vector2 center, int size, Color color)
    {
        int cx = (int)center.x;
        int cy = (int)center.y;
        color.a = 1f;

        int sqrSize = size * size;
        for (int x = -size; x <= size; x++)
        {
            for (int y = -size; y <= size; y++)
            {
                if (x * x + y * y > sqrSize) continue;

                int px = cx + x;
                int py = cy + y;
                if (px >= 0 && px < totalPixelsX && py >= 0 && py < totalPixelsY)
                    generatedTexture.SetPixel(px, py, color);
            }
        }
    }

    public void ClearCanvas()
    {
        strokes.Clear();
        Color clearColor = Color.white;
        clearColor.a = 1f;

        Color[] fill = new Color[totalPixelsX * totalPixelsY];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = clearColor;

        generatedTexture.SetPixels(fill);
        generatedTexture.Apply();
    }
}
