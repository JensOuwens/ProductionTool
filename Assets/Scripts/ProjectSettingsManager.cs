using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ProjectSettingsManager : MonoBehaviour
{
    [System.NonSerialized] public ProjectSettings currentProjectSettings = new ProjectSettings();
    
    [SerializeField] private TMP_InputField ProjectNameInputField;
    [SerializeField] private TMP_InputField BallColorInputField;
    [SerializeField] private TMP_InputField BallPositionxInputField;
    [SerializeField] private TMP_InputField BallPositionyInputField;
    
    private void Awake()
    {
        // Subscribe to value changed events
        ProjectNameInputField.onValueChanged.AddListener(ChangeProjectName);
        BallColorInputField.onValueChanged.AddListener(ChangeBallColor);
        BallPositionxInputField.onValueChanged.AddListener(ChangeBallPositionx);
        BallPositionyInputField.onValueChanged.AddListener(ChangeBallPositiony);
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events
        ProjectNameInputField.onValueChanged.RemoveAllListeners();
        BallColorInputField.onValueChanged.RemoveAllListeners();
        BallPositionxInputField.onValueChanged.RemoveAllListeners();
        BallPositionyInputField.onValueChanged.RemoveAllListeners();
    }

// Your existing methods remain unchanged
    private void ChangeProjectName(string newText)
    {
        currentProjectSettings.projectName = newText;
    }

    private void ChangeBallColor(string newText)
    {
        currentProjectSettings.ballColor = newText;
    }

    private void ChangeBallPositionx(string newText)
    {
        if (int.TryParse(newText, out int parsedValue))
        {
            currentProjectSettings.ballPositionx = parsedValue;
        }
        else
        {
            // Handle invalid input gracefully
            currentProjectSettings.ballPositionx = 0; // Default value
        }
    }

    private void ChangeBallPositiony(string newText)
    {
        if (int.TryParse(newText, out int parsedValue))
        {
            currentProjectSettings.ballPositiony = parsedValue;
        }
        else
        {
            // Handle invalid input gracefully
            currentProjectSettings.ballPositiony = 0; // Default value
        }
    }


    public ProjectSettings GetProjectSettings()
    {
        return currentProjectSettings;
    }
}
