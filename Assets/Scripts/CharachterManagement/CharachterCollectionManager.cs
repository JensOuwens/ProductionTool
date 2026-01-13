using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// TODO
/// load all charachters into the collection
/// switch between different letters
/// </summary>

public class CharachterCollectionManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject CharachterCollectionContent;
    [SerializeField] private GameObject characterButtonPrefab;
    [SerializeField] private TMP_Text currentCharacterText;

    
    private DrawingManager drawingManager;

    private void Awake()
    {
        drawingManager = FindObjectOfType<DrawingManager>();
    }

    public void LoadCharachtersIntoCollection()
    {
        // CLEAR OLD BUTTONS
        foreach (Transform child in CharachterCollectionContent.transform)
            Destroy(child.gameObject);

        ProjectSettings ps = ProjectSettingsManager.Instance.currentProjectSettings;

        // SAFETY
        CharacterDefaults.EnsureCharacters(ps);

        // CREATE BUTTON FOR EACH CHARACTER
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
        
        // RESTORE LAST CHARACTER
        CharacterData startChar = null;

        if (!string.IsNullOrEmpty(ps.currentCharacter))
        {
            startChar = ps.characters
                .Find(c => c.character == ps.currentCharacter);
        }

        // FALLBACK
        if (startChar == null && ps.characters.Count > 0)
            startChar = ps.characters[0];

        // APPLY
        if (startChar != null)
            OnCharacterSelected(startChar);

    }

    private void OnCharacterSelected(CharacterData characterData)
    {
        ProjectSettings ps = ProjectSettingsManager.Instance.currentProjectSettings;

        // SAVE CURRENT CHARACTER
        ps.currentCharacter = characterData.character;

        // UPDATE DRAWING MANAGER
        drawingManager.SetCurrentCharacter(characterData);

        // REBUILD UNDO/REDO STACK FOR THIS CHARACTER
        drawingManager.LoadCharacterHistory(characterData);

        // UPDATE UI TEXT
        if (currentCharacterText != null)
            currentCharacterText.text = characterData.character;
    }

    
    

}
