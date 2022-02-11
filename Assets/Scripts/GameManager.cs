using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Action
{
    BUILD, PLAY, FIRST_MOVE, SECOND_MOVE, SELECT, OPP_TURN
}

public class GameManager : MonoBehaviour
{
    public GameObject board;
    public GameObject game_object;

    public GameObject UI_Oppoenent_Object;
    public OpponentManager oppMan;

    public Game g;

    GameObject worker_1;
    GameObject worker_2;
    //The actual instantiated prefab
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


    public ref Gamecore.Player getMe() { return ref me; }
    public ref Gamecore.Player getOpponenet() { return ref opponent; }
    public Gamecore.Worker getGameCoreWorker1() { return gameCoreWorker1; }
    public Gamecore.Worker getGameCoreWorker2() { return gameCoreWorker2; }

    public Gamecore.Worker opponentWorker1;
    public Gamecore.Worker opponentWorker2;

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
        Debug.Log(g.playerGoesFirst);


        UI_Oppoenent_Object = GameObject.Find("Opponent");
        oppMan = UI_Oppoenent_Object.GetComponent<OpponentManager>();

        initializePlayers();



        gameCoreWorker1 = new Gamecore.Worker(me, true);
        gameCoreWorker2 = new Gamecore.Worker(me, false);
        opponentWorker1 = new Gamecore.Worker(opponent, true);
        opponentWorker2 = new Gamecore.Worker(opponent, false);

        if(!oppMan.multiplayer)
        {
            oppMan.AI_Game();
            assignRandAIWorkers();
        }

        if(!g.playerGoesFirst)
        {
            placeAIWorkers();
            
        }




        //Get Prefabs for corresponding tags
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

    public void initializePlayers()
    {
        //Initialize players
        if (!g.netWorkGame)
        {
            me = new Gamecore.Player(g.playerGoesFirst, Gamecore.Identification.Human);
            opponent = new Gamecore.Player(!g.playerGoesFirst, Gamecore.Identification.AI);
        }
        else
        {
            if (g.host)
            {
                me = new Gamecore.Player(g.playerGoesFirst, Gamecore.Identification.Host);
                opponent = new Gamecore.Player(!g.playerGoesFirst, Gamecore.Identification.Client);
            }
            else
            {
                me = new Gamecore.Player(g.playerGoesFirst, Gamecore.Identification.Client);
                opponent = new Gamecore.Player(!g.playerGoesFirst, Gamecore.Identification.Host);
            }
        }
    }


    //Places the AI's first two pieces
    public void placeAIWorkers()
    {
        Tuple<Move, Move> moves = oppMan.getOpp().getWorkerPlacements(g.game);

        bool succeed1 = g.game.placePiece(opponentWorker1, moves.Item1.toTile.getRow(), moves.Item1.toTile.getCol());
        bool succeed2 = g.game.placePiece(opponentWorker2, moves.Item2.toTile.getRow(), moves.Item2.toTile.getCol());
        Debug.Log(opponentWorker1.getOwner());
        Debug.Log(opponentWorker2.getOwner());
        foreach (Transform child in board.transform)
        {
            string tileName = moves.Item1.toTile.getRow() + ", " + moves.Item1.toTile.getCol();
            string tile2Name = moves.Item2.toTile.getRow() + ", " + moves.Item2.toTile.getCol();
            if (child.gameObject.name == tileName)
            {
                enemy_1 = Instantiate(oppMan.getOpp().getWorker1(), child.GetComponent<Tile>().middle, Quaternion.Euler(new Vector3(0, 180, 0)));
            }
            else if(child.gameObject.name == tile2Name)
            {
                enemy_2 = Instantiate(oppMan.getOpp().getWorker2(), child.GetComponent<Tile>().middle, Quaternion.Euler(new Vector3(0, 180, 0)));
            }
        }
    }

    //Maybe change later
    void assignRandAIWorkers()
    {
        System.Random rnd = new System.Random();
        int num = rnd.Next(characters.Length);

        oppMan.getOpp().setWorker1(characters[num]);
        num = rnd.Next(characters.Length );
        oppMan.getOpp().setWorker2(characters[num]);

    }

    public void updateGUI(Tuple<Move, Move> moves) 
    {

        Gamecore.Worker work;
        Debug.Log("Worker: " + moves.Item1.worker);
        if(moves.Item1.worker.workerOne)
        {
            work = opponentWorker1;
            
        }
        else
        {
            work = opponentWorker2;
        }
        g.game.movePlayer(work, opponent, moves.Item1.fromTile.getRow(), moves.Item1.fromTile.getCol(),
                              moves.Item1.toTile.getRow(), moves.Item1.toTile.getCol());
        GameObject toTile = null;
        GameObject fromTile = null;
        foreach (Transform child in board.transform) {
            string tileName = moves.Item1.toTile.getRow() + ", " + moves.Item1.toTile.getCol();
            string fromTileName = moves.Item1.fromTile.getRow() + ", " + moves.Item1.fromTile.getCol();
            //Debug.Log("To: " + tileName);
            //Debug.Log("From: " + fromTileName);
            if (child.gameObject.name == tileName)
            {
                toTile = child.gameObject;
                //Debug.Log("Made it to " + tileName);
                //Debug.Log("Middle is: " + child.GetComponent<Tile>().middle);
                ////oppMan.getOpp().getWorker1().transform.position = child.GetComponent<Tile>().middle;
                //if (moves.Item1.worker == "1")
                //    //oppMan.getOpp().changeWorker1Pos(child.GetComponent<Tile>().middle);
                    
                //else
                //    oppMan.getOpp().changeWorker2Pos(child.GetComponent<Tile>().middle);
                //child.GetComponent<Tile>().worker = oppMan.getOpp().getWorker1();
            }
            else if(child.gameObject.name == fromTileName)
            {
                fromTile = child.gameObject;
                child.GetComponent<Tile>().worker = null;
            }

        }
        if(moves.Item1.worker == opponentWorker1)
        {

            toTile.GetComponent<Tile>().moveToTile(enemy_1, fromTile.GetComponent<Tile>());
        }
        else
        {

            toTile.GetComponent<Tile>().moveToTile(enemy_2, fromTile.GetComponent<Tile>());
        }
        Gamecore.TileBuildInfo f = g.game.workerBuild(work, opponent, moves.Item2.fromTile.getRow(), moves.Item2.fromTile.getCol(),
                           moves.Item2.toTile.getRow(), moves.Item2.toTile.getCol());

        Debug.Log("Building: " +  f.wasBuildSuccessful());

        foreach (Transform child in board.transform)
        {
            string tileName = moves.Item2.toTile.getRow() + ", " + moves.Item2.toTile.getCol();
            //Debug.Log("Building on: " + tileName);
            if (child.gameObject.name == tileName)
            {
                child.GetComponent<Tile>().buildOnTile();
            }
        }
    }

    public void toggleAction()
    {
        if (action == Action.BUILD)
        {
            deselectAll();
            Tuple<Move, Move> moves = oppMan.getOpp().GetMove(g.game);
            updateGUI(moves);
            action = Action.SELECT;
            toggleWorkerTiles();
        }
        else if (action == Action.SELECT)
        {
            action = Action.PLAY;
            List<Gamecore.Tile> t = g.game.getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().row,
                                                          selectedWorker_tile.GetComponent<Tile>().col,
                                                          Gamecore.MoveAction.Move);
            //Debug.Log(selectedWorker_tile.name + " can move to: ");
            //Debug.Log("t size: " + t.Count);
            List<GameObject> movableTiles = new List<GameObject>();
            foreach(Gamecore.Tile ti in t)
            {
                string name = ti.getRow() + ", " + ti.getCol();
                //Debug.Log(name);
                GameObject go = GameObject.Find(name);
                movableTiles.Add(go);
            }
            movableTiles.Add(selectedWorker_tile);
            toggleSelectedTiles(movableTiles);
        }
        else if (action == Action.PLAY)
        {
            deselectAll();
            //Debug.Log("Selected Tile: " + selectedWorker_tile.GetComponent<Tile>().row + ", " +
            //                                              selectedWorker_tile.GetComponent<Tile>().col);
            //Debug.Log(g.game.getGameboard()[1, 0].getWorker());
            List<Gamecore.Tile> t = g.game.getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().row,
                                                          selectedWorker_tile.GetComponent<Tile>().col,
                                                          Gamecore.MoveAction.Build);
            List<GameObject> buildableTiles = new List<GameObject>();
            //Debug.Log("Buildable spaces: ");
            foreach (Gamecore.Tile ti in t)
            {
                string name = ti.getRow() + ", " + ti.getCol();
                //Debug.Log(name);
                
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
            if(g.playerGoesFirst)
            {
                placeAIWorkers();
            }
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
                if (t.worker != enemy_1 && t.worker != enemy_2)
                {
                    t.selectable = true;
                }
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
