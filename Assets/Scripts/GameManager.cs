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
    public GameObject game_object;

    Game g;

    GameObject worker_1;
    GameObject worker_2;
    public GameObject enemy_1;
    public GameObject enemy_2;

    public GameObject selectedWorker;

    public GameObject selectedWorker_tile;
    Action action;

    public GameObject[] characters;
    public string[] tags = { "Mario", "Luigi", "Peach", "Goomba" };




    List<GameObject> allTiles;

    public GameObject getWorker1()
    {
        return worker_1;
    }

    public GameObject getWorker2()
    {
        return worker_2;
    }

    public void Start()
    {
        game_object = GameObject.Find("Game");
        Debug.Log(game_object.name);
        g = game_object.GetComponent<Game>();
        Debug.Log(g.netWorkGame);

        worker_1 = translateTag(g.worker1_tag);
        worker_2 = translateTag(g.worker2_tag);

        action = Action.FIRST_MOVE;
        allTiles = new List<GameObject>();
        foreach (Transform child in board.transform)
            allTiles.Add(child.gameObject);
        Debug.Log(allTiles.Count);
        toggleSelectedTiles(allTiles);
    }

    public void toggleAction()
    {
        if (action == Action.BUILD)
        {
            action = Action.SELECT;
            toggleWorkerTiles();
        }
        else if (action == Action.SELECT)
        {
            action = Action.PLAY;
            List<Gamecore.Tile> t = g.game.getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().row,
                                                          selectedWorker_tile.GetComponent<Tile>().col,
                                                          Gamecore.Action.Move);
            Debug.Log("t size: " + t.Count);
            List<GameObject> movableTiles = new List<GameObject>();
            foreach(Gamecore.Tile ti in t)
            {
                string name = ti.getRow() + ", " + ti.getCol();
                GameObject go = GameObject.Find(name);
                movableTiles.Add(go);
            }
            toggleSelectedTiles(movableTiles);
        }
        else if (action == Action.PLAY)
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
        foreach (GameObject tile in tiles) {
            Tile s = tile.GetComponent<Tile>();
            s.selectable = true;
        }
    }

    public void toggleWorkerTiles()
    {
        foreach (GameObject go in allTiles)
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

    GameObject translateTag(string tag)
    {
        for(int i = 0; i < tags.Length; i++)
        {
            if(tags[i] == tag)
            {
                return characters[i];
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
