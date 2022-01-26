using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{


    [SerializeField]
    Gameboard board = default;

    void Awake()
    {
        Instantiate(board);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool CanMoveToSpecifiedTile (int x, int y) {

        return board.GetBoard()[x,y].CanBuildOnTile();
    }

}
