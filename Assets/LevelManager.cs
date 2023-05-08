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
        SetUpLevel(1);
        //cells = new Dictionary<Vector3, GameObject>();
        //walls = new Dictionary<Vector3, GameObject>();
        //goals = new Dictionary<Vector3, GameObject>();
        //buttons = new Dictionary<Vector3, GameObject>();
        //players = new Dictionary<Vector3, GameObject>();
        ////Vector3 myVector = new Vector3(myArray[0], myArray[1], myArray[2]);
        //
        //
        //levels = new GameObject[3];
        //
        //GameObject level = Instantiate(LevelPrefab, new Vector3(0,0), Quaternion.identity);
        //int[,] cl = { {0,0,0}, {0,1,0}, {0,2,0}, {0,3,0}, {0,4,0}, 
        //              {1,0,0}, {1,1,0}, {1,2,0}, {1,3,0}, {1,4,0}, 
        //              {2,0,0}, {2,1,0}, {2,2,0}, {2,3,0}, {2,4,0}, 
        //              {3,0,0}, {3,1,0}, {3,2,0}, {3,3,0}, {3,4,0}, 
        //              {4,0,0}, {4,1,0}, {4,2,0}, {4,3,0}, {4,4,0}
        //            };
        //int[,] wl = { {0,0,1}, {1,0,1}, {0,1,1}, {0,3,1}, {0,4,1}, 
        //              {1,4,1}, {3,4,1}, {4,4,1}, {4,3,1}, {4,1,1}, 
        //              {4,0,1}, {3,0,1}, {2,2,1} };
        //int[,] pl = { {0,2,1}, {2,4,1} };                      
        //int[,] gl = { {1,3,1}, {3,1,1} };
        //int[,] bl = { {2,0,1};
        //level.GetComponent<LevelInfo>().SetLevel(1, 5, 5, cl, wl, gl, pl);
        //levels[0] = level;
        //
        //level = Instantiate(LevelPrefab, new Vector3(0,0), Quaternion.identity);
        //cl = new int[,]{ {0,0,0}, {0,1,0}, {0,2,0}, {0,3,0}, {0,4,0}, 
        //               {1,0,0}, {1,1,0}, {1,2,0}, {1,3,0}, {1,4,0}, 
        //               {2,0,0}, {2,1,0}, {2,2,0}, {2,3,0}, {2,4,0}, 
        //               {3,0,0}, {3,1,0}, {3,2,0}, {3,3,0}, {3,4,0}, 
        //               {4,0,0}, {4,1,0}, {4,2,0}, {4,3,0}, {4,4,0}
        //               };
        //wl = new int[,]{ {0,0,1}, {1,0,1}, {0,1,1}, {0,3,1}, {0,4,1}, 
        //                 {1,4,1}, {3,4,1}, {4,4,1}, {4,3,1}, {4,1,1}, 
        //                 {4,0,1}, {3,0,1}, {2,2,1} 
        //               };
        //gl = new int[,]{ {1,3,1}, {3,1,1} };
        //pl = new int[,]{ {0,2,1}, {2,4,1} };
        //level.GetComponent<LevelInfo>().SetLevel(1, 6, 6, cl, wl, gl, pl);
        //levels[1] = level;
    }
    public void SetUpLevel(int _level_num)
    {
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
