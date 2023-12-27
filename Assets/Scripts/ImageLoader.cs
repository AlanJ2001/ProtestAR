using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class ImageLoader : MonoBehaviour
{
    RawImage rawImage;
    FirebaseStorage storage;
    StorageReference storageReference;

    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://ar-projects-403118.appspot.com/uploads");

        StorageReference image = storageReference.Child("12:51:0768599.jpeg");

        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task => 
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(task.Result.ToString()));
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });
    }

    IEnumerator LoadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}
