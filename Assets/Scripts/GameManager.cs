using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float delay = .75f;


    public GameObject board;
    public OpponentManager oppMan;

    public GameObject opp_marker;

    public Game game;

    GameObject worker_1;
    GameObject worker_2;
    //The actual instantiated prefab
    public GameObject enemy_1;
    public GameObject enemy_2;

    public GameObject selectedWorker;

    public GameObject selectedWorker_tile;
    Action action;

    public TMP_Text tm;
    public Button mainMenu;

    public GameObject[] characters;
    public string[] tags = { "Mario", "Luigi", "Peach", "Goomba", "Yoshi", "BowserJr"};


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
        if (workerNum == 1) {
            game.gameController.placePiece(gameCoreWorker1, row, col);
        } else {
            game.gameController.placePiece(gameCoreWorker2, row, col);
        }
    }

    public void Start()
    {
        initializeGameObjects();
        workerStartUp();
        setWorkerAsset();
        assignPlayers();
        setWorkersInGameCore();

        if (game.goesFirst()) {
            startPlay();
        }
    }

    void initializeGameObjects () {

        game = GameObject.Find("Game").GetComponent<Game>();
        oppMan = GameObject.Find("Opponent").GetComponent<OpponentManager>();

        tm.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
    }

    void assignPlayers () {

        if(game.netWorkGame == false) {

            if(game.playerGoesFirst) {

                Gamecore.Player[] players = game.gameController.assignPlayers(Gamecore.Identification.Human, Gamecore.Identification.AI);
                me = players[0];
                opponent = players[1];
            } else {

                Gamecore.Player[] players = game.gameController.assignPlayers(Gamecore.Identification.AI, Gamecore.Identification.Human);
                opponent = players[0];
                me = players[1];
            }
        } else {

            if (game.host) {

                Gamecore.Player[] players = game.gameController.assignPlayers(Gamecore.Identification.Host, Gamecore.Identification.Client);
                me = players[0];
                opponent = players[1];
            } else {

                Gamecore.Player[] players = game.gameController.assignPlayers(Gamecore.Identification.Host, Gamecore.Identification.Client);
                opponent = players[0];
                me = players[1];
            }
        }
    }

    void workerStartUp () {

        if (!oppMan.multiplayer) {
            assignRandAIWorkers();

            if (!game.playerGoesFirst) {
                StartCoroutine(placeOpponentWorkers());
            }
        } else {

            assignOpponentWorkers();

            if(!game.goesFirst()) {
                StartCoroutine(placeOpponentWorkers());
            }
        }
    }

    void setWorkerAsset () {

        worker_1 = translateTag(game.worker1_tag);
        worker_2 = translateTag(game.worker2_tag);
    }

    void setWorkersInGameCore() {

        gameCoreWorker1 = new Gamecore.Worker(me, true);
        gameCoreWorker2 = new Gamecore.Worker(me, false);
        opponentWorker1 = new Gamecore.Worker(opponent, true);
        opponentWorker2 = new Gamecore.Worker(opponent, false);
    }

    public void startPlay()
    {
        waiting = false;
        action = Action.FIRST_MOVE;
        List<GameObject> unoccupied = new List<GameObject>();
        foreach (Transform child in board.transform)
        {
            if (child.gameObject.GetComponent<Tile>().worker == null)
            {
                unoccupied.Add(child.gameObject);
            }
        }
        toggleSelectedTiles(unoccupied);
    }

    public void returnToSelect()
    {
        action = Action.SELECT;
        toggleWorkerTiles();
    }

    bool gotPlacement()
    {
        return oppMan.getOpp().HasMove();
    }

    public IEnumerator placeOpponentWorkers()
    {
    
        yield return new WaitUntil(gotPlacement);

        Tuple<Move, Move> moves = oppMan.getOpp().GetWorkerPlacements(game.gameController);

        game.gameController.placePiece(opponentWorker1, moves.Item1.toTile.getRow(), moves.Item1.toTile.getCol());
        game.gameController.placePiece(opponentWorker2, moves.Item2.toTile.getRow(), moves.Item2.toTile.getCol());

        foreach (Transform child in board.transform)
        {
            string tileName = moves.Item1.toTile.getRow() + ", " + moves.Item1.toTile.getCol();
            string tile2Name = moves.Item2.toTile.getRow() + ", " + moves.Item2.toTile.getCol();
            GameObject marker;
            if (child.gameObject.name == tileName)
            {
                enemy_1 = Instantiate(oppMan.getOpp().getWorker1(), child.GetComponent<Tile>().getCharacterSpawn(), Quaternion.Euler(new Vector3(0, 180, 0)));
                Vector3 place = enemy_1.transform.position;
                place.y += 2;
                marker = Instantiate(opp_marker, place, Quaternion.Euler(new Vector3(180, 180, 180)));
                marker.transform.SetParent(enemy_1.transform);
                child.GetComponent<Tile>().worker = enemy_1;
            }
            else if(child.gameObject.name == tile2Name)
            {
                enemy_2 = Instantiate(oppMan.getOpp().getWorker2(), child.GetComponent<Tile>().getCharacterSpawn(), Quaternion.Euler(new Vector3(0, 180, 0)));
                Vector3 place = enemy_2.transform.position;
                place.y += 2;
                marker = Instantiate(opp_marker, place, Quaternion.Euler(new Vector3(180, 180, 180)));
                marker.transform.SetParent(enemy_2.transform);
                child.GetComponent<Tile>().worker = enemy_2;
            }
        }

        spinUpGame();
    }

    void spinUpGame () {

        if(!game.goesFirst()) {
            startPlay();
        } else {
            action = Action.SELECT;
            toggleWorkerTiles();
        }
    }

    void assignOpponentWorkers()
    {
        Tuple<string, string> tags = oppMan.getOpp().GetWorkerTags();
        oppMan.getOpp().setWorker1(translateTag(tags.Item1));
        oppMan.getOpp().setWorker2(translateTag(tags.Item2));
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

    public bool gotMove()
    {
        return oppMan.getOpp().HasMove();
    }

    //Updates the gui and gameboad. Has pauses in between AI moves
    public IEnumerator updateGUI(float delay) 
    {
        yield return new WaitUntil(gotMove);

        Tuple<Move, Move> moves = oppMan.getOpp().GetMove(game.gameController);
        yield return new WaitForSeconds(delay);
        Gamecore.Worker work;
        if (moves.Item1.worker.workerOne)
        {
            work = opponentWorker1;
            
        }
        else
        {
            work = opponentWorker2;
        }
        game.gameController.movePlayer(work, opponent, moves.Item1.fromTile.getRow(), moves.Item1.fromTile.getCol(),
                              moves.Item1.toTile.getRow(), moves.Item1.toTile.getCol());

        if(game.gameController.checkForWin().getGameHasWinner())
        {
            endGame(false);
            yield break;
        }
        GameObject toTile = null;
        GameObject fromTile = null;
        foreach (Transform child in board.transform) {
            string tileName = moves.Item1.toTile.getRow() + ", " + moves.Item1.toTile.getCol();
            string fromTileName = moves.Item1.fromTile.getRow() + ", " + moves.Item1.fromTile.getCol();
            if (child.gameObject.name == tileName)
            {
                toTile = child.gameObject;
                if (moves.Item1.worker == opponentWorker1)
                {
                    toTile.GetComponent<Tile>().worker = enemy_1;
                }
                else
                {
                    toTile.GetComponent<Tile>().worker = enemy_2;
                }
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

        yield return new WaitForSeconds(delay);

        Gamecore.TileBuildInfo f = game.gameController.workerBuild(work, opponent, moves.Item2.fromTile.getRow(), moves.Item2.fromTile.getCol(),
                           moves.Item2.toTile.getRow(), moves.Item2.toTile.getCol());


        foreach (Transform child in board.transform)
        {
            string tileName = moves.Item2.toTile.getRow() + ", " + moves.Item2.toTile.getCol();
            if (child.gameObject.name == tileName)
            {
                child.GetComponent<Tile>().buildOnTile();
            }
        }
        waiting = false;
        action = Action.SELECT;
        toggleWorkerTiles();
    }

    public void toggleAction()
    {
        if (action == Action.BUILD) { 
            actionBuild();  
        }
        else if (action == Action.SELECT) {
            actionSelect();
        }
        else if (action == Action.PLAY) {
            actionPlay();   
        }
        else if (action == Action.FIRST_MOVE) {
            actionFirstMove();
        }
        else if (action == Action.SECOND_MOVE) {
            actionSecondMove();
        }
    }

    void actionBuild () {
        waiting = true;
        deselectAll();
        oppMan.getOpp().SendMoves(new Tuple<Move, Move>(move1, move2));

        if(!hasMoreMoves(opponent, Gamecore.MoveAction.Move))
        {
            endGame(true);
            return;
        }
        StartCoroutine(updateGUI(delay));
    }

    void actionSelect () {

        if(!hasMoreMoves(me, Gamecore.MoveAction.Move))
        {
            endGame(false);
            return;
        }
        action = Action.PLAY;
        List<Gamecore.Tile> t = game.gameController.getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().row,
                                                        selectedWorker_tile.GetComponent<Tile>().col,
                                                        Gamecore.MoveAction.Move);

        List<GameObject> movableTiles = new List<GameObject>();
        foreach(Gamecore.Tile ti in t)
        {
            string name = ti.getRow() + ", " + ti.getCol();
            GameObject go = GameObject.Find(name);
            
            movableTiles.Add(go);
        }
        movableTiles.Add(selectedWorker_tile);
        toggleSelectedTiles(movableTiles);
        selectedWorker_tile.GetComponent<Tile>().keepSelect();
    }

    void actionPlay (){

        if(game.gameController.checkForWin().getGameHasWinner()) {

            oppMan.getOpp().SendMoves(new Tuple<Move, Move>(move1, move2));
            endGame(true);
            return;
        }
        deselectAll();
        List<Gamecore.Tile> t = game.gameController.getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().row,
                                                        selectedWorker_tile.GetComponent<Tile>().col,
                                                        Gamecore.MoveAction.Build);
        List<GameObject> buildableTiles = new List<GameObject>();
        foreach (Gamecore.Tile ti in t)
        {
            string name = ti.getRow() + ", " + ti.getCol();
            
            GameObject go = GameObject.Find(name);
            buildableTiles.Add(go);
        }

        toggleSelectedTiles(buildableTiles);
        action = Action.BUILD;
    }

    void actionFirstMove () {

        deselectAll();
        List<GameObject> unoccupied = new List<GameObject>();

        foreach (Transform child in board.transform) {
            if (child.GetComponent<Tile>().worker == null) {
                unoccupied.Add(child.gameObject);
            }
        }
        toggleSelectedTiles(unoccupied);
        action = Action.SECOND_MOVE;
    }

    void actionSecondMove () {

        if (game.playerGoesFirst || game.goesFirst()) {
            deselectAll();
            StartCoroutine(placeOpponentWorkers());
        } else {
            deselectAll();
            StartCoroutine(updateGUI(delay));
        }

        oppMan.getOpp().SendWorkerPlacements(new Tuple<Move, Move>(move1, move2));
    }

    void endGame(bool won)
    {
        deselectAll();

        if(won) {
            tm.text = "You won!";
        } else {
            tm.text = "You lost!";
        }
        tm.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(true);

        game.reset();
    }

    public void returnToMain()
    {
        SceneManager.LoadScene("Main Menu");
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

        foreach (Transform tile in board.transform)
        {
            Tile s = tile.gameObject.GetComponent<Tile>();
            s.selectable = false;
        }
    } 

    public void toggleWorkerTiles()
    {
        foreach (Transform go in board.transform)
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

    bool waiting = true;
    public bool getWaiting() 
    {
        return waiting;
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

    public bool hasMoreMoves(Gamecore.Player p, Gamecore.MoveAction a)
    {
        List<Gamecore.Tile> myWorkers = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile ti in game.gameController.getOccupiedTiles())
        {
            if(ti.getWorker().isCorrectOwner(p))
            {
                myWorkers.Add(ti);
            }
        }
        int moves = 0;
        foreach(Gamecore.Tile ti in myWorkers)
        {
            List<Gamecore.Tile> tiles = game.gameController.getValidSpacesForAction(ti.getRow(), ti.getCol(), a);
            moves += tiles.Count;
        }
        return moves > 0;
    }


    public Move move1 { set; get; }
    public Move move2 { set; get; }

}
