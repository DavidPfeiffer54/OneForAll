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

        //LevelData levelData = JsonUtility.FromJson<LevelData>(jsonData);

        // Access the parsed data
        levelData = wrapper.levels;
        Debug.Log(levelData.Count);

        // Loop through each level and print the number of cells
        foreach (LevelData level in levelData)
        {
            Debug.Log(level);
            //Debug.Log(level.walls);
            //Debug.Log(level.cells);
            //Debug.Log(level.cells[0]);
            //Debug.Log(level.cells[1]);
        }      
    }
}


[System.Serializable]
public class LevelData {
    public Vector3[] cells;
    public Vector3[] walls;
    public LocColorData[] goals;
    public LocColorData[] playerStarts;
    public Vector3[] buttons;
    //public List<ColorChangeData> colorChange;
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
//
//[System.Serializable]
//public class LevelData
//{
//    public int[][] cells;
//    public int[][] walls;
//    public int[][] goals;
//    public int[][] playerStarts;
//    public int[][] buttons;
//    public ColorChange[] colorChange;
//}
//
//[System.Serializable]
//public class ColorChange
//{
//    public int[] loc;
//    public string color;
//}