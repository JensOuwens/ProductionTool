using UnityEngine;

public class SquareShape : IBrushShape
{
    public float GetDistance(int x, int y, int r, Stroke s)
    {
        return Mathf.Max(Mathf.Abs(x), Mathf.Abs(y)) / r;
    }
}
