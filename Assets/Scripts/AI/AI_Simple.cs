using System;
using System.Diagnostics;
using System.Collections.Generic;
using Gamecore;
using System.Linq;

public struct ScoredMove
{
    public Tuple<Move, Move> move;
    public float score; 
};

//requires integration with Gamecore classes
//Iktinos iteration 1: basic heuristic + minimax and alpha/beta to depth 1-2
public class AI_Simple : Opponent
{
    private Gamecore.Tile[,] initBoard;

    const float MAX_SCORE = 100.0f;
    const float MIN_SCORE = -100.0f;
    const int MAX_DEPTH = 1;

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

    //MAIN FUNCTION
    //  currently returns Tuple of Move object, one of Action.Move, one of Action.Build, with to and from tiles
    public override Tuple<Move, Move> GetMove(GameController gc)
    {
        Tuple<Move, Move> bestMove;

        //initBoard = gc.getGameboard();
        //clear undo/redo stack here?


        //PICKS RANDOM MOVE
        //var rand = new Random();
        //List<Tuple<Move, Move>> possibleTurns = getAllPossibleMoves(gc,Identification.AI);
        //int moveIndex = rand.Next(possibleTurns.Count);
        //bestMove = possibleTurns[moveIndex];

        //MINIMAX + HEURISTIC
        //return minimax(gc, Identification.AI, MAX_DEPTH, 0).move;
        bestMove = minimaxAlphaBeta(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity).move;
        return bestMove;
    }

    //helper function for getAllPossibleMoves?
    private void addWorkerMoves(GameController gc, Gamecore.Tile workerTile, Gamecore.Tile moveTile, ref List<Tuple<Move, Move>> possibleTurns)
    {
        Gamecore.GameController tempGC = gc.Clone();

        //"move" worker so temp GameController correctly generates valid build spaces
        tempGC.movePlayer(workerTile.getWorker(), workerTile.getWorker().getOwner(),
                  workerTile.getRow(), workerTile.getCol(), moveTile.getRow(), moveTile.getCol());

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
        List<Gamecore.Tile> playerTiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == playerId)
            {
                playerTiles.Add(t);
            }
        }

        //for worker 1
        List<Gamecore.Tile> validMoveTiles1 = gc.getValidSpacesForAction(playerTiles[0].getRow(), playerTiles[0].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles1)
        {
            addWorkerMoves(gc, playerTiles[0], t, ref possibleTurns);
        }

        //UnityEngine.Debug.Log(possibleTurns.Count);

        //for worker 2
        List<Gamecore.Tile> validMoveTiles2 = gc.getValidSpacesForAction(playerTiles[1].getRow(), playerTiles[1].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles2)
        {
            addWorkerMoves(gc, playerTiles[1], t, ref possibleTurns);
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
    //DEPTH PAST 1 EXCEEDS TIME LIMIT
    //ALPHA BETA FUNCTIONALITY QUESTIONABLE PAST DEPTH OF 1???
    private ScoredMove minimaxAlphaBeta(GameController gc, Identification playerId, int maxDepth, int currDepth, float alpha, float beta)
    {
        ScoredMove result;

        if (gc.checkForWin().getGameHasWinner() || currDepth == maxDepth)
        {
            result.score = evalBoard(gc, playerId);
            result.move = null;

            return result;
        }

        Tuple<Move, Move> bestTurn = null;
        float bestScore;

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
            //make new gc to make full move
            GameController newGC = gc.Clone();

            Worker chosenWorker = m.Item1.fromTile.getWorker();
            newGC.movePlayer(chosenWorker, chosenWorker.getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
                                        m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
            newGC.workerBuild(chosenWorker, chosenWorker.getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
                                        m.Item2.toTile.getRow(), m.Item2.toTile.getCol());

            //recurse
            ScoredMove currScoredMove = minimaxAlphaBeta(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, alpha, beta);

            if (playerId == Identification.AI)
            {
                if (currScoredMove.score > bestScore)
                {
                    bestScore = currScoredMove.score;
                    bestTurn = m;
                    //UnityEngine.Debug.Log(bestTurn);
                }

                //alpha beta part
                alpha = Math.Max(alpha, bestScore);
                if(beta <= alpha)
                {
                    break;
                }
            }
            else
            {
                if (currScoredMove.score < bestScore)
                {
                    bestScore = currScoredMove.score;
                    bestTurn = m;
                }

                //alpha beta part
                beta = Math.Min(beta, bestScore);
                if (beta <= alpha)
                {
                    break;
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

    bool humanCanWin(GameController gc)
    {
        //get human player's worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> humanTiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == Identification.Human)
            {
                humanTiles.Add(t);
            }
        }

        //if either worker is on level 2 and can move to a level 3, return true
        if(humanTiles[0].getHeight() == 2)
        {
            List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(humanTiles[0].getRow(), humanTiles[0].getCol(), Gamecore.MoveAction.Move);
            foreach(Gamecore.Tile t in validMoveTiles)
            {
                if (t.getHeight() == 3)
                {
                    return true;
                }
            }
        }
        if (humanTiles[1].getHeight() == 2)
        {
            List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(humanTiles[1].getRow(), humanTiles[1].getCol(), Gamecore.MoveAction.Move);
            foreach (Gamecore.Tile t in validMoveTiles)
            {
                if (t.getHeight() == 3)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //bool enemyCanWin(Gamecore.Tile tile, Identification id)
    //{
    //    foreach (Gamecore.Tile ti in tile.getAdjacentTiles())
    //    {
    //        UnityEngine.Debug.Log("Adjacent tile at: " + ti.getRow() + ", " + ti.getCol());
    //        if (ti.getWorker() != null && ti.getHeight() == 2)
    //        {
    //            if (ti.getWorker().getOwner().getTypeOfPlayer() != id)
    //            {
    //                return true;
    //            }
    //        }
    //    }
    //    return false;
    //}

    ////
    //bool canBlockwin(Gamecore.GameController gc, ref Gamecore.Tile blockTile, Identification id)
    //{
    //    List<Gamecore.Tile> moveableTiles = gc.getValidSpacesForAction(blockTile.getRow(), blockTile.getCol(), MoveAction.Move);
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

        if (humanCanWin(gc))
        {
            return MIN_SCORE + 1;
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
