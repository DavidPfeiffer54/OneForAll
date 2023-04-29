using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName;
    public bool isMoving = false;
    public float moveSpeed = 3f;

    public Vector2Int moveTo;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //if (isMoving == false)
        //{
        //    if (Input.GetKey(KeyCode.UpArrow))
        //    {
        //        StartCoroutine(movePlayer(Vector3.up*5));
        //    }
        //    if (Input.GetKey(KeyCode.LeftArrow))
        //    {
        //        StartCoroutine(movePlayer(Vector3.left*5));
        //
        //    }
        //    if (Input.GetKey(KeyCode.RightArrow))
        //    {
        //        StartCoroutine(movePlayer(Vector3.right*5));
        //
        //    }
        //    if (Input.GetKey(KeyCode.DownArrow))
        //    {
        //        StartCoroutine(movePlayer(Vector3.down*5));
        //    }
        //    Debug.Log(getPosition());
        //    Debug.Log(transform.position);
        //}
    }
    public IEnumerator movePlayerFall(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime=0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + new Vector3(0,0,500) + direction*200;

        SpriteRenderer spriteRenderer = transform.Find("player").GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color targetColor = Color.white;

        while(elapsedTime < 2f)
        {
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, (elapsedTime/2f));
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/2f));
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

        while(elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        transform.position = targetPos;
        isMoving = false;
    }
    public void resetPosition()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    public Vector2Int getPosition()
    {
        return new Vector2Int((int)transform.position.x/5, (int)transform.position.y/5);
    }
    public void setPosition(int x, int y)
    {
        transform.position = new Vector3(x*5 + 2.5f, y*5 + 2.5f, transform.position.z);
    }
    public Vector2Int getMoveTo()
    {
        return moveTo;
    }
    public void setMoveTo(int x, int y)
    {
        moveTo = new Vector2Int(x,y);
    }
}
