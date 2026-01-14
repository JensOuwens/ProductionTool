using UnityEngine;

public class ShortcutManager : MonoBehaviour
{
    [Header("Tool References")]
    [SerializeField] private DrawingManager drawingManager;
    [SerializeField] private UndoRedoManager undoRedoManager;
    [SerializeField] private SaveFile saveFile;
    [SerializeField] private OpenFile openFile;
    [SerializeField] private NewFile newFile;
    [SerializeField] private ExportFile exportFile;
    [SerializeField] private BrushUI brushUI;


    [Header("Brush Settings")]
    [SerializeField] private float brushStep = 2f;

    void Update()
    {
        HandleToolShortcuts();
        HandleEditShortcuts();
        HandleFileShortcuts();
        HandleBrushSizeShortcuts();
    }

    void HandleToolShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.B))
            drawingManager.SetTool(ToolType.Brush);

        if (Input.GetKeyDown(KeyCode.E))
            drawingManager.SetTool(ToolType.Eraser);

        if (Input.GetKeyDown(KeyCode.G))
            drawingManager.SetTool(ToolType.Fill);

        if (Input.GetKeyDown(KeyCode.U))
            drawingManager.CycleShapeTool();
    }

    private void HandleEditShortcuts()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            undoRedoManager.Undo();

        if (Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKeyDown(KeyCode.Y))
            undoRedoManager.Redo();
    }

    void HandleFileShortcuts()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
            newFile.OnClickCreateNewFile();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
            openFile.OnClickOpenFile();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
            saveFile.OnClickSaveFile();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.E))
            exportFile.ExportFontAtlas();
    }

    void HandleBrushSizeShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            int newSize = Mathf.Max(1, DrawingManager.brushSize - 1);
            drawingManager.SetBrushSize(newSize);
            brushUI.RefreshFromProjectSettings();
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            int newSize = DrawingManager.brushSize + 1;
            drawingManager.SetBrushSize(newSize);
            brushUI.RefreshFromProjectSettings();
        }
    }


}