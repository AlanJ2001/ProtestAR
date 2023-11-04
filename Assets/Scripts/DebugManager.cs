using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public TextMeshProUGUI logText;

    private void Start()
    {
        // Clear the initial text content
        logText.text = "";
    }

    // Method to append log messages to the Text UI element
    public void AppendLogMessage(string message)
    {
        logText.text += message + "\n";
    }
}
