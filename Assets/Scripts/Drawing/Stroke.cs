using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System;


/// <summary>
/// TODO
/// once more brushes/settings are added, make sure to add updates
/// add drawing different shapes (square, circle, straight line) (different script?)
/// </summary>
[Serializable]
public class Stroke
{
    public float startX, startY;
    public float endX, endY;

    public int brushSize;
    public float opacity;
    public float hardness;
    public float spacing;

    public BrushShape shape;
    public bool isEraser;

    public float angle; // calligraphy
    public float aspectRatio; // calligraphy

    public float r, g, b, a;

    // XML
    public Stroke()
    {
    }

    public Stroke(
        Vector2 start,
        Vector2 end,
        int size,
        Color col,
        float opacity,
        float hardness,
        float spacing,
        BrushShape shape,
        bool eraser,
        float angle,
        float aspectRatio
    )
    {
        startX = start.x;
        startY = start.y;
        endX = end.x;
        endY = end.y;

        brushSize = size;
        this.opacity = opacity;
        this.hardness = hardness;
        this.spacing = spacing;
        this.shape = shape;
        isEraser = eraser;
        this.angle = angle;
        this.aspectRatio = aspectRatio;

        r = col.r;
        g = col.g;
        b = col.b;
        a = col.a;
    }

    public Vector2 Start => new(startX, startY);
    public Vector2 End => new(endX, endY);
    public Color Color => new(r, g, b, a);
}