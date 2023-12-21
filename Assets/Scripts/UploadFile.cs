using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;

public class UploadFile : MonoBehaviour
{
    FirebaseStorage storage;
    StorageReference storageReference;

    // public placementIndicator placementIndicatorScript;

    void Start()
	{
		FileBrowser.SetFilters( true, new FileBrowser.Filter( "Images", ".jpg", ".png" ), new FileBrowser.Filter( "Text Files", ".txt", ".pdf" ) );

		FileBrowser.SetDefaultFilter( ".jpg" );

		FileBrowser.SetExcludedExtensions( ".lnk", ".tmp", ".zip", ".rar", ".exe" );

		FileBrowser.AddQuickLink( "Users", "C:\\Users", null );

        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://ar-projects-403118.appspot.com"); //complete

	}

    public void OnButtonClick()
    {
        StartCoroutine( ShowLoadDialogCoroutine());
    }


	IEnumerator ShowLoadDialogCoroutine()
	{
		yield return FileBrowser.WaitForLoadDialog( FileBrowser.PickMode.FilesAndFolders, true, null, null, "Load Files and Folders", "Load" );

		Debug.Log( FileBrowser.Success );

		if( FileBrowser.Success )
		{
			for( int i = 0; i < FileBrowser.Result.Length; i++ )
				Debug.Log( FileBrowser.Result[i] );

			byte[] bytes = FileBrowserHelpers.ReadBytesFromFile( FileBrowser.Result[0] );

			// string destinationPath = Path.Combine( Application.persistentDataPath, FileBrowserHelpers.GetFilename( FileBrowser.Result[0] ) );
			// FileBrowserHelpers.CopyFile( FileBrowser.Result[0], destinationPath );

            //Editing Metadata
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = "image/jpeg";

            StorageReference uploadRef = storageReference.Child("uploads/newFile.jpeg");
            Debug.Log("File upload started");
            uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => {
                if (task.IsFaulted || task.IsCanceled){
                    Debug.Log(task.Exception.ToString());
                }
                else{
                    Debug.Log("File uploaded successfully");
                    // placementIndicatorScript.image = YourConversionMethod(bytes);
                }
            });
		}
	}

    // private GameObject YourConversionMethod(byte[] bytes)
    // {
    //     // Implement your conversion logic here
    //     // For example, create a new GameObject with a SpriteRenderer
    //     GameObject imageObject = new GameObject("ImageObject");
    //     SpriteRenderer spriteRenderer = imageObject.AddComponent<SpriteRenderer>();
        
    //     // Load the image bytes into a Texture2D
    //     Texture2D texture = new Texture2D(2, 2);
    //     texture.LoadImage(bytes);
        
    //     // Assign the texture to the SpriteRenderer
    //     spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

    //     return imageObject;
    // }
}
