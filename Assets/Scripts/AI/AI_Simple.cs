using System;
using System.Collections.Generic;
using Gamecore;

public struct ScoredMove
{
    public Tuple<Move, Move> move;
    //Move move;
    //Move build;
    public float score; 
};

//using Turn = Tuple<Move,Move>;

//requires integration with Gamecore classes
//public class AI_Rand : Opponent
public class AI_Rand : Opponent
{
    private Gamecore.Tile[,] initBoard;

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

    //generate random move
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
        List<Tuple<Move, Move>> possibleTurns = getAllPossibleMoves(gc,Identification.AI);
        
        //CURRENTLY PICKS RANDOM MOVE
        int moveIndex = rand.Next(possibleTurns.Count);
        bestMove = possibleTurns[moveIndex];


        return bestMove;
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
            }
        }

        //for worker 1
        List<Gamecore.Tile> validMoveTiles1 = gc.getValidSpacesForAction(AITiles[0].getRow(), AITiles[0].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles1)
        {
            //"move" worker so GameController correctly generates valid build spaces (move back when done?)
            gc.movePlayer(AITiles[0].getWorker(), AITiles[0].getWorker().getOwner(),
                      AITiles[0].getRow(), AITiles[0].getCol(), t.getRow(), t.getCol());

            List<Gamecore.Tile> validBuildTiles = gc.getValidSpacesForAction(t.getRow(), t.getCol(), Gamecore.MoveAction.Build);

            //for every valid tile to build on from Tile t
            foreach (Gamecore.Tile b in validBuildTiles)
            {
                Move AIMove = new Move(AITiles[0], t, Gamecore.MoveAction.Move, t.getWorker());
                Move AIBuild = new Move(t, b, Gamecore.MoveAction.Build, t.getWorker());

                Tuple<Move, Move> turn = new Tuple<Move, Move>(AIMove, AIBuild);

                possibleTurns.Add(turn);
            }

            //"move" player back once possible builds are found
            gc.movePlayer(t.getWorker(), t.getWorker().getOwner(),
                      t.getRow(), t.getCol(), AITiles[0].getRow(), AITiles[0].getCol());
        }

        //for worker 2
        List<Gamecore.Tile> validMoveTiles2 = gc.getValidSpacesForAction(AITiles[1].getRow(), AITiles[1].getCol(), Gamecore.MoveAction.Move);
        //for every valid tile to move to
        foreach (Gamecore.Tile t in validMoveTiles2)
        {
            //"move" worker so GameController correctly generates valid build spaces (move back when done?)
            gc.movePlayer(AITiles[1].getWorker(), AITiles[1].getWorker().getOwner(),
                      AITiles[1].getRow(), AITiles[1].getCol(), t.getRow(), t.getCol());

            List<Gamecore.Tile> validBuildTiles = gc.getValidSpacesForAction(t.getRow(), t.getCol(), Gamecore.MoveAction.Build);

            //for every valid tile to build on from Tile t
            foreach (Gamecore.Tile b in validBuildTiles)
            {
                Move AIMove = new Move(AITiles[1], t, Gamecore.MoveAction.Move, t.getWorker());
                Move AIBuild = new Move(t, b, Gamecore.MoveAction.Build, t.getWorker());

                Tuple<Move, Move> turn = new Tuple<Move, Move>(AIMove, AIBuild);

                possibleTurns.Add(turn);
            }

            //"move" player back once possible builds are found
            gc.movePlayer(t.getWorker(), t.getWorker().getOwner(),
                      t.getRow(), t.getCol(), AITiles[1].getRow(), AITiles[1].getCol());
        }


        return possibleTurns;
    }

    //private Identification getNextPlayer(Identification playerId)
    //{
    //    if (playerId == Identification.AI)
    //        return Identification.Human;
    //    else
    //        return Identification.AI;
    //}

    //private ScoredMove minimax(GameController gc, Identification playerId, int maxDepth, int currDepth, float& bestScore)
    //{
    //    ScoredMove result;

    //    if (gc.checkForWin().getGameHasWinner() || currDepth == maxDepth)
    //    {
    //        result.score = evalBoard(gc, id);
    //        result.move = null;
    //        //result.build = null;

    //        return result;
    //    }

    //    Tuple<Move, Move> bestTurn = null;
    //    float bestScore;

    //    if(playerId == Identification.AI)
    //    {
    //        bestScore = float.NegativeInfinity; 
    //    }
    //    else
    //    {
    //        bestScore = float.PositiveInfinity;
    //    }

    //    //for every possible move
    //    List<Tuple<Move, Move>> validMoves = getAllPossibleMoves(gc, playerId);

    //    foreach (Tuple<Move,Move> m in validMoves)
    //    {
    //        //make new gc to make full move?
    //        GameController newGC = gc;
    //        newGC.movePlayer(m.Item1.fromTile.getWorker(), m.Item1.fromTile.getWorker().getOwner(), m.Item1.fromTile.getRow(), m.Item1.fromTile.getCol(),
    //                                    m.Item1.toTile.getRow(), m.Item1.toTile.getCol());
    //        newGC.workerBuild(m.Item2.fromTile.getWorker(), m.Item2.fromTile.getWorker().getOwner(), m.Item2.fromTile.getRow(), m.Item2.fromTile.getCol(),
    //                                    m.Item2.toTile.getRow(), m.Item2.toTile.getCol());

    //        //recurse
    //        ScoredMove currScoredMove = minimax(newGC, getNextPlayer(playerId), maxDepth, currDepth + 1, bestScore);

    //        if(playerId == Identification.AI)
    //        {
    //            if(currScoredMove.score > bestScore)
    //            {
    //                bestScore = currScoredMove.score;
    //                bestTurn = currScoredMove.move;
    //            }
    //        }
    //        else
    //        {
    //            if (currScoredMove.score < bestScore)
    //            {
    //                bestScore = currScoredMove.score;
    //                bestTurn = currScoredMove.move;
    //            }
    //        }
    //    }


    //    result.move = bestTurn;
    //    result.score = bestScore;
    //    return result;
    //}

    //private float evalBoard(GameController gc, Identification id)
    //{
    //    if (gc.checkForWin().getGameHasWinner())
    //    {

    //    }
    //}
}
