using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudAnchor : MonoBehaviour
{
    public string cloudAnchorID;
    public double latitude;
    public double longitude;

    public CloudAnchor(string cloudAnchorID, double latitude, double longitude)
    {
        this.cloudAnchorID = cloudAnchorID;
        this.latitude = latitude;
        this.longitude = longitude;
    }
}
