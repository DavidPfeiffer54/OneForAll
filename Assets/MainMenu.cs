using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//https://www.youtube.com/watch?v=76WOa6IU_s8&t=109s

public class MainMenu : MonoBehaviour
{

    public string firstLevel;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        GameManager.CurrentMode = GameMode.Gameplay;

        SceneManager.LoadScene("LevelSelectScene");
    }

    public void OpenOptions()
    {
        GameManager.CurrentMode = GameMode.LevelEdit;

        SceneManager.LoadScene("LevelSelectScene");
    }

    public void CloseOptions()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game");
    }
}
