using System;
using System.Collections.Generic;
using System.Linq;
using Gamecore;

//requires integration with Gamecore classes
//public class AI_Rand : Opponent
public class AI_Rand_Base : Opponent
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

        Gamecore.Tile chosenMoveTile = new Gamecore.Tile(-1, -1);

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

        
        //Get possible moves for both
        List<Gamecore.Tile> validMoveTiles1 = gc.getValidSpacesForAction(AITiles[0].getRow(), AITiles[0].getCol(), Gamecore.MoveAction.Move);
        List<Gamecore.Tile> validMoveTiles2 = gc.getValidSpacesForAction(AITiles[1].getRow(), AITiles[1].getCol(), Gamecore.MoveAction.Move);

        List<Gamecore.Tile> validMoveTiles = new List<Gamecore.Tile>();
        int moveIndex = rand.Next(validMoveTiles.Count);

        //If we can't block a win, 
        if (canBlockwin(gc, validMoveTiles1, ref chosenMoveTile))
        {
            chosenWorkerTile = AITiles[0];

        }
        else if (canBlockwin(gc, validMoveTiles2, ref chosenMoveTile))
        {
            chosenWorkerTile = AITiles[1];
        }
        else {

            List<Gamecore.Tile> possibleTilesWorker1 = new List<Gamecore.Tile>();
            int worker1moveHeight = getHighestTiles(AITiles[0].getHeight(), validMoveTiles1, ref possibleTilesWorker1);


            List<Gamecore.Tile> possibleTilesWorker2 = new List<Gamecore.Tile>();
            int worker2moveHeight = getHighestTiles(AITiles[1].getHeight(), validMoveTiles2, ref possibleTilesWorker2);
           
            if (worker1moveHeight > worker2moveHeight)
            {
                chosenWorkerTile = AITiles[0];
                validMoveTiles = possibleTilesWorker1;
            }
            else
            {
                chosenWorkerTile = AITiles[1];
                validMoveTiles = possibleTilesWorker2;
            }

            //randomly choose a tile to move to
            chosenMoveTile = validMoveTiles[moveIndex];

            //Make the move:
            
        }

        gc.movePlayer(chosenWorkerTile.getWorker(), chosenWorkerTile.getWorker().getOwner(),
                          chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), chosenMoveTile.getRow(), chosenMoveTile.getCol());
        List<Gamecore.Tile> validBuildTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Gamecore.MoveAction.Build);

        //(if chosen move tile has no valid build options, choose another tile to move to)
        if (validBuildTiles.Count == 0)
        {
            //moveIndex = rand.Next(validMoveTiles.Count);
            if (chosenWorkerTile == AITiles[0])
            {
                chosenWorkerTile = AITiles[1];
            }
            else
            {
                chosenWorkerTile = AITiles[0];
            }
            validMoveTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Gamecore.MoveAction.Move);
            List<Gamecore.Tile> alternateTiles = new List<Gamecore.Tile>();
            getHighestTiles(chosenWorkerTile.getHeight(), validMoveTiles, ref alternateTiles);
            chosenMoveTile = alternateTiles[moveIndex];

            gc.movePlayer(chosenWorkerTile.getWorker(), chosenWorkerTile.getWorker().getOwner(),
                          chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), chosenMoveTile.getRow(), chosenMoveTile.getCol());

            validBuildTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Gamecore.MoveAction.Build);
        }

        validBuildTiles = getBuildOnSameLevel(chosenMoveTile.getHeight(), validBuildTiles);
        //if(chosenWorkerTile.getWorker().)

        Gamecore.Worker moved = chosenMoveTile.getWorker();

        Gamecore.Tile chosenBuildTile = new Gamecore.Tile(-1, -1);
        //randomly choose tile to build on
        int buildIndex = rand.Next(validBuildTiles.Count);
        if (validBuildTiles.Count > 0)
        {
            chosenBuildTile = validBuildTiles[buildIndex];
        }

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

    int getHighestTiles(int currentWorkerHeight, List<Gamecore.Tile> tiles, ref List<Gamecore.Tile> newTiles)
    {
        int highestTile = -1;

        foreach(Gamecore.Tile ti in tiles)
        {
            if(ti.getHeight() > highestTile && ti.canMoveTo(currentWorkerHeight))
            {
                newTiles.Clear();
                highestTile = ti.getHeight();
                newTiles.Add(ti);
            }
            else if(ti.getHeight() == highestTile)
            {
                newTiles.Add(ti);
            }
        }

        return highestTile;

    }

    List<Gamecore.Tile> getBuildOnSameLevel(int currentWorkerHeight, List<Gamecore.Tile> tiles)
    {

        List<Gamecore.Tile> newTiles = new List<Gamecore.Tile>();
        List<Gamecore.Tile> oneBelow = new List<Gamecore.Tile>();

        foreach (Gamecore.Tile ti in tiles)
        {
            if(ti.getHeight() == 3 && enemyCanWin(ti))
            {
                newTiles.Clear();
                newTiles.Add(ti);
                return newTiles;
            }
            else if(ti.getHeight() == currentWorkerHeight)
            {
                if(ti.getHeight() == 2 && enemyCanWin(ti))
                {
                    continue;
                }
                newTiles.Add(ti);
            }
            else if(ti.getHeight() == currentWorkerHeight -1)
            {
                oneBelow.Add(ti);
            }
        }

        if(newTiles.Count == 0)
        {
            if (oneBelow.Count == 0)
            {
                return tiles;
            }
            else
            {
                return oneBelow;
            }
        }
        else
        {
            return newTiles;
        }
    }

    bool enemyCanWin(Gamecore.Tile tile)
    {
        foreach(Gamecore.Tile ti in tile.getAdjacentTiles())
        {
            UnityEngine.Debug.Log("Adjacent tile at: " + ti.getRow() + ", " + ti.getCol());
            if (ti.getWorker() != null && ti.getHeight() == 2)
            {
                if(ti.getWorker().getOwner().getTypeOfPlayer() != Gamecore.Identification.AI)
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool canBlockwin(Gamecore.GameController gc, List<Gamecore.Tile> moveableTiles, ref Gamecore.Tile blockTile)
    {
        foreach(Gamecore.Tile ti in gc.getGameboard())
        {
            if(ti.getHeight() == 3)
            {
                UnityEngine.Debug.Log("Found height of 3");
            }
            if(ti.getHeight() == 3 && enemyCanWin(ti))
            {
                UnityEngine.Debug.Log("Found a tile where opponent can win");
                var blockTiles = moveableTiles.Intersect(ti.getAdjacentTiles());
                if(blockTiles.Count() > 0)
                {
                    blockTile = blockTiles.First();
                    return true;
                }
            }
        }
        return false;
    }
}
