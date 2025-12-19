using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CharacterData
{
    public string character;
    public List<Stroke> strokes = new List<Stroke>();

    public CharacterData() { }
    public int codepoint;

    public CharacterData(string c)
    {
        character = c;
        strokes = new List<Stroke>();
    }
}

/// <summary>
/// TODO
/// add different brushes (enum?)
/// add more brush settings
/// </summary>
[Serializable]
public class ProjectSettings
{
    public string projectName;
    public string brushColor;
    public float brushSize;
    public string currentCharacter;

    // ONE ENTRY PER CHARACTER
    public List<CharacterData> characters = new List<CharacterData>();

    public ProjectSettings()
    {
        characters = new List<CharacterData>();
    }
}