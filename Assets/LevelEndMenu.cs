using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndMenu : MonoBehaviour
{
    public static bool isLevelEndMenuActive = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void endLevel()
    {
        isLevelEndMenuActive = true;
    }
    public void startNewLevel()
    {
        isLevelEndMenuActive = false;
    }
}
