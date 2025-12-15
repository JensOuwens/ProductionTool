using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
/// <summary>
/// TODO
/// make new start template with all charachters
/// load all charachters (call charactercollectionmanager)
/// </summary>
public class NewFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    private OpenFile openFile;
    private string DefaultXMLText =
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
        "<ProjectSettings>\n" +
        "    <projectName>NewProject</projectName>\n" +
        "    <brushColor>#000000</brushColor>\n" +
        "    <brushSize>0</brushSize>\n" +
        "    <strokes />\n" +
        "</ProjectSettings>";

    private XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));
    
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
                
                var dm = FindObjectOfType<DrawingManager>();
                dm.ClearCanvas();
            }

            outputText.text = "File Created!";
            ProjectSettingsManager.Instance.DisplayProjectSettings();
        }
    }
}
