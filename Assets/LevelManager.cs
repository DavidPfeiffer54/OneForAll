using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] levels;
    public int currentLevel;
    [SerializeField] private GameObject LevelPrefab;

    void Start()
    {

    }
    void Awake()
    {


        levels = new GameObject[3];

        GameObject level = Instantiate(LevelPrefab, new Vector3(0,0), Quaternion.identity);
        int[,] wl = { {0,0}, {1,0}, {0,1}, {0,3}, {0,4}, {1,4}, {3,4}, {4,4}, {4,3}, {4,1}, {4,0}, {3,0}, {2,2} };
        int[,] gl = { {1,3}, {3,1} };
        int[,] pl = { {0,2}, {2,4} };
        level.GetComponent<LevelInfo>().SetLevel(1, 5, 5, wl, gl, pl);
        levels[0] = level;

        level = Instantiate(LevelPrefab, new Vector3(0,0), Quaternion.identity);
        wl = new int[,]{ {0,0}, {1,0}, {0,1}, {0,3}, {0,4}, {1,4}, {3,4}, {4,4}, {4,3}, {4,1}, {4,0}, {3,0}, {2,2} };
        gl = new int[,]{ {1,3}, {3,1} };
        pl = new int[,]{ {0,2}, {2,4} };
        level.GetComponent<LevelInfo>().SetLevel(1, 6, 6, wl, gl, pl);
        levels[1] = level;


        
    }

    public GameObject getCurrentLevel()
    {
        return levels[currentLevel];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
