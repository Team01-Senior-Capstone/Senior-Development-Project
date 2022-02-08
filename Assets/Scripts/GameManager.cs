using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    BUILD, PLAY, FIRST_MOVE, SECOND_MOVE, SELECT
}

public class GameManager : MonoBehaviour
{
    public GameObject board;


    public GameObject worker_1;
    public GameObject worker_2;
    public GameObject enemy_1;
    public GameObject enemy_2;

    public GameObject selectedWorker;

    public GameObject selectedWorker_tile;
    Action action;


    List<GameObject> allTiles;

    public void toggleAction()
    {
        if(action == Action.BUILD)
        {
            action = Action.SELECT;
            toggleWorkerTiles();
        }
        else if(action == Action.SELECT)
        {
            action = Action.PLAY;
            toggleSelectedTiles(allTiles);
        }
        else if(action == Action.PLAY)
        {
            action = Action.BUILD;
        }
        else if (action == Action.FIRST_MOVE)
        {
            action = Action.SECOND_MOVE;
        }
        else if (action == Action.SECOND_MOVE)
        {
            action = Action.SELECT;
            toggleWorkerTiles();
        }
    }

    void toggleSelectedTiles(List<GameObject> tiles)
    {
        foreach(GameObject tile in tiles) {
            Tile s = tile.GetComponent<Tile>();
            s.selectable = true;
        }
    }

    public void toggleWorkerTiles()
    {
        foreach(GameObject go in allTiles)
        {
            Tile t = go.GetComponent<Tile>();
            if (t.worker != null)
            {
                t.selectable = true;
            }
            else
            {
                t.selectable = false;
            }
        }
    }

    public Action getAction()
    {
        return action;
    }
    // Start is called before the first frame update
    void Start()
    {
        action = Action.FIRST_MOVE;
        allTiles = new List<GameObject>();
        foreach (Transform child in board.transform)
            allTiles.Add(child.gameObject);
        Debug.Log(allTiles.Count);
        toggleSelectedTiles(allTiles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
