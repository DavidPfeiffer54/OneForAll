using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public virtual Color getColor()
    {
        return new Color(0, 0, 0);
    }
    public virtual void setColor(Color c)
    {
        Debug.Log("Item doesnt have color");
    }
    public virtual Vector3 getPosition()
    {
        return new Vector3(0, 0, 0);
    }
    public virtual void setPosition(Vector3 _loc)
    {
        Debug.Log("Shouldnt be called");
    }

    public void editMove(Vector3 dir)
    {
        StartCoroutine(moveItem(dir));
    }
    public virtual IEnumerator moveItem(Vector3 dir)
    {

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + dir * 5;
        float elapsedTime = 0;

        while (elapsedTime < .2f)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / .2f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //transform.Find("Cube").position = transform.position;//+new Vector3(0,0,-2.5f);
        transform.position = targetPos;
        setPosition(getPosition() + dir);
        Debug.Log("MOVING_______");
        Debug.Log(getPosition());
        Debug.Log(transform.position);

    }
}
