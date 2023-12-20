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

    public IEnumerator GetAllIDs(System.Action<List<string>> onCallback)
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
            List<string> cloudAnchorIDs = new List<string>();

            foreach (var childSnapshot in snapshot.Children)
            {
                string anchorID = childSnapshot.Child("cloudAnchorID").Value.ToString();
                cloudAnchorIDs.Add(anchorID);
            }

            onCallback.Invoke(cloudAnchorIDs);
        }
    }

    public string test()
    {
        return "database working";
    }
}
