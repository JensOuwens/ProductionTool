using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using TMPro;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;

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

        // SYNC RUNTIME BRUSH STATE → PROJECT SETTINGS
        ps.brushSize = DrawingManager.brushSize;
        ps.brushOpacity = DrawingManager.opacity;
        ps.brushHardness = DrawingManager.hardness;
        ps.brushSpacing = DrawingManager.spacing;
        ps.brushShape = DrawingManager.brushShape;
        ps.currentTool = DrawingManager.currentTool;
        ps.calligraphyAngle = DrawingManager.calligraphyAngle;
        ps.calligraphyAspect = DrawingManager.calligraphyAspect;

        ps.brushColor = "#" + ColorUtility.ToHtmlStringRGB(DrawingManager.brushColor);

        using (FileStream stream = new(openFile.CurrentFilePath, FileMode.Create))
            xmlSerializer.Serialize(stream, ps);

        outputText.text = "file saved!";
    }
}