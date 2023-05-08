using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] levels;
    public int currentLevelID;
    [SerializeField] private GameObject LevelPrefab;
    GameObject currentLevel;


    private Dictionary<string, int> cells;
    private Dictionary<string, int> walls;
    private Dictionary<string, int> goals;
    private Dictionary<string, int> buttons;
    private Dictionary<string, int> players;

    [SerializeField] private GameObject jsonParserPrefab;
    private GameObject jsonParser;

    void Start()
    {

    }
    void Awake()
    {
        Debug.Log("LOADING lm");
        jsonParser = Instantiate (jsonParserPrefab, new Vector3(0,0), Quaternion.identity);
        jsonParser.GetComponent<JsonParser>().readFile();
        //SetUpLevel(1);
    }
    public void setUpLevel(int _level_num)
    {
        Destroy(currentLevel);
        currentLevel = Instantiate (LevelPrefab, new Vector3(0,0), Quaternion.identity);
        currentLevel.GetComponent<LevelInfo>().setLevel(0, jsonParser.GetComponent<JsonParser>().levelData[_level_num]);
    }
    public void setUpLevel()
    {

    }

    public bool isCellAt(Vector3 loc)
    {
        return currentLevel.GetComponent<LevelInfo>().cells.ContainsKey(loc);
    }
    public bool isWallAt(Vector3 loc)
    {
        return currentLevel.GetComponent<LevelInfo>().walls.ContainsKey(loc);
    }
    public bool isPlayerAt(Vector3 loc)
    {
        return false;
    }
    public bool isGoalAt(Vector3 loc)
    {
        return false;
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
        foreach(KeyValuePair<Vector3, GameObject> goal in currentLevel.GetComponent<LevelInfo>().goals)
        {
            goal.Value.GetComponent<GameGoal>().setPlayerOn(false);
            foreach(GameObject player in players)
            {
                if(player.GetComponent<PlayerController>().getPosition() == goal.Value.GetComponent<GameGoal>().getPosition()
                   && player.GetComponent<PlayerController>().getCol() == goal.Value.GetComponent<GameGoal>().getCol())
                {
                    goal.Value.GetComponent<GameGoal>().setPlayerOn(true);
                }
            }
        }
    }
    public bool isLevelComplete()
    {
        foreach(KeyValuePair<Vector3, GameObject> goal in currentLevel.GetComponent<LevelInfo>().goals)
        {
            if(goal.Value.GetComponent<GameGoal>().goalCompleted == false)
                return false;
        }
        return true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
