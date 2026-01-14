using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;

public class OpenFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;

    public string CurrentFilePath;
    private XmlSerializer xmlSerializer = new(typeof(ProjectSettings));
    
    private DrawingManager drawingManager;

    private void Awake()
    {
        drawingManager = FindObjectOfType<DrawingManager>();
    }

    public void OnClickOpenFile()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel(
            "Open File", "", "xml", false);

        if (path.Length == 0)
            return;

        CurrentFilePath = path[0];

        ProjectSettings loaded;
        using (Stream reader = new FileStream(CurrentFilePath, FileMode.Open))
            loaded = (ProjectSettings)xmlSerializer.Deserialize(reader);

        CharacterDefaults.EnsureCharacters(loaded);
        ProjectSettingsManager.Instance.currentProjectSettings = loaded;
        
        foreach (var ch in loaded.characters)
        {
            ch.cachedTexture = new Texture2D(drawingManager.totalPixelsX,
                drawingManager.totalPixelsY,
                TextureFormat.RGBA32, false);


            drawingManager.RenderCharacterToTexture(ch);
        }


        ApplyBrushSettings(loaded);

        CharacterData activeChar = loaded.characters.Find(
            c => c.character == loaded.currentCharacter);

        if (activeChar == null && loaded.characters.Count > 0)
            activeChar = loaded.characters[0];

        FindObjectOfType<DrawingManager>()
            .SetCurrentCharacter(activeChar);

        FindObjectOfType<CharachterCollectionManager>()
            .LoadCharachtersIntoCollection();
        
        FindObjectOfType<EnableCanvasOnLoad>()
            .EnableCanvases();


        ProjectSettingsManager.Instance.DisplayProjectSettings();
        outputText.text = "File opened!";
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
