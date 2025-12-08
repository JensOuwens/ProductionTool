using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectSettings
{
    public string projectName;
    public string ballColor;
    public float ballPositionx;
    public float ballPositiony;

    // IMPORTANT: XML MUST SEE THIS
    public List<Stroke> strokes = new List<Stroke>();

    public ProjectSettings()
    {
        // ensure strokes always created so never null
        strokes = new List<Stroke>();
    }
}