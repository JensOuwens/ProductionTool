using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
using System.IO;

public class OpenFile : MonoBehaviour
{   
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private TMP_InputField inputField;

    public string CurrentFilePath;
    public XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));

    public void OnClickOpenFile()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "xml", false);

        if (path.Length > 0)
        {
            CurrentFilePath = path[0];

            ProjectSettings CurrentProjectSettings = new ProjectSettings();

            using (Stream reader = new FileStream(CurrentFilePath, FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                CurrentProjectSettings = (ProjectSettings)xmlSerializer.Deserialize(reader);
            }
            
            Debug.Log(CurrentProjectSettings.projectName + CurrentProjectSettings.ballColor + CurrentProjectSettings.ballPositionx + CurrentProjectSettings.ballPositiony);

            // OPTIONAL: Show XML text inside input field
            //inputField.text = File.ReadAllText(CurrentFilePath);

            outputText.text = "File opened!";
        }
    }
}