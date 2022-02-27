using System.Collections.Generic;
using System;

namespace Gamecore {

    [Serializable]
    class GameboardController {

        private Tile[,] board;
        private List<Tile> occupiedTiles;

        public GameboardController () {

            this.board = new Tile[5,5];
            occupiedTiles = new List<Tile> {};

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

        public List<Tile> getOccupiedTiles () {
            
            return this.occupiedTiles;
        }

        public void addTileToOccupied (Tile tile) {

            if (occupiedTiles.Count == 0 || !occupiedTiles.Contains(tile)) {
                occupiedTiles.Add(tile);
            }
        }

        public void removedOccupiedTile (Tile tile) {

            if (occupiedTiles.Count != 0 && occupiedTiles.Contains(tile)) {
                occupiedTiles.Remove(tile);
            }
        }

    }
}