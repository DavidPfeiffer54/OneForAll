using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerName;
    public bool isMoving = false;
    private float moveSpeed = .2f;
    private float fallSpeed = 2f;

    public Vector3 myPosition;
    public Vector2Int moveTo;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public IEnumerator movePlayerFall(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime=0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + new Vector3(0,0,300) + direction*150;

        SpriteRenderer spriteRenderer = transform.Find("player").GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color targetColor = Color.white;

        while(elapsedTime < fallSpeed)
        {
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

        while(elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime/moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        myPosition=myPosition+direction;
        Debug.Log(myPosition);
        isMoving = false;
    }
    public Vector2Int getPosition()
    {
        return new Vector2Int((int)myPosition.x, (int)myPosition.y);
    }
    public void setPosition(int x, int y)
    {
        myPosition = new Vector3(x, y, 0);
        transform.position = new Vector3(x*5 + 2.5f, y*5 + 2.5f, 0);
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
