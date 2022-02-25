using System;
using System.Diagnostics;
using System.Collections.Generic;
using Gamecore;
using System.Linq;

public struct ScoredMove
{
    public Tuple<Move, Move> move;
    //Move move;
    //Move build;
    public float score; 
};

//using Turn = Tuple<Move,Move>;

//requires integration with Gamecore classes
//public class AI_Rand_Base : Opponent
public class AI_Simple : Opponent
{
    private Gamecore.Tile[,] initBoard;

    const float MAX_SCORE = 100.0f;
    const float MIN_SCORE = -100.0f;

    const float WORKER_HEIGHT = 10f;
    const float MOVES = 1f;
    const float PIPE_ON_SAME_LEVEL = 10f;

    //Useless functions
    public override void SendMoves(Tuple<Move, Move> m) { }
    public override void SendWorkerPlacements(Tuple<Move, Move> m) { }
    public override void SendWorkerTags(string s1, string s2) { }
    public override void SendReady(bool r) { }
    public override Tuple<string, string> GetWorkerTags() { return new Tuple<string, string>("", ""); }
    public override bool GetReady() { return true; }
    public override bool HasMove()
    {
        return true;
    }


    //returns tuple of Move objects with fromTiles set to null and toTiles set to the tiles to place workers on
    //  action is currently set to Action.Move, this may need changed
    public override Tuple<Move, Move> GetWorkerPlacements(GameController gc)
    {
        initBoard = gc.getGameboard();
        var rand = new Random();

        int randRow1 = rand.Next(1,4);
        int randCol1 = rand.Next(1,4);
        int randRow2 = rand.Next(1,4);
        int randCol2 = rand.Next(1,4);

        //choose indecies for different empty tiles in center
        while (initBoard[randRow1, randCol1].getWorker() != null || initBoard[randRow2, randCol2].getWorker() != null
                || ((randRow1 == randRow2) && (randCol1 == randCol2)))
        {
            randRow1 = rand.Next(1,4);
            randCol1 = rand.Next(1,4);
            randRow2 = rand.Next(1,4);
            randCol2 = rand.Next(1,4);
        }

        Gamecore.Tile tile1 = initBoard[randRow1, randCol1];
        Gamecore.Tile tile2 = initBoard[randRow2, randCol2];

        Move AIPlace1 = new Move(null, tile1, Gamecore.MoveAction.Move, null);

        Move AIPlace2 = new Move(null, tile2, Gamecore.MoveAction.Move, null);

        return new Tuple<Move, Move>(AIPlace1, AIPlace2);
    }

    //  currently returns Tuple of Move object, one of Action.Move, one of Action.Build, with to and from tiles
    //  can restructure if need be
    public override Tuple<Move, Move> GetMove(GameController gc)
    {
        Tuple<Move, Move> bestMove;

        //get gameboard and init rand
        initBoard = gc.getGameboard();
        var rand = new Random();
        //clear undo/redo stack here?

        //possibleTurns contains what it sounds like
        //Logic narrowing down to moves that take us higher? Let us build on the same level? Block losing senarios?
        //OR start implementing algorithm?

        //PICKS RANDOM MOVE
        //List<Tuple<Move, Move>> possibleTurns = getAllPossibleMoves(gc,Identification.AI);
        //int moveIndex = rand.Next(possibleTurns.Count);
        //bestMove = possibleTurns[moveIndex];

        //MINIMAX + HEURISTIC
        float bestScore = 0.0f;//don't need???
        //bestMove = minimax(gc, Identification.AI, 2, 0, ref bestScore).move;

        //UnityEngine.Debug.Log("Best move from tile: " + bestMove.Item1.fromTile.getRow() + " , " + bestMove.Item1.fromTile.getCol());
        //UnityEngine.Debug.Log("Best move to tile: " + bestMove.Item1.toTile.getRow() + " , " + bestMove.Item1.toTile.getCol());
        //UnityEngine.Debug.Log("Best build from tile: " + bestMove.Item2.fromTile.getRow() + " , " + bestMove.Item2.fromTile.getCol());
        //UnityEngine.Debug.Log("Best build to tile: " + bestMove.Item2.toTile.getRow() + " , " + bestMove.Item2.toTile.getCol());
        //UnityEngine.Debug.Log(bestMove.Item1.fromTile.getWorker());
        //UnityEngine.Debug.Log(bestMove.Item1.toTile.getWorker());
        //UnityEngine.Debug.Log(bestMove.Item2.toTile.getWorker());

        return minimax(gc, Identification.AI, 1, 0, ref bestScore).move;
    }

    //helper function for getAllPossibleMoves?
    private void addWorkerMoves(GameController gc, Gamecore.Tile workerTile, Gamecore.Tile moveTile, ref List<Tuple<Move, Move>> possibleTurns)
    {
        Gamecore.GameController tempGC = gc.Clone();
        //UnityEngine.Debug.Log("Take 1: " + tempGC.getOccupiedTiles()[0].getRow() + " , " + tempGC.getOccupiedTiles()[0].getCol());
        //UnityEngine.Debug.Log(tempGC.getOccupiedTiles()[1].getRow() + " , " + tempGC.getOccupiedTiles()[1].getCol());
        //UnityEngine.Debug.Log(tempGC.getOccupiedTiles()[2].getRow() + " , " + tempGC.getOccupiedTiles()[2].getCol());
        //UnityEngine.Debug.Log(tempGC.getOccupiedTiles()[3].getRow() + " , " + tempGC.getOccupiedTiles()[3].getCol());

        //"move" worker so temp GameController correctly generates valid build spaces
        tempGC.movePlayer(workerTile.getWorker(), workerTile.getWorker().getOwner(),
                  workerTile.getRow(), workerTile.getCol(), moveTile.getRow(), moveTile.getCol());

        //UnityEngine.Debug.Log("Take 2: " + tempGC.getOccupiedTiles()[0].getRow() + " , " + tempGC.getOccupiedTiles()[0].getCol());
        //UnityEngine.Debug.Log(tempGC.getOccupiedTiles()[1].getRow() + " , " + tempGC.getOccupiedTiles()[1].getCol());
        //UnityEngine.Debug.Log(tempGC.getOccupiedTiles()[2].getRow() + " , " + tempGC.getOccupiedTiles()[2].getCol());
        //UnityEngine.Debug.Log(tempGC.getOccupiedTiles()[3].getRow() + " , " + tempGC.getOccupiedTiles()[3].getCol());

        List<Gamecore.Tile> validBuildTiles = tempGC.getValidSpacesForAction(moveTile.getRow(), moveTile.getCol(), Gamecore.MoveAction.Build);

        //for every valid tile to build on from Tile t
        foreach (Gamecore.Tile b in validBuildTiles)
        {
            Worker chosenWorker = workerTile.getWorker();
            Move AIMove = new Move(workerTile, moveTile, Gamecore.MoveAction.Move, chosenWorker);
            Move AIBuild = new Move(moveTile, b, Gamecore.MoveAction.Build, chosenWorker);

            Tuple<Move, Move> turn = new Tuple<Move, Move>(AIMove, AIBuild);

            possibleTurns.Add(turn);

            //UnityEngine.Debug.Log(chosenWorker);
        }
    }

    private List<Tuple<Move, Move>> getAllPossibleMoves(GameController gc, Identification playerId)
    {
        List<Tuple<Move, Move>> possibleTurns = new List<Tuple<Move, Move>>();
        
        //get worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> AITiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == playerId)
            {
                AITiles.Add(t);
                //Debug.WriteLine(t.getWorker().getOwner().getTypeOfPlayer());
            }
        }

        //GameController tempGC = new GameController(false);
        //int test = 0;

        //for worker 1
        List<Gamecore.Tile> validMoveTiles1 = gc.getValidSpacesForAction(AITiles[0].getRow(), AITiles[0].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles1)
        {
            addWorkerMoves(gc, AITiles[0], t, ref possibleTurns);
        }

        //UnityEngine.Debug.Log(possibleTurns.Count);

        //for worker 2
        List<Gamecore.Tile> validMoveTiles2 = gc.getValidSpacesForAction(AITiles[1].getRow(), AITiles[1].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles2)
        {
            addWorkerMoves(gc, AITiles[1], t, ref possibleTurns);
        }

        //UnityEngine.Debug.Log(possibleTurns.Count);

        return possibleTurns;
    }


    private Identification getNextPlayer(Identification playerId)
    {
        if (playerId == Identification.AI)
            return Identification.Human;
        else
            return Identification.AI;
    }

    //actual algorithm
    //DOESN'T WORK PAST DEPTH OF 1
    private ScoredMove minimax(GameController gc, Identification playerId, int maxDepth, int currDepth, ref float bestScore)
    {
        ScoredMove result;

        if (gc.checkForWin().getGameHasWinner() || currDepth == maxDepth)
        {
            result.score = evalBoard(gc, playerId);
            result.move = null;
            //result.build = null;

            return result;
        }

        Tuple<Move, Move> bestTurn = null;

        if (playerId == Identification.AI)
        {
            bestScore = float.NegativeInfinity;
        }
        else
        {
            bestScore = float.PositiveInfinity;
        }

        //for every possible move
        List<Tuple<Move, Move>> validMoves = getAllPossibleMoves(gc, playerId);

        foreach (Tuple<Move, Move> m in validMoves)
        {
            //make new gc to make full move?
            GameController newGC = gc.Clone();

            Worker chosenWorker = m.Item1.fromTile.getWorker();
            newGC.movePlayer(chosenWorker, chosenWorker.getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
                                        m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
            newGC.workerBuild(chosenWorker, chosenWorker.getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
                                        m.Item2.toTile.getRow(), m.Item2.toTile.getCol());

            //recurse
            ScoredMove currScoredMove = minimax(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, ref bestScore);

            if (playerId == Identification.AI)
            {
                if (currScoredMove.score > bestScore)
                {
                    bestScore = currScoredMove.score;
                    bestTurn = m;
                    //UnityEngine.Debug.Log(bestTurn);
                }
            }
            else
            {
                if (currScoredMove.score < bestScore)
                {
                    bestScore = currScoredMove.score;
                    bestTurn = m;
                }
            }
        }


        result.move = bestTurn;
        result.score = bestScore;
        return result;
    }


    //HEURISTIC HELPERS
    float numMoves(GameController gc, Identification id)
    {
        List<Gamecore.Tile> tiles = gc.getOccupiedTiles();
        int moves = 0;
        foreach (Gamecore.Tile ti in tiles)
        {
            if (ti.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                moves += gc.getValidSpacesForAction(ti.getRow(), ti.getCol(), MoveAction.Move).Count;
            }
        }

        return moves * MOVES;
    }

    float workerHeight(GameController gc, Identification id)
    {
        List<Gamecore.Tile> tiles = gc.getOccupiedTiles();
        int height = 0;
        foreach(Gamecore.Tile ti in tiles)
        {
            if(ti.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                height += ti.getHeight();
            }
        }

        return height * WORKER_HEIGHT;
    }

    float buildOnSameLevel(GameController gc, Identification id)
    {
        List<Gamecore.Tile> occupied = gc.getOccupiedTiles();
        int height = 0;
        float score = 0f;
        foreach (Gamecore.Tile ti in occupied)
        {
            if (ti.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                height = ti.getHeight();
                List<Gamecore.Tile> nextTiles = ti.getAdjacentTiles();
                foreach(Gamecore.Tile adjTi in nextTiles)
                {
                    if(adjTi.getHeight() == height)
                    {
                        score += PIPE_ON_SAME_LEVEL;
                    }
                }
            }
        }
        return score;
    }

    bool enemyCanWin(Gamecore.Tile tile, Identification id)
    {
        foreach (Gamecore.Tile ti in tile.getAdjacentTiles())
        {
            UnityEngine.Debug.Log("Adjacent tile at: " + ti.getRow() + ", " + ti.getCol());
            if (ti.getWorker() != null && ti.getHeight() == 2)
            {
                if (ti.getWorker().getOwner().getTypeOfPlayer() != id)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //bool canBlockwin(Gamecore.GameController gc, ref Gamecore.Tile blockTile, Identification id)
    //{
    //    List<Gamecore.Tile> moveableTiles = gc.getValidSpacesForAction();
    //    foreach (Gamecore.Tile ti in gc.getGameboard())
    //    {
    //        if (ti.getHeight() == 3 && enemyCanWin(ti, id))
    //        {
    //            var blockTiles = moveableTiles.Intersect(ti.getAdjacentTiles());
    //            if (blockTiles.Count() > 0)
    //            {
    //                blockTile = blockTiles.First();
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}

    //HEURISTIC
    private float evalBoard(GameController gc, Identification id)
    {
        float score = 0;
        
        //obviously, if game is over, score accordingly
        Gamecore.Winner winresults = gc.checkForWin();
        if (winresults.getGameHasWinner())
        {
            if(winresults.getWinner().getTypeOfPlayer() == Identification.AI)
            {
                return MAX_SCORE;
            }
            else
            {
                return MIN_SCORE;
            }
        }



        //heuristic factors
        score += numMoves(gc, Identification.AI);
        score -= numMoves(gc, Identification.Human);

        score += workerHeight(gc, Identification.AI);
        score -= workerHeight(gc, Identification.Human);


        return score;
    }


    //Check for win
    //Check for lose
    //Check for opponent win on next turn
    //Check for blocking opponent win

    //Check our height <
    //Check opponent height <
    //Check build on same level <
    //Check available moves <
    //Check build not on base
}
