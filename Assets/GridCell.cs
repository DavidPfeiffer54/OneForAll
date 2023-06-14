using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{

    private int posX;
    private int posY;
    private int posZ;
    public bool isMoving;
    private float moveSpeed = .2f;

    public GameObject objectInThisGridSpace = null;

    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void startMoveCellPushed(Vector3 direction)
    {
        StartCoroutine(moveCellPush(direction));
    }
    public IEnumerator moveCellPush(Vector3 direction)
    {
        isMoving = true;
        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + direction*5;
        Debug.Log(origPos);
        Debug.Log(targetPos);
        float elapsedTime=0;
        while(elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //transform.Find("Cube").position = transform.position;//+new Vector3(0,0,-2.5f);

        transform.position = targetPos;
        posX=posX+(int)direction.x;
        posY=posY+(int)direction.y;
        posZ=posZ+(int)direction.z;

        isMoving = false;
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

    public Vector3 getLoc()
    {
        return new Vector3(posX, posY, posZ);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
