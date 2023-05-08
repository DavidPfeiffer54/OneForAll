using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public int level_num;
    public int width;
    public int height;
    public Dictionary<Vector3, GameObject> cells;
    public Dictionary<Vector3, GameObject> walls;
    public Dictionary<Vector3, GameObject> playerStarts;
    public Dictionary<Vector3, GameObject> goals;


    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject gridWallPrefab;
    [SerializeField] private GameObject playerStartPrefab;
    [SerializeField] private GameObject gridGoalPrefab;

    public Dictionary<string, Color> colorDictionary;



    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void awake()
    {

        Debug.Log("LOADING!!!!!!!!!!");
        cells = new Dictionary<Vector3, GameObject>();
        walls = new Dictionary<Vector3, GameObject>();
        goals = new Dictionary<Vector3, GameObject>();
        playerStarts = new Dictionary<Vector3, GameObject>();

        Debug.Log("LOADING12345");

    }
    public void setLevel(int  level_num, LevelData ld)
    {
        colorDictionary = new Dictionary<string, Color>();
        colorDictionary["red"]=Color.red;
        colorDictionary["blue"]=Color.blue;
        colorDictionary["green"]=Color.green;

        cells = new Dictionary<Vector3, GameObject>();
        walls = new Dictionary<Vector3, GameObject>();
        goals = new Dictionary<Vector3, GameObject>();
        playerStarts = new Dictionary<Vector3, GameObject>();

        foreach(Vector3 cell in ld.cells)
        {
            Debug.Log(gridCellPrefab);
            cells[cell] = Instantiate (gridCellPrefab, cell*5 + new Vector3(0,0,-2.5f), Quaternion.identity);
            cells[cell].GetComponent<GridCell>().setPosition((int)cell.x, (int)cell.y, (int)cell.z);
            cells[cell].transform.parent = transform;
            cells[cell].gameObject.name = "Grid Space(" + cell.x.ToString() + " , " + cell.y.ToString()  + " , " + cell.z.ToString() + " )";
            Debug.Log(cell.GetType());
        }
        foreach(Vector3 wall in ld.walls)
        {
            Debug.Log(gridWallPrefab);
            walls[wall] = Instantiate (gridWallPrefab, wall*5 + new Vector3(0,0,-2.5f), Quaternion.identity);
            walls[wall].GetComponent<GameWall>().setPosition((int)wall.x, (int)wall.y, (int)wall.z);
            walls[wall].transform.parent = transform;
            walls[wall].gameObject.name = "Grid Wall(" + wall.x.ToString() + " , " + wall.y.ToString()  + " , " + wall.z.ToString() + " )";
            Debug.Log(walls[wall]);
        }
        foreach(LocColorData ps in ld.playerStarts)
        {
            playerStarts[ps.loc] = Instantiate(playerStartPrefab, ps.loc*5 + new Vector3(0,0,-2.5f), Quaternion.identity);
            playerStarts[ps.loc].GetComponent<PlayerStart>().setPosition(new Vector3(ps.loc.x, ps.loc.y, ps.loc.z));
            playerStarts[ps.loc].GetComponent<PlayerStart>().setCol(colorDictionary[ps.col]);
            playerStarts[ps.loc].transform.parent = transform;
            playerStarts[ps.loc].gameObject.name = "Player Start(" + ps.loc.ToString() + " )";
            Debug.Log(playerStarts[ps.loc]);            
        }
        foreach(LocColorData gg in ld.goals)
        {
            goals[gg.loc] = Instantiate(gridGoalPrefab, gg.loc*5, Quaternion.identity);
            goals[gg.loc].GetComponent<GameGoal>().setPosition(new Vector3(gg.loc.x, gg.loc.y, gg.loc.z));
            goals[gg.loc].GetComponent<GameGoal>().setCol(colorDictionary[gg.col]);
            goals[gg.loc].transform.parent = transform;
            goals[gg.loc].gameObject.name = "GOAL(" + gg.loc.ToString() + " )";
            Debug.Log(goals[gg.loc]);            
        }
    }

}
