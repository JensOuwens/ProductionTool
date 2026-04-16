using UnityEngine;

public class CaligraphyShape : IBrushShape
{
    public float GetDistance(int x, int y, int r, Stroke s)
    {
        return CalligraphyDistance(x, y, r, s);
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
}
