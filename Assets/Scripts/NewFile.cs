using System.IO;
using UnityEngine;
using SFB;
using TMPro;
public class NewFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private TMP_InputField inputField;
    private OpenFile openFile;
    
    private void Awake()
    {
        openFile = GetComponent<OpenFile>();
    }
    public void OnClickCreateNewFile()
    {
        // Let user type file name inside panel
        string path = StandaloneFileBrowser.SaveFilePanel("Create New File", "", "NewFile", "txt" );

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, "TEXT");
            openFile.CurrentFilePath = path;

            outputText.text = "File Created!";
            inputField.text = File.ReadAllText(path);
        }
    }
}
