using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;

public class QualityIcon : MonoBehaviour
{
    Image imageComponent;
    int imageIndex = 0;
    int previousImageIndex = 0;
    public Sprite[] images;
    public ARAnchorManager anchorManager;

    // Start is called before the first frame update
    void Start()
    {
        imageComponent = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        LogFeatureMapQuality();
        UpdateImage();
    }

    void UpdateImage()
    {
        // Check if the current index is within the bounds of the array
        if (imageIndex >= 0 && imageIndex < images.Length)
        {
            // Update the source image
            imageComponent.sprite = images[imageIndex];
        }
        else
        {
            Debug.LogWarning("Invalid imageIndex value. Please use an index within the range of the 'images' array.");
        }
    }

    private void LogFeatureMapQuality()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Quaternion cameraRotation = cameraTransform.rotation;
        Pose cameraPose = new Pose(cameraPosition, cameraRotation);
        var quality = anchorManager.EstimateFeatureMapQualityForHosting(cameraPose);
        if (quality == FeatureMapQuality.Insufficient)
        {
            imageIndex = 2;
        }
        else if (quality == FeatureMapQuality.Sufficient)
        {
            imageIndex = 1;
        }
        else if (quality == FeatureMapQuality.Good)
        {
            imageIndex = 0;
        }
    }
}
