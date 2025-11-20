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

            ProjectSettings loadedSettings;

            using (FileStream stream = new FileStream(CurrentFilePath, FileMode.Open))
            {
                loadedSettings = (ProjectSettings)xmlSerializer.Deserialize(stream);
            }

            // OPTIONAL: Show XML text inside input field
            inputField.text = File.ReadAllText(CurrentFilePath);

            outputText.text = "File opened!";
        }
    }
}