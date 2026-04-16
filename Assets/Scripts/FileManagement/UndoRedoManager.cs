using System.Collections.Generic;
using UnityEngine;

public class UndoRedoManager : MonoBehaviour
{
    public static UndoRedoManager Instance { get; private set; }
    
    private Dictionary<string, Stack<Stroke>> undoStacks = new Dictionary<string, Stack<Stroke>>();
    private Dictionary<string, Stack<Stroke>> redoStacks = new Dictionary<string, Stack<Stroke>>();

    [SerializeField] private DrawingManager drawingManager;
    [SerializeField] private CanvasRenderManager canvasRenderManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private Stack<Stroke> GetUndoStack(string character)
    {
        if (!undoStacks.ContainsKey(character))
            undoStacks[character] = new Stack<Stroke>();
        return undoStacks[character];
    }

    private Stack<Stroke> GetRedoStack(string character)
    {
        if (!redoStacks.ContainsKey(character))
            redoStacks[character] = new Stack<Stroke>();
        return redoStacks[character];
    }

    public void RegisterStroke(Stroke stroke)
    {
        string charKey = drawingManager.GetCurrentCharacter()?.character;
        if (string.IsNullOrEmpty(charKey)) return;

        GetUndoStack(charKey).Push(stroke);
        GetRedoStack(charKey).Clear(); 
    }

    public void Undo()
    {
        var character = drawingManager.GetCurrentCharacter();
        if (character == null) return;

        string charKey = character.character;
        var undoStack = GetUndoStack(charKey);
        var redoStack = GetRedoStack(charKey);

        if (undoStack.Count == 0) return;

        Stroke last = undoStack.Pop();
        redoStack.Push(last);

        if (character.strokes.Contains(last))
            character.strokes.Remove(last);


        canvasRenderManager.RedrawFromStrokes();
    }

    public void Redo()
    {
        var character = drawingManager.GetCurrentCharacter();
        if (character == null) return;

        string charKey = character.character;
        var undoStack = GetUndoStack(charKey);
        var redoStack = GetRedoStack(charKey);

        if (redoStack.Count == 0) return;

        Stroke s = redoStack.Pop();
        undoStack.Push(s);

        character.strokes.Add(s);


        canvasRenderManager.RedrawFromStrokes();
    }


    public void ClearHistory(string character = null)
    {
        if (character != null)
        {
            undoStacks[character]?.Clear();
            redoStacks[character]?.Clear();
        }
        else
        {
            undoStacks.Clear();
            redoStacks.Clear();
        }
    }


    public void OnCharacterSwitched(CharacterData character)
    {
        string charKey = character?.character;
        if (string.IsNullOrEmpty(charKey)) return;

        if (!undoStacks.ContainsKey(charKey))
            undoStacks[charKey] = new Stack<Stroke>();
        if (!redoStacks.ContainsKey(charKey))
            redoStacks[charKey] = new Stack<Stroke>();
    }
}
