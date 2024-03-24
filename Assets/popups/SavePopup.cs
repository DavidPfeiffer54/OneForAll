using UnityEngine;
using TMPro;

public class SavePopup : MonoBehaviour
{
    private LevelData levelData;
    public TMP_InputField saveFileNameInput;

    // Method to handle the Save button click
    public void setLevelData(LevelData _levelData)
    {
        levelData = _levelData;
    }
    public void OnSaveButtonClick()
    {
        string saveFileName = saveFileNameInput.text;
        // Call the saveFile function with the entered filename
        JsonParser.Instance.saveFile(saveFileName, levelData);
        Debug.Log("SAVING TO:");
        Debug.Log(saveFileName);
        // Optionally, deactivate the popup after saving
        gameObject.SetActive(false);
    }

    // Method to handle the Cancel button click
    public void OnCancelButtonClick()
    {
        // Optionally, implement cancel logic
        gameObject.SetActive(false);
    }
}