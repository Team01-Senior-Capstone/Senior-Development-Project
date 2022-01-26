using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour 
{

    [SerializeField]
    GameTile[,] board;

    [SerializeField]
    GameTile Tile = default;

    // Start is called before the first frame update
    void Awake() 
    {
        board = new GameTile[5, 5];

        for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++) {
                board[i,j] = Instantiate(Tile, new Vector3(i, j, 0), Quaternion.identity);
                board[i,j].Init(i, j);
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
