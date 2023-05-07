using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 pos;
    public Color col;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 getPosition()
    {
        return pos;
    }
    public void setPosition(Vector3 newPos)
    {
        pos = newPos;
    }
    public Color getColor()
    {
        return col;
    }
    public void setColor(Color newCol)
    {
        col = newCol;
    }


}
