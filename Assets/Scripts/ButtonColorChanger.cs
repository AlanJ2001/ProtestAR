using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    private Button myButton;
    public bool selected = false;
    public string cloudAnchorId;
    PostsToDelete postsToDeleteScript;

    void Start()
    {
        postsToDeleteScript = FindObjectOfType<PostsToDelete>();
    }

    // Function to change the color of the button
    public void ChangeButtonColor()
    {
        myButton = GetComponent<Button>();
        Image tickImage = myButton.transform.Find("tick").GetComponent<Image>();
        float newAlpha;
        if (selected == false)
        {
            newAlpha = 1f;
            selected = true;
            postsToDeleteScript.postsToDelete.Add(cloudAnchorId);
        }
        else
        {
            newAlpha = 0f;
            selected = false;
            postsToDeleteScript.postsToDelete.Remove(cloudAnchorId);
        }
        Color currentColor = tickImage.color;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
        tickImage.color = newColor;
    }
}