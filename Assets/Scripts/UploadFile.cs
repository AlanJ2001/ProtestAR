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
    public byte[] bytes;

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

			bytes = FileBrowserHelpers.ReadBytesFromFile( FileBrowser.Result[0] );

			// string destinationPath = Path.Combine( Application.persistentDataPath, FileBrowserHelpers.GetFilename( FileBrowser.Result[0] ) );
			// FileBrowserHelpers.CopyFile( FileBrowser.Result[0], destinationPath );

            //Editing Metadata
            var newMetadata = new MetadataChange();
            newMetadata.ContentType = "image/jpeg";

            StorageReference uploadRef = storageReference.Child("uploads/" + GetTimeWithRandomDigits() + ".jpeg");
            Debug.Log("File upload started");
            uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => {
                if (task.IsFaulted || task.IsCanceled){
                    Debug.Log(task.Exception.ToString());
                }
                else{
                    Debug.Log("File uploaded successfully");
                }
            });
		}
	}

    // Call this function to get the concatenated string
    private string GetTimeWithRandomDigits()
    {
        // Get the current time as a string
        string currentTime = System.DateTime.Now.ToString("HH:mm:ss");

        // Generate a random string of digits with length 5
        string randomDigits = GenerateRandomDigits(5);

        // Concatenate the time and random digits
        string result = currentTime + randomDigits;

        return result;
    }

    // Function to generate a random string of digits with a specified length
    private string GenerateRandomDigits(int length)
    {
        System.Random random = new System.Random();
        string randomDigits = "";

        for (int i = 0; i < length; i++)
        {
            randomDigits += random.Next(0, 10).ToString();
        }

        return randomDigits;
    }
}
