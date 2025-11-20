using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using UnityEngine;
using SFB;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    
    public XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectSettings));
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
            //saveText = inputField.text;
            //outputText.text = "file saved!";
            //System.IO.File.WriteAllText(openFile.CurrentFilePath, saveText);
        }
    }
}
