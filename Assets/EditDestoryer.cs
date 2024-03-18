using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditDestoryer : GameItem
{
    public Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public override Vector3 getPosition()
    {
        return pos;
    }
    public override void setPosition(Vector3 newPos)
    {
        pos = newPos;
    }
}
