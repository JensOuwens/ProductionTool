using UnityEngine;

public class EreaserBrush : IBrush
{
        private PencilBrush internalBrush = new PencilBrush();

        public void OnMouseDown(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
            => internalBrush.OnMouseDown(tex, pos, stroke, ctx);

        public void OnMouseDrag(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
            => internalBrush.OnMouseDrag(tex, pos, stroke, ctx);

        public void OnMouseUp(Texture2D tex, Vector2Int pos, Stroke stroke, DrawingManager ctx)
            => internalBrush.OnMouseUp(tex, pos, stroke, ctx);
}
