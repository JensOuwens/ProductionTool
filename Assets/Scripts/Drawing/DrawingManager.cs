using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private RawImage drawImage;
    [SerializeField] private int totalPixelsX = 1024;
    [SerializeField] private int totalPixelsY = 512;

    public static int brushSize = 6;
    public static Color brushColor = Color.black;

    public static ToolType currentTool = ToolType.Brush;
    public static BrushShape brushShape = BrushShape.Circle;

    public static float opacity = 1f;
    public static float hardness = 1f;
    public static float spacing = 0.25f;

    public static float calligraphyAngle = 45f;
    public static float calligraphyAspect = 0.3f;

    private Texture2D generatedTexture;
    private RectTransform rectTransform;

    private Vector2 lastPos;
    private Vector2 strokeStart;
    private bool hasLast;

    private CharacterData currentCharacter;
    private bool isReady;
    
    private BrushCursor brushCursor;


    void Start()
    {
        rectTransform = drawImage.rectTransform;

        generatedTexture = new Texture2D(totalPixelsX, totalPixelsY, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;

        ClearCanvasVisual();
        drawImage.texture = generatedTexture;
        
        brushCursor = FindObjectOfType<BrushCursor>();

        isReady = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            hasLast = false;

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

        // Fill tool
        if (currentTool == ToolType.Fill)
        {
            Color target = generatedTexture.GetPixel((int)x, (int)y);
            FloodFill((int)x, (int)y, target, brushColor);
            generatedTexture.Apply();
            return;
        }

        if (!hasLast)
        {
            strokeStart = curPos;
            lastPos = curPos;
            hasLast = true;
            return;
        }

        Vector2 endPos = curPos;

        // Straight line constraint with middle mouse button
        if (Input.GetMouseButton(2))
        {
            Vector2 d = curPos - strokeStart;
            endPos = Mathf.Abs(d.x) > Mathf.Abs(d.y)
                ? new Vector2(curPos.x, strokeStart.y)
                : new Vector2(strokeStart.x, curPos.y);
        }

        bool isEraser = currentTool == ToolType.Eraser;
        Color col = isEraser ? Color.clear : brushColor;
        col.a = opacity;

        Stroke stroke = new Stroke(
            lastPos,
            endPos,
            brushSize,
            col,
            opacity,
            hardness,
            spacing,
            brushShape,
            isEraser,
            calligraphyAngle,
            calligraphyAspect
        );

        currentCharacter.strokes.Add(stroke);
        lastPos = endPos;

        DrawStrokePixels(stroke);
        generatedTexture.Apply();
    }

    void DrawStrokePixels(Stroke s)
    {
        float dist = Vector2.Distance(s.Start, s.End);
        float step = Mathf.Max(1f, s.brushSize * s.spacing);
        int count = Mathf.CeilToInt(dist / step);

        for (int i = 0; i <= count; i++)
        {
            float t = i / (float)count;
            Vector2 p = Vector2.Lerp(s.Start, s.End, t);
            DrawBrushStamp(p, s);
        }
    }

    void DrawBrushStamp(Vector2 pos, Stroke s)
    {
        int cx = (int)pos.x;
        int cy = (int)pos.y;
        int r = s.brushSize;

        for (int x = -r; x <= r; x++)
        for (int y = -r; y <= r; y++)
        {
            int px = cx + x;
            int py = cy + y;
            if (px < 0 || py < 0 || px >= totalPixelsX || py >= totalPixelsY)
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
                // Only erase pixels with alpha > 0
                Color dst = generatedTexture.GetPixel(px, py);
                if (dst.a > 0f)
                    generatedTexture.SetPixel(px, py, Color.clear);
            }
            else
            {
                Color dst = generatedTexture.GetPixel(px, py);
                Color outCol = Color.Lerp(dst, s.Color, a);
                generatedTexture.SetPixel(px, py, outCol);
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

    void FloodFill(int x, int y, Color target, Color replacement)
    {
        if (target == replacement) return;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(x, y));

        while (stack.Count > 0)
        {
            var p = stack.Pop();
            if (p.x < 0 || p.y < 0 || p.x >= totalPixelsX || p.y >= totalPixelsY)
                continue;

            if (generatedTexture.GetPixel(p.x, p.y) != target)
                continue;

            generatedTexture.SetPixel(p.x, p.y, replacement);

            stack.Push(p + Vector2Int.up);
            stack.Push(p + Vector2Int.down);
            stack.Push(p + Vector2Int.left);
            stack.Push(p + Vector2Int.right);
        }
    }

    void ClearCanvasVisual()
    {
        Color[] fill = new Color[totalPixelsX * totalPixelsY];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = Color.white;

        generatedTexture.SetPixels(fill);
        generatedTexture.Apply();
    }

    public void ClearCanvas()
    {
        currentCharacter?.strokes.Clear();
        ClearCanvasVisual();
    }

    public void SetCurrentCharacter(CharacterData cd)
    {
        currentCharacter = cd;
        if (isReady)
            RedrawFromStrokes();
    }

    public void RedrawFromStrokes()
    {
        ClearCanvasVisual();
        if (currentCharacter == null) return;

        foreach (Stroke s in currentCharacter.strokes)
            DrawStrokePixels(s);

        generatedTexture.Apply();
    }

    public static void UpdateBrushSize()
    {
        brushSize = (int)ProjectSettingsManager.Instance.currentProjectSettings.brushSize;
    }
    

    // ===========================
    // Brush setters (UI -> PS -> DM)
    // ===========================

    public void SetTool(ToolType tool)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.currentTool = tool;
        currentTool = ProjectSettingsManager.Instance.currentProjectSettings.currentTool;
    }

    public void SetBrushShape(BrushShape shape)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.brushShape = shape;
        brushShape = ProjectSettingsManager.Instance.currentProjectSettings.brushShape;
        
        brushCursor?.RebuildCursor();
    }

    public void SetBrushSize(int size)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.brushSize = size;
        brushSize = ProjectSettingsManager.Instance.currentProjectSettings.brushSize;
        
        brushCursor?.RebuildCursor();
    }

    public void SetOpacity(float value)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.brushOpacity = Mathf.Clamp01(value);
        opacity = ProjectSettingsManager.Instance.currentProjectSettings.brushOpacity;
    }

    public void SetHardness(float value)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.brushHardness = Mathf.Clamp01(value);
        hardness = ProjectSettingsManager.Instance.currentProjectSettings.brushHardness;
    }

    public void SetSpacing(float value)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.brushSpacing = Mathf.Clamp01(value);
        spacing = ProjectSettingsManager.Instance.currentProjectSettings.brushSpacing;
    }

    public void SetCalligraphyAngle(float angle)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.calligraphyAngle = angle;
        calligraphyAngle = ProjectSettingsManager.Instance.currentProjectSettings.calligraphyAngle;
        
        brushCursor?.RebuildCursor();
    }

    public void SetCalligraphyAspect(float aspect)
    {
        ProjectSettingsManager.Instance.currentProjectSettings.calligraphyAspect = Mathf.Clamp01(aspect);
        calligraphyAspect = ProjectSettingsManager.Instance.currentProjectSettings.calligraphyAspect;
        
        brushCursor?.RebuildCursor();
    }
    
    public static void UpdateBrushColor()
    {
        string hex = ProjectSettingsManager.Instance.currentProjectSettings.brushColor; if (ColorUtility.TryParseHtmlString(hex, out Color c)) brushColor = c;
    }
}
