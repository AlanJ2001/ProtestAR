using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    private Button myButton;
    private bool selected = false;

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
        }
        else
        {
            newAlpha = 0f;
            selected = false;
        }
        Color currentColor = tickImage.color;
        Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
        tickImage.color = newColor;
    }
}