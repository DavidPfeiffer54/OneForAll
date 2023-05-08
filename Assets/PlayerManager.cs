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
    public bool isPlayerAt(Vector3 loc)
    {
        foreach (GameObject p in players)
        {
            if(p.GetComponent<PlayerController>().getPosition() == loc)
                return true;
        }
        return false;
    }
    public bool isPlayerMovingTo(Vector3 loc)
    {
        foreach (GameObject p in players)
        {
            if(p.GetComponent<PlayerController>().getMoveTo() == loc)
            return true;
        }
        return false;
    }

    public void setUpPlayers(GameObject leveInfo)
    {
        Debug.Log("hihihihihihihihihihih");
        foreach (GameObject obj in players)
        {
            Destroy(obj);
        }
        List<GameObject> newPlayers = new List<GameObject>();
        Dictionary<Vector3, GameObject> playerStarts = leveInfo.GetComponent<LevelInfo>().playerStarts;
        foreach (KeyValuePair<Vector3, GameObject> kvp in playerStarts)
        {
            GameObject newPlayer = Instantiate (playerPrefab, kvp.Value.GetComponent<PlayerStart>().getPosition() + new Vector3(0,0,-2.5f), Quaternion.identity);
            newPlayer.GetComponent<PlayerController>().setPosition(kvp.Value.GetComponent<PlayerStart>().getPosition());
            newPlayer.GetComponent<PlayerController>().playerName="player"+kvp.Key.ToString();
            newPlayer.transform.Find("player").GetComponent<SpriteRenderer>().color=kvp.Value.GetComponent<PlayerStart>().getCol();
            MeshRenderer cubeRenderer = newPlayer.transform.Find("Cube").GetComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.color = kvp.Value.GetComponent<PlayerStart>().getCol();
            cubeRenderer.material = newMaterial;
            newPlayer.GetComponent<PlayerController>().setCol(kvp.Value.GetComponent<PlayerStart>().getCol());
            newPlayers.Add(newPlayer);
        }
        players = newPlayers.ToArray();
        Debug.Log("bybybybybybybybyb");

    }
    public GameObject[] SortPlayersByDirection(Vector2Int direction)
    {
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
