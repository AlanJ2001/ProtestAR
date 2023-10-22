using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class placementIndicator : MonoBehaviour
{
    public ARSessionOrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject indicator;
    private bool placementPoseIsValid = false; 
    private Pose hitPose;
    public GameObject image;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (placementPoseIsValid && Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        image = Instantiate(image, hitPose.position, hitPose.rotation);
        image.transform.Rotate(90, 0, 0);
    }

    private void UpdatePlacementIndicator() 
    {
        if (placementPoseIsValid)
        {
            indicator.SetActive(true);
            indicator.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            indicator.transform.Rotate(90, 0, 0);
        }
        else
        {
            indicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var ray = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var raycastHits = new List<ARRaycastHit>();
        raycastManager.Raycast(ray, raycastHits, TrackableType.PlaneWithinPolygon);

        placementPoseIsValid = raycastHits.Count > 0;
        if (placementPoseIsValid)
        {
            hitPose = raycastHits[0].pose;
        }
    }
}
