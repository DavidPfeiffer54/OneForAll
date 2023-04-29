using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGoal : MonoBehaviour
{
    private int posX;
    private int posY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void setPosition(int x, int y)
    {
        posX = x;
        posY = y;
        transform.position = new Vector3(x*5 + 2.5f,y*5 + 2.5f, transform.position.z);

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
