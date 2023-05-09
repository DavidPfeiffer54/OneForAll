using UnityEngine;
using System.Collections.Generic;

public class JsonParser : MonoBehaviour
{
    public List<LevelData> levelData;

    void Start()
    {

    }
    public void readFile()
    {
        // Load the JSON file from the Resources folder
        TextAsset jsonFile = Resources.Load<TextAsset>("levelDescriptor");
        string jsonString = jsonFile.text;

        // Parse the JSON file using JsonUtility class
        LevelWrapper wrapper = JsonUtility.FromJson<LevelWrapper>(jsonString);
        levelData = wrapper.levels;   
    }
}

[System.Serializable]
public class LevelData {
    public Vector3[] cells;
    public Vector3[] walls;
    public LocColorData[] goals;
    public LocColorData[] playerStarts;
    public Vector3[] buttons;
    public LocColorData[] colorChange;
}

[System.Serializable]
public class LocColorData {
    public Vector3 loc;
    public string col;
}

[System.Serializable]
public class LevelWrapper
{
    public List<LevelData> levels;
}