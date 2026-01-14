using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CharacterData
{
    public string character;
    public List<Stroke> strokes = new List<Stroke>();

    [XmlIgnore]
    [System.NonSerialized] public Texture2D cachedTexture;
    
    public CharacterData() { }
    public int codepoint;

    public CharacterData(string c)
    {
        character = c;
        strokes = new List<Stroke>();
    }
}

[Serializable]
public class ProjectSettings
{
    public string projectName;

    // CURRENT BRUSH STATE (saved with project)
    public string brushColor;
    public int brushSize;

    public float brushOpacity;
    public float brushHardness;
    public float brushSpacing;

    public BrushShape brushShape;
    public ToolType currentTool;

    public float calligraphyAngle;
    public float calligraphyAspect;

    public string currentCharacter;

    // ONE ENTRY PER CHARACTER
    public List<CharacterData> characters = new();

    public ProjectSettings()
    {
        brushColor = "#000000";
        brushSize = 6;

        brushOpacity = 1f;
        brushHardness = 1f;
        brushSpacing = 0.25f;

        brushShape = BrushShape.Circle;
        currentTool = ToolType.Brush;

        calligraphyAngle = 45f;
        calligraphyAspect = 0.3f;
    }
}
