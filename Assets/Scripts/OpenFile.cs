using UnityEngine;
using SFB;
using TMPro;

public class OpenFile : MonoBehaviour
{   
    [SerializeField] private TMP_Text outputText;

    public void OnClickOpenFile()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "txt", false);
        if (path.Length > 0)
        {
            string filePath = path[0];
            
            string fileContent = System.IO.File.ReadAllText(filePath);
            
            outputText.text = fileContent;
        }
    }
}
