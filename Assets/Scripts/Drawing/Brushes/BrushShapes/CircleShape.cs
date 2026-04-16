using UnityEngine;

public class CircleShape : IBrushShape
{
    public float GetDistance(int x, int y, int r, Stroke s)
    {
        return Mathf.Sqrt(x * x + y * y) / r;
    }
}
