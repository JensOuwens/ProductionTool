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
            

            using (Stream reader = new FileStream(CurrentFilePath, FileMode.Open))
            {
                ProjectSettingsManager.Instance.currentProjectSettings  = (ProjectSettings)xmlSerializer.Deserialize(reader);
            }
            
            ProjectSettingsManager.Instance.DisplayProjectSettings();

            outputText.text = "File opened!";
        }
    }
}