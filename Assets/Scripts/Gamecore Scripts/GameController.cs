using System.Collections.Generic;

namespace Gamecore {

    class GameController {

        // Player A is the host or the Human playing the AI. Player B is the 
        // AI or the Client in a network game

        private Player playerA, playerB;
        private Stack<StateInfo> undoStack, redoStack;
        private bool isNetworkGame;
        private GameboardController gameboardController;

        GameController (bool isNetworkGame) {

            this.gameboardController = new GameboardController();
            this.isNetworkGame = isNetworkGame;

            if (!isNetworkGame) {
                this.undoStack = new Stack<StateInfo>();
                this.redoStack = new Stack<StateInfo>();
            }
        }

        /*
            This is the method where the decision is made for which player
            is Player One. I have it set to take an enum value of first to go
            but I am sure there is a better way so this can be changed if need
            be

            THE FIRST PARAMETER IS FOR WHICHEVER PLAYER GOES FIRST AND THE SECOND
            IS FOR WHOEVER GOES SECOND
        */
        public Player[] assignPlayers (Identification firstToGo, Identification secondToGo) {

            playerA = new Player(true, firstToGo);
            playerB = new Player(false, secondToGo);

            return new Player[2] { this.playerA, this.playerB }; 
        }

        public bool placePiece (Worker worker, int row, int col) {

            if (this.gameboardController.getGameboard()[row, col].getWorker() == null) {
                this.gameboardController.getGameboard()[row, col].setWorker(worker);
                this.gameboardController.addTileToOccupied(this.gameboardController.getGameboard()[row, col]);
                return true;
            }
            else return false;
        }

        public WorkerMoveInfo movePlayer(Worker worker, Player player, int curRow, int curCol, 
                                        int destinationRow, int destinationCol) {

            if (worker.isCorrectOwner(player)) {

                List<Tile> validTilesToMoveTo = getValidSpacesForAction(curRow, curCol, Action.Move);
                Tile destinationTile = gameboardController.getGameboard()[destinationRow, destinationCol];
                Tile currentTile = gameboardController.getGameboard()[curRow, curCol];

                if (validTilesToMoveTo.Contains(destinationTile)) {
                    
                    destinationTile.setWorker(worker);
                    currentTile.setWorker(null);

                    gameboardController.addTileToOccupied(destinationTile);
                    gameboardController.removedOccupiedTile(currentTile);

                    WorkerMoveInfo workerMoveInfo = new WorkerMoveInfo(true, currentTile, destinationTile, worker, player);
                    
                    if (!isNetworkGame) {
                        undoStack.Push(workerMoveInfo);
                        redoStack.Clear();
                    }

                    return workerMoveInfo;
                }
            }

            return new WorkerMoveInfo(false);
        }


        public TileBuildInfo workerBuild (Worker worker, Player player, int curRow, int curCol,
                                        int destinationRow, int destinationCol) {

            if (worker.isCorrectOwner(player)) {

                List<Tile> validTilesToBuildOn = getValidSpacesForAction(curRow, curCol, Action.Build);
                Tile destinationTile = gameboardController.getGameboard()[destinationRow, destinationCol];

                if (validTilesToBuildOn.Contains(destinationTile)) {

                    Tile origCopy = destinationTile.Clone();
                    destinationTile.build();

                    TileBuildInfo tileBuildInfo = new TileBuildInfo(true, origCopy, player);

                    if (!isNetworkGame) {
                        undoStack.Push(tileBuildInfo);
                        redoStack.Clear();
                    }

                    return tileBuildInfo;
                }
            }

            return new TileBuildInfo(false);
        }

        public List<Tile> getValidSpacesForAction (int row, int col, Action action) {

            List<Tile> tiles = new List<Tile>();
            List<Tile> temp = this.gameboardController.getGameboard()[row, col].getAdjacentTiles();
            int heightOfCurTile = this.gameboardController.getGameboard()[row, col].getHeight();

            foreach (Tile t in temp) {

                if (action == Action.Move && t.canMoveTo(heightOfCurTile))
                    tiles.Add(t);
                else if (action == Action.Build && t.canBuildOn())    
                    tiles.Add(t);    
            }

            return tiles;
        } 

        // I have configured both the undo and redo button functionality to return the player 
        // who's turn it should be after the action has taken place. If this is not a good way
        // of doing it then we can definitely change it
        public Player undoMove () {

            if (!isNetworkGame && undoStack.Count != 0) {

                StateInfo popped = undoStack.Pop();

                if (popped is WorkerMoveInfo) {
                    
                    WorkerMoveInfo move = (WorkerMoveInfo)popped;
                    resetGameMove(move);
                    redoStack.Push(move);

                    return move.getPlayer();
                }
                else if (popped is TileBuildInfo) {

                    TileBuildInfo build = (TileBuildInfo)popped;
                    resetGameBuild(build);
                    redoStack.Push(build);

                    return build.getPlayer();
                }
            }

            return null;
        }

        public Player redoMove () {
            
            if (!isNetworkGame && redoStack.Count != 0) {
                
                StateInfo popped = redoStack.Pop();

                if (popped is WorkerMoveInfo) {

                    WorkerMoveInfo move = (WorkerMoveInfo)popped;
                    resetGameMove(move);
                    undoStack.Push(popped);

                    return move.getPlayer();
                }
                else if (popped is TileBuildInfo) {

                    TileBuildInfo build = (TileBuildInfo)popped;
                    resetGameBuild(build);
                    undoStack.Push(popped);

                    return build.getPlayer();
                }
            }

            return null;
        }

        private void resetGameMove (WorkerMoveInfo gameState) {

            int origRow = gameState.getTileMovedFrom().getRow(), origCol = gameState.getTileMovedFrom().getCol();
            gameboardController.getGameboard()[origRow, origCol].setWorker(gameState.getWorker());
            int destRow = gameState.getTileMovedTo().getRow(), destCol = gameState.getTileMovedTo().getCol();
            gameboardController.getGameboard()[destRow, destCol].setWorker(null);
        }

        private void resetGameBuild (TileBuildInfo gameState) {
            
            int row = gameState.getTileOrigCopy().getRow(), col = gameState.getTileOrigCopy().getCol();
            gameboardController.getGameboard()[row, col] = gameState.getTileOrigCopy();
        }

        public bool canRedo () {
            return !isNetworkGame && this.redoStack.Count > 0;
        }

        public bool canUndo () {
            return !isNetworkGame && this.undoStack.Count > 0;
        }

        public Tile[,] getGameboard () {
            return this.gameboardController.getGameboard();
        }

        // This method should be called every single time a game move takes place
        // This method will check the board to see if any of the pieces on the board
        // are in a winning state or if any of the pieces are in a losing state
        // If there is a piece in a winning/losing state a Winner object with the 
        // appropriate info will be returned. If not, the Winner object will simply
        // contain a false value implying there is no winner

        public Winner checkForWin () {

            List<Tile> occupiedTiles = gameboardController.getOccupiedTiles();

            foreach (Tile tile in occupiedTiles) {

                if (tile.getHeight() == 3) {

                    Player loser = tile.getWorker().getOwner() == playerA ? playerB : playerA;
                    return new Winner(true, tile.getWorker().getOwner(), loser, tile.getWorker(), CauseOfWin.ReachedDestinationBlock);
                }
                else {

                    // STILL FIGURING THIS OUT
                }
            }

            return new Winner(false);
        }
    }
}