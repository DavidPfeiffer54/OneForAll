using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    private int posX;
    private int posY;

    public GameObject objectInThisGridSpace = null;

    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int getPosition()
    {
        return new Vector2Int(posX, posY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
