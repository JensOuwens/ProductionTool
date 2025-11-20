using UnityEngine;
using SFB;
using TMPro;

public class OpenFile : MonoBehaviour
{   
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private TMP_InputField inputField;
    [System.NonSerialized] public string CurrentFilePath;

    public void OnClickOpenFile()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "txt", false);
        if (path.Length > 0)
        {
            CurrentFilePath = path[0];
            
            string fileContent = System.IO.File.ReadAllText(CurrentFilePath);
            
            outputText.text = "File opened!";
            inputField.text = fileContent;
        }
    }
}
