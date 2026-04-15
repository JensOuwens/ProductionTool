using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using TMPro;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private DrawingManager drawingManager;

    private XmlSerializer xmlSerializer = new(typeof(ProjectSettings));
    private OpenFile openFile;

    private void Awake()
    {
        openFile = GetComponent<OpenFile>();
    }

    public void OnClickSaveFile()
    {
        if (string.IsNullOrEmpty(openFile.CurrentFilePath))
            return;

        ProjectSettings ps = ProjectSettingsManager.Instance.currentProjectSettings;
        
        ps.brushSize = drawingManager.brushSize;
        ps.brushOpacity = drawingManager.opacity;
        ps.brushHardness = drawingManager.hardness;
        ps.brushSpacing = drawingManager.spacing;
        ps.brushShape = drawingManager.brushShape;
        ps.currentTool = drawingManager.currentTool;
        ps.calligraphyAngle = drawingManager.calligraphyAngle;
        ps.calligraphyAspect = drawingManager.calligraphyAspect;

        ps.brushColor = "#" + ColorUtility.ToHtmlStringRGB(drawingManager.brushColor);

        using (FileStream stream = new(openFile.CurrentFilePath, FileMode.Create))
            xmlSerializer.Serialize(stream, ps);

        outputText.text = "file saved!";
    }
}