using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class GamePlayController : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject gameGridPrefab;

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

    // Start is called before the first frame update
    void Start()
    {
        currentLevel=0;
        loadlevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (isMoving == false)
        {
            if (Input.GetKey(KeyCode.UpArrow)) moveCoroutine = StartCoroutine(canMove(Vector3.up));
            else if (Input.GetKey(KeyCode.LeftArrow)) moveCoroutine = StartCoroutine(canMove(Vector3.left));
            else if (Input.GetKey(KeyCode.RightArrow))moveCoroutine = StartCoroutine(canMove(Vector3.right));
            else if (Input.GetKey(KeyCode.DownArrow))moveCoroutine = StartCoroutine(canMove(Vector3.down));
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
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];
        levelManager.GetComponent<LevelManager>().setUpLevel(currentLevel);
        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
        isMoving=false;
    }

    public IEnumerator canMove(Vector3 dir)
    {
        isMoving = true;
        players = playerManager.GetComponent<PlayerManager>().SortPlayersByDirection(new Vector2Int((int)dir.x, (int)dir.y));
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
        playerManager.GetComponent<PlayerManager>().resetMoveTo();
        levelManager.GetComponent<LevelManager>().setPlayersOnGoals(players);


        foreach (GameObject p in players)
        {
            GameObject colorChange = levelManager.GetComponent<LevelManager>().getColorChangeAt(p.GetComponent<PlayerController>().getPosition());
            if(colorChange && p.GetComponent<PlayerController>().getCol() != colorChange.GetComponent<ColorChange>().getCol())
            {
                p.GetComponent<PlayerController>().startPlayerChangeColor(colorChange);
            }
        }
        while(players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        playerManager.GetComponent<PlayerManager>().resetMoveTo();

        bool playerFallToInfinity = false;
        bool checkForFallers = true;
        while(checkForFallers)
        {
            while(players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
            checkForFallers = false;


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
                            checkForFallers=true;
                            p.GetComponent<PlayerController>().setMoveTo(tmpvec);
                            p.GetComponent<PlayerController>().startMovePlayerFallDown();
                        }
                    }
                }
            }
            //checkForFallers=false;
        }

        while(players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        playerManager.GetComponent<PlayerManager>().resetMoveTo();

        foreach (GameObject p in players)
        {
            GameObject colorChange = levelManager.GetComponent<LevelManager>().getColorChangeAt(p.GetComponent<PlayerController>().getPosition());
            if(colorChange && p.GetComponent<PlayerController>().getCol() != colorChange.GetComponent<ColorChange>().getCol())
            {
                p.GetComponent<PlayerController>().startPlayerChangeColor(colorChange);
            }
        }

        while(players.Any(p => p.GetComponent<PlayerController>().isMoving)) yield return null;
        playerManager.GetComponent<PlayerManager>().resetMoveTo();


        //reset if necessary 
        if(playerFallToInfinity){ resetLevel(); }

        levelManager.GetComponent<LevelManager>().setPlayersOnGoals(players);

        if(levelManager.GetComponent<LevelManager>().isLevelComplete())
        {
            Debug.Log("**************");
            Debug.Log("Level Complete");
            Debug.Log("**************");
            StartCoroutine(levelComplete());
        }
        
        isMoving = false;
        yield return null;
    }
    public IEnumerator levelComplete()
    {
        currentLevel = currentLevel + 1;
        GameObject youwin = Instantiate(youWinPrefab, new Vector3(0,0), Quaternion.identity);

        if(currentLevel>=maxLevel)
        {
            youwin.transform.Find("TheText").GetComponent<TMPro.TextMeshProUGUI>().text = "YOU Win game!!!";
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2.0f);
            Destroy(youwin);   
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenu);  
        }
        else
        {
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
        //players[0].GetComponent<PlayerController>().setPosition(1,3,1);
    }
    private bool checkOutOfBounds(GameObject p)
    {
        if(p.GetComponent<PlayerController>().getMoveTo().x > 4) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().y > 4) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().x < 0) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().y < 0) return true;
        return false;
    }
}
