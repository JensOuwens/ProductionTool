using HSVPicker;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// TODO
/// once more brushes are added and more settings, add them here too
/// </summary>
/// 
public class ProjectSettingsManager : MonoBehaviour
{
    public static ProjectSettingsManager Instance { get; private set; }

    [System.NonSerialized] public ProjectSettings currentProjectSettings = new ProjectSettings();

    [SerializeField] private TMP_InputField ProjectNameInputField;
    [SerializeField] private ColorPicker BrushColorInputField;
    [SerializeField] private TMP_InputField BrushSizeInputField;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // avoid duplicate singletons
            return;
        }

        Instance = this;
        
        currentProjectSettings.brushSize = 4;

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

    private void ChangeBrushColor(Color newColor)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(newColor);
        currentProjectSettings.brushColor = "#" + hexColor;

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
        BrushSizeInputField.text = currentProjectSettings.brushSize.ToString();
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(currentProjectSettings.brushColor, out parsedColor))
        {
            BrushColorInputField.CurrentColor = parsedColor;
        }
        else
        {
            Debug.LogWarning("Failed to parse brush color: " + currentProjectSettings.brushColor);
            BrushColorInputField.CurrentColor = Color.black; // fallback
            BrushSizeInputField.text = currentProjectSettings.brushSize.ToString();
        }
    }
}
