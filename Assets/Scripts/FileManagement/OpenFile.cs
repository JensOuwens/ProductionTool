using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
using System.IO;

/// <summary>
/// TODO
/// start on wich canvas is the current one
/// display all charachters in the collection. (probably charachterCollectionManager call)
/// </summary>
public class OpenFile : MonoBehaviour
{   
    [SerializeField] private TMP_Text outputText;

    public string CurrentFilePath;
    private XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));

    public void OnClickOpenFile()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "xml", false);

        if (path.Length == 0)
            return;

        CurrentFilePath = path[0];

        ProjectSettings loaded;

        using (Stream reader = new FileStream(CurrentFilePath, FileMode.Open))
        {
            loaded = (ProjectSettings)xmlSerializer.Deserialize(reader);
        }

        // APPLY SETTINGS
        ProjectSettingsManager.Instance.currentProjectSettings = loaded;

        // ENSURE CHARACTERS EXIST
        CharacterDefaults.EnsureCharacters(loaded);

        // FIND LAST USED CHARACTER
        CharacterData activeChar = null;

        if (!string.IsNullOrEmpty(loaded.currentCharacter))
        {
            activeChar = loaded.characters
                .Find(c => c.character == loaded.currentCharacter);
        }

        // FALLBACK SAFETY
        if (activeChar == null && loaded.characters.Count > 0)
            activeChar = loaded.characters[0];

        // APPLY TO DRAWING MANAGER
        var dm = FindObjectOfType<DrawingManager>();
        dm.SetCurrentCharacter(activeChar);

        // REBUILD CHARACTER BUTTON UI
        FindObjectOfType<CharachterCollectionManager>()
            .LoadCharachtersIntoCollection();

        // UPDATE PROJECT SETTINGS UI
        ProjectSettingsManager.Instance.DisplayProjectSettings();

        outputText.text = "File opened!";
    }
}