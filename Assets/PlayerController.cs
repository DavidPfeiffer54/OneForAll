using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public string playerName;
    public bool isMoving = false;
    private float moveSpeed = .2f;
    private float fallSpeed = 1.25f;

    public Vector3 myPosition;
    public Vector3 moveTo;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator movePlayerFallDown(Vector3 direction)
    {
        Debug.Log("Falling Down");
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
        transform.Find("Cube").position = transform.position;//+new Vector3(0,0,-2.5f);

        transform.position = targetPos;
        myPosition=myPosition+direction;

        //Debug.Log(myPosition);
        isMoving = false;
    }
    public IEnumerator movePlayerFall(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime=0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + new Vector3(0,0,100) + direction*50;

        SpriteRenderer spriteRenderer = transform.Find("player").GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color targetColor = Color.white;

        var anchor = transform.position+new Vector3(0,0,-5f);
        var axis = Vector3.Cross(Vector3.back, direction);

        while(elapsedTime < fallSpeed)
        {
            transform.Find("Cube").RotateAround(anchor, axis, .2f);
            transform.Find("Cube").position = Vector3.Lerp(origPos, targetPos, (elapsedTime/fallSpeed));

            spriteRenderer.color = Color.Lerp(originalColor, targetColor, (elapsedTime/fallSpeed));
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/fallSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMoving = false;


        transform.position = targetPos;
    }

    public IEnumerator movePlayer(Vector3 direction)
    {
        isMoving = true;

        float elapsedTime=0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + direction*5;

        var anchor = transform.position+new Vector3(0,0,0) + (Vector3.forward + direction) * 2.5f;
        var axis = Vector3.Cross(Vector3.back, direction);
        Debug.Log(anchor);
        Debug.Log(axis);
        Debug.Log(transform.Find("Cube").position);

        for (var i = 0; i < 90 / 3; i++) {
            transform.Find("Cube").RotateAround(anchor, axis, 3);
            yield return new WaitForSeconds(0.01f);
        }

        //while(elapsedTime < moveSpeed)
        //{
        //    transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/moveSpeed));
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}
        transform.Find("Cube").position = transform.position;//+new Vector3(0,0,-2.5f);

        transform.position = targetPos;
        myPosition=myPosition+direction;

        //Debug.Log(myPosition);
        isMoving = false;
    }
    public Vector3 getPosition()
    {

        return new Vector3((int)myPosition.x, (int)myPosition.y, (int)myPosition.z);
    }
    public void setPosition(Vector3 newPos)
    {
        myPosition = newPos;
        transform.position = new Vector3(myPosition.x*5 + 2.5f, myPosition.y*5 + 2.5f, myPosition.z*5 - 2.5f);
        transform.Find("Cube").position = transform.position;
        transform.Find("Cube").rotation = Quaternion.identity;
    }
    public Vector3 getMoveTo()
    {
        return moveTo;
    }
    public void setMoveTo(int x, int y, int z)
    {
        moveTo = new Vector3(x,y,z);
    }
}
