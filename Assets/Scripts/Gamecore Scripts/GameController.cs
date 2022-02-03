using System.Collections.Generic;

namespace Gamecore {

    class GameController {

        // Player A is the host or the Human playing the AI. Player B is the 
        // AI or the Client in a network game

        private Player playerA, playerB;
        private Stack<StateInfo> undoStack, redoStack;
        private bool isNetworkGame;
        private Gameboard gameboard;

        GameController (bool isNetworkGame) {

            this.gameboard = new Gameboard();
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
        */
        public Player[] assignPlayers (FirstToGo firstToGo) {

            if (firstToGo == FirstToGo.Host || firstToGo == FirstToGo.Human) {
                this.playerA = new Player(true);
                this.playerB = new Player(false);
            }
            else if (firstToGo == FirstToGo.AI || firstToGo == FirstToGo.Client) {
                this.playerB = new Player(true);
                this.playerA = new Player(false);
            }  

            return new Player[2] { playerA, playerB }; 
        }

        private WorkerMoveInfo movePlayer(Worker worker, Player player, int curRow, int curCol, 
                                        int destinationRow, int destinationCol) {

            if (worker.isCorrectOwner(player)) {

                List<Tile> validTilesToMoveTo = getValidSpacesForAction(curRow, curCol, Action.Move);
                Tile destinationTile = gameboard.getGameboard()[destinationRow, destinationCol];
                Tile currentTile = gameboard.getGameboard()[curRow, curCol];

                if (validTilesToMoveTo.Contains(destinationTile)) {
                    
                    destinationTile.setWorker(worker);
                    currentTile.setWorker(null);

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
                Tile destinationTile = gameboard.getGameboard()[destinationRow, destinationCol];

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
            List<Tile> temp = this.gameboard.getGameboard()[row, col].getAdjacentTiles();
            int heightOfCurTile = this.gameboard.getGameboard()[row, col].getHeight();

            for (int i = 0; i < temp.Count; i++) {
                Tile temporary = (Tile)temp[i];

                if (action == Action.Move && temporary.canMoveTo(heightOfCurTile))
                    tiles.Add(temp[i]);
                else if (action == Action.Build && temporary.canBuildOn())    
                    tiles.Add(temp[i]);    
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
            gameboard.getGameboard()[origRow, origCol].setWorker(gameState.getWorker());
            int destRow = gameState.getTileMovedTo().getRow(), destCol = gameState.getTileMovedTo().getCol();
            gameboard.getGameboard()[destRow, destCol].setWorker(null);
        }

        private void resetGameBuild (TileBuildInfo gameState) {
            
            int row = gameState.getTileOrigCopy().getRow(), col = gameState.getTileOrigCopy().getCol();
            gameboard.getGameboard()[row, col] = gameState.getTileOrigCopy();
        }

        public bool canRedo () {
            return this.redoStack.Count > 0;
        }

        public bool canUndo () {
            return this.undoStack.Count > 0;
        }

        public Tile[,] getGameboard () {
            return this.gameboard.getGameboard();
        }
    }
}