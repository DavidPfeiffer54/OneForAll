using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] levels;
    public int currentLevelID;
    GameObject currentLevel;


    [SerializeField] private GameObject LevelPrefab;
    [SerializeField] private GameObject jsonParserPrefab;


    private Dictionary<string, int> cells;
    private Dictionary<string, int> walls;
    private Dictionary<string, int> goals;
    private Dictionary<string, int> buttons;
    private Dictionary<string, int> players;

    private GameObject jsonParser;

    void Start()
    {

    }
    void Awake()
    {
        Debug.Log("LOADING lm");
        jsonParser = Instantiate(jsonParserPrefab, new Vector3(0, 0), Quaternion.identity);
        jsonParser.GetComponent<JsonParser>().readFile();
        //SetUpLevel(1);
    }
    public void setUpLevel(int _level_num)
    {
        Destroy(currentLevel);
        currentLevel = Instantiate(LevelPrefab, new Vector3(0, 0), Quaternion.identity);
        currentLevel.GetComponent<LevelInfo>().setLevel(0, jsonParser.GetComponent<JsonParser>().levelData[_level_num]);
    }
    public void setUpEditorLevel()
    {
        Destroy(currentLevel);
        GameObject jsonParserEdit = Instantiate(jsonParserPrefab, new Vector3(0, 0), Quaternion.identity);
        jsonParserEdit.GetComponent<JsonParser>().readEditorFile();
        currentLevel = Instantiate(LevelPrefab, new Vector3(0, 0), Quaternion.identity);
        currentLevel.GetComponent<LevelInfo>().setLevel(0, jsonParserEdit.GetComponent<JsonParser>().levelData[0]);
    }
    public void setUpLevel()
    {

    }
    public void moveCell(GameObject cellToMove, Vector3 newLocation)
    {
        currentLevel.GetComponent<LevelInfo>().moveCell(cellToMove, newLocation);
    }

    public bool isCellAt(Vector3 loc)
    {
        //return currentLevel.GetComponent<LevelInfo>().cells.ContainsKey(loc);
        return getCells().Any(c => c.GetComponent<GridCell>().getLoc() == loc);

    }
    public GameObject getCellAt(Vector3 loc)
    {
        if (!isCellAt(loc)) return null;
        //return currentLevel.GetComponent<LevelInfo>().cells[loc];
        return getCells().FirstOrDefault(obj => obj.GetComponent<GridCell>().getLoc() == loc);
    }
    public bool isWallAt(Vector3 loc)
    {
        return currentLevel.GetComponent<LevelInfo>().walls.ContainsKey(loc);
    }
    public bool isPlayerAt(Vector3 loc)
    {//???????????????????????????????????????????
        return false;
    }
    public bool isCellMoveTo(Vector3 loc)
    {

        return getCells().Any(c => c.GetComponent<GridCell>().getMoveTo() == loc);
    }
    public bool isGoalAt(Vector3 loc)
    {
        return currentLevel.GetComponent<LevelInfo>().goals.ContainsKey(loc);
    }
    public GameObject getColorChangeAt(Vector3 loc)
    {
        if (currentLevel.GetComponent<LevelInfo>().colorChanges.ContainsKey(loc))
            return currentLevel.GetComponent<LevelInfo>().colorChanges[loc];
        return null;
    }

    public GameObject getCurrentLevel()
    {
        return currentLevel;
    }

    public Vector3 findNextAvaliableLocation(Vector3 loc)
    {
        //if(isCellAt(loc)){Debug.Log("findNextAvaliableLocation theres already something here, ");return loc;}
        //for(int i = 0; i < 10; i++)
        //{
        //    if(isCellAt(loc + new Vector3(0,0,i)))
        //    {
        //        return loc + new Vector3(0,0,i);
        //    }
        //}
        return loc;
    }

    public void setPlayersOnGoals(GameObject[] players)
    {
        foreach (KeyValuePair<Vector3, GameObject> goal in currentLevel.GetComponent<LevelInfo>().goals)
        {
            goal.Value.GetComponent<GameGoal>().setPlayerOn(false);
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PlayerController>().getPosition() == goal.Value.GetComponent<GameGoal>().getPosition()
                   && player.GetComponent<PlayerController>().getCol() == goal.Value.GetComponent<GameGoal>().getCol())
                {
                    goal.Value.GetComponent<GameGoal>().setPlayerOn(true);
                }
            }
        }
    }
    public bool isLevelComplete()
    {
        foreach (KeyValuePair<Vector3, GameObject> goal in currentLevel.GetComponent<LevelInfo>().goals)
        {
            if (goal.Value.GetComponent<GameGoal>().goalCompleted == false)
                return false;
        }
        return true;

    }


    public GameObject[] getCells()
    {
        return currentLevel.GetComponent<LevelInfo>().cells.Values.ToArray();
    }
    public GameObject[] getButtons()
    {
        return currentLevel.GetComponent<LevelInfo>().buttons.Values.ToArray();
    }


    public int getTwoStarThreshold()
    {
        return currentLevel.GetComponent<LevelInfo>().twoStarThreshold;
    }
    public int getThreeStarThreshold()
    {
        return currentLevel.GetComponent<LevelInfo>().threeStarThreshold;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void addWall(Vector3 location)
    {
        currentLevel.GetComponent<LevelInfo>().addWall(location);
    }
}
