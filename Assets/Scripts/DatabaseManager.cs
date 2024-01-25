using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;


public class DatabaseManager : MonoBehaviour
{

    private DatabaseReference dbreference;
    private string localID;
    DebugManager db;

    // Start is called before the first frame update
    void Start()
    {
        db = FindObjectOfType<DebugManager>();
        localID = SystemInfo.deviceUniqueIdentifier;
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateCloudAnchor(string cloudAnchorID, double latitude, double longitude, string imageFileName, double angleSliderNumber, double scaleSliderNumber)
    {
        CloudAnchor newCloudAnchor = new CloudAnchor(cloudAnchorID, latitude, longitude, imageFileName, angleSliderNumber, scaleSliderNumber);
        string json = JsonUtility.ToJson(newCloudAnchor);
        // db.AppendLogMessage(json);
        dbreference.Child("cloud anchors").Child(cloudAnchorID).SetRawJsonValueAsync(json);
    }

    public IEnumerator GetID(System.Action<string> onCallback)
    {
        var cloudAnchorID = dbreference.Child("cloud anchors").Child("228699459dc45296b6255cbdc5ab51ca2024939f").Child("cloudAnchorID").GetValueAsync();
        yield return new WaitUntil(predicate: () => cloudAnchorID.IsCompleted);

        if (cloudAnchorID != null)
        {
            DataSnapshot snapshot = cloudAnchorID.Result;
            onCallback.Invoke(snapshot.Value.ToString());
        }
    }

    public IEnumerator GetAllIDs(System.Action<List<Dictionary<string, string>>> onCallback)
    {
        var cloudAnchorsRef = dbreference.Child("cloud anchors");

        var cloudAnchorIDTask = cloudAnchorsRef.GetValueAsync();
        yield return new WaitUntil(() => cloudAnchorIDTask.IsCompleted);

        if (cloudAnchorIDTask.Exception != null)
        {
            Debug.LogError($"Error fetching cloud anchor IDs: {cloudAnchorIDTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = cloudAnchorIDTask.Result;
            List<Dictionary<string, string>> cloudAnchorIDs = new List<Dictionary<string, string>>();


            foreach (var childSnapshot in snapshot.Children)
            {
                string anchorID = childSnapshot.Child("cloudAnchorID").Value.ToString();
                string filename = childSnapshot.Child("imageFileName").Value.ToString();
                string angleSliderNumber = childSnapshot.Child("angleSliderNumber").Value.ToString();
                string scaleSliderNumber = childSnapshot.Child("scaleSliderNumber").Value.ToString();
                Dictionary<string, string> anchorData = new Dictionary<string, string>
                {
                    { "cloudAnchorID", anchorID },
                    { "filename", filename },
                    { "angleSliderNumber", angleSliderNumber },
                    { "scaleSliderNumber", scaleSliderNumber },
                };
                cloudAnchorIDs.Add(anchorData);
            }

            onCallback.Invoke(cloudAnchorIDs);
        }
    }

    public async void DeleteCloudAnchor(string cloudAnchorID)
    {
        // Assuming 'CloudAnchors' is the name of the node in your database where the CloudAnchor objects are stored
        DatabaseReference cloudAnchorRef = dbreference.Child("cloud anchors").Child(cloudAnchorID);

        try
        {
            await cloudAnchorRef.RemoveValueAsync();
            Debug.Log("CloudAnchor deleted successfully");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete CloudAnchor: " + e.Message);
        }
    }

    public void test(string str)
    {
        db.AppendLogMessage(str);
    }
}
