using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudAnchor : MonoBehaviour
{
    public string cloudAnchorID;
    public double latitude;
    public double longitude;
    public string imageFileName;
    public double angleSliderNumber;
    public double scaleSliderNumber;

    public CloudAnchor(string cloudAnchorID, double latitude, double longitude, string imageFileName, double angleSliderNumber, double scaleSliderNumber)
    {
        this.cloudAnchorID = cloudAnchorID;
        this.latitude = latitude;
        this.longitude = longitude;
        this.imageFileName = imageFileName;
        this.angleSliderNumber = angleSliderNumber;
        this.scaleSliderNumber = scaleSliderNumber;
    }
}
