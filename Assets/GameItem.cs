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
    public void FlyIn(Vector3 targetPosition, float flySpeed)
    {
        StartCoroutine(FlyToTarget(getPosition() * 5, flySpeed));
    }

    IEnumerator FlyToTarget(Vector3 targetPosition, float flySpeed)
    {
        float smoothTime = 0.3f; // Adjust the smooth time for the desired deceleration effect
        Vector3 currentVelocity = Vector3.zero;

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // Calculate the new position using SmoothDamp
            Vector3 newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, flySpeed);

            // Move the object to the new position
            transform.position = newPosition;

            yield return null;
        }
        transform.position = targetPosition;
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
