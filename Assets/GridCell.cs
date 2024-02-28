using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : GameItem
{

    private int posX;
    private int posY;
    private int posZ;
    public bool isMoving;
    public Vector3 moveTo;
    private float moveSpeed = .2f;

    public GameObject objectInThisGridSpace = null;
    public GameObject arrows = null;
    [SerializeField] private GameObject arrowPrefab;

    public bool isOccupied = false;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void startMoveCellPushed(Vector3 direction)
    {
        StartCoroutine(moveCellPush(direction));
    }
    public IEnumerator moveCellPush(Vector3 direction)
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
        //transform.Find("Cube").position = transform.position;//+new Vector3(0,0,-2.5f);

        transform.position = targetPos;
        posX = posX + (int)direction.x;
        posY = posY + (int)direction.y;
        posZ = posZ + (int)direction.z;

        isMoving = false;
        setMoveTo(new Vector3(-1, -1, -1));

    }
    public void setPosition(int x, int y, int z)
    {
        posX = x;
        posY = y;
        posZ = z;
    }

    public override Vector3 getPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    public Vector3 getLoc()
    {
        return new Vector3(posX, posY, posZ);
    }
    public void createArrows()
    {
        arrows = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrows.transform.SetParent(transform);
    }

    public void setArrowDir(Vector3 dir)
    {
        arrows.GetComponent<Arrows>().setDirectionOfMoving(dir);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public Vector3 getMoveTo()
    {
        return moveTo;
    }
    public void setMoveTo(Vector3 _moveTo)
    {
        moveTo = _moveTo;
    }
    public bool getIsMoving()
    {
        return isMoving;
    }
    public void setIsMoving(bool _isMoving)
    {
        isMoving = _isMoving;
    }
}
