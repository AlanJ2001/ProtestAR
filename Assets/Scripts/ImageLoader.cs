using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        rawImage = gameObject.GetComponent<RawImage>();
        StartCoroutine(LoadImage("https://firebasestorage.googleapis.com/v0/b/ar-projects-403118.appspot.com/o/ash-v0_MCllHY9M-unsplash.jpg?alt=media&token=f45f1fdb-af98-4f1e-9990-d472ce06e967"));
    }

    // Update is called once per frame
    void Update()
    {
        
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
