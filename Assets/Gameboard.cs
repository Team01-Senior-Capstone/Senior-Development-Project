using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour 
{
    GameTile[,] board;

    // Start is called before the first frame update
    void Awake() 
    {
        board = new GameTile[5,5];

        for (int i = 0; i < 5; i++)
            for (int y = 0; y < 5; y++) {
                GameTile cur = new GameTile();
                board[i,y] = Instantiate(cur);
                board[i,y].Init(i, y);
            }
    }

    // Update is called once per frame
    void Update() 
    {
        
    }
}
