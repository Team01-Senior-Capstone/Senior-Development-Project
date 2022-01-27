using System.IO;
using System.Collections;


namespace Gamecore {

    class Gameboard {

        Tile[,] board;

        public Gameboard () {

            board = new Tile[5,5];

            for (int i  = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    board[i,j] = new Tile(i, j);
                }
            }

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++)
                    board[i,j].generateAdjacentTiles(board);
            }
        }

        public Tile[,] getGameboard () {
            return this.board;
        }
    }
}