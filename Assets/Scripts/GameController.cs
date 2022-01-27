using System.IO;
using System.Collections;

namespace Gamecore {

    class GameController {

        private Gameboard gameboard;
        private Player playerOne, playerTwo;

        GameController () {

            gameboard = new Gameboard();
        }

        public ArrayList getValidSpacesForMove (int row, int col) {

            ArrayList tiles = new ArrayList();
            ArrayList temp = this.gameboard.getGameboard()[row, col].getAdjacentTiles();

            for (int i = 0; i < temp.Count; i++) {
                Tile temporary = (Tile)temp[i];
                if (temporary.canMoveTo())
                    tiles.Add(temp[i]);
            }

            return tiles;
        } 
    }
}