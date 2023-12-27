using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudAnchor : MonoBehaviour
{
    public string cloudAnchorID;
    public double latitude;
    public double longitude;
    public string imageFileName;

    public CloudAnchor(string cloudAnchorID, double latitude, double longitude, string imageFileName)
    {
        this.cloudAnchorID = cloudAnchorID;
        this.latitude = latitude;
        this.longitude = longitude;
        this.imageFileName = imageFileName;
    }
}
