using System.Collections.Generic;
using UnityEngine;

public class FloodFillBrush : IBrush
{
    public void OnMouseDown(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
    {
        Color target = tex.GetPixel(pos.x, pos.y);
        Color replacement = stroke.color;

        if (target == replacement) return;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(pos);

        int w = tex.width;
        int h = tex.height;

        while (stack.Count > 0)
        {
            var p = stack.Pop();

            if (p.x < 0 || p.y < 0 || p.x >= w || p.y >= h)
                continue;

            if (tex.GetPixel(p.x, p.y) != target)
                continue;

            tex.SetPixel(p.x, p.y, replacement);

            stack.Push(p + Vector2Int.up);
            stack.Push(p + Vector2Int.down);
            stack.Push(p + Vector2Int.left);
            stack.Push(p + Vector2Int.right);
        }

        tex.Apply();
        stroke.isFill = true;
    }

    public void OnMouseDrag(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx) { }
    public void OnMouseUp(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx) { }
}
