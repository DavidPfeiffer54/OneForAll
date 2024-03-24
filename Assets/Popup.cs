using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CustomPopup : MonoBehaviour
{
    public GameObject popupPanel;
    public InputField filenameInputField;

    private void Update()
    {
        // Show/hide the popup when the M key is pressed
        if (Input.GetKeyDown(KeyCode.M))
        {
            popupPanel.SetActive(!popupPanel.activeSelf);
        }
    }
}