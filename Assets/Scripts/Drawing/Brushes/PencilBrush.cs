using UnityEngine;

public class PencilBrush : IBrush
{
    private Vector2 lastPos;
    private Vector2 lineStart;

    public void OnMouseDown(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
    {
        lastPos = pos;
        lineStart = pos;
    }

    public void OnMouseDrag(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
    {
        Vector2 current = pos;

        if (Vector2.Distance(current, lastPos) >= stroke.brushSize * stroke.spacing)
        {
            stroke.AddPoint(current);

            ctx.DrawLinePixels(lastPos, current, stroke); // reuse your existing function
            lastPos = current;

            tex.Apply();
        }
    }

    public void OnMouseUp(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
    {
    }
}
