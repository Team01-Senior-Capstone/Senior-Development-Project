using System.IO;
using System.Collections;

namespace Gamecore {

    enum Action { Build, Move }

    class GameController {

        private Gameboard gameboard;
        private Player playerOne, playerTwo;
        private Stack undoStack, redoStack;
        private bool isNetworkGame;

        GameController (bool isNetworkGame) {

            this.gameboard = new Gameboard();
            this.isNetworkGame = isNetworkGame;

            if (!isNetworkGame) {
                this.undoStack = new Stack();
                this.redoStack = new Stack();
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

            undoStack.Push(playerMoveAttempt);
        }

        private WorkerMoveInfo movePlayer(Worker worker, Player player, int curRow, int curCol, 
                                        int destinationRow, int destinationCol) {

            if (worker.isCorrectOwner(player)) {

                ArrayList validTilesToMoveTo = getValidSpacesForAction(curRow, curCol, Action.Move);
                Tile destinationTile = this.gameboard.getGameboard()[destinationRow, destinationCol];
                Tile currentTile = this.gameboard.getGameboard()[curRow, curCol];

                if (validTilesToMoveTo.Contains(destinationTile)) {
                    
                    destinationTile.setWorker(worker);
                    currentTile.setWorker(null);
                    

                    return new WorkerMoveInfo(true, currentTile, destinationTile, worker, player);
                }
            }

            return new WorkerMoveInfo(false);
        }


        // Needs to be implemented
        public bool moveBuilder() {

            return false;
        }

        public ArrayList getValidSpacesForAction (int row, int col, Action action) {

            ArrayList tiles = new ArrayList();
            ArrayList temp = this.gameboard.getGameboard()[row, col].getAdjacentTiles();
            int heightOfCurTile = this.gameboard.getGameboard()[row,col].getHeight();

            for (int i = 0; i < temp.Count; i++) {
                Tile temporary = (Tile)temp[i];

                if (action == Action.Move && temporary.canMoveTo(heightOfCurTile))
                    tiles.Add(temp[i]);
                else if (action == Action.Build && temporary.canBuildOn())    
                    tiles.Add(temp[i]);    
            }

            return tiles;
        } 

        private void undoMove () {

            if (!isNetworkGame && undoStack.Count != 0) {

            }
        }

        private void redoMove () {
            
            if (!isNetworkGame && redoStack.Count != 0) {
                
            }
        }

    }
}