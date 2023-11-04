using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;

public class placementIndicator : MonoBehaviour
{
    public ARSessionOrigin sessionOrigin;
    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;
    public GameObject indicator;
    private bool placementPoseIsValid = false; 
    private Pose hitPose;
    public GameObject image;

    public ARAnchorManager anchorManager;

    ARPlane plane;

    DebugManager db;

    // Start is called before the first frame update
    void Start()
    {
        db = FindObjectOfType<DebugManager>();
        db.AppendLogMessage("testing");
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
        GameObject instantiatedImage = Instantiate(image, hitPose.position, hitPose.rotation);
        instantiatedImage.transform.Rotate(90, 0, 0);
        instantiatedImage.AddComponent<ARAnchor>();
        ARAnchor _anchor = anchorManager.AttachAnchor(plane, hitPose);
        instantiatedImage.transform.SetParent(_anchor.transform);
        db.AppendLogMessage("anchor created");
        CreatePromise(_anchor);
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
            plane = planeManager.GetPlane(raycastHits[0].trackableId);
            hitPose = raycastHits[0].pose;
        }
    }

    public void CreatePromise(ARAnchor _anchor)
    {
        HostCloudAnchorPromise cloudAnchorPromise = anchorManager.HostCloudAnchorAsync(_anchor, 1);
        StartCoroutine(CheckCloudAnchorPromise(cloudAnchorPromise));
    }
    private IEnumerator CheckCloudAnchorPromise(HostCloudAnchorPromise promise)
    {
        yield return promise;
        if (promise.State == PromiseState.Cancelled) yield break;
        var result = promise.Result;
        /// Use the result of your promise here.
    }
}
