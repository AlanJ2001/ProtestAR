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
    StorageReference storageReference;
    FirebaseStorage storage;

    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://ar-projects-403118.appspot.com");
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
        string filename;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (index < len)
                {
                    cloudAnchorId = cloudAnchorsList[index]["cloudAnchorID"];
                    filename = cloudAnchorsList[index]["filename"];
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

                StorageReference imageRef = storageReference.Child(filename);

                imageRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task => 
                {
                    if (!task.IsFaulted && !task.IsCanceled)
                    {
                        StartCoroutine(LoadImage(task.Result.ToString(), imageObject));
                    }
                    else
                    {
                        Debug.Log(task.Exception);
                    }
                });
            }
        }
    }

    IEnumerator LoadImage(string MediaUrl, GameObject instantiatedImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            if (instantiatedImage != null)
            {
                Image image = instantiatedImage.GetComponent<Image>();
                if (image != null)
                {
                    Texture2D newTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    if (newTexture != null)
                    {
                        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.one * 0.5f);
                        image.sprite = newSprite;
                        newTexture = null;
                    }
                }
            }
            // rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}
