using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ProjectSettingsManager : MonoBehaviour
{
    public static ProjectSettingsManager Instance { get; private set; }
    
    [System.NonSerialized] public ProjectSettings currentProjectSettings = new ProjectSettings();
    
    [SerializeField] private TMP_InputField ProjectNameInputField;
    [SerializeField] private TMP_InputField BrushColorInputField;
    [SerializeField] private TMP_InputField BrushSizeInputField;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // avoid duplicate singletons
            return;
        }
        Instance = this;
        
        // Subscribe to value changed events
        ProjectNameInputField.onValueChanged.AddListener(ChangeProjectName);
        BrushColorInputField.onValueChanged.AddListener(ChangeBrushColor);
        BrushSizeInputField.onValueChanged.AddListener(ChangeBrushSize);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events
        ProjectNameInputField.onValueChanged.RemoveAllListeners();
        BrushColorInputField.onValueChanged.RemoveAllListeners();
        BrushSizeInputField.onValueChanged.RemoveAllListeners();
    }

// Your existing methods remain unchanged
    private void ChangeProjectName(string newText)
    {
        currentProjectSettings.projectName = newText;
    }

    private void ChangeBrushColor(string newText)
    {
        currentProjectSettings.brushColor = newText;
        DrawingManager.UpdateBrushColor();
    }

    private void ChangeBrushSize(string newText)
    {
        if (int.TryParse(newText, out int parsedValue))
        {
            currentProjectSettings.brushSize = parsedValue;
            DrawingManager.UpdateBrushSize();
        }
        else
        {
            // Handle invalid input gracefully
            currentProjectSettings.brushSize = 0; // Default value
        }
    }
    


    public ProjectSettings GetProjectSettings()
    {
        return currentProjectSettings;
    }

    public void DisplayProjectSettings()
    {
        ProjectNameInputField.text = currentProjectSettings.projectName;
        BrushColorInputField.text = currentProjectSettings.brushColor;
        BrushSizeInputField.text = currentProjectSettings.brushSize.ToString();
    }
}
