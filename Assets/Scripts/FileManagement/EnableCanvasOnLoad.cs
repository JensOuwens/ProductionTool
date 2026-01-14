using UnityEngine;
using System.Collections.Generic;

public class EnableCanvasOnLoad : MonoBehaviour
{
    [SerializeField] private List<GameObject> canvases;

    private bool enabledOnce = false;

    public void EnableCanvases()
    {
        if (enabledOnce)
            return;

        foreach (var canvas in canvases)
        {
            if (canvas != null)
                canvas.SetActive(true);
        }

        enabledOnce = true;
    }
}