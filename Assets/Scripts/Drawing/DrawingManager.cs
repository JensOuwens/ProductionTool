using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using UnityEngine;
using System;


/// <summary>
/// TODO
/// once more brushes/settings are added, make sure to add updates
/// add drawing different shapes (square, circle, straight line) (different script?)
/// </summary>
public class Stroke
{
    public float startX;
    public float startY;

    public float endX;
    public float endY;

    public int brushSize;

    public float r;
    public float g;
    public float b;
    public float a;

    // XML NEED THIS
    public Stroke() {}

    public Stroke(Vector2 start, Vector2 end, int size, Color col)
    {
        startX = start.x;
        startY = start.y;

        endX = end.x;
        endY = end.y;

        brushSize = size;

        r = col.r;
        g = col.g;
        b = col.b;
        a = col.a;
    }

    public Vector2 GetStart() => new Vector2(startX, startY);
    public Vector2 GetEnd() => new Vector2(endX, endY);
    public Color GetColor() => new Color(r, g, b, a);
}

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private RawImage drawImage;
    [SerializeField] private int totalPixelsX = 1024;
    [SerializeField] private int totalPixelsY = 512;
    [SerializeField] private static int brushSize = 6;
    [SerializeField] private static Color brushColor = Color.black;

    private Texture2D generatedTexture;
    private RectTransform rectTransform;

    private Vector2 lastPos;
    private bool hasLast = false;

    // CURRENT CHARACTER DATA (IMPORTANT)
    private CharacterData currentCharacter;

    private bool isReady = false;

    void Start()
    {
        rectTransform = drawImage.rectTransform;

        generatedTexture = new Texture2D(totalPixelsX, totalPixelsY, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;

        ClearCanvasVisual();
        drawImage.texture = generatedTexture;

        isReady = true;
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
        if (currentCharacter == null)
            return;

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPos))
            return;

        Vector2 size = rectTransform.rect.size;
        float x = (localPos.x + size.x * 0.5f) / size.x * totalPixelsX;
        float y = (localPos.y + size.y * 0.5f) / size.y * totalPixelsY;

        if (x < 0 || y < 0 || x >= totalPixelsX || y >= totalPixelsY)
            return;

        Vector2 curPos = new Vector2(x, y);

        Stroke stroke;
        if (!hasLast)
        {
            stroke = new Stroke(curPos, curPos, brushSize, brushColor);
            hasLast = true;
        }
        else
        {
            stroke = new Stroke(lastPos, curPos, brushSize, brushColor);
        }

        currentCharacter.strokes.Add(stroke);
        lastPos = curPos;

        DrawLinePixels(stroke.GetStart(), stroke.GetEnd(), stroke.brushSize, stroke.GetColor());
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

    private void ClearCanvasVisual()
    {
        Color clearColor = Color.white;
        clearColor.a = 1f;

        Color[] fill = new Color[totalPixelsX * totalPixelsY];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = clearColor;

        generatedTexture.SetPixels(fill);
        generatedTexture.Apply();
    }

    // CLEAR CURRENT CHARACTER ONLY
    public void ClearCanvas()
    {
        if (currentCharacter != null)
            currentCharacter.strokes.Clear();

        ClearCanvasVisual();
    }

    // SWITCH ACTIVE CHARACTER
    public void SetCurrentCharacter(CharacterData cd)
    {
        currentCharacter = cd;

        if (isReady)
            RedrawFromStrokes();
    }

    public void RedrawFromStrokes()
    {
        ClearCanvasVisual();

        if (currentCharacter == null)
            return;

        foreach (Stroke s in currentCharacter.strokes)
            DrawLinePixels(s.GetStart(), s.GetEnd(), s.brushSize, s.GetColor());

        generatedTexture.Apply();
    }

    public static void UpdateBrushSize()
    {
        brushSize = (int)ProjectSettingsManager.Instance.currentProjectSettings.brushSize;
    }

    public static void UpdateBrushColor()
    {
        string colorString = ProjectSettingsManager.Instance.currentProjectSettings.brushColor;

        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(colorString, out parsedColor))
            brushColor = parsedColor;
        else
            brushColor = Color.black;
    }
}