using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWall : GameItem
{
    private int posX;
    private int posY;
    private int posZ;

    // Start is called before the first frame update
    void Start()
    {

    }
    public override void setPosition(Vector3 newPos)
    {
        posX = (int)newPos.x;
        posY = (int)newPos.y;
        posZ = (int)newPos.z;
        transform.position = new Vector3(posX * 5 + 2.5f, posY * 5 + 2.5f, posZ * 5 - 2.5f);

    }

    public void setPosition(int x, int y, int z)
    {
        posX = x;
        posY = y;
        posZ = z;
        transform.position = new Vector3(x * 5 + 2.5f, y * 5 + 2.5f, z * 5 - 2.5f);
    }

    public override Vector3 getPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
