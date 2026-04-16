using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] public RawImage drawImage;
    [SerializeField] public int totalPixelsX = 1024;
    [SerializeField] public int totalPixelsY = 512;
    public CanvasRenderManager canvasRenderManager;

    public int brushSize = 6;
    public Color brushColor = Color.black;

    public ToolType currentTool = ToolType.Brush;
    public BrushShape brushShape = BrushShape.Circle;

    public float opacity = 1f;
    public float hardness = 1f;
    public float spacing = 0.25f;

    public float calligraphyAngle = 45f;
    public float calligraphyAspect = 0.3f;

    public Texture2D generatedTexture;
    private RectTransform rectTransform;

    private Vector2 lastPos;
    private Vector2 lineStart;

    public CharacterData currentCharacter;
    private bool isReady;

    private BrushCursor brushCursor;
    public Stroke currentStroke;
    
    private PencilBrush pencilBrush;
    private FloodFillBrush floodFillBrush;
    private EreaserBrush ereaserBrush;
    private IBrush currentBrush;

    private void Awake()
    {
        pencilBrush = new PencilBrush();
        floodFillBrush = new FloodFillBrush();
        ereaserBrush = new EreaserBrush();
    
        UpdateCurrentBrush();
    }

    void Start()
    {
        rectTransform = drawImage.rectTransform;
        
        if (currentCharacter == null)
        {
            generatedTexture = new Texture2D(totalPixelsX, totalPixelsY, TextureFormat.RGBA32, false);
            generatedTexture.filterMode = FilterMode.Point;
            canvasRenderManager.ClearCanvasVisual();
            drawImage.texture = generatedTexture;
        }

        brushCursor = FindObjectOfType<BrushCursor>();
        isReady = true;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
            DrawFromMouse();
    }

void DrawFromMouse()
{
    if (currentCharacter == null || currentCharacter.cachedTexture == null)
        return;
    
    if (Input.GetMouseButtonDown(0) && currentTool == ToolType.Fill)
    {
        currentStroke = null;
    }

    Vector2 localPos;
    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, Input.mousePosition, null, out localPos))
        return;

    Vector2 size = rectTransform.rect.size;
    float x = (localPos.x + size.x * 0.5f) / size.x * totalPixelsX;
    float y = (localPos.y + size.y * 0.5f) / size.y * totalPixelsY;
    Vector2 curPos = new Vector2(x, y);

    if (x < 0 || y < 0 || x >= totalPixelsX || y >= totalPixelsY)
        return;
    
    if (currentTool == ToolType.Fill)
    {
        if (Input.GetMouseButtonDown(0))
        {
            Color fillCol = brushColor;
            fillCol.a = opacity;

            Stroke fillStroke = new Stroke(
                new Vector2((int)x, (int)y),
                0,
                fillCol,
                opacity,
                1f,
                1f,
                BrushShape.Circle,
                false,
                0,
                1f
            );

            fillStroke.isFill = true;

            currentCharacter.strokes.Add(fillStroke);
            UndoRedoManager.Instance.RegisterStroke(fillStroke);

            currentBrush.OnMouseDown(
                currentCharacter.cachedTexture,
                new Vector2Int((int)x, (int)y),
                fillStroke,
                this
            );
        }

        return;
    }


    bool isEraser = currentTool == ToolType.Eraser;
    Color col = isEraser ? Color.clear : brushColor;
    col.a = opacity;
    
    if (Input.GetMouseButtonDown(0))
    {
        currentStroke = new Stroke(
            curPos, brushSize, col, opacity, hardness, spacing,
            brushShape, isEraser, calligraphyAngle, calligraphyAspect
        );

        currentCharacter.strokes.Add(currentStroke);
        UndoRedoManager.Instance.RegisterStroke(currentStroke);
        
        currentBrush.OnMouseDown(
            currentCharacter.cachedTexture,
            new Vector2Int((int)curPos.x, (int)curPos.y),
            currentStroke,
            this
        );

        lastPos = curPos;
        lineStart = curPos;
        return;
    }
    
    if (currentStroke != null)
    {
        Vector2 drawTarget = curPos;

        if (Input.GetKey(KeyCode.LeftShift))
            drawTarget = SnapTo8Directions(lineStart, curPos);

        currentBrush.OnMouseDrag(
            currentCharacter.cachedTexture,
            new Vector2Int((int)drawTarget.x, (int)drawTarget.y),
            currentStroke,
            this
        );

        if (Vector2.Distance(drawTarget, lastPos) >= brushSize * spacing)
        {
            currentStroke.AddPoint(drawTarget);
            canvasRenderManager.DrawLinePixels(lastPos, drawTarget, currentStroke);
            lastPos = drawTarget;
            currentCharacter.cachedTexture.Apply();
        }
    }

    if (Input.GetMouseButtonUp(0))
    {
        if (currentStroke != null)
        {
            currentBrush.OnMouseUp(
                currentCharacter.cachedTexture,
                new Vector2Int((int)curPos.x, (int)curPos.y),
                currentStroke,
                this
            );
        }

        currentStroke = null;
    }
}

Vector2 SnapTo8Directions(Vector2 origin, Vector2 current)
{
    Vector2 delta = current - origin;
    if (delta.sqrMagnitude < 0.0001f)
        return origin;

    float angle = Mathf.Atan2(delta.y, delta.x);
    float snappedAngle = Mathf.Round(angle / (Mathf.PI / 4f)) * (Mathf.PI / 4f);

    Vector2 snappedDir = new Vector2(Mathf.Cos(snappedAngle), Mathf.Sin(snappedAngle));

    float projectedLength = Vector2.Dot(delta, snappedDir);

    return origin + snappedDir * projectedLength;
}
    
    public CharacterData GetCurrentCharacter() => currentCharacter;

    public void UpdateCurrentBrush()
    {
        currentBrush = currentTool switch
        {
            ToolType.Brush => pencilBrush,
            ToolType.Eraser => ereaserBrush,
            ToolType.Fill => floodFillBrush,
            _ => pencilBrush
        };
    }

    public float GetBrushSizs()
    {
        return brushSize;
    }
}
