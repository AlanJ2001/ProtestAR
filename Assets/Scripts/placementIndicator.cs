using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System.Linq;
using Firebase;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine.UI;
using UnityEngine.Networking;

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
    DatabaseManager database;
    List<ResolveCloudAnchorPromise> resolveRequests;
    List<string> previousCloudAnchorsList;
    ARAnchor anchorToHost;
    GameObject instantiatedImage;
    public UploadFile uploadFileScript;
    FirebaseStorage storage;
    StorageReference storageReference;
    HashSet<string> instantiatedAnchorsSet;

    void Start()
    {
        db = FindObjectOfType<DebugManager>();
        database = FindObjectOfType<DatabaseManager>();
        db.AppendLogMessage("start debugging");
        resolveRequests = new List<ResolveCloudAnchorPromise>();
        previousCloudAnchorsList = new List<string>();
        storage = FirebaseStorage.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://ar-projects-403118.appspot.com");
        instantiatedAnchorsSet = new HashSet<string>();
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        if (placementPoseIsValid && Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            float y = touch.position.y;
            if (y > 301.4)
            {
                PlaceObject();
            }
        }
        FindAndResolveCloudAnchors();
        LogFeatureMapQuality();
    }

    private void LogFeatureMapQuality()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraPosition = cameraTransform.position;
        Quaternion cameraRotation = cameraTransform.rotation;
        Pose cameraPose = new Pose(cameraPosition, cameraRotation);
        // db.UpdateLogMessage(anchorManager.EstimateFeatureMapQualityForHosting(cameraPose).ToString());
    }

    private void FindAndResolveCloudAnchors()
    {
        StartCoroutine(database.GetAllIDs((List<Dictionary<string, string>> cloudAnchorsList) => 
        {
            List<string> idList = cloudAnchorsList.Select(item => item["cloudAnchorID"]).ToList();
            if (!previousCloudAnchorsList.SequenceEqual(idList))
            {
                foreach (ResolveCloudAnchorPromise item in resolveRequests)
                {
                    item.Cancel();
                }
                resolveRequests = new List<ResolveCloudAnchorPromise>();
                foreach (Dictionary<string, string> item in cloudAnchorsList)
                {
                    if (!instantiatedAnchorsSet.Contains(item["cloudAnchorID"]))
                    {
                        CreatePromiseResolveAnchor(item["cloudAnchorID"], item["filename"]);
                    }
                }
                previousCloudAnchorsList = new List<string>(idList);
            }
        }));
    }

    private void PlaceObject()
    {
        if (instantiatedImage != null)
        {
            Destroy(instantiatedImage);
        }
        instantiatedImage = Instantiate(image, hitPose.position, hitPose.rotation);
        instantiatedImage.transform.Rotate(90, 0, 0);
        instantiatedImage.AddComponent<ARAnchor>();
        ARAnchor _anchor = anchorManager.AttachAnchor(plane, hitPose);
        instantiatedImage.transform.SetParent(_anchor.transform);
        db.AppendLogMessage("anchor created");
        // CreatePromise(_anchor);
        anchorToHost = _anchor;

        Texture2D uploadedTexture = new Texture2D(2, 2); // You may need to adjust the size
        uploadedTexture.LoadImage(uploadFileScript.bytes);

        if (instantiatedImage != null)
        {
            Renderer renderer = instantiatedImage.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = renderer.material;
                if (material != null)
                {
                    material.mainTexture = uploadedTexture;
                }
            }
        }
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
        db.AppendLogMessage(result.CloudAnchorState.ToString());
        db.AppendLogMessage(result.CloudAnchorId);
        uploadFileScript.uploadSelectedImage();
        database.CreateCloudAnchor(result.CloudAnchorId, 55.32, -4.05, uploadFileScript.filename);
        ResetScene();
    }

    private void ResetScene()
    {
        GameObject[] imagePrefabs = GameObject.FindGameObjectsWithTag("imagePrefab");

        foreach (GameObject imagePrefab in imagePrefabs)
        {
            Destroy(imagePrefab);
        }
        previousCloudAnchorsList = new List<string>();
        instantiatedAnchorsSet = new HashSet<string>();
    }

    public void CreatePromiseResolveAnchor(string id, string filename)
    {
        ResolveCloudAnchorPromise cloudAnchorPromise = anchorManager.ResolveCloudAnchorAsync(id);
        resolveRequests.Add(cloudAnchorPromise);
        StartCoroutine(CheckResolveCloudAnchorPromise(cloudAnchorPromise, filename, id));
    }
    
    private IEnumerator CheckResolveCloudAnchorPromise(ResolveCloudAnchorPromise promise, string filename, string id)
    {
        yield return promise;
        if (promise.State == PromiseState.Cancelled) yield break;
        var result = promise.Result;
        db.AppendLogMessage(result.CloudAnchorState.ToString());
        Pose pose = result.Anchor.pose;
        db.AppendLogMessage(pose.ToString());

        StorageReference image = storageReference.Child(filename);
        db.AppendLogMessage(filename);
        db.AppendLogMessage("filename above");

        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task => 
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(task.Result.ToString(), pose, id));
            }
            else
            {
                Debug.Log(task.Exception);
            }
        });

        // GameObject instantiatedImage = Instantiate(image, pose.position, pose.rotation);
        // instantiatedImage.transform.Rotate(90, 0, 0);
    }

    IEnumerator LoadImage(string MediaUrl, Pose pose, string id)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            GameObject instantiatedImage = Instantiate(image, pose.position, pose.rotation);
            instantiatedImage.transform.Rotate(90, 0, 0);
            instantiatedAnchorsSet.Add(id);
            if (instantiatedImage != null)
            {
                Renderer renderer = instantiatedImage.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;
                    if (material != null)
                    {
                        material.mainTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    }
                }
            }
            // rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }

    public void test(){
        CreatePromise(anchorToHost);
        anchorToHost = null;
    }
}
