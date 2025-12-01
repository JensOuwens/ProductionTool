using System;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    [SerializeField] private Camera MainCamera;
    [SerializeField] private int TotalPixelsX = 1024;
    [SerializeField] private int TotalPixelsY = 512;
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
        colorMap = new Color[TotalPixelsX * TotalPixelsX];
        generatedTexture = new Texture2D(TotalPixelsX, TotalPixelsX, TextureFormat.RGBA32, false);
        generatedTexture.filterMode = FilterMode.Point;
        material.SetTexture("baseMap", generatedTexture);

        xMult = TotalPixelsX / (bottomRightCorner.localPosition.x - topLeftCorner.localPosition.x);
        yMult = TotalPixelsY / (bottomRightCorner.localPosition.y - topLeftCorner.localPosition.y);
    }

    private void update()
    {
        
    }

    private void CalculatePixel()
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f))
        {
            point.position = hit.point;
            xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult);
            yPixel = (int)((point.localPosition.y - topLeftCorner.localPosition.y) * yMult);
        }
    }
}
