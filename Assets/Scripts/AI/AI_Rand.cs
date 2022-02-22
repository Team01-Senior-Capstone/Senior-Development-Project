using System;
using System.Collections.Generic;
using Gamecore;

//requires integration with Gamecore classes
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
    //  WORKER OBJECT HAS NO STRING IDENTIFIER YET
    public override Tuple<Move, Move> GetWorkerPlacements(GameController gc)
    {
        initBoard = gc.getGameboard();
        var rand = new Random();

        int randRow1 = rand.Next(5);
        int randCol1 = rand.Next(5);
        int randRow2 = rand.Next(5);
        int randCol2 = rand.Next(5);

        //choose indecies for different empty tiles
        while (initBoard[randRow1, randCol1].getWorker() != null || initBoard[randRow2, randCol2].getWorker() != null
                || ((randRow1 == randRow2) && (randCol1 == randCol2)))
        {
            randRow1 = rand.Next(5);
            randCol1 = rand.Next(5);
            randRow2 = rand.Next(5);
            randCol2 = rand.Next(5);
        }

        Gamecore.Tile tile1 = initBoard[randRow1, randCol1];
        Gamecore.Tile tile2 = initBoard[randRow2, randCol2];

        Move AIPlace1 = new Move(null, tile1, Gamecore.MoveAction.Move, null);
        //AIPlace1.fromTile = null;
        //AIPlace1.toTile = tile1;
        //AIPlace1.action = Action.Move; //different action?
        //AIPlace1.worker = tile1.getWorker(). ; //some get string method goes here?

        Move AIPlace2 = new Move(null, tile2, Gamecore.MoveAction.Move, null);
        //AIPlace2.fromTile = null;
        //AIPlace2.toTile = tile2;
        //AIPlace2.action = Action.Move; //different action?
        //AIPlace2.worker = tile2.getWorker(). ; //some get string method goes here?

        return new Tuple<Move, Move>(AIPlace1, AIPlace2);
    }

    //generate random move
    //  currently returns Tuple of Move object, one of Action.Move, one of Action.Build, with to and from tiles
    //  WORKER CLASS NEEDS METHOD THAT RETURNS STRING TO IDENTIFY WORKER
    //  can restructure if need be
    public override Tuple<Move, Move> GetMove(GameController gc)
    {
        //get gameboard and init rand
        initBoard = gc.getGameboard();
        var rand = new Random();

        //get AI workers
        List<Gamecore.Tile> occupiedTiles = gc.getOccupiedTiles();
        List<Gamecore.Tile> AITiles = new List<Gamecore.Tile>();

        foreach (Gamecore.Tile t in occupiedTiles)
        {
            if (t.getWorker().getOwner().getTypeOfPlayer() == Identification.AI)
            {
                AITiles.Add(t);
            }
        }

        //randomly choose worker and make list from board of all valid worker moves
        int workerIndex = rand.Next(2);
        Gamecore.Tile chosenWorkerTile = AITiles[workerIndex];
        List<Gamecore.Tile> validMoveTiles = gc.getValidSpacesForAction(chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), Gamecore.MoveAction.Move);

        //(if chosen worker has no possible moves, swap)
        if (validMoveTiles.Count == 0)
        {
            workerIndex = 1 - workerIndex;
            chosenWorkerTile = AITiles[workerIndex];
            validMoveTiles = gc.getValidSpacesForAction(chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), Gamecore.MoveAction.Move);
        }


        //randomly choose a tile to move to
        int moveIndex = rand.Next(validMoveTiles.Count);
        Gamecore.Tile chosenMoveTile = validMoveTiles[moveIndex];

        //Make the move:
        gc.movePlayer(chosenWorkerTile.getWorker(), chosenWorkerTile.getWorker().getOwner(),
                      chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), chosenMoveTile.getRow(), chosenMoveTile.getCol());

        List<Gamecore.Tile> validBuildTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Gamecore.MoveAction.Build);

        //(if chosen move tile has no valid build options, choose another tile to move to)
        while (validBuildTiles.Count == 0)
        {
            moveIndex = rand.Next(validMoveTiles.Count);
            chosenMoveTile = validMoveTiles[moveIndex];
            validBuildTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Gamecore.MoveAction.Build);
        }

        //if(chosenWorkerTile.getWorker().)

        Gamecore.Worker moved = chosenMoveTile.getWorker();

        //randomly choose tile to build on
        int buildIndex = rand.Next(validBuildTiles.Count);

        Gamecore.Tile chosenBuildTile = validBuildTiles[buildIndex];

        //randomly chosen move coordinates are now in chosenWorkerTile, chosenMoveTile, and chosenBuildTile!
        //move should be valid(assuming there is at least one single valid move the AI can make)

        Move AIMovement = new Move(chosenWorkerTile, chosenMoveTile, Gamecore.MoveAction.Move, moved);
        //AIMovement.fromTile = chosenWorkerTile;
        //AIMovement.toTile = chosenMoveTile;
        //AIMovement.action = Action.Move;
        //AIMovement.worker = chosenWorkerTile.getWorker(). ; //some get string method goes here?

        Move AIBuild = new Move(chosenMoveTile, chosenBuildTile, Gamecore.MoveAction.Move, moved);
        //AIBuild.fromTile = chosenMoveTile;
        //AIBuild.toTile = chosenBuildTile;
        //AIBuild.action = Action.Build;
        //AIBuild.worker = chosenWorkerTile.getWorker(). ; //some get string method goes here?

        return new Tuple<Move, Move>(AIMovement, AIBuild);
    }
}
