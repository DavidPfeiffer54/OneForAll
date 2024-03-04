using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public string mainMenu = "MainMenu";
    [SerializeField] private GameObject youWinPrefab;
    [SerializeField] private GameObject levelEditorPrefab;
    [SerializeField] private GameObject levelManagerPrefab;
    [SerializeField] private GameObject playerManagerPrefab;
    [SerializeField] private GameObject gamePlayControllerPrefab;


    public static GameManager instance;

    public PlayerManager playerManager;
    public LevelEditor levelEditor;
    public LevelManager levelManager;
    public MovementController movementController;
    public GameObject levelInfo;
    public GamePlayController gamePlayController;

    public int currentLevelID;
    public bool editMode;

    public LevelInfo currentLevelInfo; // This holds the current level information
    public static GameMode CurrentMode { get; set; } = GameMode.Gameplay;

    private void Awake()
    {
        editMode = false;
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the loaded scene is the specific scene you are interested in
        if (scene.name == "Gameplay")
        {
            // Call your function here
            InitalizeManager(LevelSelectItem.selectedLevel);
        }
    }
    void Start()
    {

    }

    void InitalizeManager(int levelID)
    {
        movementController = MovementController.instance;


        levelManager = Instantiate(levelManagerPrefab, new Vector3(0, 0), Quaternion.identity).GetComponent<LevelManager>();
        playerManager = Instantiate(playerManagerPrefab, new Vector3(0, 0), Quaternion.identity).GetComponent<PlayerManager>();
        gamePlayController = Instantiate(gamePlayControllerPrefab, new Vector3(0, 0), Quaternion.identity).GetComponent<GamePlayController>();

        gamePlayController.levelManager = levelManager.gameObject;
        gamePlayController.playerManager = playerManager.gameObject;

        movementController.levelManager = levelManager;
        movementController.playerManager = playerManager;


        switch (CurrentMode)
        {
            case GameMode.Gameplay:
                Debug.Log("Entering Gameplay Mode");
                InitalizeLevel(LevelSelectItem.selectedLevel);
                break;

            case GameMode.LevelEdit:
                levelEditor = Instantiate(levelEditorPrefab, new Vector3(0, 0), Quaternion.identity).GetComponent<LevelEditor>();
                levelEditor.levelManager = levelManager;

                Debug.Log("Entering Level Editing Mode");
                InitalizeEditorLevel(LevelSelectItem.selectedLevel);
                break;

            default:
                break;
        }

    }
    void InitalizeLevel(int levelID)
    {
        currentLevelID = levelID;
        levelManager.setUpLevel(currentLevelID, false);
        startLevel();
    }
    void InitalizeEditorLevel(int levelID)
    {
        gamePlayController.updateDisabled = true;
        currentLevelID = levelID;
        levelManager.setUpLevel(currentLevelID, true);
        levelEditor.setupEditor();
    }
    public void startLevel()
    {
        levelInfo = levelManager.getCurrentLevel();
        playerManager.setUpPlayers(levelInfo);
        GameObject[] players = playerManager.players;
        gamePlayController.players = players;
        if (CurrentMode == GameMode.Gameplay)
        {
            StartCoroutine(movementController.flyInLevel());
        }
        else if (CurrentMode == GameMode.LevelEdit)
        {
            levelEditor.deactivateEditor();
        }

        gamePlayController.updateDisabled = false;
    }
    // Method to start the gameplay from the level select page
    public void StartGameFromLevelSelect(LevelInfo levelInfo)
    {
        currentLevelInfo = levelInfo;
        SceneManager.LoadScene("GameplayScene");
    }

    // Method to start the gameplay from the level editor page
    public void StartGameFromLevelEditor(LevelInfo levelInfo)
    {
        // You would need to implement functionality in your level editor
        // to save the current level to currentLevelInfo before calling this method
        currentLevelInfo = levelInfo;
        SceneManager.LoadScene("GameplayScene");
    }

    // Method to go back to the level select page from the gameplay scene
    public void GoBackToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public void finishedLevel()
    {
        StartCoroutine(FinishLevelCoroutine());
    }

    IEnumerator FinishLevelCoroutine()
    {

        if (CurrentMode == GameMode.Gameplay)
        {
            //StartCoroutine(movementController.flyInLevel());


            gamePlayController.updateDisabled = true;
            int starsScored = 0;

            //score stars based on number of moves
            if (gamePlayController.moveLists.Count <= levelManager.GetComponent<LevelManager>().getThreeStarThreshold()) { starsScored = 3; }
            else if (gamePlayController.moveLists.Count <= levelManager.GetComponent<LevelManager>().getTwoStarThreshold()) { starsScored = 2; }
            else { starsScored = 1; }

            //set record if it were broken
            int previousStarRecord = PlayerPrefs.GetInt("Level_" + currentLevelID.ToString(), -1);
            if (starsScored > previousStarRecord)
                PlayerPrefs.SetInt("Level_" + currentLevelID.ToString(), starsScored);

            //set up the next level
            int nextLevel = currentLevelID + 1;
            //get the star raiting for the level. if it hasnt been played before, set to 0
            int nextLevelStars = PlayerPrefs.GetInt("Level_" + nextLevel.ToString(), -1);
            if (nextLevelStars == -1)
                PlayerPrefs.SetInt("Level_" + nextLevel.ToString(), 0);

            GameObject youwin = Instantiate(youWinPrefab, new Vector3(0, 0), Quaternion.identity);
            currentLevelID = nextLevel;

            if (currentLevelID >= LevelSelector.maxLevels)
            {
                //you have finished all of the levels! return to the main menu
                youwin.transform.Find("TheText").GetComponent<TMPro.TextMeshProUGUI>().text = "YOU Win game!!!";

                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(2.0f);
                Destroy(youwin);
                Time.timeScale = 1f;
                SceneManager.LoadScene(mainMenu);
            }
            else
            {
                LevelEndMenu.isLevelEndMenuActive = true;

                StartCoroutine(movementController.flyOutLevel());
                yield return new WaitForSeconds(2f);
                Destroy(youwin);
                Debug.Log("level:: " + currentLevelID);
                InitalizeLevel(currentLevelID);
                LevelEndMenu.isLevelEndMenuActive = false;
            }
        }
        else if (CurrentMode == GameMode.LevelEdit)
        {
            foreach (GameObject obj in playerManager.players)
            {
                Destroy(obj);
            }
            levelManager.resetLevel();

            gamePlayController.updateDisabled = true;
            levelEditor.setupEditor();
        }
    }
    public LevelInfo getNextLevel()
    {
        return null;
    }


}