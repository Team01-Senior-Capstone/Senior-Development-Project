using System.IO;
using System.Collections;

namespace Gamecore {

    enum Action {
        Build, Move
    }

    class GameController {

        private Gameboard gameboard;
        private Player playerOne, playerTwo;

        GameController () {

            gameboard = new Gameboard();
        }

        public ArrayList getValidSpacesForAction (int row, int col, Action action) {

            ArrayList tiles = new ArrayList();
            ArrayList temp = this.gameboard.getGameboard()[row, col].getAdjacentTiles();

            for (int i = 0; i < temp.Count; i++) {
                Tile temporary = (Tile)temp[i];

                if (action == Action.Move && temporary.canMoveTo())
                    tiles.Add(temp[i]);
                else if (action == Action.Build && temporary.canBuildOn())    
                    tiles.Add(temp[i]);    
            }

            return tiles;
        } 
    }
}