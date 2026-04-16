using UnityEngine;

public class CanvasRenderManager : MonoBehaviour
{
    [SerializeField] private DrawingManager drawingManager;
    
    private int totalPixelsX;
    private int totalPixelsY;

    private static readonly IBrushShape circle = new CircleShape();
    private static readonly IBrushShape diamond = new DiamondShape();
    private static readonly IBrushShape square = new SquareShape();
    private static readonly IBrushShape caligraphy = new CaligraphyShape();
    private IBrushShape currentBrushShape;

    private void Awake()
    {
        totalPixelsX = drawingManager.totalPixelsX;
        totalPixelsY = drawingManager.totalPixelsY;
    }
    
   public void DrawLinePixels(Vector2 start, Vector2 end, Stroke s)
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
        
        if (drawingManager.currentCharacter == null || drawingManager.currentCharacter.cachedTexture == null)
            return;

        Texture2D tex = drawingManager.currentCharacter.cachedTexture;

        int cx = (int)pos.x;
        int cy = (int)pos.y;
        int r = s.brushSize;
        GetShape(s);

        for (int x = -r; x <= r; x++)
        for (int y = -r; y <= r; y++)
        {
            int px = cx + x;
            int py = cy + y;
            if (px < 0 || py < 0 || px >= totalPixelsX || py >= totalPixelsY)
                continue;

            float d = currentBrushShape.GetDistance(px,py,r,s);

            if (d > 1f) continue;

            float falloff = Mathf.Pow(1f - d, s.hardness * 4f);
            float a = falloff * s.opacity;

            if (s.isEraser)
            {
                Color dst = tex.GetPixel(px, py);
                if (dst.a > 0f)
                    tex.SetPixel(px, py, Color.clear);
            }
            else
            {
                Color dst = tex.GetPixel(px, py);
                Color outCol = Color.Lerp(dst, s.color, a);
                tex.SetPixel(px, py, outCol);
            }
        }
    }
    
    
    public void ClearCanvasVisual()
    {
        Color[] fill = new Color[totalPixelsX * totalPixelsY];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = Color.white;

        drawingManager.generatedTexture.SetPixels(fill);
        drawingManager.generatedTexture.Apply();
    }
    
    public void ClearCanvas()
    {
        drawingManager.currentCharacter?.strokes.Clear();
        ClearCanvasVisual();
        UndoRedoManager.Instance.ClearHistory();
    }
    
    public void SetCurrentCharacter(CharacterData cd)
    {
        drawingManager.currentCharacter = cd;
        UndoRedoManager.Instance.OnCharacterSwitched(cd);


        if (cd.cachedTexture != null)
        {
            drawingManager.drawImage.texture = cd.cachedTexture;
        }
        else
        {

            cd.cachedTexture = new Texture2D(totalPixelsX, totalPixelsY, TextureFormat.RGBA32, false);
            RenderCharacterToTexture(cd);
            drawingManager.drawImage.texture = cd.cachedTexture;
        }
        
        drawingManager.drawImage.texture = cd.cachedTexture;


        LoadCharacterHistory(cd);
    }
    
    public void LoadCharacterHistory(CharacterData character)
    {
        UndoRedoManager.Instance.ClearHistory(character.character);

        foreach (var stroke in character.strokes)
            UndoRedoManager.Instance.RegisterStroke(stroke);
    }
    
    public void RedrawFromStrokes()
    {
        if (drawingManager.currentCharacter == null) return;


        Color[] fill = new Color[totalPixelsX * totalPixelsY];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = Color.white;

        drawingManager.currentCharacter.cachedTexture.SetPixels(fill);


        foreach (var s in drawingManager.currentCharacter.strokes)
            DrawStrokeIntoTexture(drawingManager.currentCharacter.cachedTexture, s);

        drawingManager.currentCharacter.cachedTexture.Apply();


        drawingManager.drawImage.texture = drawingManager.currentCharacter.cachedTexture;
    }
    
    private void DrawStrokeIntoTexture(Texture2D tex, Stroke s)
    {
        if (s.points.Count < 2) return;

        for (int i = 1; i < s.points.Count; i++)
            DrawLinePixelsIntoTexture(tex, s.points[i - 1], s.points[i], s);
    }

    private void DrawLinePixelsIntoTexture(Texture2D tex, Vector2 start, Vector2 end, Stroke s)
    {
        float dist = Vector2.Distance(start, end);
        float step = Mathf.Max(1f, s.brushSize * s.spacing);
        int count = Mathf.CeilToInt(dist / step);

        for (int i = 0; i <= count; i++)
        {
            Vector2 p = Vector2.Lerp(start, end, i / (float)count);
            DrawBrushStampIntoTexture(tex, p, s);
        }
    }
    
    private void DrawBrushStampIntoTexture(Texture2D tex, Vector2 pos, Stroke s)
    {
        int cx = (int)pos.x;
        int cy = (int)pos.y;
        int r = s.brushSize;
        GetShape(s);

        for (int x = -r; x <= r; x++)
        for (int y = -r; y <= r; y++)
        {
            int px = cx + x;
            int py = cy + y;
            if (px < 0 || py < 0 || px >= totalPixelsX || py >= totalPixelsY)
                continue;

            float d = currentBrushShape.GetDistance(px,py,r,s);

            if (d > 1f) continue;

            float falloff = Mathf.Pow(1f - d, s.hardness * 4f);
            float a = falloff * s.opacity;

            if (s.isEraser)
            {
                Color dst = tex.GetPixel(px, py);
                if (dst.a > 0f)
                    tex.SetPixel(px, py, Color.clear);
            }
            else
            {
                Color dst = tex.GetPixel(px, py);
                Color outCol = Color.Lerp(dst, s.color, a);
                tex.SetPixel(px, py, outCol);
            }
        }
    }

    
private void DrawStrokeIntoBuffer(Stroke s, Color[] buffer)
{
    if (s.points.Count < 2) return;

    for (int i = 1; i < s.points.Count; i++)
    {
        Vector2 start = s.points[i - 1];
        Vector2 end = s.points[i];
        float dist = Vector2.Distance(start, end);
        float step = Mathf.Max(1f, s.brushSize * s.spacing);
        int count = Mathf.CeilToInt(dist / step);

        for (int j = 0; j <= count; j++)
        {
            Vector2 p = Vector2.Lerp(start, end, j / (float)count);
            DrawBrushStampIntoBuffer(p, s, buffer);
        }
    }
}

private void DrawBrushStampIntoBuffer(Vector2 pos, Stroke s, Color[] buffer)
{
    int cx = (int)pos.x;
    int cy = (int)pos.y;
    int r = s.brushSize;
    GetShape(s);

    for (int x = -r; x <= r; x++)
    for (int y = -r; y <= r; y++)
    {
        int px = cx + x;
        int py = cy + y;
        if (px < 0 || py < 0 || px >= totalPixelsX || py >= totalPixelsY)
            continue;

        float d = currentBrushShape.GetDistance(px,py,r,s);

        if (d > 1f) continue;

        float falloff = Mathf.Pow(1f - d, s.hardness * 4f);
        float a = falloff * s.opacity;

        int index = py * totalPixelsX + px;
        if (s.isEraser)
        {
            Color dst = buffer[index];
            if (dst.a > 0f)
                buffer[index] = Color.white;
        }
        else
        {
            Color dst = buffer[index];
            Color outCol = Color.Lerp(dst, s.color, a);
            buffer[index] = outCol;
        }
    }
}

public void RenderCharacterToTexture(CharacterData ch)
{
    Color[] pixels = new Color[totalPixelsX * totalPixelsY];
    for (int i = 0; i < pixels.Length; i++)
        pixels[i] = Color.white;

    foreach (var s in ch.strokes)
        DrawStrokeIntoBuffer(s, pixels);

    ch.cachedTexture.SetPixels(pixels);
    ch.cachedTexture.Apply();
}

void GetShape(Stroke s)
{
    switch (s.shape)
    {
        case BrushShape.Circle: currentBrushShape = circle; break;
        case BrushShape.Square: currentBrushShape = square; break;
        case BrushShape.Diamond: currentBrushShape = diamond; break;
        case BrushShape.Calligraphy: currentBrushShape = caligraphy; break;
    }
}
}
