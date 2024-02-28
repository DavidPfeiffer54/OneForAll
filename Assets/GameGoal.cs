using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGoal : GameItem
{
    public Vector3 pos;
    public Color col;
    public bool goalCompleted;
    [SerializeField] private Material transMat;
    // Start is called before the first frame update
    void Start()
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
    public override Color getColor()
    {
        return col;
    }

    public override void setColor(Color newCol)
    {
        col = newCol;

        // Create a new material with the Standard shader
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = col;

        // Get references to the child GameObjects and update their materials
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }

        // Create a transparent material for the cylinder
        Material newTransparentMaterial = Instantiate(transMat);
        Color transparentColor = newCol;
        transparentColor.a = 0.1f;
        newTransparentMaterial.color = transparentColor;

        // Update the material of the cylinder
        Transform cylinder = transform.Find("Cylinder");
        if (cylinder != null)
        {
            MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();
            if (cylinderRenderer != null)
            {
                cylinderRenderer.material = newTransparentMaterial;
            }
        }
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
