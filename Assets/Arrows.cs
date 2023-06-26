using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrows : MonoBehaviour
{
    public Vector3 myDir;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDirectionOfMoving(Vector3 dir)
    {
        if(myDir == dir) return;

        myDir=dir;

        Transform baseTransform = transform.Find("base");

        if(dir==Vector3.right)
        {
            baseTransform.transform.Rotate(Vector3.forward, -90f);
            Debug.Log("Going Right -90z");
        }
        else if(dir==Vector3.left)
        {
            baseTransform.transform.Rotate(Vector3.forward, 90f);

            Debug.Log("Going left 90z");
        }
        else if(dir==Vector3.up)
        {
            baseTransform.transform.Rotate(Vector3.right, 180f);

            Debug.Log("Going up");
        }
        else if(dir==Vector3.down)
        {
            //baseTransform.transform.Rotate(Vector3.right, 180f);

            Debug.Log("Going Down 180x");
        }
        else if(dir==Vector3.forward)
        {
            baseTransform.transform.Rotate(Vector3.right, 180f);

            Debug.Log("Going forward 90x");
        }
        else if(dir==Vector3.back)
        {
            baseTransform.transform.Rotate(Vector3.right, -90f);

            Debug.Log("Going Down -90x"); 
        }
        else
        {
            Debug.Log("Dont know direction going");
        }
    }
}
