using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class MovementController : MonoBehaviour
{
    public static MovementController instance;
    public LevelManager levelManager;
    public PlayerManager playerManager;
    private static int activeAnimations = 0;

    public static bool isAnimating => activeAnimations > 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator flyInLevel()
    {
        activeAnimations++;
        //TODO24 
        /*
            2 opotions
                1 - we make the thing you pass a list of lists 
                    we send everything in the array at once
                    [[a,b,c][d,e][f]]
                    so [a,b,c] all get sent at the same time.
                    this is so we can send buttons+colorchangers with the cell they are attached to
                    the issue with this is that then buttons/goals/colorchange can change color.
                    and the basic structre we use might brike 
                2 - we add buttons/goals/colorchanges to children of the cell themselfves.
                    so then we only ever have to move the cell and evrything will go with it.
        */
        GameObject[] combinedArray = levelManager.getCells().Concat(levelManager.getWalls())
                                    .Concat(levelManager.getGoals())
                                    .Concat(levelManager.getPlayerStarts())
                                    .Concat(levelManager.getButtons())
                                   .ToArray();
        combinedArray = sortObjs(combinedArray);
        GameObject[] players = playerManager.players;

        setObjectLocation(combinedArray, new Vector3(0, 0, -40));
        setObjectLocation(players, new Vector3(0, 0, -40));

        Coroutine flyInCoroutine = StartCoroutine(FlyObjs(combinedArray, 50f, .1f, new Vector3(0, 0, 0)));
        yield return flyInCoroutine;
        flyInCoroutine = StartCoroutine(FlyObjs(players, 200f, .1f, new Vector3(0, 0, 0)));
        yield return flyInCoroutine;
        activeAnimations--;
    }

    public IEnumerator flyOutLevel()
    {
        activeAnimations++;
        GameObject previousLevel = levelManager.currentLevel;
        GameObject[] previousPlayers = playerManager.players;

        GameObject[] combinedArray = levelManager.getCells().Concat(levelManager.getWalls())
                                    .Concat(levelManager.getGoals())
                                    .Concat(levelManager.getPlayerStarts())
                                    .Concat(levelManager.getButtons())
                                   .ToArray();
        //GameObject[] players = playerManager.players;
        combinedArray = sortObjs(combinedArray);

        Coroutine flyInCoroutine;
        flyInCoroutine = StartCoroutine(FlyObjs(combinedArray, 200f, .02f, new Vector3(0, 0, 100)));
        yield return flyInCoroutine;
        flyInCoroutine = StartCoroutine(FlyObjs(previousPlayers, 200f, .02f, new Vector3(0, 0, 100)));
        yield return flyInCoroutine;



        Destroy(previousLevel);
        foreach (GameObject obj in previousPlayers)
        {
            Destroy(obj);
        }
        activeAnimations--;
    }

    public IEnumerator FlyObjs(GameObject[] objects, float flySpeed, float waitBetween, Vector3 addLoc)
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();
        //float flySpeed = 200f;

        foreach (GameObject obj in objects)
        {
            GameItem objItem = obj.GetComponent<GameItem>();
            Coroutine newCoroutine = StartCoroutine(objItem.FlyToTarget(objItem.getPosition() * 5 + addLoc, flySpeed));
            runningCoroutines.Add(newCoroutine);
            yield return new WaitForSeconds(waitBetween); // Delay between flying each button
        }
        foreach (Coroutine coroutine in runningCoroutines)
        {
            yield return coroutine;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void setObjectLocation(GameObject[] objects, Vector3 locAdd)
    {
        foreach (GameObject obj in objects)
        {
            obj.transform.position = obj.transform.position + locAdd;
        }
    }
    GameObject[] sortObjs(GameObject[] objects)
    {
        return objects.OrderByDescending(obj => obj.GetComponent<GameItem>().getPosition().z)
                      .ThenByDescending(obj => obj.GetComponent<GameItem>().getPosition().y)
                      .ThenByDescending(obj => obj.GetComponent<GameItem>().getPosition().x)
                      .ToArray();
    }

}
