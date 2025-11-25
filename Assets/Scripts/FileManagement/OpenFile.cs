using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
using System.IO;

public class OpenFile : MonoBehaviour
{   
    [SerializeField] private TMP_Text outputText;

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
                CurrentProjectSettings = (ProjectSettings)xmlSerializer.Deserialize(reader);
            }
            
            Debug.Log(CurrentProjectSettings.projectName + CurrentProjectSettings.ballColor + CurrentProjectSettings.ballPositionx + CurrentProjectSettings.ballPositiony);

            outputText.text = "File opened!";
        }
    }
}