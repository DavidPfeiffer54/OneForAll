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

    public GameObject [] players;
    public GameObject [] cells;
    public GameObject [] walls;
    public GameObject [] goals;
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
        currentLevel=0;
        currentLevel=LevelSelectItem.selectedLevel;
        moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        loadlevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        
        //checkButtonPresses();
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
            else if (Input.GetKey(KeyCode.RightArrow))moveCoroutine = StartCoroutine(canMove(Vector3.right));
            else if (Input.GetKey(KeyCode.DownArrow))moveCoroutine = StartCoroutine(canMove(Vector3.down));
            else if (Input.GetKeyDown(KeyCode.B)) goBack();
        }
    }

    void Awake()
    {
        levelManager = Instantiate(levelManagerPrefab, new Vector3(0,0), Quaternion.identity);
        playerManager = Instantiate(playerManagerPrefab, new Vector3(0,0), Quaternion.identity);
        loadlevel();
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
        isMoving=false;
    }
    public void goBack()
    {
        if(moveLists.Count > 0)
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
        for(int d = 0; d<5; d++)
        {
            GameObject[] PlayersAtDepth = playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(d);

            for(int i =0; i<PlayersAtDepth.Length; i++)
            {
                PlayerController p=PlayersAtDepth[i].GetComponent<PlayerController>();
                Vector3 playerMoveTo = new Vector3 (p.getPosition().x + (int)dir.x, p.getPosition().y + (int)dir.y, p.getPosition().z);

                bool hitSomething = false;

                if(levelManager.GetComponent<LevelManager>().isWallAt(playerMoveTo)) hitSomething = true;
                if(levelManager.GetComponent<LevelManager>().isCellAt(playerMoveTo)) hitSomething = true;
                if(playerManager.GetComponent<PlayerManager>().isPlayerMovingTo(playerMoveTo, PlayersAtDepth.Take(i).ToArray())) hitSomething = true;
                if(playerManager.GetComponent<PlayerManager>().isPlayerAt(p.getPosition() + new Vector3(0,0,-1))) hitSomething = true;

                if(!hitSomething)
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

        while(players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        playerManager.GetComponent<PlayerManager>().resetMoveTo(); // playerController could do this on its own when its done moving

        isMoving = false;
        yield return null;
    }


    public void checkFinishOrRestart()
    {
        levelManager.GetComponent<LevelManager>().setPlayersOnGoals(players); //Level Manager could do this on its own without being prompted

        if(levelManager.GetComponent<LevelManager>().isLevelComplete())
        {
            Debug.Log("**************");
            Debug.Log("Level Complete");
            Debug.Log("**************");
            StartCoroutine(levelComplete());
        }
        if(playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(9).Length > 0)
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
            if(colorChange && p.GetComponent<PlayerController>().getCol() != colorChange.GetComponent<ColorChange>().getCol())
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
        for(int d = 10; d>=0; d--) 
        {
            GameObject[] PlayersAtDepth = playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(d);
            foreach (GameObject p in PlayersAtDepth)
            {
                if(levelManager.GetComponent<LevelManager>().isCellAt(p.GetComponent<PlayerController>().getPosition() + new Vector3(0,0,1)) == false
                   && playerManager.GetComponent<PlayerManager>().isPlayerAt(p.GetComponent<PlayerController>().getPosition() + new Vector3(0,0,1)) == false)
                {

                    Vector3 tmpvec = p.GetComponent<PlayerController>().getPosition() + new Vector3(0,0,1);
                    //TODO USE ACTUAL DEPTH
                    if(tmpvec.z>=10)
                    {
                        playerFallToInfinity = true;
                    }
                    else
                    {
                        p.GetComponent<PlayerController>().setMoveTo(tmpvec);
                        p.GetComponent<PlayerController>().startMovePlayerFallDown();
                    }
                }
            }
        }
        
        checkButtonPresses();

        while(players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        cells = levelManager.GetComponent<LevelManager>().getCells();
        while(cells.Any(p => p.GetComponent<GridCell>().isMoving)) yield return null;
        isMoving = false;
    }
    public bool isButtonPressed(GameObject button)
    {
        foreach (GameObject p in players){
            if(button.GetComponent<GameButton>().getPosition() == p.GetComponent<PlayerController>().getPosition())
            {
                return true;
            }
        }
        return false;
    }
    public void checkButtonPresses()
    {
        //isMoving = true;
        
        foreach(GameObject button in levelManager.GetComponent<LevelManager>().getButtons())
        {
            if(isButtonPressed(button))
            {
                Debug.Log("ButtonPressed@@@@@@@@@@@@@@");
                if(button.GetComponent<GameButton>().getObjectToMove().GetComponent<GridCell>().getLoc() != button.GetComponent<GameButton>().getToMoveEnd())
                {
                    Debug.Log("NEEDS TO MOVE@@@@@@");
                    Vector3 directionToMove = GetDirection(button.GetComponent<GameButton>().getObjectToMove().GetComponent<GridCell>().getLoc(), button.GetComponent<GameButton>().getToMoveEnd());
                    pushItems(button.GetComponent<GameButton>().getObjectToMove(), directionToMove);
                }
            }
            else
            {
                if(button.GetComponent<GameButton>().getObjectToMove().GetComponent<GridCell>().getLoc() != button.GetComponent<GameButton>().getToMoveStart())
                {
                    Debug.Log("NEEDS TO MOVENEEDS GO BACL@@@");

                    Vector3 directionToMove = GetDirection(button.GetComponent<GameButton>().getObjectToMove().GetComponent<GridCell>().getLoc(), button.GetComponent<GameButton>().getToMoveStart());
                    pushItems(button.GetComponent<GameButton>().getObjectToMove(), directionToMove);
                }
            }
        }
        
        //while(RecursiveCanMovTo(bp.itemToMove, bp.toMoveTo))
        //{
        //    p.GetComponent<PlayerController>().startMovePlayerPush();
        //}
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

    public bool pushItems(GameObject obj, Vector3 dir)
    {
        Debug.Log(obj);
        Debug.Log(dir);
        // Check if the GameObject has the GridCell or Player component
        GridCell gridCell = obj.GetComponent<GridCell>();
        PlayerController player = obj.GetComponent<PlayerController>();

        if (gridCell != null)
        {
            if(levelManager.GetComponent<LevelManager>().isWallAt(gridCell.getLoc()+dir) || levelManager.GetComponent<LevelManager>().isCellAt(gridCell.getLoc()+dir) || playerManager.GetComponent<PlayerManager>().isPlayerMoveTo(gridCell.getLoc()+dir))
            {
                return false;
            }
            if(playerManager.GetComponent<PlayerManager>().isPlayerAt(gridCell.getLoc()+dir))
            {
                if(playerManager.GetComponent<PlayerManager>().getPlayerAt(gridCell.getLoc()+dir).GetComponent<PlayerController>().isMoving)
                {
                    return false;
                }

                if(!pushItems(playerManager.GetComponent<PlayerManager>().getPlayerAt(gridCell.getLoc()+dir), dir))
                {
                    return false;
                }
            }
            Debug.Log("gridCell can move!");
            //TODO Move players that are on top of the grid
            //TODO and move the players that are on top of those players
            gridCell.GetComponent<GridCell>().startMoveCellPushed(dir);
            levelManager.GetComponent<LevelManager>().moveCell(obj, gridCell.getPosition()+dir);

        }
        else if (player != null)
        {
            if(levelManager.GetComponent<LevelManager>().isWallAt(player.getPosition()+dir) || levelManager.GetComponent<LevelManager>().isCellAt(player.getPosition()+dir))
            {
                return false;
            }
            if(playerManager.GetComponent<PlayerManager>().isPlayerAt(player.getPosition()+dir))
            {
                if(!pushItems(playerManager.GetComponent<PlayerManager>().getPlayerAt(player.getPosition()+dir), dir))
                {
                    return false;
                }
            }
            Debug.Log("player can move!");
            //TODO: MOVE Players that are on top of the player (if they can)
            //TODO: and Move the players that are on top of them as well
            player.GetComponent<PlayerController>().startMovePlayerPushed(dir);
        }
        else
        {
            // GameObject doesn't have the required component
            Debug.LogError("Object does not have GridCell or Player component.");
            return false;
        }

        return true;
    }

    public bool RecursiveCanMovTo(Vector3 loc, Vector3 dir)
    {

        //if(isWallAt(loc+dir) || isCellAt(loc+dir)) return False;
        //if(isPlayerAt(loc+dir))
        //  return RecursiveCanMovTo(loc+dir, dir);
        return true;
    }

    public IEnumerator levelComplete()
    {

        int starsScored = 0;
        if(moveLists.Count <= levelManager.GetComponent<LevelManager>().getThreeStarThreshold())
        {
            starsScored=3;
            //PlayerPrefs.SetInt("Level_" + currentLevel.ToString(), 3);
        }
        else if(moveLists.Count <= levelManager.GetComponent<LevelManager>().getTwoStarThreshold())
        {
            starsScored=2;
            //PlayerPrefs.SetInt("Level_" + currentLevel.ToString(), 2);
        }
        else
        {
            starsScored=1;
            //PlayerPrefs.SetInt("Level_" + currentLevel.ToString(), 1);
        }
        int previousStarRecord = PlayerPrefs.GetInt("Level_" + currentLevel.ToString(), -1);


        if(starsScored > previousStarRecord)
            PlayerPrefs.SetInt("Level_" + currentLevel.ToString(), starsScored);

        int nextLevel = currentLevel + 1;
        int nextLevelStars = PlayerPrefs.GetInt("Level_" + nextLevel.ToString(), -1);
        if(nextLevelStars == -1)
            PlayerPrefs.SetInt("Level_" + nextLevel.ToString(), 0); //Set the next level to be available UNLESS ITS ALREADY BEEN DONE

        GameObject youwin = Instantiate(youWinPrefab, new Vector3(0,0), Quaternion.identity);

        currentLevel = nextLevel;
        if(nextLevel>=LevelSelector.maxLevels)
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
