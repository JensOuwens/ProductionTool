using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows;
/// <summary>
/// TODO
/// add saving for all different letters/charachters
/// save wich of the letters is the current one.
/// </summary>
public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;

    private XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));
    private OpenFile openFile;
    
    private string saveText;

    private void Awake()
    {
        openFile = GetComponent<OpenFile>();
    }

    public void OnClickSaveFile()
    {
        if (!string.IsNullOrEmpty(openFile.CurrentFilePath))
        {
            ProjectSettings SaveProjectSettings = ProjectSettingsManager.Instance.GetProjectSettings();

            // GET STROKES FROM DRAWING MANAGER
            SaveProjectSettings.strokes = FindObjectOfType<DrawingManager>().GetStrokes();

            using (FileStream stream = new FileStream(openFile.CurrentFilePath, FileMode.Create))
            {
                xmlSerializer.Serialize(stream, SaveProjectSettings);
            }

            outputText.text = "file saved!";
        }
    }

}
