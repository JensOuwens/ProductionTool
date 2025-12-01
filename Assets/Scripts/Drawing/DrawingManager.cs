using UnityEngine;
using UnityEngine.UI;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private RawImage drawImage;
    [SerializeField] private int totalPixelsX = 1024;
    [SerializeField] private int totalPixelsY = 512;
    [SerializeField] private int brushsize = 6;
    [SerializeField] private Color brushColor = Color.black;

    private Texture2D generatedTexture;
    private RectTransform rectTransform;

    private Vector2 lastPos;
    private bool hasLast = false;

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

        if (!hasLast)
        {
            DrawCircle(curPos);
            lastPos = curPos;
            hasLast = true;
        }
        else
        {
            DrawLine(lastPos, curPos);
            lastPos = curPos;
        }

        generatedTexture.Apply();
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int steps = (int)Vector2.Distance(start, end);
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(start, end, t);
            DrawCircle(point);
        }
    }

    void DrawCircle(Vector2 center)
    {
        int cx = (int)center.x;
        int cy = (int)center.y;

        // ensure brush is opaque
        Color color = brushColor;
        color.a = 1f;

        for (int x = -brushsize; x <= brushsize; x++)
        {
            for (int y = -brushsize; y <= brushsize; y++)
            {
                int px = cx + x;
                int py = cy + y;
                if (px >= 0 && px < totalPixelsX && py >= 0 && py < totalPixelsY)
                {
                    if (x * x + y * y <= brushsize * brushsize)
                        generatedTexture.SetPixel(px, py, color);
                }
            }
        }
    }

    public void ClearCanvas()
    {
        Color clearColor = Color.white; // opaque white
        clearColor.a = 1f;

        for (int x = 0; x < totalPixelsX; x++)
        {
            for (int y = 0; y < totalPixelsY; y++)
            {
                generatedTexture.SetPixel(x, y, clearColor);
            }
        }

        generatedTexture.Apply();
    }
}
