using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System.Linq;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DisplayPosts : MonoBehaviour
{
    public DebugManager debugManagerScript;
    public DatabaseManager databaseManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        debugManagerScript.AppendLogMessage("display posts works");
        PrintCloudAnchors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintCloudAnchors()
    {
        StartCoroutine(databaseManagerScript.GetAllIDs((List<Dictionary<string, string>> cloudAnchorsList) => 
        {
                foreach (Dictionary<string, string> item in cloudAnchorsList)
                {
                        debugManagerScript.AppendLogMessage(item["cloudAnchorID"]);
                }
        }));
    }
}
