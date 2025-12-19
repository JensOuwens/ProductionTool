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

        FindObjectOfType<DrawingManager>()
            .SetCurrentCharacter(ps.characters[0]);

        FindObjectOfType<CharachterCollectionManager>()
            .LoadCharachtersIntoCollection();

        ProjectSettingsManager.Instance.DisplayProjectSettings();
        outputText.text = "File Created!";
    }

    void ApplyBrushSettings(ProjectSettings ps)
    {
        DrawingManager.brushSize = ps.brushSize;
        DrawingManager.opacity = ps.brushOpacity;
        DrawingManager.hardness = ps.brushHardness;
        DrawingManager.spacing = ps.brushSpacing;
        DrawingManager.brushShape = ps.brushShape;
        DrawingManager.currentTool = ps.currentTool;
        DrawingManager.calligraphyAngle = ps.calligraphyAngle;
        DrawingManager.calligraphyAspect = ps.calligraphyAspect;

        if (ColorUtility.TryParseHtmlString(ps.brushColor, out Color c))
            DrawingManager.brushColor = c;
    }
}