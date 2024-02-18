using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGoal : MonoBehaviour
{
    public Vector3 pos;
    public Color col;
    public bool goalCompleted;
    [SerializeField] private Material transMat;
    // Start is called before the first frame update
    void Start()
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
    public Color getCol()
    {
        return col;
    }
    public void setCol(Color newCol)
    {
        Debug.Log("***********");
        col = new Color(newCol.r, newCol.g, newCol.b, newCol.a);
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = col;
        transform.Find("Cube00").GetComponent<MeshRenderer>().material = newMaterial;
        transform.Find("Cube01").GetComponent<MeshRenderer>().material = newMaterial;
        transform.Find("Cube10").GetComponent<MeshRenderer>().material = newMaterial;
        transform.Find("Cube11").GetComponent<MeshRenderer>().material = newMaterial;

        Material newMaterial2 = Instantiate(transMat);
        Color nc2 = new Color(newCol.r, newCol.g, newCol.b, newCol.a);
        nc2.a = .1f;
        newMaterial2.color = nc2;
        transform.Find("Cylinder").GetComponent<MeshRenderer>().material = newMaterial2;

    }

    public void setPlayerOn(bool isOn)
    {
        if (isOn)
        {
            goalCompleted = true;
            Material newMaterial2 = Instantiate(transMat);
            Color c = new Color(col.r, col.g, col.b, .6f);
            newMaterial2.color = c;
            transform.Find("Cylinder").GetComponent<MeshRenderer>().material = newMaterial2;
        }
        else
        {
            goalCompleted = false;
            Material newMaterial2 = Instantiate(transMat);
            Color c = new Color(col.r, col.g, col.b, .1f);
            newMaterial2.color = c;
            transform.Find("Cylinder").GetComponent<MeshRenderer>().material = newMaterial2;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
