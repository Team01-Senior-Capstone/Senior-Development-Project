using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.Networking;
using System.Net.NetworkInformation;

public class GameManager : MonoBehaviour
{
    public float DELAY = .75f;

    public GameObject board, selectedWorker, selectedWorker_tile, opp_marker, enemy_1, enemy_2, worker_1, worker_2, disconnected, _meDisconnected, explosion;

    public OpponentManager oppMan;

    public Game game;
    
    Action action;
    public Slider volumeSlider;
    public string worker1_tag, worker2_tag;
    public TMP_Text tm;
    public Button mainMenu;
    public Button undo, help, settings;

    public GameObject[] characters;
    public string[] tags = { "Mario", "Luigi", "Peach", "Goomba", "Yoshi", "Bowser Jr."};

    public bool waiting = true;
    public bool gameOver = false;

    Gamecore.Player me, opponent;
    Gamecore.Worker gameCoreWorker1, gameCoreWorker2;

    public ref Gamecore.Player getMe() { return ref me; }
    public ref Gamecore.Player getOpponenet() { return ref opponent; }
    public Gamecore.Worker getGameCoreWorker1() { return gameCoreWorker1; }
    public Gamecore.Worker getGameCoreWorker2() { return gameCoreWorker2; }

    public Gamecore.Worker opponentWorker1, opponentWorker2;

    List<GameObject> allTiles;

    public GameObject getWorker1()
    {
        return worker_1;
    }

    public GameObject getWorker2()
    {
        return worker_2;
    }

    public void _update()
    {
        if (game.netWorkGame) {
            Thread t = new Thread(new ThreadStart(testCon));
            t.Start();
            if (!_connected)
            {
                meDisconnected();
            }
        }
    }
    bool _connected = true;
    public void testCon()
    {
        _connected = testConnection();
    }

    bool testConnection()
    {
        try
        {
            System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();
            String host = "google.com";
            byte[] buffer = new byte[32];
            int timeout = 1000;
            PingOptions pingOptions = new PingOptions();
            PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
            return (reply.Status == IPStatus.Success);
        }
        catch (Exception)
        {
            return false;
        }
    }
    public void playerDisconnected()
    {
        int len = GameObject.FindGameObjectsWithTag("Overlay").Length;
        Debug.Log(len);
        if (len != 0) return;
        GameObject go = Instantiate(disconnected, new Vector3(0, 100, -100), Quaternion.identity);
        go.GetComponentInChildren<Button>().GetComponent<Button>().onClick.AddListener(delegate { returnToMain(); });
        go.name = "OppDisconnect";
        //go.transform.localScale = new Vector3(1, 1, 1);
        //Quaternion q = new Quaternion(0, 0, 0, 0);
        //go.transform.rotation = q;

        GameObject canvas = GameObject.Find("Canvas");
        go.transform.SetParent(canvas.transform, false);
        
    }

    public void meDisconnected()
    {
        if (GameObject.FindGameObjectsWithTag("Overlay").Length != 0) return;
        GameObject go = Instantiate(_meDisconnected, new Vector3(0, 100, -100), Quaternion.identity);
        go.GetComponentInChildren<Button>().GetComponent<Button>().onClick.AddListener(delegate { returnToMain(); });
        go.name = "MeDisconnect";
        //go.transform.localScale = new Vector3(1, 1, 1);
        //Quaternion q = new Quaternion(0, 0, 0, 0);
        //go.transform.rotation = q;

        GameObject canvas = GameObject.Find("Canvas");
        go.transform.SetParent(canvas.transform, false);
    }

    public void playerReconnected()
    {
        GameObject go = GameObject.Find("OppDisconnect");
        if(go != null)
        {
            Destroy(go);
        }
    }


    public void gameCorePlaceWorker(int row, int col, int workerNum)
    {
        if (workerNum == 1) {
            game.getGameController().placePiece(gameCoreWorker1, row, col);
        } else {
            game.getGameController().placePiece(gameCoreWorker2, row, col);
        }
    }
    public void Awake()
    {
        initializeGameObjects();
    }
    public void Start()
    {
        if(oppMan.getOpp().GetType() == typeof(AI_Best))
        {
            DELAY = 0;
        }
        InvokeRepeating("_update", 0f, 1f);
        StartCoroutine(startUpGame());
    }
    IEnumerator startUpGame()
    {

        workerStartUp();
        yield return new WaitUntil(() => gotTags);

        setWorkerAsset();
        assignPlayers();
        setWorkersInGameCore();

        if (game.netWorkGame)
        {
            undo.gameObject.SetActive(false);
        }
        else
        {
            undo.gameObject.SetActive(true);
            updateUndo();
        }

        if (game.goesFirst())
        {
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

                Gamecore.Player[] players = game.getGameController().assignPlayers(Gamecore.Identification.Human, Gamecore.Identification.AI);
                me = players[0];
                opponent = players[1];
            } else {

                Gamecore.Player[] players = game.getGameController().assignPlayers(Gamecore.Identification.AI, Gamecore.Identification.Human);
                opponent = players[0];
                me = players[1];
            }
        } else {

            if (game.host) {

                Gamecore.Player[] players = game.getGameController().assignPlayers(Gamecore.Identification.Host, Gamecore.Identification.Client);
                me = players[0];
                opponent = players[1];
            } else {

                Gamecore.Player[] players = game.getGameController().assignPlayers(Gamecore.Identification.Host, Gamecore.Identification.Client);
                opponent = players[0];
                me = players[1];
            }
        }
    }

    public void poof(Vector3 pos)
    {
        GameObject go = Instantiate(explosion, pos, Quaternion.identity);
        StartCoroutine(waitDestroy(go, 2));
    }
    IEnumerator waitDestroy(GameObject go, float wait)
    {
        yield return new WaitForSeconds(wait);
        Destroy(go);
    }

    void workerStartUp () {

        if (!game.netWorkGame) {
            assignRandAIWorkers();

            if (!game.playerGoesFirst) {
                StartCoroutine(placeOpponentWorkers());
            }
        } else {

            StartCoroutine(assignOpponentWorkers());
            
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
            if (child.gameObject.GetComponent<Tile>().getWorker() == null)
            {
                unoccupied.Add(child.gameObject);
            }
        }
        toggleSelectedTiles(unoccupied, false);
    }

    public void adjustVolume()
    {

        AudioManager.changeVolume(volumeSlider.value);
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
        Debug.Log("Made it to placing opponenent workers");
        if (game.netWorkGame)
        {
            yield return new WaitUntil(gotPlacement);
        }

        Tuple<Move, Move> moves = oppMan.getOpp().GetWorkerPlacements(game.getGameController());

        if (!game.netWorkGame)
        {
            yield return new WaitUntil(gotPlacement);
        }
        Debug.Log("Made it past got Placement");
        yield return new WaitUntil(() => gotTags);
        //Tuple<Move, Move> moves = oppMan.getOpp().GetWorkerPlacements(game.getGameController());

        game.getGameController().placePiece(opponentWorker1, moves.Item1.toTile.getRow(), moves.Item1.toTile.getCol());
        game.getGameController().placePiece(opponentWorker2, moves.Item2.toTile.getRow(), moves.Item2.toTile.getCol());

        foreach (Transform child in board.transform)
        {
            string tileName = moves.Item1.toTile.getRow() + ", " + moves.Item1.toTile.getCol();
            string tile2Name = moves.Item2.toTile.getRow() + ", " + moves.Item2.toTile.getCol();
            GameObject marker;
            if (child.gameObject.name == tileName)
            {
                poof(child.GetComponent<Tile>().getCharacterSpawn());
                enemy_1 = Instantiate(oppMan.getOpp().getWorker1(), child.GetComponent<Tile>().getCharacterSpawn(), Quaternion.Euler(new Vector3(0, 180, 0)));
                Vector3 place = enemy_1.transform.position;
                place.y += 2;
                marker = Instantiate(opp_marker, place, Quaternion.Euler(new Vector3(180, 180, 180)));
                marker.transform.SetParent(enemy_1.transform);
                child.GetComponent<Tile>().setWorker(enemy_1);
            }
            else if(child.gameObject.name == tile2Name)
            {
                poof(child.GetComponent<Tile>().getCharacterSpawn());
                enemy_2 = Instantiate(oppMan.getOpp().getWorker2(), child.GetComponent<Tile>().getCharacterSpawn(), Quaternion.Euler(new Vector3(0, 180, 0)));
                Vector3 place = enemy_2.transform.position;
                place.y += 2;
                marker = Instantiate(opp_marker, place, Quaternion.Euler(new Vector3(180, 180, 180)));
                marker.transform.SetParent(enemy_2.transform);
                child.GetComponent<Tile>().setWorker(enemy_2);
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
    bool gotTags = false;
    IEnumerator assignOpponentWorkers()
    {
        Tuple<string, string> tags = oppMan.getOpp().GetWorkerTags();
        while (tags == null || tags.Item1 == null || tags.Item2 == null)
        {
            yield return null;
            tags = oppMan.getOpp().GetWorkerTags();
        }
        
        Debug.Log("Tag 1: " + tags.Item1 + ", Tag 2: " + tags.Item2);
        oppMan.getOpp().setWorker1(translateTag(tags.Item1));
        oppMan.getOpp().setWorker2(translateTag(tags.Item2));
        gotTags = true;
    }

    //Maybe change later
    void assignRandAIWorkers()
    {
        System.Random rnd = new System.Random();
        int num = rnd.Next(characters.Length);

        oppMan.getOpp().setWorker1(characters[num]);
        num = rnd.Next(characters.Length );
        oppMan.getOpp().setWorker2(characters[num]);
        gotTags = true;
    }

    public bool gotMove()
    {
        return oppMan.getOpp().HasMove();
    }
    Tuple<Move, Move> oppMoves;

    void getMovesInOtherThread()
    {
        oppMoves = oppMan.getOpp().GetMove(game.getGameController());

    }
    //Updates the gui and gameboad. Has pauses in between AI moves
    public IEnumerator updateGUI(float delay) 
    {
        waiting = true;
        if (game.netWorkGame)
        {
            yield return new WaitUntil(gotMove);
        }
        undo.interactable = false;
        // =  oppMan.getOpp().GetMove(game.getGameController());
        //StartCoroutine(getMovesInOtherThread());
        Thread thread1 = new Thread(getMovesInOtherThread);
        thread1.Start();

        yield return new WaitForSeconds(.75f);

        if (!game.netWorkGame)
        {
            yield return new WaitUntil(gotMove);
            yield return new WaitUntil(() => oppMoves != null);
        }
        //yield return new WaitUntil(gotMove);
        //undo.interactable = false;
        //Tuple<Move, Move> moves = oppMan.getOpp().GetMove(game.getGameController());
        Gamecore.Worker work;
        if (oppMoves.Item1.worker.workerOne)
        {
            work = opponentWorker1;
            
        }
        else
        {
            work = opponentWorker2;
        }

        //Debug.Log("Recieved from tile: " + moves.Item1.fromTile.getRow() + ", " + moves.Item1.fromTile.getCol());
        //Debug.Log("Recieved to tile: " + moves.Item1.toTile.getRow() + ", " + moves.Item1.toTile.getCol());
        Gamecore.WorkerMoveInfo wi = game.getGameController().movePlayer(work, opponent, oppMoves.Item1.fromTile.getRow(), oppMoves.Item1.fromTile.getCol(),
                              oppMoves.Item1.toTile.getRow(), oppMoves.Item1.toTile.getCol());

        
        if (!wi.wasMoveSuccessful())
        {
            Debug.Log(work.isCorrectOwner(opponent));
            Debug.Log("DEATH!!!!! DEATH TO THE AI!!!!!");
        }

        GameObject toTile = null;
        GameObject fromTile = null;
        foreach (Transform child in board.transform) {
            string tileName = oppMoves.Item1.toTile.getRow() + ", " + oppMoves.Item1.toTile.getCol();
            string fromTileName = oppMoves.Item1.fromTile.getRow() + ", " + oppMoves.Item1.fromTile.getCol();
            if (child.gameObject.name == tileName)
            {
                toTile = child.gameObject;
                if (oppMoves.Item1.worker == opponentWorker1)
                {
                    toTile.GetComponent<Tile>().setWorker(enemy_1);
                }
                else
                {
                    toTile.GetComponent<Tile>().setWorker(enemy_2);
                }
            }
            else if(child.gameObject.name == fromTileName)
            {
                fromTile = child.gameObject;
                child.GetComponent<Tile>().setWorker(null);
            }

        }
        if(oppMoves.Item1.worker == opponentWorker1)
        {

            toTile.GetComponent<Tile>().moveToTile(enemy_1, fromTile.GetComponent<Tile>());
        }
        else
        {

            toTile.GetComponent<Tile>().moveToTile(enemy_2, fromTile.GetComponent<Tile>());
        }


        if (game.getGameController().checkForWin().getGameHasWinner())
        {
            endGame(false);
            yield break;
        }
        yield return new WaitForSeconds(delay);

        Gamecore.TileBuildInfo f = game.getGameController().workerBuild(work, opponent, oppMoves.Item2.fromTile.getRow(), oppMoves.Item2.fromTile.getCol(),
                           oppMoves.Item2.toTile.getRow(), oppMoves.Item2.toTile.getCol());

        if(!f.wasBuildSuccessful())
        {
            Debug.Log("BURN!!!! BURN THE WITCH!!!!");
        }

        foreach (Transform child in board.transform)
        {
            string tileName = oppMoves.Item2.toTile.getRow() + ", " + oppMoves.Item2.toTile.getCol();
            if (child.gameObject.name == tileName)
            {
                child.GetComponent<Tile>().buildOnTile();
            }
        }
        if (!hasMoreMoves(me, Gamecore.MoveAction.Move))
        {
            endGame(false);
            yield break;
        }
        oppMoves = null;
        waiting = false;
        action = Action.SELECT;
        toggleWorkerTiles();
        undo.interactable = true;
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

    public void undoGUIMove(Gamecore.StateInfo move)
    {
        if (move is Gamecore.WorkerMoveInfo)
        {
            Gamecore.WorkerMoveInfo workerMove = (Gamecore.WorkerMoveInfo)move;
            //Debug.Log(workerMove.getWorker());
            //Debug.Log("From tile: ");
            //printTile(workerMove.getTileMovedFrom());
            //Debug.Log("To tile: ");
            //printTile(workerMove.getTileMovedTo());
            undoWorkerMove(workerMove);
        }
        else {
            Gamecore.TileBuildInfo ti = (Gamecore.TileBuildInfo)move;
            undoPipeBuild(ti);
        }
    }

    public Tile translateTile(Gamecore.Tile ti)
    {
        return GameObject.Find(ti.getRow() + ", " + ti.getCol()).GetComponent<Tile>();
    }

    public void printTile(Gamecore.Tile ti)
    {
        Debug.Log(ti.getRow() + ", " + ti.getCol());
    }

    public void undoPipeBuild(Gamecore.TileBuildInfo ti)
    {

        GameObject movedWorker;
        Gamecore.Tile t = game.getGameController().getGameboard()[ti.getBuildRow(), ti.getBuildCol()];
        Debug.Log(t);
        Tile fromTile = translateTile(t);
        if (ti.getPlayer().getTypeOfPlayer() == Gamecore.Identification.AI)
        {
            if (ti.getBuiltFrom().getWorker().workerOne)
            {
                movedWorker = enemy_1;
            }
            else
            {
                movedWorker = enemy_2;
            }
        }
        else
        {
            if (ti.getBuiltFrom().getWorker().workerOne)
            {
                movedWorker = worker_1;
            }
            else
            {
                movedWorker = worker_2;
            }
        }
        selectedWorker_tile = translateTile(ti.getBuiltFrom()).gameObject;
        selectedWorker = movedWorker;

        fromTile.undoPipeBuild();
    }
    public void undoWorkerMove(Gamecore.WorkerMoveInfo workerMove)
    {
        GameObject movedWorker;
        
        if (workerMove.getPlayer().getTypeOfPlayer() == Gamecore.Identification.AI)
        {
            if (workerMove.getWorker().workerOne)
            {
                movedWorker = enemy_1;
            }
            else
            {
                movedWorker = enemy_2;
            }
        }
        else
        {
            if (workerMove.getWorker().workerOne)
            {
                movedWorker = worker_1;
            }
            else
            {
                movedWorker = worker_2;
            }
        }

        Tile ti = translateTile(workerMove.getTileMovedFrom());
        ti.setWorker(movedWorker);
        Tile to = translateTile(workerMove.getTileMovedTo());
        to.setWorker(null);
        selectedWorker_tile = ti.gameObject;
        selectedWorker = movedWorker;
        movedWorker.transform.position = ti.getCharacterSpawn();
    }

    void undoMove()
    {
        Gamecore.StateInfo move = game.getGameController().getLastMove();
        undoGUIMove(move);
        game.getGameController().undoMove();
    }

    public void goBackAction()
    {
        undo.interactable = false;
        if(waiting)
        {
            return;
        }
        if(action == Action.BUILD)
        {
            Debug.Log("Build");

            undoMove();
            actionSelect();
            //action = Action.SELECT;
        }
        else if(action == Action.SELECT || action == Action.PLAY)
        {
            //Debug.Log("Select");
            //This includes AI moves
            Debug.Log(game.netWorkGame);
            Gamecore.StateInfo enemyBuild = game.getGameController().getLastMove();
            undoGUIMove(enemyBuild);
            game.getGameController().undoMove();


            Gamecore.StateInfo enemyMove = game.getGameController().getLastMove();
            undoGUIMove(enemyMove);
            game.getGameController().undoMove();
            
            undoMove();
            actionPlay();
            selectedWorker_tile.GetComponent<Tile>().removeSelect();
            //action = Action.PLAY;
            Debug.Log(action);
        }
        else if(action == Action.PLAY)
        {
            Debug.Log("Play");
            undoMove();

            action = Action.SELECT;
        }
        else if(action == Action.SECOND_MOVE)
        {
            Debug.Log("Second Move");
            undoMove();
            actionFirstMove();
            action = Action.FIRST_MOVE;
        }
        updateUndo();
    }


    private void updateUndo()
    {
        undo.interactable = game.getGameController().canUndo();     
    }

    void actionBuild () {
        waiting = true;
        deselectAll();
        if (!hasMoreMoves(me, Gamecore.MoveAction.Move))
        {
            endGame(false);
            return;
        }
        oppMan.getOpp().SendMoves(new Tuple<Move, Move>(move1, move2));

        if(!hasMoreMoves(opponent, Gamecore.MoveAction.Move))
        {
            endGame(true);
            return;
        }
        updateUndo();
        StartCoroutine(updateGUI(DELAY));
    }

    public void actionSelect () {

        
        action = Action.PLAY;
        List<Gamecore.Tile> t = game.getGameController().getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().getRow(),
                                                        selectedWorker_tile.GetComponent<Tile>().getCol(),
                                                        Gamecore.MoveAction.Move);

        List<GameObject> movableTiles = new List<GameObject>();
        foreach(Gamecore.Tile ti in t)
        {
            string name = ti.getRow() + ", " + ti.getCol();
            GameObject go = GameObject.Find(name);
            
            movableTiles.Add(go);
        }

        toggleSelectedTiles(movableTiles, true);
        movableTiles.Clear();

        foreach (Gamecore.Tile ti in game.getGameController().getOccupiedTiles())
        {
            if (ti.getWorker().isCorrectOwner(me))
            {
                string name = ti.getRow() + ", " + ti.getCol();
                GameObject go = GameObject.Find(name);

                movableTiles.Add(go);
            }
        }

        addToggledTiles(movableTiles, false);

        //If its one of our workers, keep the select
        if (selectedWorker == worker_1 || selectedWorker == worker_2)
        {
            selectedWorker_tile.GetComponent<Tile>().keepSelect();
        }
    }

    void actionPlay (){

        if(game.getGameController().checkForWin().getGameHasWinner()) {

            oppMan.getOpp().SendMoves(new Tuple<Move, Move>(move1, move2));
            endGame(true);
            return;
        }
        deselectAll();
        List<Gamecore.Tile> t = game.getGameController().getValidSpacesForAction(selectedWorker_tile.GetComponent<Tile>().getRow(),
                                                        selectedWorker_tile.GetComponent<Tile>().getCol(),
                                                        Gamecore.MoveAction.Build);
        List<GameObject> buildableTiles = new List<GameObject>();
        foreach (Gamecore.Tile ti in t)
        {
            string name = ti.getRow() + ", " + ti.getCol();
            
            GameObject go = GameObject.Find(name);
            buildableTiles.Add(go);
        }

        toggleSelectedTiles(buildableTiles, true);
        updateUndo();
        action = Action.BUILD;
    }

    void actionFirstMove () {

        deselectAll();
        List<GameObject> unoccupied = new List<GameObject>();

        foreach (Transform child in board.transform) {
            if (child.GetComponent<Tile>().getWorker() == null) {
                unoccupied.Add(child.gameObject);
            }
        }
        toggleSelectedTiles(unoccupied, false);
        action = Action.SECOND_MOVE;
    }

    void actionSecondMove () {
        //game.playerGoesFirst || 
        if (game.goesFirst()) {
            deselectAll();
            Debug.Log("Placed second Worker");
            StartCoroutine(placeOpponentWorkers());
        } else {
            deselectAll();
            StartCoroutine(updateGUI(DELAY));
        }

        oppMan.getOpp().SendWorkerPlacements(new Tuple<Move, Move>(move1, move2));
    }

    void endGame(bool won)
    {
        deselectAll();
        gameOver = true;
        if (won) {
            tm.text = "You won!";
            AudioManager.playWinSound();
            Animator anim = worker_1.GetComponent<Animator>();
            anim.Play("Win");

            anim = worker_2.GetComponent<Animator>();

            anim.Play("Win");
        } else {
            tm.text = "You lost!";
            AudioManager.playLoseSound();
        }

        //Destroy(GameObject.FindGameObjectWithTag ("HelpButton").GetComponent<Button>().image);
        undo.interactable = false;
        help.interactable = false;
        settings.interactable = false;

        tm.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(true);

        //game.reset();
        
    }
    public void otherPlayerQuit()
    {
        if (gameOver) return;
        deselectAll();
        gameOver = true;
        tm.text = "Other player quit!";
        undo.interactable = false;
        help.interactable = false;
        settings.interactable = false;

        tm.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(true);
    }

    public void cleanup()
    {
        //oppMan.disconnect();
        GameObject goGame = GameObject.Find("Game");
        if (goGame != null)
        {
            Destroy(goGame);
        }
        //oppMan.reset();
        GameObject opp = GameObject.Find("Opponent");
        if (opp != null)
        {
            Destroy(opp);
        }
    }

    public void returnToMain()
    {
        GameObject audio = GameObject.Find("AudioManager");
        Destroy(audio);
        GameObject server = GameObject.Find("Server");
        Destroy(server);
        cleanup();
        SceneManager.LoadScene("Main Menu");
    }

    void toggleSelectedTiles(List<GameObject> tiles, bool highlight)
    {
        deselectAll();
        foreach (GameObject tile in tiles) {
            Tile s = tile.GetComponent<Tile>();
            s.setSelectable(true);
            if(highlight) s.highlight();
        }
    }

    void addToggledTiles(List<GameObject> tiles, bool highlight)
    {
        foreach (GameObject tile in tiles)
        {
            Tile s = tile.GetComponent<Tile>();
            s.setSelectable(true);
            if (highlight) s.highlight();
        }
    }

    public void deselectAll()
    {

        foreach (Transform tile in board.transform)
        {
            Tile s = tile.gameObject.GetComponent<Tile>();
            s.setSelectable(false);
            s.unhighlight();
        }
    } 

    public List<Boolean> disableBoard () {

        List<Boolean> activeTiles = new List<bool>();

        foreach (Transform tile in board.transform) {
            
            Tile s = tile.gameObject.GetComponent<Tile>();
            activeTiles.Add(s.getSelectable());
            s.setSelectable(false);
        }

        return activeTiles;
    }

    public void enableBoard (List<Boolean> activeTiles) {

        int index = 0;

        foreach (Transform tile in board.transform) {

            tile.gameObject.GetComponent<Tile>().setSelectable(activeTiles[index]);
            index++;
        }
    }

    public void toggleWorkerTiles()
    {
        foreach (Transform go in board.transform) {
            
            Tile t = go.GetComponent<Tile>();

            if (t.getWorker() != null) {
                if (t.getWorker() != enemy_1 && t.getWorker() != enemy_2) {
                    t.setSelectable(true);
                }
            } else {
                t.setSelectable(false);
                t.unhighlight();
            }
        }
    }

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
        foreach (Gamecore.Tile ti in game.getGameController().getOccupiedTiles())
        {

            if(ti.getWorker().isCorrectOwner(p))
            {
                myWorkers.Add(ti);
            }
        }
        int moves = 0;
        foreach(Gamecore.Tile ti in myWorkers)
        {
            List<Gamecore.Tile> tiles = game.getGameController().getValidSpacesForAction(ti.getRow(), ti.getCol(), a);
            moves += tiles.Count;
        }
        return moves > 0;
    }

    public Move move1 { set; get; }
    public Move move2 { set; get; }

}
