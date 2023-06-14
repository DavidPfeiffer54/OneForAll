using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    public bool isPressed=false;
    public GameObject objectToMove;
    public Vector3 toMoveStart;
    public Vector3 toMoveEnd;
    public Vector3 pos;
    public Color col;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool getIsPressed()
    {
        return isPressed;
    }
    public void setIsPressed(bool _isPressed)
    {
        isPressed = _isPressed;
    }
    public GameObject getObjectToMove()
    {
        return objectToMove;
    }
    public void setObjectToMove(GameObject _objectToMove)
    {
        objectToMove = _objectToMove;
    }
    public Vector3 getToMoveStart()
    {
        return toMoveStart;
    }
    public void setToMoveStart(Vector3 _toMoveStart)
    {
        toMoveStart = _toMoveStart;
    }
    public Vector3 getToMoveEnd()
    {
        return toMoveEnd;
    }
    public void setToMoveEnd(Vector3 _toMoveEnd)
    {
        toMoveEnd = _toMoveEnd;
    }
    public Vector3 getPosition()
    {
        return pos;
    }
    public void setPosition(Vector3 newPos)
    {
        pos = newPos;
    }
    public Color getCol()
    {
        return col;
    }
    public void setCol(Color newCol)
    {
        col = new Color(newCol.r, newCol.g, newCol.b, newCol.a);
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = newCol;
        //transform.Find("Button").GetComponent<MeshRenderer>().material = newMaterial;
    }
}
