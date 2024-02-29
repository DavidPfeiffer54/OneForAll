using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager : MonoBehaviour
{

    [SerializeField] private GameObject playerPrefab;
    public GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator flyInPlayers()
    {
        foreach (GameObject player in players)
        {
            GameItem playerItem = player.GetComponent<GameItem>();
            playerItem.transform.position = playerItem.transform.position + new Vector3(0, 0, -100);
        }
        Coroutine flyButtonsCoroutine = StartCoroutine(FlyPlayers());
        yield return flyButtonsCoroutine;
    }

    IEnumerator FlyPlayers()
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();
        // Disable user input during movement
        foreach (GameObject player in players)
        {
            float flySpeed = 50f;
            GameItem playerItem = player.GetComponent<GameItem>();
            //playerItem.FlyIn(playerItem.getPosition() * 5 + new Vector3(2.5f, 2.5f, 0f), flySpeed);
            Coroutine newCoroutine = StartCoroutine(playerItem.FlyToTarget(playerItem.getPosition() * 5 + new Vector3(2.5f, 2.5f, 0f), flySpeed));
            runningCoroutines.Add(newCoroutine);
            yield return new WaitForSeconds(0.1f); // Delay between flying each button
        }
        foreach (Coroutine coroutine in runningCoroutines)
        {
            yield return coroutine;
        }
        // Enable user input after all buttons have flown in
    }
    public GameObject getPlayerAt(Vector3 loc)
    {
        foreach (GameObject p in players)
        {
            if (p.GetComponent<PlayerController>().getPosition() == loc)
            {
                return p;
            }
        }
        return null;
    }




    public bool isPlayerAt(Vector3 loc)
    {
        return players.Any(p => p.GetComponent<PlayerController>().getPosition() == loc);
    }
    public bool isPlayerMoveTo(Vector3 loc)
    {
        return players.Any(p => p.GetComponent<PlayerController>().getMoveTo() == loc);
    }

    public bool isPlayerMovingTo(Vector3 loc)
    {
        return players.Any(p => p.GetComponent<PlayerController>().getMoveTo() == loc);
    }

    public bool isPlayerMovingTo(Vector3 loc, GameObject[] playerlist)
    {
        return playerlist.Any(p => p.GetComponent<PlayerController>().getMoveTo() == loc);
    }

    public void resetMoveTo()
    {
        foreach (GameObject p in players)
        {
            p.GetComponent<PlayerController>().setMoveTo(new Vector3(-1, -1, -1));
        }
    }

    public void setUpPlayers(GameObject leveInfo)
    {
        foreach (GameObject obj in players)
        {
            Destroy(obj);
        }
        List<GameObject> newPlayers = new List<GameObject>();
        Dictionary<Vector3, GameObject> playerStarts = leveInfo.GetComponent<LevelInfo>().playerStarts;
        foreach (KeyValuePair<Vector3, GameObject> kvp in playerStarts)
        {
            GameObject newPlayer = Instantiate(playerPrefab, kvp.Value.GetComponent<PlayerStart>().getPosition(), Quaternion.identity);
            newPlayer.GetComponent<PlayerController>().setPosition(kvp.Value.GetComponent<PlayerStart>().getPosition());
            newPlayer.GetComponent<PlayerController>().playerName = "player" + kvp.Key.ToString();
            newPlayer.transform.Find("player").GetComponent<SpriteRenderer>().color = kvp.Value.GetComponent<PlayerStart>().getColor();
            MeshRenderer cubeRenderer = newPlayer.transform.Find("Cube").GetComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = kvp.Value.GetComponent<PlayerStart>().getColor();
            cubeRenderer.material = newMaterial;
            newPlayer.GetComponent<PlayerController>().setCol(kvp.Value.GetComponent<PlayerStart>().getColor());
            newPlayers.Add(newPlayer);
            Debug.Log("---");
            Debug.Log(newPlayer.transform.position);
        }
        players = newPlayers.ToArray();

    }

    public GameObject[] SortPlayersByDirection(Vector2Int direction)
    {
        //var result = _db.Employee
        //      .OrderBy( e => e.EmpName )  //OrderBy and SortBy empname  Ascending
        //      .ThenBy( ed => ed.EmpAddress ); //orderby address ascending

        // Check if the direction vector is valid
        if (Mathf.Abs(direction.x) + Mathf.Abs(direction.y) != 1)
        {
            Debug.LogError("Direction vector must be a unit vector (i.e., have magnitude 1).");
            return null;
        }

        // Sort the players based on the specified direction
        if (direction.x == 1)
        {
            // Sort by x (right)
            System.Array.Sort(players, (a, b) => b.GetComponent<PlayerController>().getPosition().x.CompareTo(a.GetComponent<PlayerController>().getPosition().x));
        }
        else if (direction.x == -1)
        {
            // Sort by x (left)
            System.Array.Sort(players, (a, b) => a.GetComponent<PlayerController>().getPosition().x.CompareTo(b.GetComponent<PlayerController>().getPosition().x));
        }
        else if (direction.y == 1)
        {
            // Sort by y (up)
            System.Array.Sort(players, (a, b) => b.GetComponent<PlayerController>().getPosition().y.CompareTo(a.GetComponent<PlayerController>().getPosition().y));
        }
        else if (direction.y == -1)
        {
            // Sort by y (down)
            System.Array.Sort(players, (a, b) => a.GetComponent<PlayerController>().getPosition().y.CompareTo(b.GetComponent<PlayerController>().getPosition().y));
        }
        return players;
    }

    public GameObject[] GetPlayersAtDepth(int depth)
    {
        return players.Where(player => player.GetComponent<PlayerController>().getPosition().z == depth).ToArray();
    }

}
