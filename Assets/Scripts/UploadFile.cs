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
    byte[] bytes;
    public string filename;
    DebugManager db;
    public string FinalPath;
    public Texture2D uploadedTexture;

    void Start()
	{
        db = FindObjectOfType<DebugManager>();
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://ar-projects-403118.appspot.com");
	}

    public void OnButtonClick()
    {
        LoadFile();
    }

    public void LoadFile()
    {        
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("Operation Cancelled");
            }
            else
            {
                FinalPath = path;
                db.AppendLogMessage(FinalPath);
                bytes = File.ReadAllBytes(FinalPath);
                uploadedTexture = new Texture2D(2, 2);
                uploadedTexture.LoadImage(bytes);
            }
        }, NativeFilePicker.ConvertExtensionToFileType("png"), NativeFilePicker.ConvertExtensionToFileType("jpg"), NativeFilePicker.ConvertExtensionToFileType("jpeg"));

    }

    public void uploadSelectedImage()
    {
        //Editing Metadata
        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "image/jpeg";

        filename = "uploads/" + GetTimeWithRandomDigits() + ".jpeg";

        StorageReference uploadRef = storageReference.Child(filename);
        Debug.Log("File upload started");
        uploadRef.PutBytesAsync(bytes, newMetadata).ContinueWithOnMainThread((task) => {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                Debug.Log("File uploaded successfully");
            }
        });
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
