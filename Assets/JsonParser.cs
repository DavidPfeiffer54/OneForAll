using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class JsonParser
{
    public List<LevelData> levelData;

    private static JsonParser _instance;

    public static JsonParser Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new JsonParser();
            }
            return _instance;
        }
    }

    public void readFile(string filename)
    {
        // Load the JSON file from the Resources folder
        TextAsset jsonFile = Resources.Load<TextAsset>(filename);
        string jsonString = jsonFile.text;

        // Parse the JSON file using JsonUtility class
        LevelWrapper wrapper = JsonUtility.FromJson<LevelWrapper>(jsonString);
        levelData = wrapper.levels;
    }
    public void saveFile(string filename, LevelData newLevel)
    {
        levelData = new List<LevelData>();
        levelData.Add(newLevel);
        // Create a wrapper to hold the list of levels
        LevelWrapper wrapper = new LevelWrapper();
        wrapper.levels = levelData;

        // Serialize the wrapper object to JSON format
        string jsonString = JsonUtility.ToJson(wrapper, true);

        string directoryPath = Path.Combine(Application.dataPath, "LevelDataFiles");
        filename = $"level_{GetNextAvailableValue():D3}_{filename}";
        string filePath = Path.Combine(directoryPath, filename);

        // Write the JSON string to the file
        File.WriteAllText(filePath, jsonString);

        Debug.Log("Level data saved to: " + filePath);
    }
    private int GetNextAvailableValue()
    {
        string[] fileNames = Directory.GetFiles("Assets/LevelDataFiles");

        // Filter filenames that start with "level_"
        fileNames = fileNames.Where(fileName =>
            Path.GetFileNameWithoutExtension(fileName).StartsWith("level_")
        ).ToArray();

        // Extract numeric part from each filename
        var numericParts = fileNames.Select(fileName =>
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string[] parts = fileNameWithoutExtension.Split('_');
            return int.Parse(parts[1]); // Assuming the numeric part is the second part
        });

        // Find the maximum numeric value
        int maxNumericValue = numericParts.DefaultIfEmpty(0).Max();

        // Increment to get the next available value
        int nextAvailableValue = maxNumericValue + 1;

        return nextAvailableValue;
    }
}


[System.Serializable]
public class LevelData
{
    public int twoStarThreshold;
    public int threeStarThreshold;
    public Vector3[] cells;
    public Vector3[] walls;
    public LocColorData[] goals;
    public LocColorData[] playerStarts;
    public ButtonPressedData[] buttons;
    public LocColorData[] colorChange;
}

[System.Serializable]
public class LocColorData
{
    public Vector3 loc;
    public string col;
}

[System.Serializable]
public class ButtonPressedData
{
    public Vector3 buttonLoc;
    public Vector3 buttonMoveToStart;
    public Vector3 buttonMoveToEnd;
    public string col;
}

[System.Serializable]
public class LevelWrapper
{
    public List<LevelData> levels;
}