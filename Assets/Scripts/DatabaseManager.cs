using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

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
        db.AppendLogMessage("database manager");
        dbreference = FirebaseDatabase.DefaultInstance.RootReference;
        db.AppendLogMessage(dbreference.ToString());

    }

    public void CreateCloudAnchor(string cloudAnchorID, double latitude, double longitude)
    {
        CloudAnchor newCloudAnchor = new CloudAnchor(cloudAnchorID, latitude, longitude);
        string json = JsonUtility.ToJson(newCloudAnchor);
        db.AppendLogMessage(json);
        dbreference.Child("cloud anchors").Child(localID).SetRawJsonValueAsync(json);
    }

    public string test()
    {
        return "database working";
    }
}
