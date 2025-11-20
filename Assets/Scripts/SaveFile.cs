using System;
using System.Windows.Forms;
using UnityEngine;
using SFB;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private TMP_InputField inputField;
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
            saveText = inputField.text;
            outputText.text = "file saved!";
            System.IO.File.WriteAllText(openFile.CurrentFilePath, saveText);
        }
    }
}
