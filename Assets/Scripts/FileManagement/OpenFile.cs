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
    
    [SerializeField] private DrawingManager drawingManager;
    [SerializeField] private CanvasRenderManager canvasRenderManager;
    [SerializeField] private CharachterCollectionManager charachterCollectionManager;
    [SerializeField] private EnableCanvasOnLoad enableCanvasOnLoad;

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


            canvasRenderManager.RenderCharacterToTexture(ch);
        }


        ApplyBrushSettings(loaded);

        CharacterData activeChar = loaded.characters.Find(
            c => c.character == loaded.currentCharacter);

        if (activeChar == null && loaded.characters.Count > 0)
            activeChar = loaded.characters[0];

        canvasRenderManager.SetCurrentCharacter(activeChar);

        charachterCollectionManager.LoadCharachtersIntoCollection();
        
        enableCanvasOnLoad.EnableCanvases();


        ProjectSettingsManager.Instance.DisplayProjectSettings();
        outputText.text = "File opened!";
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
