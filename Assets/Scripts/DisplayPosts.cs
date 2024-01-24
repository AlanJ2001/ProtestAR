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
    public GameObject imagePrefab; // Reference to your image prefab
    public Transform gridParent; // Parent transform for grid layout
    public float spacing = 10f; // Spacing between images
    public int columns = 2; // Number of columns in the grid
    public int rows = 4; // Number of rows in the grid
    // Start is called before the first frame update
    void Start()
    {
        debugManagerScript.AppendLogMessage("display posts works");
        GetPostsAndDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetPostsAndDisplay()
    {
        StartCoroutine(databaseManagerScript.GetAllIDs((List<Dictionary<string, string>> cloudAnchorsList) => 
        {
                GenerateImageGrid(cloudAnchorsList);
        }));
    }

    private void GenerateImageGrid(List<Dictionary<string, string>> cloudAnchorsList)
    {
        var index = 0;
        var len = cloudAnchorsList.Count;
        string cloudAnchorId;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (index < len)
                {
                    cloudAnchorId = cloudAnchorsList[index]["cloudAnchorID"];
                }
                else
                {
                    return;
                }
                // Instantiate image prefab
                GameObject imageObject = Instantiate(imagePrefab, gridParent);

                // Calculate position based on row and column
                float posX = col * (imagePrefab.GetComponent<RectTransform>().sizeDelta.x + spacing) + 250;
                float posY = -row * (imagePrefab.GetComponent<RectTransform>().sizeDelta.y + spacing) - 250;

                // Set position
                imageObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);

                ButtonColorChanger buttonColorChangerScript = imageObject.GetComponent<ButtonColorChanger>();
                buttonColorChangerScript.cloudAnchorId = cloudAnchorId;
                index += 1;
            }
        }
    }
}
