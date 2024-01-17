using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using System;

public class UploadFile : MonoBehaviour
{
    FirebaseStorage storage;
    StorageReference storageReference;
    byte[] bytes;
    public string filename;
    DebugManager db;
    public string FinalPath;
    public Texture2D uploadedTexture;
    string fileExtension;
    public GameObject errorMessage;

    void Start()
	{
        db = FindObjectOfType<DebugManager>();
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://ar-projects-403118.appspot.com");
	}

    public void OnButtonClick()
    {
        errorMessage.transform.localScale = new Vector3(0f, 0f, 0f);
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
                fileExtension = GetFileExtension(FinalPath);
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
        newMetadata.ContentType = "image/" + fileExtension;

        filename = "uploads/" + GetTimeWithRandomDigits() + "." + fileExtension;

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

    public static string GetFileExtension(string filePath)
    {
        // Check if the file path is not null or empty
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        try
        {
            // Use Path.GetExtension method to get the file extension
            string extension = Path.GetExtension(filePath);

            // Check if the extension is not empty
            if (!string.IsNullOrEmpty(extension))
            {
                // Remove the leading dot from the extension
                extension = extension.TrimStart('.');
            }

            return extension;
        }
        catch (Exception ex)
        {
            // Handle exceptions, e.g., if the file path is invalid
            return null;
        }
    }

}
