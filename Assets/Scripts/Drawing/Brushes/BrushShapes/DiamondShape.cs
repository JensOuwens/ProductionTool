using UnityEngine;

public class DiamondShape : IBrushShape
{
    public float GetDistance(int x, int y, int r, Stroke s)
    {
        return (Mathf.Abs(x) + Mathf.Abs(y)) / r;
    }
}
