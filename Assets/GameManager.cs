using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool editMode;

    public LevelInfo currentLevelInfo; // This holds the current level information

    private void Awake()
    {
        editMode = false;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (editMode)
        {
            SceneManager.LoadScene("LevelEditScene");
        }
        else
        {
            currentLevelInfo = getNextLevel();
            SceneManager.LoadScene("GamePlayScene");
        }
    }
    public LevelInfo getNextLevel()
    {
        return null;
    }
}