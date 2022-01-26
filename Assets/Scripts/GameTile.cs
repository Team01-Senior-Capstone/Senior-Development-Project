using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{

    [SerializeField]
    private int row, col;

    [SerializeField]
    Piece curPiece = null;

    [SerializeField]
    Pipe pipe = null;

    void Awake() {
        this.row = 0;
        this.col = 0;
    }

    void Start()
    {
        
    }

    public void Init(int row, int col) {
        this.row = row;
        this.col = col;
    }

    void increasePipeHeight () 
    {
        if (CanBuildOnTile()) {
            if (pipe == null) {
                Pipe cur = new Pipe();
                pipe = Instantiate(cur);
                pipe.Init();
            }
            else 
                if (!pipe.IsMaxHeight())
                    pipe.IncreaseHeight();
        }
    }

    public bool CanBuildOnTile () {

        return curPiece == null && !pipe.IsMaxHeight();    
    }

    public int getRow () {
        return this.row;
    }

    public int getCol() {
        return this.col;
    }

    void Update()
    {
        
    }
}
