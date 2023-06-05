using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Vector3 posStart;
    public Vector3 posEnd;
    public Vector3 pos;
    public Color col;
    public bool isMoving;
    public float moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator moveGhost(Vector3 moveTo)
    {
        isMoving = true;

        Vector3 origPos = transform.position;
        Vector3 targetPos = moveTo;

        Debug.Log(origPos);
        Debug.Log(targetPos);

        float elapsedTime=0;
        while(elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.Find("Ghost").position = transform.position;
        transform.position = targetPos;
        pos = targetPos;

        isMoving = false;
    }

    public Vector3 getPosition()
    {
        return pos;
    }
    public void setPosition(Vector3 newPos)
    {
        pos = newPos;
    }

    public Vector3 getPositionStart()
    {
        return posStart;
    }
    public void setPositionStart(Vector3 newPosStart)
    {
        posStart = newPosStart;
    }

    public Vector3 getPositionEnd()
    {
        return posEnd;
    }
    public void setPositionEnd(Vector3 newPosEnd)
    {
        posEnd = newPosEnd;
    }
    public Color getCol()
    {
        return col;
    }
    public void setCol(Color newCol)
    {
        col = new Color(newCol.r, newCol.g, newCol.b, newCol.a);
        //Material newMaterial = new Material(Shader.Find("Standard"));
        //newMaterial.color = newCol;
        //transform.Find("Cube").GetComponent<MeshRenderer>().material = newMaterial;
    }

}
