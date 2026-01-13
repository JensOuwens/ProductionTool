using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class Stroke
{
    public List<Vector2> points = new List<Vector2>();

    public int brushSize;
    public float opacity;
    public float hardness;
    public float spacing;

    public BrushShape shape;
    public bool isEraser;

    public float angle; // calligraphy
    public float aspectRatio; // calligraphy

    public float r, g, b, a;

    public Color color => new(r, g, b, a);

    public Stroke() { }

    public Stroke(Vector2 start, int size, Color col, float opacity, float hardness,
        float spacing, BrushShape shape, bool eraser, float angle, float aspectRatio)
    {
        points.Add(start);
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

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
    }
}