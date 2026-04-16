using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;

public class NewFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;

    private OpenFile openFile;
    private XmlSerializer xmlSerializer = new(typeof(ProjectSettings));
    [SerializeField] private DrawingManager drawingManager;
    [SerializeField] private CanvasRenderManager canvasRenderManager;
    [SerializeField] private CharachterCollectionManager charachterCollectionManager;
    [SerializeField] private EnableCanvasOnLoad enableCanvasOnLoad;

    private void Awake()
    {
        openFile = GetComponent<OpenFile>();
    }

    public void OnClickCreateNewFile()
    {
        string path = StandaloneFileBrowser.SaveFilePanel(
            "Create New File", "", "NewProject", "xml");

        if (string.IsNullOrEmpty(path))
            return;

        ProjectSettings ps = new ProjectSettings
        {
            projectName = "NewProject"
        };

        CharacterDefaults.EnsureCharacters(ps);

        using (FileStream stream = new(path, FileMode.Create))
            xmlSerializer.Serialize(stream, ps);

        openFile.CurrentFilePath = path;
        ProjectSettingsManager.Instance.currentProjectSettings = ps;

        ApplyBrushSettings(ps);

        canvasRenderManager.SetCurrentCharacter(ps.characters[0]);

        charachterCollectionManager.LoadCharachtersIntoCollection();
        
        enableCanvasOnLoad.EnableCanvases();


        ProjectSettingsManager.Instance.DisplayProjectSettings();
        outputText.text = "File Created!";
    }

    void ApplyBrushSettings(ProjectSettings ps)
    {
        drawingManager.brushSize = ps.brushSize;
        drawingManager.opacity = ps.brushOpacity;
        drawingManager.hardness = ps.brushHardness;
        drawingManager.spacing = ps.brushSpacing;
        drawingManager.brushShape = ps.brushShape;
        drawingManager.currentTool = ps.currentTool;
        drawingManager.calligraphyAngle = ps.calligraphyAngle;
        drawingManager.calligraphyAspect = ps.calligraphyAspect;

        if (ColorUtility.TryParseHtmlString(ps.brushColor, out Color c))
            drawingManager.brushColor = c;
    }
}