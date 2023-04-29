using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject gameGrid;

    public GameObject [] players;
    public GameObject [] walls;
    public GameObject [] goals;

    public bool isMoving = false;
    // Start is called before the first frame update
    void Start()
    {
        
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
        loadlevel();
    }

    public void loadlevel()
    {
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];

        players[0] = Instantiate (playerPrefab, new Vector3(0,0), Quaternion.identity);
        players[0].GetComponent<PlayerController>().setPosition(1,3);
        players[0].GetComponent<PlayerController>().playerName="bob";
        players[0].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.red;
        players[1] = Instantiate (playerPrefab, new Vector3(0,0), Quaternion.identity);
        players[1].GetComponent<PlayerController>().setPosition(3,1);
        players[1].GetComponent<PlayerController>().playerName="susan";
        players[1].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.blue;

        int[,] level1Walls = new int[,] {{0,0},{1,0},{0,1},{0,3},{0,4},{1,4},{3,4},{4,4},{4,3},{4,1},{4,0},{3,0},{2,2}};
        for(int i = 0; i < level1Walls.Length/2; i++)
        {
            Debug.Log(i);
            Debug.Log(level1Walls.Length);
            walls[i] = Instantiate (wallPrefab, new Vector3(0,0), Quaternion.identity);
            walls[i].GetComponent<GameWall>().setPosition(level1Walls[i,0], level1Walls[i,1]);
        }
        //for(int i = 0; i < 6; i++)
        //{
        //    walls[i] = Instantiate (wallPrefab, new Vector3(0,0), Quaternion.identity);
        //    walls[i].GetComponent<GameWall>().setPosition(i+1, i+1);
        //}

        goals[0] = Instantiate (goalPrefab, new Vector3(0,0), Quaternion.identity);
        goals[0].GetComponent<GameGoal>().setPosition(0,2);
        goals[0].transform.Find("goal").GetComponent<SpriteRenderer>().color=Color.red;

        goals[1] = Instantiate (goalPrefab, new Vector3(0,0), Quaternion.identity);
        goals[1].GetComponent<GameGoal>().setPosition(2,4);
        goals[1].transform.Find("goal").GetComponent<SpriteRenderer>().color=Color.blue;

    }

    public IEnumerator canMove(Vector3 dir)
    {
        isMoving = true;
        players = SortPlayersByDirection(players,new Vector2Int((int)dir.x, (int)dir.y));


        for(int i =0; i<players.Length; i++)
        {
            PlayerController p=players[i].GetComponent<PlayerController>();
            Vector2Int playerMoveTo = new Vector2Int (p.getPosition().x + (int)dir.x, p.getPosition().y + (int)dir.y);
            

            bool hitSomething = false;
            foreach (GameObject w in walls)
            {
                if(playerMoveTo == w.GetComponent<GameWall>().getPosition())
                {
                    hitSomething = true;
                }
            }
            for(int j =0; j<i; j++)
            {
                if(playerMoveTo == players[j].GetComponent<PlayerController>().getMoveTo())
                {
                    hitSomething = true;
                }                
            }

            //Debug.Log(p.playerName);
            //Debug.Log(hitSomething);
            if(!hitSomething)
            {
                p.setMoveTo(playerMoveTo.x,playerMoveTo.y);
                StartCoroutine(p.movePlayer(dir));
            }
            else
            {
                p.setMoveTo(p.getPosition().x,p.getPosition().y);
            }

        }


        foreach (GameObject p in players)
        {
            while(p.GetComponent<PlayerController>().isMoving)
            {
                yield return null;
            }
        }

        bool playerFall = false;
        foreach (GameObject p in players)
        {
            PlayerController player=p.GetComponent<PlayerController>();
            if(checkOutOfBounds(p))
            {
                playerFall = true;
                StartCoroutine(player.movePlayerFall(dir));
            }
        } 

        foreach (GameObject p in players)
        {
            while(p.GetComponent<PlayerController>().isMoving)
            {
                yield return null;
            }
        }

        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().setMoveTo(-1,-1);
        }




        if(playerFall)
        {
            resetLevel();
        }
        isMoving = false;
        yield return null;
    }
    private void resetLevel()
    {
        players[0].GetComponent<PlayerController>().setPosition(1,3);
        players[0].GetComponent<PlayerController>().resetPosition();
        players[0].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.red;
        players[1].GetComponent<PlayerController>().setPosition(3,1);
        players[1].GetComponent<PlayerController>().resetPosition();
        players[1].transform.Find("player").GetComponent<SpriteRenderer>().color=Color.blue;

    }
    private bool checkOutOfBounds(GameObject p)
    {
        if(p.GetComponent<PlayerController>().getMoveTo().x > 4) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().y > 4) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().x < 0) return true;
        if(p.GetComponent<PlayerController>().getMoveTo().y < 0) return true;
        return false;
    }


    public static GameObject[] SortPlayersByDirection(GameObject[] players, Vector2Int direction)
    {
        // Check if the direction vector is valid
        if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) != 1)
        {
            Debug.LogError("Direction vector must be a unit vector (i.e., have magnitude 1).");
            return null;
        }

        // Sort the players based on the specified direction
        if (direction.x == 1)
        {
            // Sort by x (right)
            System.Array.Sort(players, (a, b) => b.GetComponent<PlayerController>().getPosition().x.CompareTo(a.GetComponent<PlayerController>().getPosition().x));
        }
        else if (direction.x == -1)
        {
            // Sort by x (left)
            System.Array.Sort(players, (a, b) => a.GetComponent<PlayerController>().getPosition().x.CompareTo(b.GetComponent<PlayerController>().getPosition().x));
        }
        else if (direction.y == 1)
        {
            // Sort by y (up)
            System.Array.Sort(players, (a, b) => b.GetComponent<PlayerController>().getPosition().y.CompareTo(a.GetComponent<PlayerController>().getPosition().y));
        }
        else if (direction.y == -1)
        {
            // Sort by y (down)
            System.Array.Sort(players, (a, b) => a.GetComponent<PlayerController>().getPosition().y.CompareTo(b.GetComponent<PlayerController>().getPosition().y));
        }

        return players;
    }





}
