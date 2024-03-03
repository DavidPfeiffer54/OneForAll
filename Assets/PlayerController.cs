using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : GameItem
{
    public string playerName;
    public bool isMoving = false;
    private float moveSpeed = .2f;
    private float fallSpeed = 1.25f;
    public Color col;

    public Vector3 myPosition;
    public Vector3 moveTo;
    public Color startingColor;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void setStartingColor(Color col)
    {
        startingColor = col;
    }
    public void startPlayerChangeColor(Color col)
    {
        StartCoroutine(playerChangeColor(col));
    }
    public IEnumerator playerChangeColor(Color col)
    {
        Color origCol = getCol();
        Color targetCol = col;
        float elapsedTime = 0;
        while (elapsedTime < moveSpeed)
        {
            Color currentColor = Color.Lerp(origCol, targetCol, elapsedTime / moveSpeed);

            MeshRenderer cubeRenderer = transform.Find("Cube").GetComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = currentColor;
            cubeRenderer.material = newMaterial;
            setCol(targetCol);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void startMovePlayerFallDown()
    {
        StartCoroutine(movePlayerFallDown(new Vector3(0, 0, 1)));
    }
    public void startMovePlayerPushed(Vector3 direction)
    {
        StartCoroutine(movePlayerFallDown(direction));
    }

    public IEnumerator movePlayerFallDown(Vector3 direction)
    {
        isMoving = true;
        setMoveTo(getPosition() + direction);

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + direction * 5;
        float elapsedTime = 0;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.Find("Cube").position = transform.position;//+new Vector3(0,0,-2.5f);
        transform.position = targetPos;
        myPosition = myPosition + direction;

        isMoving = false;
        setMoveTo(new Vector3(-1, -1, -1));
        //transform.Find("Cube").position = new Vector3(.5f, .5f, 0f);
        transform.Find("Cube").localPosition = new Vector3(.5f, .5f, 0);
    }
    public IEnumerator movePlayerFall(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + new Vector3(0, 0, 50);// + direction*50;

        SpriteRenderer spriteRenderer = transform.Find("player").GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color targetColor = Color.white;

        var anchor = transform.position + new Vector3(0, 0, -5f);
        var axis = Vector3.Cross(Vector3.back, direction);

        while (elapsedTime < fallSpeed)
        {
            Debug.Log("nononon");
            transform.Find("Cube").RotateAround(anchor, axis, .2f);
            transform.Find("Cube").position = Vector3.Lerp(origPos, targetPos, (elapsedTime / fallSpeed));

            spriteRenderer.color = Color.Lerp(originalColor, targetColor, (elapsedTime / fallSpeed));
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / fallSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMoving = false;


        transform.position = targetPos;
        //transform.Find("Cube").position = new Vector3(.5f, .5f, 0f);
        transform.Find("Cube").localPosition = new Vector3(.5f, .5f, 0);
    }

    public void startMovePlayer(Vector3 direction)
    {
        StartCoroutine(movePlayer(direction));
    }
    public IEnumerator movePlayer(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + direction * 5;
        var anchor = transform.Find("Cube").position + (Vector3.forward + direction) * 2.5f;
        var axis = Vector3.Cross(Vector3.back, direction);

        for (var i = 0; i < 90 / 3; i++)
        {
            transform.Find("Cube").RotateAround(anchor, axis, 3);
            yield return new WaitForSeconds(0.01f);
        }

        // Reset position after rotation
        transform.Find("Cube").localPosition = Vector3.zero;

        // Assign final position
        transform.position = targetPos;
        myPosition = myPosition + direction;

        // Reset position again
        transform.Find("Cube").localPosition = new Vector3(.5f, .5f, 0f);

        //transform.Find("Cube").position = new Vector3(.5f, .5f, 0f);
        isMoving = false;



    }
    public void startRockPlayer(Vector3 direction)
    {
        StartCoroutine(rockPlayer(direction));
    }
    public IEnumerator rockPlayer(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0;

        Vector3 origPos = transform.position;
        Vector3 targetPos = origPos + direction * 5;
        var anchor = transform.position + new Vector3(0, 0, 0) + (Vector3.forward + direction) * 2.5f;
        var axis = Vector3.Cross(Vector3.back, direction);

        for (var i = 0; i < 10 / 3; i++)
        {
            transform.Find("Cube").RotateAround(anchor, axis, 3);
            yield return new WaitForSeconds(0.02f);
        }

        for (var i = 0; i < 10 / 3; i++)
        {
            transform.Find("Cube").RotateAround(anchor, axis, -3);
            yield return new WaitForSeconds(0.06f);
        }
        transform.Find("Cube").RotateAround(anchor, axis, -3);
        yield return new WaitForSeconds(0.01f);
        transform.Find("Cube").RotateAround(anchor, axis, 3);
        yield return new WaitForSeconds(0.01f);
        //transform.Find("Cube").position = new Vector3(.5f, .5f, 0f);//transform.position;//+new Vector3(0,0,-2.5f);
        transform.Find("Cube").localPosition = new Vector3(.5f, .5f, 0);
        //transform.position = origPos;
        //myPosition=origPos;

        isMoving = false;
    }
    public override Vector3 getPosition()
    {
        return myPosition;
    }
    public override void setPosition(Vector3 newPos)
    {
        myPosition = newPos;
        transform.position = new Vector3(myPosition.x * 5, myPosition.y * 5, myPosition.z * 5);
        transform.Find("Cube").localPosition = new Vector3(.5f, .5f, 0);
    }
    public Vector3 getMoveTo()
    {
        return moveTo;
    }
    public void setMoveTo(Vector3 _moveTo)
    {
        moveTo = _moveTo;
    }
    public Color getCol()
    {
        return col;
    }
    public void setCol(Color newCol)
    {
        col = newCol;
        MeshRenderer cubeRenderer = transform.Find("Cube").GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Standard"));
        newMaterial.color = col;
        cubeRenderer.material = newMaterial;
    }
    public override void resetPosition()
    {
        setPosition(startingLoc);
        Debug.Log(startingColor);
        startPlayerChangeColor(startingColor);
    }
}
