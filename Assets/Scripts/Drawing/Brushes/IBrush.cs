using UnityEngine;

public interface IBrush
{
        void OnMouseDown(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx);
        void OnMouseDrag(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx);
        void OnMouseUp(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx);
}
