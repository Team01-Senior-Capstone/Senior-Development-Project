//using System;
//using System.Collections.Generic;
//using Gamecore;

//requires integration with Gamecore classes
//public class AI_Rand
//{
//    private Gamecore.Tile[,] initBoard;


//    public Gamecore.Tile[,] getAIWorkerPlacements(GameController gc)
//    {
//        initBoard = gc.getGameboard();
//        var rand = new Random();

//        int randRow = rand.Next(5);
//        int randCol = rand.Next(5);

//        while (initBoard[randRow, randCol].getWorker() != null)
//        {
//            randRow = rand.Next(5);
//            randCol = rand.Next(5);
//        }

//        NEED ACCESS TO AI WORKERS TO CALL FUNCT!!!
//        place AI piece, gen new nums and place again?
//    }

//    generate random move
//      currently returns new board with AI move made bec that's what made the most sense to me
//      can restructure if need be
//      go bigger and return GameController?
//      or go smaller and return start, move to, and build on coordinates?
//    public Gamecore.Tile[,] getAIMove(GameController gc)
//    {
//        get gameboard and init rand
//        initBoard = gc.getGameboard();
//        var rand = new Random();

//        get AI workers
//        List<Gamecore.Tile> occupiedTiles = initBoard.getOccupiedTiles();
//        List<Gamecore.Tile> AITiles;

//        foreach (Gamecore.Tile t in occupiedTiles)
//        {
//            if (t.getWorker().getOwner().getTypeOfPlayer() == Identification.AI)
//            {
//                AITiles.Add(t);
//            }
//        }

//        randomly choose worker and make list from board of all valid worker moves
//        int workerIndex = rand.Next(2);
//        Tile chosenWorkerTile = AITiles[workerIndex];
//        List<Tile> validMoveTiles = gc.getValidSpacesForAction(chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), Action.Move);

//         (if chosen worker has no possible moves, swap)
//        if (validMoveTiles.Count() == 0)
//        {
//            workerIndex = 1 - workerIndex;
//            chosenWorkerTile = AITiles[workerIndex];
//            validMoveTiles = gc.getValidSpacesForAction(chosenWorkerTile.getRow(), chosenWorkerTile.getCol(), Action.Move);
//        }


//        randomly choose a tile to move to
//        int moveIndex = rand.Next(validMoveTiles.Count());
//        Tile chosenMoveTile = validMoveTiles[moveIndex];
//        List<Tile> validBuildTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Action.Build);

//         (if chosen move tile has no valid build options, choose another tile to move to)
//        while (validBuildTiles.Count == 0)
//        {
//            moveIndex = rand.Next(validMoveTiles.Count());
//            chosenMoveTile = validMoveTiles[moveIndex];
//            validBuildTiles = gc.getValidSpacesForAction(chosenMoveTile.getRow(), chosenMoveTile.getCol(), Action.Build);
//        }


//        randomly choose tile to build on
//        int buildIndex = rand.Next(validBuildTiles.Count());
//        validBuildTiles chosenBuildTile = validBuildTiles[buildIndex];

//        randomly chosen move coordinates are now in chosenWorkerTile, chosenMoveTile, and chosenBuildTile!
//        move should be valid (assuming there is at least one single valid move the AI can make)

//        make the move?
//        gc.movePlayer(chosenWorkerTile.getWorker(), chosenWorkerTile.getWorker().getOwner(), chosenWorkerTile.getRow(), chosenWorkerTile.getCol(),
//                        chosenMoveTile.getRow(), chosenMoveTile.getCol());
//        gc.workerBuild(chosenMoveTile.getWorker(), chosenMoveTile.getWorker().getOwner(), chosenMoveTile.getRow(), chosenMoveTile.getCol(),
//                        chosenBuildTile.getRow(), chosenBuildTile.getCol());

//        currently returns new gameboard? again, this can be changed?
//        return gc.getGameboard();
//    }
//}
