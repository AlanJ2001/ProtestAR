using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    public Button myButton; // Reference to the Button component

    void Start()
    {
        // // Check if the button reference is assigned
        // if (myButton != null)
        // {
        //     // Change the button's color to red
        //     ChangeButtonColor(Color.red);
        // }
        // else
        // {
        //     Debug.LogError("Button reference not assigned. Please assign the button in the Unity Editor.");
        // }
    }

    // Function to change the color of the button
    void ChangeButtonColor(Color newColor)
    {
        // Get the button's image component
        Image buttonImage = myButton.GetComponent<Image>();

        // Check if the button has an image component
        if (buttonImage != null)
        {
            // Update the color of the button's image
            buttonImage.color = newColor;
        }
        else
        {
            Debug.LogError("Button does not have an Image component. Please make sure the button has an Image component to change its color.");
        }
    }
}