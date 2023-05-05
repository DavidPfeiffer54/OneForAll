using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public int level_num;
    public int width;
    public int height;
    public int[,] walls;
    public int[,] goals;
    public int[,] players;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(int _level_num, int _width, int _height, int[,] _walls, int[,] _goals, int[,] _players)
    {
        level_num=_level_num;
        width=_width;
        height=_height;
        walls=_walls;
        goals=_goals;
        players=_players;
    }
}
