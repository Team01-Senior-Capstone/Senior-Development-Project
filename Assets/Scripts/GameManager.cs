using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Action
{
    BUILD, PLAY, FIRST_MOVE, SECOND_MOVE, SELECT, OPP_TURN
}

public class GameManager : MonoBehaviour
{
    public GameObject board;
    public GameObject game_object;

    public Game g;

    GameObject worker_1;
    GameObject worker_2;
    public GameObject enemy_1;
    public GameObject enemy_2;

    public GameObject selectedWorker;

    public GameObject selectedWorker_tile;
    Action action;

    public GameObject[] characters;
    public string[] tags = { "Mario", "Luigi", "Peach", "Goomba" };


    Gamecore.Player me;
    Gamecore.Player opponent;
    Gamecore.Worker gameCoreWorker1;
    Gamecore.Worker gameCoreWorker2;


    public Gamecore.Player getMe() { return me; }
    public Gamecore.Player getOpponenet() { return opponent; }
    public Gamecore.Worker getGameCoreWorker1() { return gameCoreWorker1; }
    public Gamecore.Worker getGameCoreWorker2() { return gameCoreWorker2; }

    List<GameObject> allTiles;

    public GameObject getWorker1()
    {
        return worker_1;
    }

    public GameObject getWorker2()
    {
        return worker_2;
    }

    public void gameCorePlaceWorker(int row, int col, int workerNum)
    {
        if (workerNum == 1)
        {
            g.game.placePiece(gameCoreWorker1, row, col);
        }
        else
        {
            g.game.placePiece(gameCoreWorker2, row, col);
        }
    }

    public void Start()
    {
        game_object = GameObject.Find("Game");
        Debug.Log(game_object.name);
        g = game_object.GetComponent<Game>();
        Debug.Log(g.netWorkGame);

        worker_1 = translateTag(g.worker1_tag);
        worker_2 = translateTag(g.worker2_tag);


        if(g.netWorkGame == false)
        {
            if(g.playerGoesFirst)
            {
                Gamecore.Player[] players = g.game.assignPlayers(Gamecore.Identification.Human, Gamecore.Identification.AI);
                me = players[0];
                opponent = players[1];
            }
            else
            {
                Gamecore.Player[] players = g.game.assignPlayers(Gamecore.Identification.AI, Gamecore.Identification.Human);
                opponent = players[0];
                me = players[1];
            }
        }


        gameCoreWorker1 = new Gamecore.Worker(me);
        gameCoreWorker2 = new Gamecore.Worker(me);

        action = Action.FIRST_MOVE;
        allTiles = new List<GameObject>();
        foreach (Transform child in board.transform)
            allTiles.Add(child.gameObject);
        toggleSelectedTiles(allTiles);
    }

    public void returnToSelect()
    {
        action = Action.SELECT;
        toggleWorkerTiles();
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
            Debug.Log(selectedWorker_tile.name + " can move to: ");
            //Debug.Log("t size: " + t.Count);
            List<GameObject> movableTiles = new List<GameObject>();
            foreach(Gamecore.Tile ti in t)
            {
                string name = ti.getRow() + ", " + ti.getCol();
                Debug.Log(name);
                GameObject go = GameObject.Find(name);
                movableTiles.Add(go);
            }
            
            toggleSelectedTiles(movableTiles);
        }
        else if (action == Action.PLAY)
        {
            Debug.Log("Selected Tile: " + selectedWorker_tile.GetComponent<Tile>().row + ", " +
                                                          selectedWorker_tile.GetComponent<Tile>().col);
            Debug.Log(g.game.getGameboard()[1, 0].getWorker());
            List<Gamecore.Tile> t = g.game.getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().row,
                                                          selectedWorker_tile.GetComponent<Tile>().col,
                                                          Gamecore.Action.Build);
            List<GameObject> buildableTiles = new List<GameObject>();
            Debug.Log("Buildable spaces: ");
            foreach (Gamecore.Tile ti in t)
            {
                string name = ti.getRow() + ", " + ti.getCol();
                Debug.Log(name);
                GameObject go = GameObject.Find(name);
                buildableTiles.Add(go);
            }

            toggleSelectedTiles(buildableTiles);
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
        deselectAll();
        foreach (GameObject tile in tiles) {
            Tile s = tile.GetComponent<Tile>();
            s.selectable = true;
        }
    }

    void deselectAll()
    {

        foreach (GameObject tile in allTiles)
        {
            Tile s = tile.GetComponent<Tile>();
            s.selectable = false;
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
