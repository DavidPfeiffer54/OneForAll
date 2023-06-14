using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public int level_num;
    public int width;
    public int height;
    public int twoStarThreshold;
    public int threeStarThreshold;
    public Dictionary<Vector3, GameObject> cells;
    public Dictionary<Vector3, GameObject> walls;
    public Dictionary<Vector3, GameObject> playerStarts;
    public Dictionary<Vector3, GameObject> goals;
    public Dictionary<Vector3, GameObject> colorChanges;
    public Dictionary<Vector3, GameObject> buttons;

    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject gridWallPrefab;
    [SerializeField] private GameObject playerStartPrefab;
    [SerializeField] private GameObject gridGoalPrefab;
    [SerializeField] private GameObject colorChangePrefab;
    [SerializeField] private GameObject buttonPrefab;

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
        cells = new Dictionary<Vector3, GameObject>();
        walls = new Dictionary<Vector3, GameObject>();
        goals = new Dictionary<Vector3, GameObject>();
        playerStarts = new Dictionary<Vector3, GameObject>();
        colorChanges = new Dictionary<Vector3, GameObject>();
        buttons = new Dictionary<Vector3, GameObject>();
    }

    public void moveCell(GameObject cellToMove, Vector3 newLocation)
    {
        cells.Remove(cellToMove.GetComponent<GridCell>().getPosition());
        cells[newLocation] = cellToMove;
    }

    public void setLevel(int  level_num, LevelData ld)
    {
        twoStarThreshold = ld.twoStarThreshold;
        threeStarThreshold = ld.threeStarThreshold;

        colorDictionary = new Dictionary<string, Color>();
        colorDictionary["grey"]=Color.grey;
        colorDictionary["red"]=Color.red;
        colorDictionary["blue"]=Color.blue;
        colorDictionary["green"]=Color.green;
        colorDictionary["gold"]=new Color(255f/255f,200f/255f,0f/255f);
        colorDictionary["tirq"]=new Color(0f/255f,255f/255f,255f/255f);

        cells = new Dictionary<Vector3, GameObject>();
        walls = new Dictionary<Vector3, GameObject>();
        goals = new Dictionary<Vector3, GameObject>();
        playerStarts = new Dictionary<Vector3, GameObject>();
        colorChanges = new Dictionary<Vector3, GameObject>();
        buttons = new Dictionary<Vector3, GameObject>();

        foreach(Vector3 cell in ld.cells)
        {
            cells[cell] = Instantiate (gridCellPrefab, cell*5 + new Vector3(0,0,-2.5f), Quaternion.identity);
            cells[cell].GetComponent<GridCell>().setPosition((int)cell.x, (int)cell.y, (int)cell.z);
            cells[cell].transform.parent = transform;
            cells[cell].gameObject.name = "Grid Space(" + cell.x.ToString() + " , " + cell.y.ToString()  + " , " + cell.z.ToString() + " )";
        }
        foreach(Vector3 wall in ld.walls)
        {
            walls[wall] = Instantiate (gridWallPrefab, wall*5 + new Vector3(0,0,-2.5f), Quaternion.identity);
            walls[wall].GetComponent<GameWall>().setPosition((int)wall.x, (int)wall.y, (int)wall.z);
            walls[wall].transform.parent = transform;
            walls[wall].gameObject.name = "Grid Wall(" + wall.x.ToString() + " , " + wall.y.ToString()  + " , " + wall.z.ToString() + " )";
        }
        foreach(LocColorData ps in ld.playerStarts)
        {
            playerStarts[ps.loc] = Instantiate(playerStartPrefab, ps.loc*5 + new Vector3(0,0,-2.5f), Quaternion.identity);
            playerStarts[ps.loc].GetComponent<PlayerStart>().setPosition(new Vector3(ps.loc.x, ps.loc.y, ps.loc.z));
            playerStarts[ps.loc].GetComponent<PlayerStart>().setCol(colorDictionary[ps.col]);
            playerStarts[ps.loc].transform.parent = transform;
            playerStarts[ps.loc].gameObject.name = "Player Start(" + ps.loc.ToString() + " )";
        }
        foreach(LocColorData gg in ld.goals)
        {
            goals[gg.loc] = Instantiate(gridGoalPrefab, gg.loc*5, Quaternion.identity);
            goals[gg.loc].GetComponent<GameGoal>().setPosition(new Vector3(gg.loc.x, gg.loc.y, gg.loc.z));
            goals[gg.loc].GetComponent<GameGoal>().setCol(colorDictionary[gg.col]);
            goals[gg.loc].transform.parent = transform;
            goals[gg.loc].gameObject.name = "GOAL(" + gg.loc.ToString() + " )";
        }
        foreach(LocColorData ccg in ld.colorChange)
        {
            colorChanges[ccg.loc] = Instantiate(colorChangePrefab, ccg.loc*5, Quaternion.identity);
            colorChanges[ccg.loc].GetComponent<ColorChange>().setPosition(new Vector3(ccg.loc.x, ccg.loc.y, ccg.loc.z));
            colorChanges[ccg.loc].GetComponent<ColorChange>().setCol(colorDictionary[ccg.col]);
            colorChanges[ccg.loc].transform.parent = transform;
            colorChanges[ccg.loc].gameObject.name = "Color Change(" + ccg.loc.ToString() + " )";
        }
        foreach(ButtonPressedData bpd in ld.buttons)
        {
            buttons[bpd.buttonLoc] = Instantiate(buttonPrefab, bpd.buttonLoc*5, Quaternion.identity);
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setPosition(new Vector3(bpd.buttonLoc.x, bpd.buttonLoc.y, bpd.buttonLoc.z));
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setToMoveStart(new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z));
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setToMoveEnd(new Vector3(bpd.buttonMoveToEnd.x, bpd.buttonMoveToEnd.y, bpd.buttonMoveToEnd.z));
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setCol(colorDictionary[bpd.col]);
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setObjectToMove(cells[buttons[bpd.buttonLoc].GetComponent<GameButton>().getToMoveStart()]);
            buttons[bpd.buttonLoc].transform.parent = transform;
            buttons[bpd.buttonLoc].gameObject.name = "Button Press(" + bpd.buttonLoc.ToString() + " )";
        }
    }
}
