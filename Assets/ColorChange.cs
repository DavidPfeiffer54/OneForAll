using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
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
        transform.Find("Cylinder").GetComponent<MeshRenderer>().material = newMaterial;

        // Get a reference to the ParticleSystem component
        ParticleSystem particleSystem = transform.Find("Particle System").GetComponent<ParticleSystem>();
        Color col2 = new Color(newCol.r, newCol.g, newCol.b, 0.2f);
        particleSystem.startColor = col2;
    }
}
