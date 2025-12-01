using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int totalPixelsX = 1024;
    [SerializeField] private int totalPixelsY = 512;
    [SerializeField] private int brushsize = 4;
    [SerializeField] private Color brushColor;

    [SerializeField] private Transform topLeftCorner;
    [SerializeField] private Transform bottomRightCorner;
    [SerializeField] private Transform point;
    
    [SerializeField] private Material material;
    
    [SerializeField] private Texture2D generatedTexture;

    private Color[] colorMap;

    private int yPixel = 0;
    private int xPixel = 0;
    
    private float xMult;
    private float yMult;

    private void Start()
    {
        colorMap = new Color[totalPixelsX * totalPixelsX];
        generatedTexture = new Texture2D(totalPixelsX, totalPixelsX, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;
        material.SetTexture("baseMap", generatedTexture);

        ResetColor();
        
        xMult = totalPixelsX / (bottomRightCorner.localPosition.x - topLeftCorner.localPosition.x);
        yMult = totalPixelsY / (bottomRightCorner.localPosition.y - topLeftCorner.localPosition.y);
    }

    private void update()
    {
        if (Input.GetMouseButton(0))
        {
            CalculatePixel();
        }
    }

    private void CalculatePixel()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            point.position = hit.point;
            xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult);
            yPixel = (int)((point.localPosition.y - topLeftCorner.localPosition.y) * yMult);
            ChangePixelsAroundPoint();
        }
    }

    private void ChangePixelsAroundPoint()
    {
        DrawBrush(xPixel, yPixel);
        SetTexture();
    }
    
    private void DrawBrush(int xPix, int yPix)
    {
        int i = xPix - brushsize + 1, j = yPix - brushsize + 1, maxi = i + brushsize - 1, maxj = j + brushsize - 1;
        if (i < 0)
            i = 0;
        if (j < 0)
            j = 0;
        if (maxi >= totalPixelsX)
            maxi = totalPixelsX - 1;
        if (maxj >= totalPixelsY)
            maxj = totalPixelsY - 1;
        for (int x = i; x <= maxi; x++)
        {
            for (int y = j; y <= maxj; y++)
            {
                if ((x - xPix) * (x - xPix) + (y - yPix) * (y - yPix) <= brushsize * brushsize)
                {
                    colorMap[x * totalPixelsY + y] = brushColor;
                }
            }
        }
    }

    private void SetTexture()
    {
        generatedTexture.SetPixels(colorMap);
        generatedTexture.Apply();
    }

    private void ResetColor()
    {
        for (int i = 0; i < colorMap.Length; i++)
        {
            colorMap[i] = Color.white;
        }
        SetTexture();
    }
}
