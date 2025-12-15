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
    private XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));

    private void Awake()
    {
        openFile = GetComponent<OpenFile>();
    }

    public void OnClickCreateNewFile()
    {
        string path = StandaloneFileBrowser.SaveFilePanel(
            "Create New File", "", "NewProject", "xml");

        if (string.IsNullOrEmpty(path))
            return;

        ProjectSettings ps = new ProjectSettings
        {
            projectName = "NewProject",
            brushColor = "#000000",
            brushSize = 0
        };

        // CREATE ALL CHARACTERS
        CharacterDefaults.EnsureCharacters(ps);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            xmlSerializer.Serialize(stream, ps);
        }

        openFile.CurrentFilePath = path;
        ProjectSettingsManager.Instance.currentProjectSettings = ps;

        FindObjectOfType<DrawingManager>()
            .SetCurrentCharacter(ps.characters[0]);

        ProjectSettingsManager.Instance.DisplayProjectSettings();
        outputText.text = "File Created!";
    }
}