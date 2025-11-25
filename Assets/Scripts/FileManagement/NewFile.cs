using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
public class NewFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    private OpenFile openFile;
    private string DefaultXMLText = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<ProjectSettings>\n    <projectName>NewProject</projectName>\n    <ballColor>green</ballColor>\n    <ballPositionx>0</ballPositionx>\n    <ballPositiony>0</ballPositiony>\n</ProjectSettings>";
    public XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));
    
    private void Awake()
    {
        openFile = GetComponent<OpenFile>();
    }
    public void OnClickCreateNewFile()
    {
        // Let user type file name inside panel
        string path = StandaloneFileBrowser.SaveFilePanel("Create New File", "", "NewProject", "xml" );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, DefaultXMLText);
            openFile.CurrentFilePath = path;
            
            using (Stream reader = new FileStream(path, FileMode.Open))
            {
                ProjectSettingsManager.Instance.currentProjectSettings  = (ProjectSettings)xmlSerializer.Deserialize(reader);
            }

            outputText.text = "File Created!";
            ProjectSettingsManager.Instance.DisplayProjectSettings();
        }
    }
}
