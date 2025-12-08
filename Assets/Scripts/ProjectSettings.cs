using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ProjectSettings
{
    public string projectName;
    public string brushColor;
    public float brushSize;

    // IMPORTANT: XML MUST SEE THIS
    public List<Stroke> strokes = new List<Stroke>();

    public ProjectSettings()
    {
        // ensure strokes always created so never null
        strokes = new List<Stroke>();
    }
}