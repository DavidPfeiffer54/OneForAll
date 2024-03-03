//TODO24 We need to seperate position/isMoving/isMovingToo
//   mainly if something is moving... is it at the place it 
//   started or the place its going WHILE ITS MOVING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MoveHistory
{
    public Vector3 position;
    public Color col;
    public MoveHistory(Vector3 _position, Color _col)
    {
        position = _position;
        col = _col;
    }
}

public class GamePlayController : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject ghostPrefab;

    [SerializeField] private GameObject levelManagerPrefab;
    [SerializeField] private GameObject playerManagerPrefab;
    [SerializeField] private GameObject youWinPrefab;

    public GameObject[] players;
    public GameObject[] cells;
    public GameObject[] walls;
    public GameObject[] goals;
    public GameObject grid;

    public GameObject levelManager;
    public GameObject playerManager;
    public GameObject levelEndMenu;

    public Coroutine moveCoroutine;

    public bool updateDisabled = true;
    public bool isMoving = false;
    public bool isAnimating = false;
    public int currentLevel = 0;
    public int maxLevel = 3;
    public string mainMenu = "MainMenu";
    public List<Dictionary<GameObject, MoveHistory>> moveLists = new List<Dictionary<GameObject, MoveHistory>>();

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        currentLevel = 0;
        currentLevel = LevelSelectItem.selectedLevel;
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        loadlevel();
    }
    void Awake()
    {

    }
    void setUpLevel(LevelInfo levelInfo)
    {
        Debug.Log("GamePlayController setUpLevel");

        moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        //levelManager.GetComponent<LevelManager>().FlyInItems();
        //playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
        isMoving = false;
    }
    // Update is called once per frame
    void Update()
    {
        /*
        Debug.Log("Update");
        Debug.Log(MovementController.isAnimating);
        Debug.Log(LevelEndMenu.isLevelEndMenuActive);
        Debug.Log(isAnimating);
        Debug.Log(Time.timeScale);
        Debug.Log(isMoving);
        */

        if (MovementController.isAnimating | LevelEndMenu.isLevelEndMenuActive | updateDisabled)
            return;
        if (isAnimating)
            return;
        if (Time.timeScale == 0)
            return;

        checkColorChangers();
        checkFinishOrRestart();
        if (isMoving == false)
        {
            StartCoroutine(checkForFallers());
        }
        if (isMoving == false)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveCoroutine = StartCoroutine(canMove(Vector3.up));
            else if (Input.GetKey(KeyCode.LeftArrow)) moveCoroutine = StartCoroutine(canMove(Vector3.left));
            else if (Input.GetKey(KeyCode.RightArrow)) moveCoroutine = StartCoroutine(canMove(Vector3.right));
            else if (Input.GetKey(KeyCode.DownArrow)) moveCoroutine = StartCoroutine(canMove(Vector3.down));
            else if (Input.GetKeyDown(KeyCode.B)) goBack();
        }
    }

    public void loadlevel()
    {
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        //levelManager.GetComponent<LevelManager>().setUpLevel(currentLevel);
        //isMoving = false;
        //StartCoroutine(FlyInItemsAndPlayersCoroutine());
    }
    public void goBack()
    {
        if (moveLists.Count > 0)
        {
            Debug.Log("back");
            Dictionary<GameObject, MoveHistory> vector3Dict = moveLists[moveLists.Count - 1];
            moveLists.RemoveAt(moveLists.Count - 1);
            foreach (KeyValuePair<GameObject, MoveHistory> pair in vector3Dict)
            {
                pair.Key.GetComponent<PlayerController>().setPosition(pair.Value.position);
                pair.Key.GetComponent<PlayerController>().setCol(pair.Value.col);
            }
        }
        levelManager.GetComponent<LevelManager>().setPlayersOnGoals(players);


    }
    public IEnumerator canMove(Vector3 dir)
    {
        Debug.Log("CAN MOVE");
        isMoving = true;

        players = playerManager.GetComponent<PlayerManager>().SortPlayersByDirection(new Vector2Int((int)dir.x, (int)dir.y));

        Dictionary<GameObject, MoveHistory> vector3Dict = new Dictionary<GameObject, MoveHistory>();
        foreach (GameObject p in players)
        {
            vector3Dict[p] = new MoveHistory(p.GetComponent<PlayerController>().getPosition(), p.GetComponent<PlayerController>().getCol());
        }
        moveLists.Add(vector3Dict);

        //TODO:: FIND DEPTH INHERENTLY
        for (int d = 0; d < 5; d++)
        {
            GameObject[] PlayersAtDepth = playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(d);
            for (int i = 0; i < PlayersAtDepth.Length; i++)
            {
                PlayerController p = PlayersAtDepth[i].GetComponent<PlayerController>();
                Vector3 playerMoveTo = new Vector3(p.getPosition().x + (int)dir.x, p.getPosition().y + (int)dir.y, p.getPosition().z);

                Vector3 abovePlayerAt = p.getPosition() + new Vector3(0, 0, -1);
                Vector3 abovePlayerMoveTo = new Vector3(p.getPosition().x + (int)dir.x, p.getPosition().y + (int)dir.y, p.getPosition().z - 1);

                bool hitSomething = false;

                if (levelManager.GetComponent<LevelManager>().isWallAt(playerMoveTo)) hitSomething = true;
                if (levelManager.GetComponent<LevelManager>().isCellAt(playerMoveTo)) hitSomething = true;
                if (playerManager.GetComponent<PlayerManager>().isPlayerMovingTo(playerMoveTo, PlayersAtDepth.Take(i).ToArray())) hitSomething = true;

                //cant move if someone is on top of you
                if (playerManager.GetComponent<PlayerManager>().isPlayerAt(abovePlayerAt)) hitSomething = true;
                if (levelManager.GetComponent<LevelManager>().isWallAt(abovePlayerAt)) hitSomething = true;
                if (levelManager.GetComponent<LevelManager>().isCellAt(abovePlayerAt)) hitSomething = true;

                //cant go into a NOOK. would clip 
                if (levelManager.GetComponent<LevelManager>().isWallAt(abovePlayerMoveTo)) hitSomething = true;
                if (levelManager.GetComponent<LevelManager>().isCellAt(abovePlayerMoveTo)) hitSomething = true;



                if (!hitSomething)
                {
                    p.setMoveTo(playerMoveTo);
                    p.startMovePlayer(dir);
                }
                else
                {
                    p.startRockPlayer(dir);
                    p.setMoveTo(p.getPosition());
                }
            }
        }

        while (players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        playerManager.GetComponent<PlayerManager>().resetMoveTo(); // playerController could do this on its own when its done moving

        isMoving = false;
        yield return null;
    }


    public void checkFinishOrRestart()
    {
        levelManager.GetComponent<LevelManager>().setPlayersOnGoals(players); //Level Manager could do this on its own without being prompted

        if (levelManager.GetComponent<LevelManager>().isLevelComplete())
        {
            StartCoroutine(levelComplete());
        }
        if (playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(9).Length > 0)
        {
            resetLevel();
        }
    }
    public void checkColorChangers()
    {

        //TODO: This could be a single line, or even taken care of by the level manager itself not being prompted
        foreach (GameObject p in players)
        {
            GameObject colorChange = levelManager.GetComponent<LevelManager>().getColorChangeAt(p.GetComponent<PlayerController>().getPosition());
            if (colorChange && p.GetComponent<PlayerController>().getCol() != colorChange.GetComponent<ColorChange>().getCol())
            {
                p.GetComponent<PlayerController>().startPlayerChangeColor(colorChange.GetComponent<ColorChange>().getCol());
            }
        }
    }
    public IEnumerator checkForFallers()
    {
        isMoving = true;

        bool playerFallToInfinity = false;

        //TODO: Bring this into the Player Manager
        //this doubleloop could be changed to a single orderby.thenby
        for (int d = 10; d >= 0; d--)
        {
            GameObject[] PlayersAtDepth = playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(d);
            foreach (GameObject p in PlayersAtDepth)
            {
                PlayerController pc = p.GetComponent<PlayerController>();
                //TODO24: DO I need to check pc. moving_to
                //ANSWER: I dont think so because everything is falling in the same direction. 
                //        so nothing could really be moving into the posistion its trying to go
                if (isCellAt(pc.getPosition() + new Vector3(0, 0, 1)) == false
                   && isPlayerAt(pc.getPosition() + new Vector3(0, 0, 1)) == false)
                {

                    Vector3 tmpvec = pc.getPosition() + new Vector3(0, 0, 1);
                    //TODO USE ACTUAL DEPTH
                    if (tmpvec.z >= 10) { playerFallToInfinity = true; }
                    else
                    {
                        pc.setMoveTo(tmpvec);
                        pc.startMovePlayerFallDown();
                    }
                }
            }
        }

        checkButtonPresses();

        //wait for players to stop moving
        while (players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        playerManager.GetComponent<PlayerManager>().resetMoveTo();

        //wait for cells to stop moving
        //TODO24:: Do i really need to wait for cells in the faller block
        cells = levelManager.GetComponent<LevelManager>().getCells();
        while (cells.Any(p => p.GetComponent<GridCell>().isMoving)) yield return null;

        isMoving = false;
    }
    public bool isButtonPressed(GameObject button)
    {
        return players.Any(p =>
            button.GetComponent<GameButton>().getPosition() == p.GetComponent<PlayerController>().getPosition());
    }

    public void checkButtonPresses()
    {
        Dictionary<GridCell, Vector3> cellsToMove = new Dictionary<GridCell, Vector3>();
        foreach (GameObject button in levelManager.GetComponent<LevelManager>().getButtons())
        {
            GameButton btn = button.GetComponent<GameButton>();
            GridCell btn_cell = btn.getObjectToMove().GetComponent<GridCell>();

            //if the button is pressed and the square isnt where its supposed to go, try to move there. 
            if (isButtonPressed(button))
            {
                if (btn_cell.getLoc() != btn.getToMoveEnd())
                {
                    Vector3 directionToMove = GetDirection(btn_cell.getLoc(), btn.getToMoveEnd());
                    cellsToMove[btn.getObjectToMove().GetComponent<GridCell>()] = directionToMove;

                    //pushItems(btn.getObjectToMove(), directionToMove);
                }
            }
            //if the button is not pressed and its not where its supposed to start, try to go there
            else
            {
                if (btn_cell.getLoc() != btn.getToMoveStart())
                {
                    Vector3 directionToMove = GetDirection(btn_cell.getLoc(), btn.getToMoveStart());
                    cellsToMove[btn.getObjectToMove().GetComponent<GridCell>()] = directionToMove;
                }
            }
        }
        moveAllPushItems(cellsToMove);
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

    public bool isWallAt(Vector3 loc)
    {
        return levelManager.GetComponent<LevelManager>().isWallAt(loc);
    }
    public bool isCellAt(Vector3 loc)
    {
        return levelManager.GetComponent<LevelManager>().isCellAt(loc);
    }
    public bool isPlayerMoveTo(Vector3 loc)
    {
        return playerManager.GetComponent<PlayerManager>().isPlayerMoveTo(loc);
    }
    public bool isCellMoveTo(Vector3 loc)
    {
        return levelManager.GetComponent<LevelManager>().isCellMoveTo(loc);
    }
    public bool isPlayerAt(Vector3 loc)
    {
        return playerManager.GetComponent<PlayerManager>().isPlayerAt(loc);
    }
    public PlayerController getPlayerAt(Vector3 loc)
    {
        return playerManager.GetComponent<PlayerManager>().getPlayerAt(loc).GetComponent<PlayerController>();
    }
    public GridCell getCellAt(Vector3 loc)
    {
        return levelManager.GetComponent<LevelManager>().getCellAt(loc).GetComponent<GridCell>();
    }

    //TODO24 cell and player should be siblings so you dont have to if else. 
    //or put them in their own thing. 
    public bool canBeMovedInDir(Dictionary<GridCell, Vector3> cellsToMove, GameObject obj, Vector3 dir)
    {
        GridCell gridCell = obj.GetComponent<GridCell>();
        PlayerController player = obj.GetComponent<PlayerController>();

        Vector3 objLocation = new Vector3();
        if (gridCell)
        {
            if (gridCell.isMoving) return false;
            objLocation = gridCell.getLoc();
        }
        if (player)
        {
            if (player.isMoving) return false;
            objLocation = player.getPosition();
        }
        Vector3 destLocation = objLocation + dir;



        if (isWallAt(destLocation) || isCellMoveTo(destLocation) || isPlayerMoveTo(destLocation))
            return false;

        if (isCellAt(destLocation))
        {
            GridCell cellAtDest = getCellAt(destLocation);
            Debug.Log("THERE IS SOMETHING THERE!");
            Debug.Log(cellAtDest);
            Debug.Log("-----------------------");
            if (cellAtDest.getIsMoving() && cellAtDest.getMoveTo() != destLocation + dir)
                return false;
            if (!cellsToMove.ContainsKey(cellAtDest) || cellsToMove[cellAtDest] != dir)
                return false;
            if (canBeMovedInDir(cellsToMove, cellAtDest.gameObject, dir) == false)
                return false;
        }
        if (isPlayerAt(destLocation))
        {
            PlayerController playerAtDest = getPlayerAt(destLocation);
            if (playerAtDest.isMoving && playerAtDest.getMoveTo() != destLocation + dir)
                return false;
            if (canBeMovedInDir(cellsToMove, playerAtDest.gameObject, dir) == false)
                return false;
        }


        //if another player is on top of moving piece, try to move it too
        Vector3 locationAbove = objLocation + new Vector3(0, 0, -1);
        if (isPlayerAt(locationAbove))
            canBeMovedInDir(cellsToMove, getPlayerAt(locationAbove).gameObject, dir);

        //TODO24 grid cell and player should be siblings that have the same getLoc, isMoving, etc
        if (gridCell)
        {
            gridCell.GetComponent<GridCell>().startMoveCellPushed(dir);
        }
        if (player)
        {
            player.GetComponent<PlayerController>().startMovePlayerPushed(dir);
        }
        return true;
    }
    public void moveAllPushItems(Dictionary<GridCell, Vector3> cellsToMove)
    {
        var sortedEntries = cellsToMove.OrderByDescending(kv => kv.Key.getLoc().z)
                                   .ThenByDescending(kv => kv.Key.getLoc().y)
                                   .ThenBy(kv => kv.Key.getLoc().x);

        foreach (var entry in sortedEntries)
        {
            GridCell cell = entry.Key;
            Vector3 dir = entry.Value;
            canBeMovedInDir(cellsToMove, cell.gameObject, dir);
        }
    }

    public IEnumerator levelComplete()
    {
        isAnimating = true;
        /*
        int starsScored = 0;

        //score stars based on number of moves
        if (moveLists.Count <= levelManager.GetComponent<LevelManager>().getThreeStarThreshold()) { starsScored = 3; }
        else if (moveLists.Count <= levelManager.GetComponent<LevelManager>().getTwoStarThreshold()) { starsScored = 2; }
        else { starsScored = 1; }

        //set record if it were broken
        int previousStarRecord = PlayerPrefs.GetInt("Level_" + currentLevel.ToString(), -1);
        if (starsScored > previousStarRecord)
            PlayerPrefs.SetInt("Level_" + currentLevel.ToString(), starsScored);

        //set up the next level
        int nextLevel = currentLevel + 1;
        //get the star raiting for the level. if it hasnt been played before, set to 0
        int nextLevelStars = PlayerPrefs.GetInt("Level_" + nextLevel.ToString(), -1);
        if (nextLevelStars == -1)
            PlayerPrefs.SetInt("Level_" + nextLevel.ToString(), 0);


        GameObject youwin = Instantiate(youWinPrefab, new Vector3(0, 0), Quaternion.identity);
        currentLevel = nextLevel;
        Debug.Log("MAX NEXT LEVEL");
        Debug.Log(currentLevel);
        Debug.Log(LevelSelector.maxLevels);
        
        if (nextLevel >= LevelSelector.maxLevels)
        {
            //you have finished all of the levels! return to the main menu
            youwin.transform.Find("TheText").GetComponent<TMPro.TextMeshProUGUI>().text = "YOU Win game!!!";
            moveLists = new List<Dictionary<GameObject, MoveHistory>>();

            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2.0f);
            Destroy(youwin);
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenu);
        }
        else
        {

            //TODO24yield return StartCoroutine(levelManager.GetComponent<LevelManager>().FlyOutItems());
            //TODO24yield return StartCoroutine(playerManager.GetComponent<PlayerManager>().FlyOutPlayers());

            //////Time.timeScale = 0f;

            GameManager.instance.finishedLevel();
            yield return new WaitForSecondsRealtime(2.0f);

            Destroy(youwin);

            ////levelManager.GetComponent<LevelManager>().setUpLevel(currentLevel);
            ////playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());

            /////players = playerManager.GetComponent<PlayerManager>().players;
            moveLists = new List<Dictionary<GameObject, MoveHistory>>();

            //////Time.timeScale = 1f;
            //TODO24playerManager.GetComponent<PlayerManager>().SetPlayersHigh();
            ////yield return StartCoroutine(FlyInItemsAndPlayersCoroutine());
        }
        */
        yield return new WaitForSecondsRealtime(2.0f);
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();

        GameManager.instance.finishedLevel();
        isAnimating = false;
    }

    private void resetLevel()
    {
        playerManager.GetComponent<PlayerManager>().resetPlayers();
        //playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        //levelManager.GetComponent<LevelManager>().setUpLevel(currentLevel);
        players = playerManager.GetComponent<PlayerManager>().players;
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();
    }
}
