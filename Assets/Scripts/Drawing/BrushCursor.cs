using UnityEngine;
using UnityEngine.EventSystems;

public class BrushCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Texture2D cursorTex;
    private Vector2 hotspot;
    private bool hovering;

    private const int MIN_CURSOR_SIZE = 40;
    private const int MAX_CURSOR_SIZE = 400;
    private const int MIN_RADIUS = 3;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        RebuildCursor();
        ApplyCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void OnBrushSettingsChanged()
    {
        if (!hovering) return;
        RebuildCursor();
        ApplyCursor();
    }

    public void RebuildCursor()
    {
        int radius = Mathf.Max(MIN_RADIUS, DrawingManager.brushSize);
        int size = Mathf.Clamp(radius * 2 + 1, MIN_CURSOR_SIZE, MAX_CURSOR_SIZE);

        if (cursorTex != null)
            Destroy(cursorTex);

        cursorTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        cursorTex.alphaIsTransparency = true;
        cursorTex.filterMode = FilterMode.Point;
        cursorTex.wrapMode = TextureWrapMode.Clamp;

        Clear(cursorTex);
        DrawOutline(cursorTex, radius);

        cursorTex.Apply();
        hotspot = new Vector2(size * 0.5f, size * 0.5f);
    }

    private void ApplyCursor()
    {
        Cursor.SetCursor(cursorTex, hotspot, CursorMode.Auto);
    }

    private void Clear(Texture2D tex)
    {
        Color clear = new Color(0, 0, 0, 0);
        Color[] pixels = new Color[tex.width * tex.height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = clear;

        tex.SetPixels(pixels);
    }

    private void DrawOutline(Texture2D tex, int radius)
    {
        int cx = tex.width / 2;
        int cy = tex.height / 2;

        float outer = 1f;
        float inner = 1f - (1f / radius);

        for (int x = -radius; x <= radius; x++)
        for (int y = -radius; y <= radius; y++)
        {
            int px = cx + x;
            int py = cy + y;

            if (px < 0 || py < 0 || px >= tex.width || py >= tex.height)
                continue;

            float d = GetShapeDistance(x + 0.5f, y + 0.5f, radius);

            if (d >= inner && d <= outer)
                tex.SetPixel(px, py, Color.black);
        }
    }

    private float GetShapeDistance(float x, float y, int r)
    {
        return DrawingManager.brushShape switch
        {
            BrushShape.Circle =>
                Mathf.Sqrt(x * x + y * y) / r,

            BrushShape.Square =>
                Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) / r,

            BrushShape.Diamond =>
                (Mathf.Abs(x) + Mathf.Abs(y)) / (r * 1.0001f),

            BrushShape.Calligraphy =>
                CalligraphyDistance(x, y, r),

            _ => 1f
        };
    }

    private float CalligraphyDistance(float x, float y, int r)
    {
        float angle = DrawingManager.calligraphyAngle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        float rx = (x * cos - y * sin) / r;
        float ry = (x * sin + y * cos) /
                   (r * Mathf.Max(0.01f, DrawingManager.calligraphyAspect));

        return Mathf.Sqrt(rx * rx + ry * ry);
    }
}
