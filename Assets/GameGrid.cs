using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    private int height = 5;
    private int width = 5;
    private float gridSpaceSize = 5f;

    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject gridWallPrefab;
    [SerializeField] private GameObject gridGoalPrefab;
    private GameObject [,] gameGrid;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void CreateGrid()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR did not set grid Cell prefab");
        }
        int z=0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                gameGrid[x,y] = Instantiate (gridCellPrefab, new Vector3(x*gridSpaceSize, y * gridSpaceSize, z * gridSpaceSize), Quaternion.identity);
                gameGrid[x,y].GetComponent<GridCell>().setPosition(x,y,z);
                gameGrid[x,y].transform.parent = transform;
                gameGrid[x,y].gameObject.name = "Grid Space(" + x.ToString() + " , " + y.ToString()  + " , " + z.ToString() + " )";
            }
        }
    }

    public void setLevel(GameObject level )
    {
        width=level.GetComponent<LevelInfo>().width;
        height=level.GetComponent<LevelInfo>().height;

        gameGrid = new GameObject[height, width];
        CreateGrid();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
