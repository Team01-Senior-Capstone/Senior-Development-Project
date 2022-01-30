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
        public void assignPlayers (FirstToGo firstToGo) {

            if (firstToGo == FirstToGo.Host || firstToGo == FirstToGo.Human) {
                this.playerA = new Player(true);
                this.playerB = new Player(false);
            }
            else if (firstToGo == FirstToGo.AI || firstToGo == FirstToGo.Client) {
                this.playerB = new Player(true);
                this.playerA = new Player(false);
            }  
        }

        public void makeNextPlayerMove (Player player) {

            WorkerMoveInfo playerMoveAttempt;

            do {
                // get info about the move somehow
                // This is garbage info just for testing
                Worker worker = new Worker(player);

                playerMoveAttempt = movePlayer(worker, player, 4, 4, 4, 4);

            } while (!playerMoveAttempt.wasMoveSuccessful());

            if (!isNetworkGame) {
                undoStack.Push(playerMoveAttempt);
                redoStack.Clear();
            }
        }

        public void makeNextPlayerBuild (Player player) {

            TileBuildInfo playerBuildAttempt;

            do {

                // get info about the build somehow
                // This is garbage info just for testing

                Worker worker = new Worker(player);

                playerBuildAttempt = workerBuild(worker, player, 4, 4, 4, 4);

            } while (!playerBuildAttempt.wasBuildSuccessful());

            if (!isNetworkGame) {
                undoStack.Push(playerBuildAttempt);
                redoStack.Clear();
            }
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
                    
                    return new WorkerMoveInfo(true, currentTile, destinationTile, worker, player);
                }
            }

            return new WorkerMoveInfo(false);
        }


        // Needs to be implemented
        public TileBuildInfo workerBuild (Worker worker, Player player, int curRow, int curCol,
                                        int destinationRow, int destinationCol) {

            if (worker.isCorrectOwner(player)) {

                List<Tile> validTilesToBuildOn = getValidSpacesForAction(curRow, curCol, Action.Build);
                Tile destinationTile = gameboard.getGameboard()[destinationRow, destinationCol];
                Tile currentTile = gameboard.getGameboard()[curRow, curCol];

                if (validTilesToBuildOn.Contains(destinationTile)) {

                    Tile origCopy = destinationTile.Clone();
                    destinationTile.build();

                    return new TileBuildInfo(true, origCopy, player);
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

    }
}