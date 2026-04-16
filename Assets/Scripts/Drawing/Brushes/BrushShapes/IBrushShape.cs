using UnityEngine;

public interface IBrushShape
{
    public float GetDistance(int x, int y, int r, Stroke s);
}
