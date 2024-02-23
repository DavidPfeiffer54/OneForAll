using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : GameItem
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
    public override Vector3 getPosition()
    {
        return pos;

    }
    public override void setPosition(Vector3 newPos)
    {
        pos = newPos;
        transform.position = new Vector3(pos.x * 5 + 2.5f, pos.y * 5 + 2.5f, pos.z * 5);

    }
    public override Color getColor()
    {
        return col;
    }
    public override void setColor(Color newCol)
    {
        col = newCol;
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
    }


}
