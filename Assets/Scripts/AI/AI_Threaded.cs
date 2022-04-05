using System;
using System.Diagnostics;
using System.Collections.Generic;
using Gamecore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using System.Timers;

//requires integration with Gamecore classes
//Iktinos iteration 4: heuristic + minimaxab depth 1-2, in parallel???
public class AI_Best : Opponent
{
    private Gamecore.Tile[,] initBoard;
    bool moveReady = false;
    const float MAX_SCORE = 250.0f;
    const float MIN_SCORE = -250.0f;
    const int MAX_DEPTH = 2;
    const int MAX_MSEC = 5750;

    const float WORKER_HEIGHT = 15f;
    const float MOVES = .5f;
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
        if (moveReady)
        {
            moveReady = false;
            return true;
        }
        else
        {
            return false;
        }
    }


    //FIRST PLACEMENTS
    //returns tuple of Move objects with fromTiles set to null and toTiles set to the tiles to place workers on
    public override Tuple<Move, Move> GetWorkerPlacements(GameController gc)
    {
        initBoard = gc.getGameboard();
        var rand = new Random();

        int randRow1 = rand.Next(1, 4);
        int randCol1 = rand.Next(1, 4);
        int randRow2 = rand.Next(1, 4);
        int randCol2 = rand.Next(1, 4);

        //choose indecies for different empty tiles in center
        while (initBoard[randRow1, randCol1].getWorker() != null || initBoard[randRow2, randCol2].getWorker() != null
                || ((randRow1 == randRow2) && (randCol1 == randCol2)))
        {
            randRow1 = rand.Next(1, 4);
            randCol1 = rand.Next(1, 4);
            randRow2 = rand.Next(1, 4);
            randCol2 = rand.Next(1, 4);
        }

        Gamecore.Tile tile1 = initBoard[randRow1, randCol1];
        Gamecore.Tile tile2 = initBoard[randRow2, randCol2];

        Move AIPlace1 = new Move(null, tile1, Gamecore.MoveAction.Move, null);

        Move AIPlace2 = new Move(null, tile2, Gamecore.MoveAction.Move, null);

        moveReady = true;
        return new Tuple<Move, Move>(AIPlace1, AIPlace2);
    }

    //MAIN FUNCTION
    //  currently returns Tuple of Move object, one of Action.Move, one of Action.Build, with to and from tiles
    public override Tuple<Move, Move> GetMove(GameController gc)
    {
        moveReady = false;
        UnityEngine.Debug.Log("Started");
        Tuple<Move, Move> bestMove;

        //clear undo/redo stack here?
        //GameController freshGC = gc.Clone();
        //freshGC.clearStack();

        //MINIMAX + HEURISTIC
        //ScoredMove bestSMove = minimaxAlphaBeta(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity);
        ScoredMove bestSMove = getMoveThreaded(gc);

        if (bestSMove.move != null)
        {
            bestMove = bestSMove.move;
            UnityEngine.Debug.Log("Best Move from " + bestSMove.move.Item1.fromTile.getRow() + "," + bestSMove.move.Item1.fromTile.getCol() + " to " +
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
        UnityEngine.Debug.Log("Finished");
        moveReady = true;
        return bestMove;
    }


    //LEGAL MOVE GENERATION
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
                //UnityEngine.Debug.Log(t.getCol() + "," + t.getRow() + " " + playerId);
            }
        }

        //for worker 1
        List<Gamecore.Tile> validMoveTiles1 = gc.getValidSpacesForAction(playerTiles[0].getRow(), playerTiles[0].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles1)
        {
            addWorkerMoves(gc, playerTiles[0], t, ref possibleTurns);
        }

        //for worker 2
        List<Gamecore.Tile> validMoveTiles2 = gc.getValidSpacesForAction(playerTiles[1].getRow(), playerTiles[1].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles2)
        {
            addWorkerMoves(gc, playerTiles[1], t, ref possibleTurns);
        }

        return possibleTurns;
    }

    //PARALLEL HELPER FUNCTION
    private List<Tuple<Move, Move>> getAllPossibleMovesForWorker(GameController gc, Gamecore.Tile workerTile)
    {
        List<Tuple<Move, Move>> possibleTurns = new List<Tuple<Move, Move>>();

        //for worker 1
        List<Gamecore.Tile> validMoveTiles1 = gc.getValidSpacesForAction(workerTile.getRow(), workerTile.getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles1)
        {
            addWorkerMoves(gc, workerTile, t, ref possibleTurns);
        }

        return possibleTurns;
    }


    //SORT HELPER FUNCTION
    //sort function
    private List<Tuple<Move, Move>> sortMoves(GameController gc, List<Tuple<Move, Move>> validMoves)
    {
        List<Tuple<Move, Move>> sortedMoves = new List<Tuple<Move, Move>>();

        List<ScoredMove> scoredMoves = new List<ScoredMove>();
        foreach (Tuple<Move, Move> m in validMoves)
        {
            //make new gc to make full move
            GameController newGC = gc.Clone();

            Worker chosenWorker = m.Item1.fromTile.getWorker();
            newGC.movePlayer(chosenWorker, chosenWorker.getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
                                        m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
            newGC.workerBuild(chosenWorker, chosenWorker.getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
                                        m.Item2.toTile.getRow(), m.Item2.toTile.getCol());

            ScoredMove sm;
            sm.move = m;
            sm.score = evalBoard(newGC, getNextPlayer(Identification.AI));

            scoredMoves.Add(sm);
        }

        //sort by score in descending order
        scoredMoves.Sort(
            (sm1, sm2) =>
            {
                return sm2.score.CompareTo(sm1.score);
            }
        );

        foreach (ScoredMove sm in scoredMoves)
        {
            sortedMoves.Add(sm.move);
        }

        return sortedMoves;
    }

    private Identification getNextPlayer(Identification playerId)
    {
        if (playerId == Identification.AI)
            return Identification.Human;
        else
            return Identification.AI;
    }


    //ALGORITHMS
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

        //gen all possible moves by given player
        List<Tuple<Move, Move>> validMoves = getAllPossibleMoves(gc, playerId);

        //IF FIRST TURN, SORT BY SCORE? To improve performance
        if (currDepth == 0)
        {
            validMoves = sortMoves(gc, validMoves);
        }

        //for every valid move
        foreach (Tuple<Move, Move> m in validMoves)
        {
            //make new gc to make full move
            GameController newGC = gc.Clone();

            Worker chosenWorker = m.Item1.fromTile.getWorker();
            newGC.movePlayer(chosenWorker, chosenWorker.getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
                                        m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
            newGC.workerBuild(chosenWorker, chosenWorker.getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
                                        m.Item2.toTile.getRow(), m.Item2.toTile.getCol());

            //UnityEngine.Debug.Log("Move from " + m.Item1.fromTile.getRow() + "," + m.Item1.fromTile.getCol() + " to " +
            //    m.Item1.toTile.getRow() + "," + m.Item1.toTile.getCol() + " building on " +
            //    m.Item2.toTile.getRow() + "," + m.Item2.toTile.getCol() + " has a score of " + evalBoard(newGC));

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
                if (beta <= alpha)
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

//PARALLEL ATTEMPT

    //get all possible moves for one worker, then another, then hand each set to a modified minimax in separate threads
    //given the moves at the end, whichever one has higher score is better, return that
    private ScoredMove minimaxABThread(GameController gc, Identification playerId, int maxDepth, int currDepth, float alpha, float beta, Gamecore.Tile workerTile, DateTime start)
    {
        ScoredMove result;

        DateTime now = DateTime.Now;
        TimeSpan ts = (now - start);

        if (gc.checkForWin().getGameHasWinner() || currDepth == maxDepth || ts.TotalMilliseconds > MAX_MSEC)
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

        //gen all possible moves by given player
        List<Tuple<Move, Move>> validMoves; //= getAllPossibleMoves(gc, playerId);

        //IF FIRST TURN, SORT BY SCORE? To improve performance
        if (currDepth == 0)
        {
            validMoves = getAllPossibleMovesForWorker(gc, workerTile);
            validMoves = sortMoves(gc, validMoves);
        }
        else
        {
            validMoves = getAllPossibleMoves(gc, playerId);
        }

        //for every valid move
        foreach (Tuple<Move, Move> m in validMoves)
        {
            //make new gc to make full move
            GameController newGC = gc.Clone();

            Worker chosenWorker = m.Item1.fromTile.getWorker();
            newGC.movePlayer(chosenWorker, chosenWorker.getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
                                        m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
            newGC.workerBuild(chosenWorker, chosenWorker.getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
                                        m.Item2.toTile.getRow(), m.Item2.toTile.getCol());

            //UnityEngine.Debug.Log("Move from " + m.Item1.fromTile.getRow() + "," + m.Item1.fromTile.getCol() + " to " +
            //    m.Item1.toTile.getRow() + "," + m.Item1.toTile.getCol() + " building on " +
            //    m.Item2.toTile.getRow() + "," + m.Item2.toTile.getCol() + " has a score of " + evalBoard(newGC));

            //recurse
            ScoredMove currScoredMove = minimaxABThread(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, alpha, beta, null, start);

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
                if (beta <= alpha)
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


    private ScoredMove getMoveThreaded(GameController gc)
    {
        //get AI worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> AITiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == Identification.AI)
            {
                AITiles.Add(t);
                //UnityEngine.Debug.Log(t.getCol() + "," + t.getRow() + " " + playerId);
            }
        }

        ScoredMove result1;
        ScoredMove result2;

        //here so it compiles
        result1.move = null;
        result1.score = 0;
        result2.move = null;
        result2.score = 0;

        Thread thread1 = new Thread(
        () =>
        {
            result1 = minimaxABThread(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity, AITiles[0], DateTime.Now);
        });

        Thread thread2 = new Thread(
        () =>
        {
            result2 = minimaxABThread(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity, AITiles[1], DateTime.Now);
        });

        thread1.Start();
        thread2.Start();
        
        //result2 = minimaxABThread(gc, Identification.AI, MAX_DEPTH, 0, float.NegativeInfinity, float.PositiveInfinity, AITiles[1]);
        
        thread1.Join();//not sure if this if right for getting result
        thread2.Join();

        //UnityEngine.Debug.Log("Move from " + result1.move.Item1.fromTile.getRow() + "," + result1.move.Item1.fromTile.getCol() + " to " +
        //result1.move.Item1.toTile.getRow() + "," + result1.move.Item1.toTile.getCol() + " building on " +
        //result1.move.Item2.toTile.getRow() + "," + result1.move.Item2.toTile.getCol() + " has a score of " + result1.score);

        //UnityEngine.Debug.Log("Move from " + result2.move.Item1.fromTile.getRow() + "," + result2.move.Item1.fromTile.getCol() + " to " +
        //result2.move.Item1.toTile.getRow() + "," + result2.move.Item1.toTile.getCol() + " building on " +
        //result2.move.Item2.toTile.getRow() + "," + result2.move.Item2.toTile.getCol() + " has a score of " + result2.score);

        if (result1.score > result2.score)
        {
            return result1;
        }
        else
        {
            return result2;
        }
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
        foreach (Gamecore.Tile ti in tiles)
        {
            if (ti.getWorker().getOwner().getTypeOfPlayer() == id)
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
                foreach (Gamecore.Tile adjTi in nextTiles)
                {
                    if (adjTi.getHeight() == height)
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
        //get player's worker tiles
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
        if (tiles[0].getHeight() == 2)
        {
            List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(tiles[0].getRow(), tiles[0].getCol(), Gamecore.MoveAction.Move);
            foreach (Gamecore.Tile t in validMoveTiles)
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

    bool canMoveUp(GameController gc, Identification id)
    {
        //get player's worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> tiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                tiles.Add(t);
            }
        }

        //if player's worker can move up next turn, return true
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

    float moveUpScore(GameController gc, Identification id)
    {
        float score = 0f;

        //get player's worker tiles
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> tiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == id)
            {
                tiles.Add(t);
            }
        }

        //if player's worker can move up next turn, return true
        List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(tiles[0].getRow(), tiles[0].getCol(), Gamecore.MoveAction.Move);
        foreach (Gamecore.Tile t in validMoveTiles)
        {
            if (t.getHeight() > tiles[0].getHeight())
            {
                score += 5f;
            }
        }
        validMoveTiles = gc.getValidSpacesForAction(tiles[1].getRow(), tiles[1].getCol(), Gamecore.MoveAction.Move);
        foreach (Gamecore.Tile t in validMoveTiles)
        {
            if (t.getHeight() > tiles[1].getHeight())
            {
                score += 5f;
            }
        }

        return score;
    }

    float proximityScore(GameController gc)
    {
        float score = 0.0f;

        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> AITiles = new List<Gamecore.Tile>();
        List<Gamecore.Tile> humanTiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == Identification.Human)
            {
                humanTiles.Add(t);
            }
            else
            {
                AITiles.Add(t);
            }
        }

        //if any human worker is too far away from both AI workers
        foreach (Gamecore.Tile t in humanTiles)
        {
            int col = t.getCol();
            int row = t.getRow();
            if (Math.Abs(AITiles[0].getCol() - col) > 1 || Math.Abs(AITiles[0].getRow() - row) > 1)
            {
                if (Math.Abs(AITiles[1].getCol() - col) > 1 || Math.Abs(AITiles[1].getRow() - row) > 1)
                {
                    score -= 20.0f * t.getHeight();
                }
            }
        }

        return score;
    }

    //
    bool cantBlockwin(Gamecore.GameController gc, Identification blockerId)
    {
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> AITiles = new List<Gamecore.Tile>();
        List<Gamecore.Tile> humanTiles = new List<Gamecore.Tile>();
        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == Identification.Human)
            {
                humanTiles.Add(t);
            }
            else
            {
                AITiles.Add(t);
            }
        }

        //for human workers
        foreach(Gamecore.Tile ht in humanTiles)
        {
            if(ht.getHeight() == 2)
            {
                List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(ht.getRow(), ht.getCol(), Gamecore.MoveAction.Move);
                foreach (Gamecore.Tile mt in validMoveTiles)
                {
                    if (mt.getHeight() == 3)
                    {
                        //if human worker can win, can AI block?
                        //APPROXIMATION: just goes off of row/col, not if AI can move there
                        int col = mt.getCol();
                        int row = mt.getRow();
                        if (Math.Abs(AITiles[0].getCol() - col) > 2 || Math.Abs(AITiles[0].getRow() - row) > 2)
                        {
                            if (Math.Abs(AITiles[1].getCol() - col) > 2 || Math.Abs(AITiles[1].getRow() - row) > 2)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    //HEURISTIC
    private float evalBoard(GameController gc, Identification nextPlayer)
    {
        float score = 0;

        //obviously, if game is over, score accordingly
        //THIS DOESN'T WORK WITH NEGAMAX AND i WISH i UNDERSTOOD WHY BUT WHATEVER
        Gamecore.Winner winresults = gc.checkForWin();
        if (winresults.getGameHasWinner())
        {
            //UnityEngine.Debug.Log(winresults.getWinner().getTypeOfPlayer());
            if (winresults.getWinner().getTypeOfPlayer() == Identification.AI)
            {
                return MAX_SCORE;
            }
            else
            {
                return MIN_SCORE;
            }
        }


        //shorthand for predicting next player turn
        //if depth = 2 (human player just moved)
        if(nextPlayer == Identification.AI)
        {
            if (canWinNextTurn(gc, Identification.AI))
            {
                return MAX_SCORE-1;
            }
            if (cantBlockwin(gc, Identification.AI))
            {
                return MIN_SCORE;
            }
        }
        else //AI just moved
        {
            if (canWinNextTurn(gc, Identification.Human))
            {
                return MIN_SCORE;
            }
        }
        
        //if (canWinNextTurn(gc, Identification.Human))
        //{
        //    score = MIN_SCORE+1;
        //}
        //if (canMoveUp(gc, Identification.Human))
        //{
        //    score -= 10.0f;
        //}
        //if (canMoveUp(gc, Identification.AI))
        //{
        //    score += 10.0f;
        //}

        //heuristic factors

        score += numMoves(gc, Identification.AI);
        score -= numMoves(gc, Identification.Human);

        score += workerHeight(gc, Identification.AI);
        score -= workerHeight(gc, Identification.Human);

        score += proximityScore(gc);

        if (canMoveUp(gc, Identification.AI) && nextPlayer == Identification.AI)
        {
            score += 15f;
        }
        else if(canMoveUp(gc, Identification.Human) && nextPlayer == Identification.Human)
        {
            score -= 15f;
        }
        //if (nextPlayer == Identification.AI)
        //{
        //    score += moveUpScore(gc, Identification.AI);
        //}
        //else
        //{
        //    score -= moveUpScore(gc, Identification.Human);
        //}

        return score;
    }

}
