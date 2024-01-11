using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLoading : MonoBehaviour {

    private RectTransform rectComponent;
    private Image imageComp;
    public float rotateSpeed = 200f;
    public placementIndicator placementIndicatorScript;

    // Use this for initialization
    void Start () {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        if (placementIndicatorScript.hosting)
        {
            transform.parent.localScale = new Vector3(2.1f, 2.1f, 2.1f);
            rectComponent.Rotate(0f, 0f, -(rotateSpeed * Time.deltaTime));
        }else
        {
            transform.parent.localScale = Vector3.zero;
        }
    }
}
