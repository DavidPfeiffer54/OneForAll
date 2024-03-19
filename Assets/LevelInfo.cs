using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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
    private void removeKeyFromDict(Dictionary<Vector3, GameObject> dict, Vector3 pos)
    {
        if (dict.ContainsKey(pos))
        {
            GameObject gameObjectToDestroy = dict[pos];
            Destroy(gameObjectToDestroy);
            dict.Remove(pos);
        }
    }
    public void destoryItemAt(Vector3 pos)
    {

        removeKeyFromDict(cells, pos);
        removeKeyFromDict(walls, pos);
        removeKeyFromDict(goals, pos);
        removeKeyFromDict(playerStarts, pos);
        //removeKeyFromDict(colorChange, pos)
        removeKeyFromDict(buttons, pos);
    }
    public void resetLevel()
    {
        foreach (var cell in cells.Values)
        {
            Debug.Log("Resetting sell");
            cell.GetComponent<GridCell>().resetPosition();
        }

        foreach (var wall in walls.Values)
        {
            wall.GetComponent<GameWall>().resetPosition();
        }

        foreach (var goal in goals.Values)
        {
            goal.GetComponent<GameGoal>().resetPosition();
        }

        foreach (var playerStart in playerStarts.Values)
        {
            playerStart.GetComponent<PlayerStart>().resetPosition();
        }

        //foreach (var colorChange in colorChanges.Values)
        //{
        //    colorChange.GetComponent<ColorChange>().resetPosition();
        //}

        foreach (var button in buttons.Values)
        {
            button.GetComponent<GameButton>().resetPosition();
        }
    }
    public void addNewItem(GameGoal goal)
    {
        Vector3 goalLoc = goal.getPosition();
        goals[goalLoc] = Instantiate(gridGoalPrefab, goalLoc * 5, Quaternion.identity);
        goals[goalLoc].GetComponent<GameGoal>().setStartingPosition(new Vector3(goalLoc.x, goalLoc.y, goalLoc.z));
        goals[goalLoc].GetComponent<GameGoal>().setColor(goal.getColor());
        goals[goalLoc].transform.parent = transform;
        goals[goalLoc].gameObject.name = "GOAL(" + goalLoc.ToString() + " )";
    }
    public void addNewItem(GameButton button)
    {
        Vector3 buttonLoc = button.getPosition();

        buttons[buttonLoc] = Instantiate(buttonPrefab, buttonLoc * 5, Quaternion.identity);
        buttons[buttonLoc].GetComponent<GameButton>().setStartingPosition(new Vector3(buttonLoc.x, buttonLoc.y, buttonLoc.z));
        //TODO24 buttons[buttonLoc].GetComponent<GameButton>().setToMoveStart(new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z));
        //TODO24 buttons[buttonLoc].GetComponent<GameButton>().setToMoveEnd(new Vector3(bpd.buttonMoveToEnd.x, bpd.buttonMoveToEnd.y, bpd.buttonMoveToEnd.z));
        buttons[buttonLoc].GetComponent<GameButton>().setColor(button.getColor());
        //TODO24 buttons[buttonLoc].GetComponent<GameButton>().setObjectToMove(cells[buttons[buttonLoc].GetComponent<GameButton>().getToMoveStart()]);
        buttons[buttonLoc].transform.parent = transform;
        buttons[buttonLoc].gameObject.name = "Button Press(" + buttonLoc.ToString() + " )";

        //TODO24 cells[new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z)].GetComponent<GridCell>().createArrows();
        //TODO24 Vector3 dir2Move = GetDirection(new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z), new Vector3(bpd.buttonMoveToEnd.x, bpd.buttonMoveToEnd.y, bpd.buttonMoveToEnd.z));
        //TODO24 cells[new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z)].GetComponent<GridCell>().setArrowDir(dir2Move);

    }
    public void addNewItem(GameWall wall)
    {
        Vector3 wallLoc = wall.getPosition();
        walls[wallLoc] = Instantiate(gridWallPrefab, wallLoc * 5, Quaternion.identity);

        walls[wallLoc].GetComponent<GameWall>().setStartingPosition(new Vector3((int)wallLoc.x, (int)wallLoc.y, (int)wallLoc.z));
        walls[wallLoc].GetComponent<GameWall>().setPosition((int)wallLoc.x, (int)wallLoc.y, (int)wallLoc.z);
        walls[wallLoc].transform.parent = transform;
        walls[wallLoc].gameObject.name = "Grid Wall(" + wallLoc.x.ToString() + " , " + wallLoc.y.ToString() + " , " + wallLoc.z.ToString() + " )";
    }

    public void addNewItem(PlayerStart ps)
    {
        Vector3 psLoc = ps.getPosition();
        playerStarts[psLoc] = Instantiate(playerStartPrefab, psLoc * 5 + new Vector3(0, 0, -2.5f), Quaternion.identity);
        playerStarts[psLoc].GetComponent<PlayerStart>().setStartingPosition(psLoc);
        playerStarts[psLoc].GetComponent<PlayerStart>().setColor(ps.getColor());
        playerStarts[psLoc].transform.parent = transform;
        playerStarts[psLoc].gameObject.name = "Player Start(" + psLoc.ToString() + " )";
    }

    public void addNewItem(GameItem item)
    {
        if (item is GameWall) addNewItem(item as GameWall);
        else if (item is GameGoal) addNewItem(item as GameGoal);
        else if (item is PlayerStart) addNewItem(item as PlayerStart);
        else if (item is GameButton) addNewItem(item as GameButton);
        else
        {
            Debug.LogError("Unknown type of GameItem!");
        }
    }

    public void setLevel(int level_num, LevelData ld)
    {
        twoStarThreshold = ld.twoStarThreshold;
        threeStarThreshold = ld.threeStarThreshold;

        colorDictionary = new Dictionary<string, Color>();
        colorDictionary["grey"] = Color.grey;
        colorDictionary["red"] = Color.red;
        colorDictionary["blue"] = Color.blue;
        colorDictionary["green"] = Color.green;
        colorDictionary["gold"] = new Color(255f / 255f, 200f / 255f, 0f / 255f);
        colorDictionary["tirq"] = new Color(0f / 255f, 255f / 255f, 255f / 255f);

        cells = new Dictionary<Vector3, GameObject>();
        walls = new Dictionary<Vector3, GameObject>();
        goals = new Dictionary<Vector3, GameObject>();
        playerStarts = new Dictionary<Vector3, GameObject>();
        colorChanges = new Dictionary<Vector3, GameObject>();
        buttons = new Dictionary<Vector3, GameObject>();

        foreach (Vector3 cell in ld.cells)
        {
            cells[cell] = Instantiate(gridCellPrefab, cell * 5, Quaternion.identity);
            cells[cell].GetComponent<GridCell>().setStartingPosition(cell);
            cells[cell].GetComponent<GridCell>().setPosition((int)cell.x, (int)cell.y, (int)cell.z);
            cells[cell].transform.parent = transform;
            cells[cell].gameObject.name = "Grid Space(" + cell.x.ToString() + " , " + cell.y.ToString() + " , " + cell.z.ToString() + " )";
        }
        foreach (Vector3 wall in ld.walls)
        {
            walls[wall] = Instantiate(gridWallPrefab, wall * 5, Quaternion.identity);
            walls[wall].GetComponent<GameWall>().setStartingPosition(wall);
            walls[wall].GetComponent<GameWall>().setPosition((int)wall.x, (int)wall.y, (int)wall.z);
            walls[wall].transform.parent = transform;
            walls[wall].gameObject.name = "Grid Wall(" + wall.x.ToString() + " , " + wall.y.ToString() + " , " + wall.z.ToString() + " )";
        }
        foreach (LocColorData ps in ld.playerStarts)
        {
            playerStarts[ps.loc] = Instantiate(playerStartPrefab, ps.loc * 5, Quaternion.identity);

            playerStarts[ps.loc].GetComponent<PlayerStart>().setStartingPosition(ps.loc);
            playerStarts[ps.loc].GetComponent<PlayerStart>().setStartingColor(colorDictionary[ps.col]);

            playerStarts[ps.loc].GetComponent<PlayerStart>().setPosition(new Vector3(ps.loc.x, ps.loc.y, ps.loc.z));
            playerStarts[ps.loc].GetComponent<PlayerStart>().setColor(colorDictionary[ps.col]);
            playerStarts[ps.loc].transform.parent = transform;
            playerStarts[ps.loc].gameObject.name = "Player Start(" + ps.loc.ToString() + " )";
        }
        foreach (LocColorData gg in ld.goals)
        {
            goals[gg.loc] = Instantiate(gridGoalPrefab, gg.loc * 5, Quaternion.identity);

            goals[gg.loc].GetComponent<GameGoal>().setStartingPosition(gg.loc);
            goals[gg.loc].GetComponent<GameGoal>().setStartingColor(colorDictionary[gg.col]);

            goals[gg.loc].GetComponent<GameGoal>().setPosition(new Vector3(gg.loc.x, gg.loc.y, gg.loc.z));
            goals[gg.loc].GetComponent<GameGoal>().setColor(colorDictionary[gg.col]);
            goals[gg.loc].transform.parent = transform;
            goals[gg.loc].gameObject.name = "GOAL(" + gg.loc.ToString() + " )";
        }
        foreach (LocColorData ccg in ld.colorChange)
        {
            colorChanges[ccg.loc] = Instantiate(colorChangePrefab, ccg.loc * 5, Quaternion.identity);

            //colorChanges[ccg.loc].GetComponent<ColorChange>().setStartingPosition(ccg.loc);
            //colorChanges[ccg.loc].GetComponent<ColorChange>().setStartingColor(colorDictionary[ccg.col]);

            colorChanges[ccg.loc].GetComponent<ColorChange>().setPosition(new Vector3(ccg.loc.x, ccg.loc.y, ccg.loc.z));
            colorChanges[ccg.loc].GetComponent<ColorChange>().setCol(colorDictionary[ccg.col]);
            colorChanges[ccg.loc].transform.parent = transform;
            colorChanges[ccg.loc].gameObject.name = "Color Change(" + ccg.loc.ToString() + " )";
        }
        foreach (ButtonPressedData bpd in ld.buttons)
        {
            buttons[bpd.buttonLoc] = Instantiate(buttonPrefab, bpd.buttonLoc * 5, Quaternion.identity);

            buttons[bpd.buttonLoc].GetComponent<GameButton>().setStartingPosition(bpd.buttonLoc);
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setStartingColor(colorDictionary[bpd.col]);

            buttons[bpd.buttonLoc].GetComponent<GameButton>().setPosition(new Vector3(bpd.buttonLoc.x, bpd.buttonLoc.y, bpd.buttonLoc.z));
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setToMoveStart(new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z));
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setToMoveEnd(new Vector3(bpd.buttonMoveToEnd.x, bpd.buttonMoveToEnd.y, bpd.buttonMoveToEnd.z));
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setColor(colorDictionary[bpd.col]);
            buttons[bpd.buttonLoc].GetComponent<GameButton>().setObjectToMove(cells[buttons[bpd.buttonLoc].GetComponent<GameButton>().getToMoveStart()]);
            buttons[bpd.buttonLoc].transform.parent = transform;
            buttons[bpd.buttonLoc].gameObject.name = "Button Press(" + bpd.buttonLoc.ToString() + " )";

            cells[new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z)].GetComponent<GridCell>().createArrows();
            Vector3 dir2Move = GetDirection(new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z), new Vector3(bpd.buttonMoveToEnd.x, bpd.buttonMoveToEnd.y, bpd.buttonMoveToEnd.z));
            cells[new Vector3(bpd.buttonMoveToStart.x, bpd.buttonMoveToStart.y, bpd.buttonMoveToStart.z)].GetComponent<GridCell>().setArrowDir(dir2Move);
        }
    }

    public Vector3 GetDirection(Vector3 currentLocation, Vector3 goalPosition)
    {
        // Calculate the difference between the goal position and the current location
        Vector3 direction = goalPosition - currentLocation;

        // Normalize the direction vector to make it unit length
        direction.Normalize();

        // Round the direction vector to the nearest axis
        direction.x = Mathf.Round(direction.x);
        direction.y = Mathf.Round(direction.y);
        direction.z = Mathf.Round(direction.z);

        return direction;
    }
    public bool isAnythingThere(Vector3 location)
    {
        return cells.ContainsKey(location)
        || walls.ContainsKey(location)
        || goals.ContainsKey(location)
        || playerStarts.ContainsKey(location)
        || colorChanges.ContainsKey(location)
        || buttons.ContainsKey(location);
    }
    public LevelData getLevelData()
    {
        LevelData ld = new LevelData();
        ld.twoStarThreshold = 1;
        ld.threeStarThreshold = 10;

        ld.cells = cells.Values.Select(go => go.GetComponent<GameItem>()?.getPosition() ?? Vector3.zero).ToArray();
        ld.walls = walls.Values.Select(go => go.GetComponent<GameItem>()?.getPosition() ?? Vector3.zero).ToArray();
        ld.goals = goals.Select(kvp => convertToLocColorData(kvp.Value.GetComponent<GameItem>())).ToArray();

        ld.playerStarts = playerStarts.Select(kvp => convertToLocColorData(kvp.Value.GetComponent<GameItem>())).ToArray();
        ld.buttons = buttons.Select(kvp => convertButtonPressedData(kvp.Value.GetComponent<GameButton>())).ToArray();
        ld.colorChange = colorChanges.Select(kvp => convertToLocColorData(kvp.Value.GetComponent<GameItem>())).ToArray();
        return ld;
    }
    public LocColorData convertToLocColorData(GameItem itm)
    {
        LocColorData lcd = new LocColorData();
        lcd.loc = itm.getPosition();

        //lcd.col = next(key for key, value in colors.items() if value == itm.getColor()) ;
        //lcd.col = itm.getColor();

        return lcd;
    }

    public ButtonPressedData convertButtonPressedData(GameButton butt)
    {
        ButtonPressedData bpd = new ButtonPressedData();
        bpd.buttonLoc = butt.getPosition();
        bpd.buttonMoveToStart = butt.getToMoveStart();
        bpd.buttonMoveToEnd = butt.getToMoveEnd();

        //bpd.col = next(key for key, value in colors.items() if value == butt.getColor()) ;
        //bpd.col = butt.getColor();

        return bpd;
    }
}


