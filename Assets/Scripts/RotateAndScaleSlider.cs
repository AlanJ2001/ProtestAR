using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateAndScaleSlider : MonoBehaviour
{
    public placementIndicator placementIndicatorScript;
    // public GameObject image;
    public Slider rotationSlider;
    public Slider scaleSlider;
    private float angleSliderNumber;
    private float scaleSliderNumber;
    private Vector3 originalScale;
    DebugManager db;
    Quaternion initialRotation;

    void Start()
    {
        originalScale = new Vector3(1, 1, 1);
        // initialRotation = Quaternion.Euler(90, 0, 0);
        db = FindObjectOfType<DebugManager>();
    }

    // Update is called once per frame
    void Update()
    {
        initialRotation = placementIndicatorScript.initialRotation;
        angleSliderNumber = rotationSlider.value * 360f;
        if (placementIndicatorScript.instantiatedImage != null)
        {
            placementIndicatorScript.instantiatedImage.transform.rotation = Quaternion.identity;
            Quaternion newRotation = Quaternion.AngleAxis(angleSliderNumber, placementIndicatorScript.instantiatedImage.transform.forward);
            placementIndicatorScript.instantiatedImage.transform.rotation = initialRotation * newRotation;
        }

        scaleSliderNumber = scaleSlider.value;
        Vector3 newScale = originalScale * scaleSliderNumber;
        if (placementIndicatorScript.instantiatedImage != null)
        {
            placementIndicatorScript.instantiatedImage.transform.localScale = newScale;
        }
    }
}
