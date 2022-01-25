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

    bool CanBuildOnTile () {

        if (curPiece != null)
            return false;
        else if (pipe.IsMaxHeight())
            return false;

        return true;    
    }

    void Update()
    {
        
    }
}
