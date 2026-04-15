using UnityEngine;

public class ShortcutManager : MonoBehaviour
{
    [Header("Tool References")]
    [SerializeField] private DrawingManager drawingManager;
    [SerializeField] private SettingsChanger settingsChanger;
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
            settingsChanger.SetTool(ToolType.Brush);

        if (Input.GetKeyDown(KeyCode.E))
            settingsChanger.SetTool(ToolType.Eraser);

        if (Input.GetKeyDown(KeyCode.G))
            settingsChanger.SetTool(ToolType.Fill);

        if (Input.GetKeyDown(KeyCode.U))
            settingsChanger.CycleShapeTool();
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

    private void HandleBrushSizeShortcuts()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            int newSize = Mathf.Max(1, drawingManager.brushSize - 1);
            settingsChanger.SetBrushSize(newSize);
            brushUI.RefreshFromProjectSettings();
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            int newSize = drawingManager.brushSize + 1;
            settingsChanger.SetBrushSize(newSize);
            brushUI.RefreshFromProjectSettings();
        }
    }


}