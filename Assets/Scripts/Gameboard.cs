using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour 
{

    [SerializeField]
    GameTile[,] board;

    [SerializeField]
    GameTile Tile = default;

    void Awake() 
    {
        board = new GameTile[5, 5];

        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++) {
                board[i,j] = Instantiate(Tile, new Vector3(i, 0, j), Quaternion.identity);
            }
    }

    public GameTile[,] GetBoard () {
        return this.board;
    }

    // Update is called once per frame
    void Update() 
    {
        
    }
}
