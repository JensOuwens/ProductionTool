using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CharachterCollectionManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject CharachterCollectionContent;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private TMP_Text currentCharacterText;

    
    [SerializeField] private DrawingManager drawingManager;
    [SerializeField] private CanvasRenderManager canvasRenderManager;
    

    public void LoadCharachtersIntoCollection()
    {

        foreach (Transform child in CharachterCollectionContent.transform)
            Destroy(child.gameObject);

        ProjectSettings ps = ProjectSettingsManager.Instance.currentProjectSettings;


        CharacterDefaults.EnsureCharacters(ps);


        foreach (CharacterData cd in ps.characters)
        {
            GameObject btnObj =
                Instantiate(characterButtonPrefab, CharachterCollectionContent.transform);

            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            Button btn = btnObj.GetComponent<Button>();

            string character = cd.character;
            txt.text = character;

            btn.onClick.AddListener(() =>
            {
                OnCharacterSelected(cd);
            });
        }
        

        CharacterData startChar = null;

        if (!string.IsNullOrEmpty(ps.currentCharacter))
        {
            startChar = ps.characters
                .Find(c => c.character == ps.currentCharacter);
        }


        if (startChar == null && ps.characters.Count > 0)
            startChar = ps.characters[0];


        if (startChar != null)
            OnCharacterSelected(startChar);

    }

    private void OnCharacterSelected(CharacterData characterData)
    {
        ProjectSettings ps = ProjectSettingsManager.Instance.currentProjectSettings;


        ps.currentCharacter = characterData.character;


        canvasRenderManager.SetCurrentCharacter(characterData);


        canvasRenderManager.LoadCharacterHistory(characterData);


        if (currentCharacterText != null)
            currentCharacterText.text = characterData.character;
    }

    
    

}
