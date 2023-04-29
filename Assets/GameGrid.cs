using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    private int height = 5;
    private int width = 5;
    private float gridSpaceSize = 5f;

    [SerializeField] private GameObject gridCellPrefab;
    private GameObject [,] gameGrid;
    // Start is called before the first frame update
    void Start()
    {
        gameGrid = new GameObject[height, width];
        CreateGrid();
    }

    private void CreateGrid()
    {
        if (gridCellPrefab == null)
        {
            Debug.LogError("ERROR did not set grid Cell prefab");
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                gameGrid[x,y] = Instantiate (gridCellPrefab, new Vector3(x*gridSpaceSize, y * gridSpaceSize), Quaternion.identity);
                gameGrid[x,y].GetComponent<GridCell>().setPosition(x,y);
                gameGrid[x,y].transform.parent = transform;
                gameGrid[x,y].gameObject.name = "Grid Space(" + x.ToString() + " , " + y.ToString() + " )";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
