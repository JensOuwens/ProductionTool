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
    }

    private void OnCharacterSelected(CharacterData characterData)
    {
        // SET CURRENT CHARACTER IN DRAWING MANAGER
        drawingManager.SetCurrentCharacter(characterData);
    }
}
