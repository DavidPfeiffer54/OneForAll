using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GamePlayController : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject gameGridPrefab;

    [SerializeField] private GameObject levelManagerPrefab;
    [SerializeField] private GameObject playerManagerPrefab;

    public GameObject [] players;
    public GameObject [] cells;
    public GameObject [] walls;
    public GameObject [] goals;
    public GameObject grid;

    public GameObject levelManager;
    public GameObject playerManager;

    public bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        //grid = Instantiate(gameGridPrefab, new Vector3(0,0), Quaternion.identity);
        Debug.Log("LOADING");


        //grid.GetComponent<GameGrid>().setLevel(levelManager.GetComponent<LevelManager>().getCurrentLevel());

    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving == false)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                StartCoroutine(canMove(Vector3.up));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                StartCoroutine(canMove(Vector3.left));

            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                StartCoroutine(canMove(Vector3.right));

            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                StartCoroutine(canMove(Vector3.down));
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
        
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];

        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());

        //players[0] = Instantiate (playerPrefab, new Vector3(0,0), Quaternion.identity);
        //players[0].GetComponent<PlayerController>().setPosition(new Vector3(1,3,1));
        //players[0].GetComponent<PlayerController>().playerName="bob";
        //players[0].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.red;
        //players[1] = Instantiate (playerPrefab, new Vector3(0,0), Quaternion.identity);
        //players[1].GetComponent<PlayerController>().setPosition(new Vector3(3,1,1));
        //players[1].GetComponent<PlayerController>().playerName="susan";
        //players[1].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.blue;
        //
        //int[,] level1Walls = new int[,] {{0,0},{1,0},{0,1},{0,3},{0,4},{1,4},{3,4},{4,4},{4,3},{4,1},{4,0},{3,0},{2,2}};
        //for(int i = 0; i < level1Walls.Length/2; i++)
        //{
        //    Debug.Log(i);
        //    Debug.Log(level1Walls.Length);
        //    walls[i] = Instantiate (wallPrefab, new Vector3(0,0), Quaternion.identity);
        //    walls[i].GetComponent<GameWall>().setPosition(level1Walls[i,0], level1Walls[i,1], 1);
        //}

        //for(int i = 0; i < 6; i++)
        //{
        //    walls[i] = Instantiate (wallPrefab, new Vector3(0,0), Quaternion.identity);
        //    walls[i].GetComponent<GameWall>().setPosition(i+1, i+1);
        //}

        //goals[0] = Instantiate (goalPrefab, new Vector3(0,0), Quaternion.identity);
        //goals[0].GetComponent<GameGoal>().setPosition(0,2);
        //goals[0].transform.Find("goal").GetComponent<SpriteRenderer>().color=Color.red;
        //
        //goals[1] = Instantiate (goalPrefab, new Vector3(0,0), Quaternion.identity);
        //goals[1].GetComponent<GameGoal>().setPosition(2,4);
        //goals[1].transform.Find("goal").GetComponent<SpriteRenderer>().color=Color.blue;

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

                //This should be isWallAt/isCellAt and checks a hash table.
                if(levelManager.GetComponent<LevelManager>().isWallAt(playerMoveTo)) hitSomething = true;
                            //foreach (GameObject w in walls) //cannot move into walls
                            //{
                            //    if(playerMoveTo == w.GetComponent<GameWall>().getPosition())
                            //    {
                            //        hitSomething = true;
                            //    }
                            //}
                if(levelManager.GetComponent<LevelManager>().isCellAt(playerMoveTo)) hitSomething = true;
                            //foreach (GameObject c in cells) //cannot move into cells on the same level
                            //{
                            //    if(playerMoveTo == c.GetComponent<GameWall>().getPosition())
                            //    {
                            //        hitSomething = true;
                            //    }
                            //}

                //is there a player in your direction
                for(int j =0; j<i; j++) //cannot move onto players on the same level. only check in dir
                {
                    if(playerMoveTo == PlayersAtDepth[j].GetComponent<PlayerController>().getMoveTo())
                    {
                        hitSomething = true;
                    }                
                }

                //is anyone on top of you
                foreach (GameObject op in players) //cannot move if someone is on top of you
                {
                    if(p.getPosition() + new Vector3(0,0,-1) ==  op.GetComponent<PlayerController>().getPosition())
                    {
                        hitSomething = true;
                    }                       
                }
                //Do i need a "is the next space a moveable onto space?" (cell or top of character)
                //so we do not move onto a wall? 


                //Debug.Log(p.playerName);
                //Debug.Log(hitSomething);

                //if you didnt hit anything, set moveto
                if(!hitSomething)
                {
                    p.setMoveTo((int)playerMoveTo.x, (int)playerMoveTo.y, (int)playerMoveTo.z);
                    StartCoroutine(p.movePlayer(dir));
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
            while(p.GetComponent<PlayerController>().isMoving)
            {
                yield return null;
            }
        }

        //if any of the characters are out of bounds, set to falling
        bool playerFallToInfinity = false;
        //foreach (GameObject p in players)
        //{
        //    PlayerController player=p.GetComponent<PlayerController>();
        //    if(checkOutOfBounds(p))
        //    {
        //        playerFallToInfinity = true;
        //        StartCoroutine(player.movePlayerFall(dir));
        //    }
        //}

        //reset moveto
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().setMoveTo(-1,-1, -1);
        }

        //THIS IS TODO
        //check fall down a level
        int breakout = 0;
        bool checkForFallers = true;
        while(checkForFallers && breakout < 20)
        {
            breakout = breakout + 1;
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
                            //StartCoroutine(p.GetComponent<PlayerController>().movePlayerFall(dir));
                        }
                        else
                        {
                            fallDown=true;
                            checkForFallers=true;
                            p.GetComponent<PlayerController>().setMoveTo((int)tmpvec.z,(int)tmpvec.y,(int)tmpvec.z);
                            StartCoroutine(p.GetComponent<PlayerController>().movePlayerFallDown(new Vector3(0,0,1)));
                        }
                    }
                }
            }
            //checkForFallers=false;
        }

        //wait for players to stop moving
        foreach (GameObject p in players)
        {
            while(p.GetComponent<PlayerController>().isMoving)
            {
                yield return null;
            }
        }

        //reset moveto
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().setMoveTo(-1,-1, -1);
        }



        //reset if necessary 
        if(playerFallToInfinity)
        {
            resetLevel();
        }

        levelManager.GetComponent<LevelManager>().setPlayersOnGoals(players);

        if(levelManager.GetComponent<LevelManager>().isLevelComplete())
        {
            Debug.Log("**************");
            Debug.Log("Level Complete");
            Debug.Log("**************");

        }
        
        isMoving = false;
        yield return null;
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
