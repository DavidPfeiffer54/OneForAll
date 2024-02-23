using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class LevelEditor : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerStartPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject ghostPrefab;

    [SerializeField] private GameObject levelManagerPrefab;
    [SerializeField] private GameObject playerManagerPrefab;

    public GameObject[] players;
    public GameObject[] cells;
    public GameObject[] walls;
    public GameObject[] goals;
    public GameObject grid;



    public GameObject createItemType;
    public Color createItemColor;

    public GameObject levelManager;
    public GameObject playerManager;

    public GameObject nextItem;

    // Start is called before the first frame update
    void Start()
    {
        createItemColor = Color.red;
        loadlevel();
        setCreateItemTypeWall();
        setCreateItemColorBlue();

        Vector3 wall = new Vector3(0, 0, 4);
        nextItem = Instantiate(wallPrefab, wall * 5, Quaternion.identity);
        nextItem.GetComponent<GameWall>().setPosition((int)wall.x, (int)wall.y, (int)wall.z);
        nextItem.transform.parent = transform;


        //nextItem.transform.position = new Vector3(0, 0, 4) * 5;
        //nextItem = Instantiate(wallPrefab, new Vector3(nextItem.x, nextItem.y, nextItem.z) * 5 + new Vector3(0, 0, -2.5f), Quaternion.identity);

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

        if (Input.GetKeyDown(KeyCode.UpArrow)) canMove(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) canMove(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) canMove(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) canMove(Vector3.down);
        else if (Input.GetKeyDown(KeyCode.P)) canMove(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.O)) canMove(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.I)) addNewItem();

        else if (Input.GetKeyDown(KeyCode.Q)) changeNextItemToGoal();
        else if (Input.GetKeyDown(KeyCode.W)) changeNextItemToWall();
        else if (Input.GetKeyDown(KeyCode.E)) changeNextItem(wallPrefab);
        else if (Input.GetKeyDown(KeyCode.R)) changeNextItem(goalPrefab);
        else if (Input.GetKeyDown(KeyCode.T)) changeNextItem(playerStartPrefab);


        else if (Input.GetKeyDown(KeyCode.A)) changeNextItemToGoal(Color.red);
        else if (Input.GetKeyDown(KeyCode.S)) changeNextItemToGoal(Color.blue);

    }
    public void changeNextItemToGoal(Color c)
    {
        createItemColor = c;
        nextItem.GetComponent<GameItem>().setColor(c);
    }
    public void changeNextItem(GameObject nextItemType)
    {
        Vector3 newLocation = nextItem.GetComponent<GameItem>().getPosition();
        Destroy(nextItem);
        nextItem = Instantiate(nextItemType, newLocation * 5, Quaternion.identity);
        nextItem.GetComponent<GameItem>().setPosition(newLocation);
        nextItem.GetComponent<GameItem>().setColor(createItemColor);
        nextItem.transform.parent = transform;
    }
    public void changeNextItemToGoal()
    {
        Vector3 newLocation = nextItem.GetComponent<GameItem>().getPosition();
        Destroy(nextItem);
        nextItem = Instantiate(goalPrefab, newLocation * 5, Quaternion.identity);
        nextItem.GetComponent<GameItem>().setPosition(newLocation);
        nextItem.GetComponent<GameGoal>().setColor(createItemColor);
        nextItem.transform.parent = transform;
    }
    public void changeNextItemToWall()
    {
        Vector3 newLocation = nextItem.GetComponent<GameItem>().getPosition();
        Destroy(nextItem);
        nextItem = Instantiate(wallPrefab, newLocation * 5, Quaternion.identity);
        nextItem.GetComponent<GameItem>().setPosition(newLocation);
        nextItem.transform.parent = transform;
    }
    public void canMove(Vector3 dir)
    {
        nextItem.GetComponent<GameItem>().editMove(dir);


        //nextItem.GetComponent<GameItem>().setPosition(nextItem.GetComponent<GameItem>().getPosition() + dir);
        //nextItem.transform.position = (nextItem.GetComponent<GameItem>().getPosition() * 5) + new Vector3(2.5f, 2.5f, -2.5f);

    }
    public void loadlevel()
    {
        //moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];
        levelManager.GetComponent<LevelManager>().setUpEditorLevel();
        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
    }

    public void addNewItem()
    {
        //Vector3 location = nextItem.GetComponent<GameItem>().getPosition();
        levelManager.GetComponent<LevelManager>().addNewItem(nextItem.GetComponent<GameItem>());
    }
    public void setCreateItemTypeWall()
    {
        createItemType = wallPrefab;
    }
    public void setCreateItemTypePlayer()
    {
        createItemType = playerPrefab;
    }
    public void setCreateItemTypeGoal()
    {
        createItemType = goalPrefab;
    }
    public void setCreateItemColorGreen()
    {
        createItemColor = Color.green;
    }
    public void setCreateItemColorRed()
    {
        createItemColor = Color.red;
    }
    public void setCreateItemColorBlue()
    {
        createItemColor = Color.blue;
    }
}
