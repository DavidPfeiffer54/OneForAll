using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    private int posX;
    private int posY;
    private int posZ;

    public GameObject objectInThisGridSpace = null;

    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setPosition(int x, int y, int z)
    {
        posX = x;
        posY = y;
        posZ = z;
    }

    public Vector3 getPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
