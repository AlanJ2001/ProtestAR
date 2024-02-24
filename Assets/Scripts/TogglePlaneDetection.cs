using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TogglePlaneDetection : MonoBehaviour
{
    public ARPlaneManager planeManager;

    void Start()
    {
        // Get the ARPlaneManager component attached to the ARSessionOrigin
    }

    public void DisablePlaneDetection()
    {
        // Check if the ARPlaneManager is not null
        if (planeManager != null)
        {
            // Disable plane detection
            planeManager.enabled = false;
        }
        else
        {
            // Log a warning if the ARPlaneManager is not found
            Debug.LogWarning("ARPlaneManager not found on ARSessionOrigin.");
        }
    }
}
