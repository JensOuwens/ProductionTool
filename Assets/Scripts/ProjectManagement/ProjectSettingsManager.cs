using HSVPicker;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ProjectSettingsManager : MonoBehaviour
{
    public static ProjectSettingsManager Instance { get; private set; }

    [System.NonSerialized] public ProjectSettings currentProjectSettings = new ProjectSettings();

    [SerializeField] private TMP_InputField ProjectNameInputField;
    [SerializeField] private ColorPicker BrushColorInputField;
    [SerializeField] private DrawingManager drawingManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        
        currentProjectSettings.brushSize = 4;

        currentProjectSettings.currentTool = ToolType.Brush;


        ProjectNameInputField.onValueChanged.AddListener(ChangeProjectName);
        BrushColorInputField.onValueChanged.AddListener(ChangeBrushColor);
    }

    private void OnDestroy()
    {

        ProjectNameInputField.onValueChanged.RemoveAllListeners();
        BrushColorInputField.onValueChanged.RemoveAllListeners();
    }

    private void ChangeProjectName(string newText)
    {
        currentProjectSettings.projectName = newText;
    }

    private void ChangeBrushColor(Color newColor)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(newColor);
        currentProjectSettings.brushColor = "#" + hexColor;


        drawingManager.brushColor = newColor; 
    }

    


    public ProjectSettings GetProjectSettings()
    {
        return currentProjectSettings;
    }

    public void DisplayProjectSettings()
    {
        ProjectNameInputField.text = currentProjectSettings.projectName;
        Color parsedColor;
        FindObjectOfType<BrushUI>().RefreshFromProjectSettings();
        if (ColorUtility.TryParseHtmlString(currentProjectSettings.brushColor, out parsedColor))
        {
            BrushColorInputField.CurrentColor = parsedColor;
        }
        else
        {
            Debug.LogWarning("Failed to parse brush color: " + currentProjectSettings.brushColor);
            BrushColorInputField.CurrentColor = Color.black; 
        }
    }
}
