using UnityEngine;
using SFB;
using TMPro;
using UnityEngine.Windows;

public class SaveFile : MonoBehaviour
{
    [SerializeField] private TMP_Text outputText;
    private string textext = "new text";

    public void OnClickSaveFile()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "untitled", "Txt");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, textext);
        }
    }
}
