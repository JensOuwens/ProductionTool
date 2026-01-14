using UnityEngine;
using UnityEngine.UI;
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

    private Stroke currentStroke;

    void DrawFromMouse()
    {
        if (currentCharacter == null) return;

        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localPos))
            return;

        Vector2 size = rectTransform.rect.size;
        float x = (localPos.x + size.x * 0.5f) / size.x * totalPixelsX;
        float y = (localPos.y + size.y * 0.5f) / size.y * totalPixelsY;
        Vector2 curPos = new(x, y);

        if (x < 0 || y < 0 || x >= totalPixelsX || y >= totalPixelsY)
            return;

        bool isEraser = currentTool == ToolType.Eraser;
        Color col = isEraser ? Color.clear : brushColor;
        col.a = opacity;

        if (Input.GetMouseButtonDown(0))
        {
            // Start a new stroke
            currentStroke = new Stroke(curPos, brushSize, col, opacity, hardness, spacing, brushShape, isEraser, calligraphyAngle, calligraphyAspect);
            currentCharacter.strokes.Add(currentStroke);
            UndoRedoManager.Instance.RegisterStroke(currentStroke);
            lastPos = curPos;
            return;
        }

        if (Input.GetMouseButton(0) && currentStroke != null)
        {
            if (Vector2.Distance(curPos, lastPos) >= brushSize * spacing)
            {
                currentStroke.AddPoint(curPos);
                DrawLinePixels(lastPos, curPos, currentStroke);
                lastPos = curPos;
                generatedTexture.Apply();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentStroke = null;
        }
    }

    void DrawStrokePixels(Stroke s)
    {
        if (s.points.Count < 2) return;

        for (int i = 1; i < s.points.Count; i++)
        {
            Vector2 a = s.points[i - 1];
            Vector2 b = s.points[i];
            DrawLinePixels(a, b, s);
        }
    }

    void DrawLinePixels(Vector2 start, Vector2 end, Stroke s)
    {
        float dist = Vector2.Distance(start, end);
        float step = Mathf.Max(1f, s.brushSize * s.spacing);
        int count = Mathf.CeilToInt(dist / step);

        for (int i = 0; i <= count; i++)
        {
            Vector2 p = Vector2.Lerp(start, end, i / (float)count);
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
                Color dst = generatedTexture.GetPixel(px, py);
                if (dst.a > 0f)
                    generatedTexture.SetPixel(px, py, Color.clear);
            }
            else
            {
                Color dst = generatedTexture.GetPixel(px, py);
                Color outCol = Color.Lerp(dst, s.color, a);
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
        UndoRedoManager.Instance.ClearHistory();
    }

    public void SetCurrentCharacter(CharacterData cd)
    {
        currentCharacter = cd;
        UndoRedoManager.Instance.OnCharacterSwitched(cd);
        LoadCharacterHistory(cd);
        RedrawFromStrokes();
    }

    public void LoadCharacterHistory(CharacterData character)
    {
        UndoRedoManager.Instance.ClearHistory(character.character);

        foreach (var stroke in character.strokes)
            UndoRedoManager.Instance.RegisterStroke(stroke);
    }

    public void RedrawFromStrokes()
    {
        ClearCanvasVisual();
        if (currentCharacter == null) return;

        foreach (var s in currentCharacter.strokes)
            DrawStrokePixels(s);

        generatedTexture.Apply();
    }

    public CharacterData GetCurrentCharacter() => currentCharacter;
    
    private static BrushCursor Cursor =>
        Object.FindObjectOfType<BrushCursor>();

    private static void NotifyCursor()
    {
        if (Cursor != null)
            Cursor.OnBrushSettingsChanged();
    }

    public void SetBrushSize(int size)
    {
        brushSize = Mathf.Clamp(size, 1, 30);
        
        var ps = ProjectSettingsManager.Instance.currentProjectSettings;
        if (ps != null)
        {
            ps.brushSize = brushSize;
        }
        
        NotifyCursor();
    }

    public void SetOpacity(float val)
    {
        opacity = val;
        NotifyCursor();
    }

    public void SetHardness(float val)
    {
        hardness = val;
        NotifyCursor();
    }

    public void SetSpacing(float val)
    {
        spacing = val;
        NotifyCursor();
    }

    public void SetCalligraphyAngle(float val)
    {
        calligraphyAngle = val;
        NotifyCursor();
    }

    public void SetCalligraphyAspect(float val)
    {
        calligraphyAspect = val;
        NotifyCursor();
    }

    public void SetBrushShape(BrushShape shape)
    {
        brushShape = shape;
        NotifyCursor();
    }

    public void SetTool(ToolType tool)
    {
        currentTool = tool;
        NotifyCursor();
    }

    // ========================
    // Cycle shape tool
    // ========================
    public void CycleShapeTool()
    {
        brushShape = brushShape switch
        {
            BrushShape.Circle => BrushShape.Square,
            BrushShape.Square => BrushShape.Diamond,
            BrushShape.Diamond => BrushShape.Calligraphy,
            _ => BrushShape.Circle
        };

        NotifyCursor();
    }
}
