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
    [SerializeField] private GameObject gameButtonPrefab;

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
        Vector3 wall = new Vector3(0, 0, 3);
        loadlevel();
        changeNextItem(wallPrefab, wall);
        changeNextItemColor(Color.blue);





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


        var outline = nextItem.GetComponent<GameItem>().GetComponent<Outline>();
        if (canPlacePiece(nextItem.GetComponent<GameItem>()))
        {
            outline.OutlineColor = Color.yellow;
        }
        else
        {
            outline.OutlineColor = Color.red;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow)) canMove(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) canMove(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) canMove(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) canMove(Vector3.down);
        else if (Input.GetKeyDown(KeyCode.P)) canMove(Vector3.back);
        else if (Input.GetKeyDown(KeyCode.O)) canMove(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.I)) addNewItem();


        else if (Input.GetKeyDown(KeyCode.Q)) changeNextItem(wallPrefab, nextItem.GetComponent<GameItem>().getPosition());
        else if (Input.GetKeyDown(KeyCode.W)) changeNextItem(goalPrefab, nextItem.GetComponent<GameItem>().getPosition());
        else if (Input.GetKeyDown(KeyCode.E)) changeNextItem(playerStartPrefab, nextItem.GetComponent<GameItem>().getPosition());
        else if (Input.GetKeyDown(KeyCode.R)) changeNextItem(gameButtonPrefab, nextItem.GetComponent<GameItem>().getPosition());


        else if (Input.GetKeyDown(KeyCode.A)) changeNextItemColor(Color.red);
        else if (Input.GetKeyDown(KeyCode.S)) changeNextItemColor(Color.blue);

        else if (Input.GetKeyDown(KeyCode.Z))
        {
            LevelSelectItem.selectedLevel = 0;
            SceneManager.LoadScene("Gameplay");
        }

    }
    public void changeNextItemColor(Color c)
    {
        createItemColor = c;
        nextItem.GetComponent<GameItem>().setColor(c);
    }
    public void changeNextItem(GameObject nextItemType, Vector3 newLocation)
    {
        //Vector3 newLocation = nextItem.GetComponent<GameItem>().getPosition();
        Destroy(nextItem);
        nextItem = Instantiate(nextItemType, newLocation * 5, Quaternion.identity);
        nextItem.GetComponent<GameItem>().setPosition(newLocation);
        nextItem.GetComponent<GameItem>().setColor(createItemColor);
        nextItem.transform.parent = transform;

        var outline = nextItem.gameObject.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
    }
    public void canMove(Vector3 dir)
    {
        nextItem.GetComponent<GameItem>().editMove(dir);
    }
    public void loadlevel()
    {
        //moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];
        levelManager.GetComponent<LevelManager>().setUpLevel(0, true);
        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
    }

    public void addNewItem()
    {
        if (canPlacePiece(nextItem.GetComponent<GameItem>()))
        {
            //Vector3 location = nextItem.GetComponent<GameItem>().getPosition();
            levelManager.GetComponent<LevelManager>().addNewItem(nextItem.GetComponent<GameItem>());
            if (nextItem.GetComponent<GameItem>() is GameButton)
            {
                changeNextItem(wallPrefab, nextItem.GetComponent<GameItem>().getPosition());
            }
        }
    }
    public bool canPlacePiece(GameItem item)
    {
        if (item is GameWall) return canPlaceWall(item as GameWall);
        else if (item is GameGoal) return canPlaceGoal(item as GameGoal);
        else if (item is PlayerStart) return canPlacePlayerStart(item as PlayerStart);
        else if (item is GameButton) return canPlaceGameButton(item as GameButton);
        else
        {
            Debug.LogError("Unknown type of GameItem!");
            return false;
        }
    }
    public bool canPlaceWall(GameWall wall)
    {
        return !levelManager.GetComponent<LevelManager>().isAnythingThere(wall.getPosition());

        //LevelManager lman = levelManager.GetComponent<LevelManager>();
        //
        //if (lman.isAnythingThere(wall.getPosition()))
        //    return false;
        //
        //Vector3 below = wall.getPosition() + new Vector3(0, 0, 1);
        //return (lman.isWallAt(below) || lman.isCellAt(below));


    }
    public bool canPlaceGoal(GameGoal goal)
    {
        LevelManager lman = levelManager.GetComponent<LevelManager>();

        if (lman.isAnythingThere(goal.getPosition()))
            return false;

        Vector3 below = goal.getPosition() + new Vector3(0, 0, 1);
        return (lman.isWallAt(below) || lman.isCellAt(below));
    }
    public bool canPlacePlayerStart(PlayerStart playerStart)
    {
        LevelManager lman = levelManager.GetComponent<LevelManager>();

        if (lman.isAnythingThere(playerStart.getPosition()))
            return false;

        Vector3 below = playerStart.getPosition() + new Vector3(0, 0, 1);
        return (lman.isWallAt(below) || lman.isCellAt(below));
    }
    public bool canPlaceGameButton(GameButton button)
    {
        LevelManager lman = levelManager.GetComponent<LevelManager>();

        if (lman.isAnythingThere(button.getPosition()))
            return false;

        Vector3 below = button.getPosition() + new Vector3(0, 0, 1);
        return (lman.isWallAt(below) || lman.isCellAt(below));
    }


}
