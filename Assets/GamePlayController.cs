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
        //grid = Instantiate(gameGridPrefab, new Vector3(0,0), Quaternion.identity);
        Debug.Log("LOADING");
        currentLevel=0;
        loadlevel();
        //grid.GetComponent<GameGrid>().setLevel(levelManager.GetComponent<LevelManager>().getCurrentLevel());

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (isMoving == false)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                moveCoroutine = StartCoroutine(canMove(Vector3.up));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                moveCoroutine = StartCoroutine(canMove(Vector3.left));

            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                moveCoroutine = StartCoroutine(canMove(Vector3.right));

            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                moveCoroutine = StartCoroutine(canMove(Vector3.down));
            }
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
        //StopCoroutine("canMove");
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
        //Debug.Log("----");
        isMoving = true;
        players = playerManager.GetComponent<PlayerManager>().SortPlayersByDirection(new Vector2Int((int)dir.x, (int)dir.y));
        
        //players = SortPlayersByDirection(players,new Vector2Int((int)dir.x, (int)dir.y));

        for(int d = 0; d<5; d++)
        {
            GameObject[] PlayersAtDepth = playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(d);
            //GameObject[] PlayersAtDepth = GetPlayersAtDepth(d);
            for(int i =0; i<PlayersAtDepth.Length; i++)
            {
                PlayerController p=PlayersAtDepth[i].GetComponent<PlayerController>();
                Vector3 playerMoveTo = new Vector3 (p.getPosition().x + (int)dir.x, p.getPosition().y + (int)dir.y, p.getPosition().z);

                bool hitSomething = false;

                if(levelManager.GetComponent<LevelManager>().isWallAt(playerMoveTo)) hitSomething = true;

                if(levelManager.GetComponent<LevelManager>().isCellAt(playerMoveTo)) hitSomething = true;


                //is there a player in your direction
                for(int j =0; j<i; j++) //cannot move onto players on the same level. only check in dir
                {
                    if(playerMoveTo == PlayersAtDepth[j].GetComponent<PlayerController>().getMoveTo())
                    {
                        hitSomething = true;
                    }                
                }

                //is anyone on top of you
                if(playerManager.GetComponent<PlayerManager>().isPlayerAt(p.getPosition() + new Vector3(0,0,-1))) hitSomething = true;
                //foreach (GameObject op in players) //cannot move if someone is on top of you
                //{
                //    if(p.getPosition() + new Vector3(0,0,-1) ==  op.GetComponent<PlayerController>().getPosition())
                //    {
                //        hitSomething = true;
                //    }                       
                //}

                //if you didnt hit anything, set moveto
                if(!hitSomething)
                {
                    p.setMoveTo((int)playerMoveTo.x, (int)playerMoveTo.y, (int)playerMoveTo.z);
                    p.startMovePlayer(dir);
                }
                else
                {
                    p.setMoveTo((int)p.getPosition().x, (int)p.getPosition().y, (int)p.getPosition().z);
                }

            }
        }

        //if any players are moving, exit
        foreach (GameObject p in players)
        {
            while(p.GetComponent<PlayerController>().isMoving){ yield return null;}
        }

        //if any of the characters are out of bounds, set to falling
        

        //reset moveto
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().setMoveTo(-1,-1, -1);
        }

        bool playerFallToInfinity = false;
        bool checkForFallers = true;
        while(checkForFallers)
        {
            foreach (GameObject p in players)
            {
                while(p.GetComponent<PlayerController>().isMoving)
                {
                    yield return null;
                }
            }
            checkForFallers = false;


            for(int d = 10; d>=0; d--) 
            {
                GameObject[] PlayersAtDepth = playerManager.GetComponent<PlayerManager>().GetPlayersAtDepth(d);
                foreach (GameObject p in PlayersAtDepth)
                {
                    bool fallDown = false;
                    if(levelManager.GetComponent<LevelManager>().isCellAt(p.GetComponent<PlayerController>().getPosition() + new Vector3(0,0,1)) == false
                       && playerManager.GetComponent<PlayerManager>().isPlayerAt(p.GetComponent<PlayerController>().getPosition() + new Vector3(0,0,1)) == false)
                    {

                        Vector3 tmpvec = p.GetComponent<PlayerController>().getPosition() + new Vector3(0,0,1);
                        if(tmpvec.z>=10)
                        {
                            playerFallToInfinity = true;
                            // // //StartCoroutine(p.GetComponent<PlayerController>().movePlayerFall(dir));
                        }
                        else
                        {
                            fallDown=true;
                            checkForFallers=true;
                            p.GetComponent<PlayerController>().setMoveTo((int)tmpvec.z,(int)tmpvec.y,(int)tmpvec.z);
                            //StartCoroutine(p.GetComponent<PlayerController>().movePlayerFallDown(new Vector3(0,0,1)));
                            p.GetComponent<PlayerController>().startMovePlayerFallDown();
                        }
                    }
                }
            }
            //checkForFallers=false;
        }

        //wait for players to stop moving
        foreach (GameObject p in players)
        {
            while(p.GetComponent<PlayerController>().isMoving){ yield return null;}
        }

        //reset moveto
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().setMoveTo(-1,-1, -1);
        }



        //reset moveto
        foreach (GameObject p in players)
        {
            GameObject colorChange = levelManager.GetComponent<LevelManager>().getColorChangeAt(p.GetComponent<PlayerController>().getPosition());
            if(colorChange && p.GetComponent<PlayerController>().getCol() != colorChange.GetComponent<ColorChange>().getCol())
            {
                p.GetComponent<PlayerController>().startPlayerChangeColor(colorChange);
            }
        }

        //wait for players to stop moving
        foreach (GameObject p in players)
        {
            while(p.GetComponent<PlayerController>().isMoving){ yield return null;}
        }

        //reset if necessary 
        if(playerFallToInfinity){ resetLevel();}

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
        players[0].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.red;

        // Get a reference to the Mesh Renderer component attached to the cube
        MeshRenderer cubeRenderer = players[0].transform.Find("Cube").GetComponent<MeshRenderer>();
        // Create a new material and set its color to red
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = Color.red;

        // Set the cube's material to the new material
        cubeRenderer.material = newMaterial;


        //players[1].GetComponent<PlayerController>().setPosition(3,1,1);
        players[1].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.blue;
        // Get a reference to the Mesh Renderer component attached to the cube
        MeshRenderer cubeRenderer1 = players[1].transform.Find("Cube").GetComponent<MeshRenderer>();
        // Create a new material and set its color to red
        Material newMaterial1 = new Material(Shader.Find("Standard"));
        newMaterial1.color = Color.blue;
        cubeRenderer1.material = newMaterial1;
    }
    private bool checkOutOfBounds(GameObject p)
    {
        if(p.GetComponent<PlayerController>().getMoveTo().x > 4) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().y > 4) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().x < 0) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().y < 0) return true;
        return false;
    }
    public void setPlayersOn()
    {
        foreach (GameObject p in players)
        {

        }
    }
}
