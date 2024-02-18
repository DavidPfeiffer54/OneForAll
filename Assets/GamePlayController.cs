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

    public Coroutine moveCoroutine;

    public bool isMoving = false;
    public int currentLevel = 0;
    public int maxLevel = 3;
    public string mainMenu = "MainMenu";
    List<Dictionary<GameObject, MoveHistory>> moveLists = new List<Dictionary<GameObject, MoveHistory>>();

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 0;
        currentLevel = LevelSelectItem.selectedLevel;
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        loadlevel();
    }
    void Awake()
    {
        levelManager = Instantiate(levelManagerPrefab, new Vector3(0, 0), Quaternion.identity);
        playerManager = Instantiate(playerManagerPrefab, new Vector3(0, 0), Quaternion.identity);
        loadlevel();
    }
    // Update is called once per frame
    void Update()
    {

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
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];
        levelManager.GetComponent<LevelManager>().setUpLevel(currentLevel);
        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
        isMoving = false;
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
                if (playerManager.GetComponent<PlayerManager>().isPlayerAt(p.getPosition() + new Vector3(0, 0, -1))) hitSomething = true;

                if (levelManager.GetComponent<LevelManager>().isWallAt(abovePlayerAt)) hitSomething = true;
                if (levelManager.GetComponent<LevelManager>().isCellAt(abovePlayerAt)) hitSomething = true;

                //if (levelManager.GetComponent<LevelManager>().isWallAt(abovePlayerMoveTo)) hitSomething = true;
                //if (levelManager.GetComponent<LevelManager>().isCellAt(abovePlayerMoveTo)) hitSomething = true;



                if (!hitSomething)
                {
                    //TODO:// SHOULDNT THE Player itself be doing this
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
                p.GetComponent<PlayerController>().startPlayerChangeColor(colorChange);
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
    //TODO24 we want to evaluate the moving pieces in a specific order. so they alway do the same thing regardles where they are on the stack
    //    sort all of the pieces by topleft
    //    evaluate push in order, setting isMoving
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
                    //pushItems(btn.getObjectToMove(), directionToMove);
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
    /*
    this isnt working properly becuase things are clipping into eachother.
    this is because one can move into a space that is currently occupied but something is moving out of.
    fix: need to make sure 
    - no one is already going to where im going. 
    - if someone is currently at where im going
    -    only go there if they are going in the same direction of me. 
    -    otherwise you gotta wait.  
    */
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


        //TODO24 grid cell and player should be siblings that have the same getLoc, isMoving, etc
        Vector3 locationAbove = objLocation + new Vector3(0, 0, -1);
        if (isPlayerAt(locationAbove))
            canBeMovedInDir(cellsToMove, getPlayerAt(locationAbove).gameObject, dir);

        if (gridCell)
        {
            //TODO should not update where player is. only where they are going and then update where they are when they get there 

            gridCell.GetComponent<GridCell>().startMoveCellPushed(dir);
            //levelManager.GetComponent<LevelManager>().moveCell(obj, gridCell.getLoc() + dir);
        }
        if (player)
        {
            //TODO should not update where player is. only where they are going and then update where they are when they get there 
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
            Debug.Log("Evaluating");
            Debug.Log(entry);
            Debug.Log("++++++++");
            GridCell cell = entry.Key;
            Vector3 dir = entry.Value;
            canBeMovedInDir(cellsToMove, cell.gameObject, dir);
        }

        /*

        //TODO24 - remove an item after it has been evalutated. 
        //So we dont get into a loop where something couldnt move and get reevealuated
        foreach (GridCells gridcell in cellsToMove)
        {
            Vector3 destLocation = gridCell.getLoc() + dir;
            if (isWallAt(destLocation) || isCellrMoveTo(destLocation) || isPlayerMoveTo(destLocation))
                return false;

            if (isCellAt(destLocation))
            {
                if (cellAtDest.isMoving and cellAtDest.movingDir != dir)
                    return false;
                if (cellAtDest not in cellsToMove)
                    return false;
                if (cellsToMove[cellAtDest] != dir)
                    return false
                if (cellAtDest.canMove() == false)
                    return false;
            }
            if (isPlayerAt(destLocation))
            {
                if (playerMoving())
                    return false;
                if (playerCanMove == false)
                    return false;
            }
            
        }*/
    }
    /*
    public bool pushItems(GameObject obj, Vector3 dir)
    {
        // Check if the GameObject has the GridCell or Player component
        GridCell gridCell = obj.GetComponent<GridCell>();
        PlayerController player = obj.GetComponent<PlayerController>();

        if (gridCell != null)
        {
            
            //if there is a wall, dont move
            //if there is a player, see if they can move them
            //    are they currently moving? 
            //    if not, can they go in the direction
            //if there is a sell GOING TO BE THERE - dont move
            //if there is a cell "currently there"
            //    if the cell is trying to move
            //        is the cell trying to move in the same direction
            //             can it do that? 
                
                
                
            //    see if it is going to move in the same direction.
            //        (if it is already moving, check the dir)
            //        (if it hasnt been evalutaed yet, evaluate it)
            //             (does it want to move? in which direction)
            
            //is a wall, cell there or player going to be there
            Vector3 destLocation = gridCell.getLoc() + dir;
            if (isWallAt(destLocation) || isCellrMoveTo(destLocation) || isPlayerMoveTo(destLocation))
                return false;

            if (isCellAt(destLocation))
            {
                GridCell cellAt = getCellAt(destLocation);
                //if theres already a cell there, only move its moving inthe same direction? 
                //how do you tell? if its already moveing check dir? 
                //
                //if cellAt.getMovingTo() == cellAt+dir OOOORRR if cellAt.getMovingDir == dir (this implys moving is not null)




                //if cellAt.isMoving and cellAt.isMovingDir!=dir
                //return false //already moving, but not in the right direction
                //if cellAt.GoingToMoveDir == None //
                //return false// not moving
                //if cellAt.GoingToMoveDir != dir
                //return false //going to move, just in a different direction
                //if 

                //if n
                //if the cell isnt planning on moving, do notheing
                //may not be nesessary
                if (cellAt.isgoingToMove == false)
                    return false

                //if its not moving in the same direction
                if (cellAt.isGoingToMoveDir != dir)
                    return false

                //is already moving the same as going to move? 
                if (cellAt.isAlreadyMoving)
                    if (cellAt.isAlreadyMovingDir != dir)
                        return false

                //try to move the new one.
                if (tryToMove(cellAt, dir) == false)
                    return false
            }
            //if a player is currently there, ill they be moving? 
            if (isPlayerAt(destLocation))
            {

                //If the player is moving its because they are falling
                //that should take precidence
                //TODO24 - Q: if the player is falling are they there or are they moveto? 
                if (getPlayerAt(destLocation).isMoving)
                    return false;

                if (!pushItems(getPlayerAt(destLocation).gameObject, dir))
                    return false;
            }


            //Move players that are on top of the moving cell 
            Vector3 locationAbove = gridCell.getLoc() + new Vector3(0, 0, -1);
            if (isPlayerAt(locationAbove) && !getPlayerAt(locationAbove).isMoving)
            {
                Debug.Log("Moving a player that is on top");
                Debug.Log(getPlayerAt(locationAbove).isMoving);
                pushItems(getPlayerAt(locationAbove).gameObject, dir);
            }
            //TODO and move the players that are on top of those players
            gridCell.GetComponent<GridCell>().startMoveCellPushed(dir);
            levelManager.GetComponent<LevelManager>().moveCell(obj, gridCell.getPosition() + dir);

        }
        else if (player != null)
        {
            //TODO: do we need to check if another player is already going to be or move here? 
            Vector3 destLocation = player.getPosition() + dir;
            if (isWallAt(destLocation) || isCellAt(destLocation) || isPlayerMoveTo(destLocation))
                return false;

            if (isPlayerAt(destLocation))
            {
                //TODO: dont we also want to check if there is 
                if (!pushItems(getPlayerAt(destLocation).gameObject, dir))
                    return false;
            }

            //MOVE Players that are on top of the player (if they can) 
            Vector3 locationAbove = player.getPosition() + new Vector3(0, 0, -1);
            if (isPlayerAt(locationAbove) && !getPlayerAt(locationAbove).isMoving)
            {
                Debug.Log("Moving a player that is on top");
                Debug.Log(getPlayerAt(locationAbove).isMoving);
                pushItems(getPlayerAt(locationAbove).gameObject, dir);
            }

            //TODO: and Move the players that are on top of them as well
            Debug.Log("MOVING A PLAYER");
            player.GetComponent<PlayerController>().startMovePlayerPushed(dir);
        }
        else
        {
            // GameObject doesn't have the required component
            Debug.LogError("Object does not have GridCell or Player component.");
            return false;
        }
        return true;
        
    }*/

    /*
    public bool RecursiveCanMovTo(Vector3 loc, Vector3 dir)
    {

        //if(isWallAt(loc+dir) || isCellAt(loc+dir)) return False;
        //if(isPlayerAt(loc+dir))
        //  return RecursiveCanMovTo(loc+dir, dir);
        return true;
    }
    */

    public IEnumerator levelComplete()
    {
        int starsScored = 0;
        if (moveLists.Count <= levelManager.GetComponent<LevelManager>().getThreeStarThreshold()) { starsScored = 3; }
        else if (moveLists.Count <= levelManager.GetComponent<LevelManager>().getTwoStarThreshold()) { starsScored = 2; }
        else { starsScored = 1; }

        int previousStarRecord = PlayerPrefs.GetInt("Level_" + currentLevel.ToString(), -1);

        if (starsScored > previousStarRecord)
            PlayerPrefs.SetInt("Level_" + currentLevel.ToString(), starsScored);

        int nextLevel = currentLevel + 1;
        int nextLevelStars = PlayerPrefs.GetInt("Level_" + nextLevel.ToString(), -1);
        if (nextLevelStars == -1)
            PlayerPrefs.SetInt("Level_" + nextLevel.ToString(), 0); //Set the next level to be available UNLESS ITS ALREADY BEEN DONE

        GameObject youwin = Instantiate(youWinPrefab, new Vector3(0, 0), Quaternion.identity);

        currentLevel = nextLevel;
        if (nextLevel >= LevelSelector.maxLevels)
        {
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
            moveLists = new List<Dictionary<GameObject, MoveHistory>>();

            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2.0f);
            Destroy(youwin);

            levelManager.GetComponent<LevelManager>().setUpLevel(currentLevel);
            playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
            players = playerManager.GetComponent<PlayerManager>().players;

            Time.timeScale = 1f;
        }
    }

    private void resetLevel()
    {
        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();

    }
}
