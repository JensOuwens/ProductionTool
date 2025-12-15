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

        if (path.Length > 0)
        {
            CurrentFilePath = path[0];
            

            using (Stream reader = new FileStream(CurrentFilePath, FileMode.Open))
            {
                ProjectSettings loaded = (ProjectSettings)xmlSerializer.Deserialize(reader);
                ProjectSettingsManager.Instance.currentProjectSettings = loaded;

                // APPLY STROKES TO DRAWING MANAGER
                var dm = FindObjectOfType<DrawingManager>();
                if (loaded.strokes == null)
                    loaded.strokes = new List<Stroke>();

                dm.SetStrokes(loaded.strokes);
            }

            
            ProjectSettingsManager.Instance.DisplayProjectSettings();

            outputText.text = "File opened!";
        }
    }
}