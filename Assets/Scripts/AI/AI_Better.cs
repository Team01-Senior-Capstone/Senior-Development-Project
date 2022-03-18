using System;
using System.Diagnostics;
using System.Collections.Generic;
using Gamecore;
using System.Linq;

//public struct ScoredMove
//{
//    public Tuple<Move, Move> move;
//    public float score;
//};

//requires integration with Gamecore classes
//Iktinos iteration 2: heuristic + negascout? depth 1-3
public class AI_Better : Opponent
{
    private Gamecore.Tile[,] initBoard;

    const float MAX_SCORE = 100.0f;
    const float MIN_SCORE = -100.0f;
    const int MAX_DEPTH = 2;

    const float WORKER_HEIGHT = 10f;
    const float MOVES = 1f;
    const float PIPE_ON_SAME_LEVEL = 1f;

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

        //clear undo/redo stack here?
        //GameController freshGC = gc.Clone();
        //freshGC.clearStack();

        //MINIMAX + HEURISTIC
        //ScoredMove bestSMove = minimax(gc, Identification.AI, MAX_DEPTH, 0);
        ScoredMove bestSMove = minimaxAlphaBeta(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity);

        //NEGAMAX + HEURISTIC???
        //ScoredMove bestSMove = negamaxAlphaBeta(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity);
        //ScoredMove bestSMove = scout(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity);

        if (bestSMove.move != null)
        {
            bestMove = bestSMove.move;
            UnityEngine.Debug.Log("Move from " + bestSMove.move.Item1.fromTile.getRow() + "," + bestSMove.move.Item1.fromTile.getCol() + " to " +
                bestSMove.move.Item1.toTile.getRow() + "," + bestSMove.move.Item1.toTile.getCol() + " building on " + 
                bestSMove.move.Item2.toTile.getRow() + "," + bestSMove.move.Item2.toTile.getCol() + " has a score of " + bestSMove.score);
        }
        else
        {
            var rand = new Random();
            List<Tuple<Move, Move>> possibleTurns = getAllPossibleMoves(gc, Identification.AI);
            int moveIndex = rand.Next(possibleTurns.Count);
            bestMove = possibleTurns[moveIndex];
        }

        return bestMove;
    }

    //helper function for getAllPossibleMoves
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


    //actual algorithms
    //this doesn't work lol
    private ScoredMove negamaxAlphaBeta(GameController gc, Identification playerId, int maxDepth, int currDepth, float alpha, float beta)
    {
        ScoredMove result;

        if (gc.checkForWin().getGameHasWinner() || currDepth == maxDepth)
        {
            result.score = evalBoard(gc, playerId);
            result.move = null;

            return result;
        }

        Tuple<Move, Move> bestTurn = null;
        float bestScore = float.NegativeInfinity;

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
            //ScoredMove currScoredMove = negamaxAlphaBeta(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, -beta, -Math.Max(alpha, bestScore));
            ScoredMove currScoredMove = negamaxAlphaBeta(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, -beta, -alpha);
            float currScore = -currScoredMove.score;

            if(currScore > bestScore)
            {
                bestScore = currScore;
                bestTurn = m;
            }

            alpha = Math.Max(alpha, bestScore);
            if(alpha >= beta)
            {
                break;
            }
        }

        result.move = bestTurn;
        result.score = bestScore;
        return result;
    }


    //aaaaaaaa
    private ScoredMove scout(GameController gc, Identification playerId, int maxDepth, int currDepth, float alpha, float beta)
    {
        ScoredMove result;

        if (gc.checkForWin().getGameHasWinner() || currDepth == maxDepth)
        {
            result.score = evalBoard(gc, playerId);
            result.move = null;

            return result;
        }

        Tuple<Move, Move> bestTurn = null;
        float bestScore = float.NegativeInfinity; //??

        //fancy
        float adaptiveBeta = beta;

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

            UnityEngine.Debug.Log("Move from " + m.Item1.fromTile.getRow() + "," + m.Item1.fromTile.getCol() + " to " +
                m.Item1.toTile.getRow() + "," + m.Item1.toTile.getCol() + " building on " +
                m.Item2.toTile.getRow() + "," + m.Item2.toTile.getCol() + " has a score of " + evalBoard(newGC,playerId));

            //recurse
            ScoredMove currScoredMove = negamaxAlphaBeta(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, -adaptiveBeta, -Math.Max(alpha, bestScore));

            //update best score
            float currScore = -currScoredMove.score;

            if(currScore > bestScore)
            {
                //widen to negamax?
                if(adaptiveBeta == beta || currDepth >= maxDepth - 2)
                {
                    bestScore = currScore;
                    bestTurn = m;
                }
                //or go narrow if not?
                else
                {
                    ScoredMove testMove = scout(newGC, getNextPlayer(playerId), maxDepth, currDepth, -beta, -(evalBoard(newGC, playerId)));
                    bestScore = -testMove.score;
                }

                //if outside of bounds, prune by exiting
                if(bestScore >= beta)
                {
                    result.move = bestTurn;
                    result.score = bestScore;
                    return result;
                }

                adaptiveBeta = Math.Max(alpha, bestScore) + 1;
            }
        }

        result.move = bestTurn;
        result.score = bestScore;

        //UnityEngine.Debug.Log("Move from " + result.move.Item1.fromTile.getRow() + "," + result.move.Item1.fromTile.getCol() + " to " +
        //        result.move.Item1.toTile.getRow() + "," + result.move.Item1.toTile.getCol() + " building on " +
        //        result.move.Item2.toTile.getRow() + "," + result.move.Item2.toTile.getCol() + " has a score of " + result.score);

        return result;
    }

    //the og
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

        //SORT BY SCORE? To improve performance
        if (currDepth == 0)
        {
            foreach (Tuple<Move, Move> m in validMoves)
            {
                //make new gc to make full move
                GameController newGC = gc.Clone();

                Worker chosenWorker = m.Item1.fromTile.getWorker();
                newGC.movePlayer(chosenWorker, chosenWorker.getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
                                            m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
                newGC.workerBuild(chosenWorker, chosenWorker.getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
                                            m.Item2.toTile.getRow(), m.Item2.toTile.getCol());
            }
        }

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

    //BASIC MINIMAX
    private ScoredMove minimax(GameController gc, Identification playerId, int maxDepth, int currDepth)
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
            ScoredMove currScoredMove = minimax(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1);

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

    float adjTilesOnSameLevel(GameController gc, Identification id)
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

    bool canWinNextTurn(GameController gc, Identification id)
    {
        //get human player's worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> tiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                tiles.Add(t);
            }
        }

        //if either worker is on level 2 and can move to a level 3, return true
        if(tiles[0].getHeight() == 2)
        {
            List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(tiles[0].getRow(), tiles[0].getCol(), Gamecore.MoveAction.Move);
            foreach(Gamecore.Tile t in validMoveTiles)
            {
                if (t.getHeight() == 3)
                {
                    return true;
                }
            }
        }
        if (tiles[1].getHeight() == 2)
        {
            List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(tiles[1].getRow(), tiles[1].getCol(), Gamecore.MoveAction.Move);
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

    //TEMPORARY FUNCTION FOR AI ROUND 1? looking at future moves will make irrelevant?
    bool humanCanMoveUp(GameController gc)
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

        //if human player's worker can move up next turn, return true
        List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(humanTiles[0].getRow(), humanTiles[0].getCol(), Gamecore.MoveAction.Move);
        foreach (Gamecore.Tile t in validMoveTiles)
        {
            if (t.getHeight() > humanTiles[0].getHeight())
            {
                return true;
            }
        }
        validMoveTiles = gc.getValidSpacesForAction(humanTiles[1].getRow(), humanTiles[1].getCol(), Gamecore.MoveAction.Move);
        foreach (Gamecore.Tile t in validMoveTiles)
        {
            if (t.getHeight() > humanTiles[1].getHeight())
            {
                return true;
            }
        }

        return false;
    }

    bool canMoveUp(GameController gc, Identification id)
    {
        //get human player's worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> tiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                tiles.Add(t);
            }
        }

        //if human player's worker can move up next turn, return true
        List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(tiles[0].getRow(), tiles[0].getCol(), Gamecore.MoveAction.Move);
        foreach (Gamecore.Tile t in validMoveTiles)
        {
            if (t.getHeight() > tiles[0].getHeight())
            {
                return true;
            }
        }
        validMoveTiles = gc.getValidSpacesForAction(tiles[1].getRow(), tiles[1].getCol(), Gamecore.MoveAction.Move);
        foreach (Gamecore.Tile t in validMoveTiles)
        {
            if (t.getHeight() > tiles[1].getHeight())
            {
                return true;
            }
        }

        return false;
    }


    //HEURISTIC
    private float evalBoard(GameController gc, Identification playerID)
    {
        float score = 0;

        //obviously, if game is over, score accordingly
        //THIS DOESN'T WORK WITH NEGAMAX AND i WISH i UNDERSTOOD WHY BUT WHATEVER
        Gamecore.Winner winresults = gc.checkForWin();
        if (winresults.getGameHasWinner())
        {
            UnityEngine.Debug.Log(winresults.getWinner().getTypeOfPlayer());
            if (winresults.getWinner().getTypeOfPlayer() == Identification.AI)
            {
                return MAX_SCORE;
            }
            else
            {
                return MIN_SCORE;
            }
        }

        ////TEMP FOR AI ROUND 1 ONLY?
        ////shorthand for predicting next turn
        if (playerID == Identification.Human)
        {
            if (canWinNextTurn(gc, Identification.Human))
            {
                return MIN_SCORE + 1;
            }
            if (canMoveUp(gc,Identification.Human))
            {
                score -= 10.0f;
            }
        }
        else
        {
            if (canWinNextTurn(gc, Identification.AI))
            {
                return MAX_SCORE - 1;
            }
            if (canMoveUp(gc, Identification.AI))
            {
                score += 10.0f;
            }
        }

        //heuristic factors
        score += numMoves(gc, Identification.AI);
        score -= numMoves(gc, Identification.Human);

        score += workerHeight(gc, Identification.AI);
        score -= workerHeight(gc, Identification.Human);

        return score;
    }

}
