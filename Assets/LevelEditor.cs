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
    [SerializeField] private GameObject destoyerPrefab;

    [SerializeField] private GameObject levelManagerPrefab;
    [SerializeField] private GameObject playerManagerPrefab;

    [SerializeField] private Material transMat;


    public GameObject[] players;
    public GameObject[] cells;
    public GameObject[] walls;
    public GameObject[] goals;
    public GameObject grid;

    public GameObject createItemType;
    public Color createItemColor;

    public LevelManager levelManager;
    public GameObject playerManager;

    public GameObject nextItem;

    public bool isEditorActive;

    // Start is called before the first frame update
    void Start()
    {
        /*
        createItemColor = Color.red;
        Vector3 wall = new Vector3(0, 0, 3);
        //loadlevel();
        changeNextItem(wallPrefab, wall);
        changeNextItemColor(Color.blue);
        //nextItem.transform.position = new Vector3(0, 0, 4) * 5;
        //nextItem = Instantiate(wallPrefab, new Vector3(nextItem.x, nextItem.y, nextItem.z) * 5 + new Vector3(0, 0, -2.5f), Quaternion.identity);
        */
    }
    public void setupEditor()
    {
        isEditorActive = true;
        createItemColor = Color.red;
        Vector3 wall = new Vector3(0, 0, 3);
        //loadlevel();
        changeNextItem(wallPrefab, wall);
        changeNextItemColor(Color.blue);
    }
    public void deactivateEditor()
    {
        isEditorActive = false;
        Destroy(nextItem);
    }
    void Awake()
    {
        //levelManager = Instantiate(levelManagerPrefab, new Vector3(0, 0), Quaternion.identity);
        //playerManager = Instantiate(playerManagerPrefab, new Vector3(0, 0), Quaternion.identity);
        //loadlevel();
    }
    // Update is called once per frame
    void Update()
    {
        if (!isEditorActive)
            return;


        var outline = nextItem.GetComponent<Outline>();

        if (!canPlacePiece(nextItem.GetComponent<GameItem>()))
        {
            outline.OutlineColor = Color.red;
        }
        else
        {
            outline.OutlineColor = Color.yellow;
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
        else if (Input.GetKeyDown(KeyCode.X)) createDestoryer(nextItem.GetComponent<GameItem>().getPosition());


        else if (Input.GetKeyDown(KeyCode.A)) changeNextItemColor(Color.red);
        else if (Input.GetKeyDown(KeyCode.S)) changeNextItemColor(Color.blue);

        else if (Input.GetKeyDown(KeyCode.Z)) GameManager.instance.startLevel();
    }

    public void createDestoryer(Vector3 pos)
    {
        Destroy(nextItem);

        // Create a cube GameObject
        nextItem = Instantiate(destoyerPrefab, pos * 5 + new Vector3(2.5f, 2.5f, 0f), Quaternion.identity); ;


        // Set the position, rotation, and scale of the cube as needed
        nextItem.GetComponent<GameItem>().setPosition(pos);

        nextItem.transform.rotation = Quaternion.identity; // No rotation
        nextItem.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); // Unity's default cube size

        // Instantiate the transparent material
        Material newMaterial2 = Instantiate(transMat); // Assuming transMat is the transparent material

        // Assign the transparent material to the cube's renderer

        //nextItem.GetComponent<MeshRenderer>().material = newMaterial2;
        nextItem.gameObject.transform.Find("Cube").GetComponent<MeshRenderer>().material = newMaterial2;

        nextItem.transform.parent = transform;

        Debug.Log("createDestoryer");



        var outline = nextItem.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        outline.OutlineColor = Color.red;
        outline.OutlineWidth = 5f;

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

        Debug.Log("OUTOUTOURLINE");

        var outline = nextItem.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
    }
    public void canMove(Vector3 dir)
    {
        nextItem.GetComponent<GameItem>().editMove(dir);
    }
    /*
    public void loadlevel()
    {
        //moveLists = new List<Dictionary<GameObject, MoveHistory>>();
        players = new GameObject[2];
        walls = new GameObject[13];
        goals = new GameObject[2];
        levelManager.GetComponent<LevelManager>().setUpLevel(0, true);
        playerManager.GetComponent<PlayerManager>().setUpPlayers(levelManager.GetComponent<LevelManager>().getCurrentLevel());
        players = playerManager.GetComponent<PlayerManager>().players;
    }*/

    public void addNewItem()
    {
        Debug.Log("=======");
        Debug.Log(nextItem is EditDestoryer);
        Debug.Log(nextItem);
        if (nextItem.GetComponent<EditDestoryer>() != null)
        {
            Debug.Log(nextItem);
            levelManager.destoryItemAt(nextItem.GetComponent<GameItem>().getPosition());
        }
        else if (canPlacePiece(nextItem.GetComponent<GameItem>()))
        {
            Debug.Log("addNewItem----");

            //Vector3 location = nextItem.GetComponent<GameItem>().getPosition();
            levelManager.addNewItem(nextItem.GetComponent<GameItem>());

            //TODO24 place button
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
        else if (item is EditDestoryer) return canDestoryItem(item as EditDestoryer);
        else
        {
            Debug.LogError("Unknown type of GameItem!");
            return false;
        }
    }
    public bool canPlaceWall(GameWall wall)
    {
        return !levelManager.isAnythingThere(wall.getPosition());

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
        if (levelManager.isAnythingThere(goal.getPosition()))
            return false;

        Vector3 below = goal.getPosition() + new Vector3(0, 0, 1);
        return (levelManager.isWallAt(below) || levelManager.isCellAt(below));
    }
    public bool canPlacePlayerStart(PlayerStart playerStart)
    {
        if (levelManager.isAnythingThere(playerStart.getPosition()))
            return false;

        Vector3 below = playerStart.getPosition() + new Vector3(0, 0, 1);
        return (levelManager.isWallAt(below) || levelManager.isCellAt(below));
    }
    public bool canPlaceGameButton(GameButton button)
    {
        /*
                    { 
                      "buttonLoc" : {"x": 1.0, "y": 1.0, "z": 3.0},
                      "buttonMoveToStart" : {"x": 1.0, "y": 2.0, "z": 3.0},
                      "buttonMoveToEnd" : {"x": 2.0, "y": 2.0, "z": 3.0},
                      "col":"red"
                    }
        */
        if (levelManager.isAnythingThere(button.getPosition()))
            return false;

        Vector3 below = button.getPosition() + new Vector3(0, 0, 1);
        return (levelManager.isWallAt(below) || levelManager.isCellAt(below));
    }
    public bool canDestoryItem(EditDestoryer destoryer)
    {
        if (levelManager.isAnythingThere(destoryer.getPosition()))
            return false;

        return true;
    }

}
