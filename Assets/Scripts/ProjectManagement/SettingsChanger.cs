using System;
using UnityEngine;

public class SettingsChanger : MonoBehaviour
{
    [SerializeField] private DrawingManager drawingManager;
    private static BrushCursor brushCursor;

    private void Awake()
    {
        brushCursor = FindObjectOfType<BrushCursor>();
    }

    public void SetBrushSize(int size)
    {
        drawingManager.brushSize = Mathf.Clamp(size, 1, 30);

        var ps = ProjectSettingsManager.Instance.currentProjectSettings;
        if (ps != null)
            ps.brushSize = drawingManager.brushSize;

        NotifyCursor();
    }

    public void SetOpacity(float val)
    {
        drawingManager.opacity = val;
        NotifyCursor();
    }

    public void SetHardness(float val)
    {
        drawingManager.hardness = val;
        NotifyCursor();
    }

    public void SetSpacing(float val)
    {
        drawingManager.spacing = val;
        NotifyCursor();
    }

    public void SetCalligraphyAngle(float val)
    {
        drawingManager.calligraphyAngle = val;
        NotifyCursor();
    }

    public void SetCalligraphyAspect(float val)
    {
        drawingManager.calligraphyAspect = val;
        NotifyCursor();
    }

    public void SetBrushShape(BrushShape shape)
    {
        drawingManager.brushShape = shape;
        NotifyCursor();
    }

    public void SetTool(ToolType tool)
    {
        drawingManager.currentTool = tool;
        drawingManager.UpdateCurrentBrush();
        drawingManager.currentStroke = null;
        NotifyCursor();
    }

    public void CycleShapeTool()
    {
        drawingManager.brushShape = drawingManager.brushShape switch
        {
            BrushShape.Circle => BrushShape.Square,
            BrushShape.Square => BrushShape.Diamond,
            BrushShape.Diamond => BrushShape.Calligraphy,
            _ => BrushShape.Circle
        };

        NotifyCursor();
    }
    
    private static void NotifyCursor()
    {
        if (brushCursor != null)
            brushCursor.OnBrushSettingsChanged();
    }
}
