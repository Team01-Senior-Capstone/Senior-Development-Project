using System.IO;
using System.Collections;

namespace Gamecore {

    class Tile {

        private int row, col;
        private Worker curWorker;
        private PipeEntity pipe;
        private ArrayList adjacentTiles;
        
        public Tile (int row, int col) {

            this.row = row;
            this.col = col;
        }

        public void setWorker (Worker worker) {
            this.curWorker = worker;
        }

        public Worker getWorker () {
            return this.curWorker;
        }

        public void generateAdjacentTiles (Tile[,] gameboard) {

            ArrayList tiles = new ArrayList();

            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    if (dx != 0 || dy != 0)
                        tiles.Add(gameboard[row + dx, col + dy]);

            this.adjacentTiles = tiles;
        }

        public ArrayList getAdjacentTiles() {
            return this.adjacentTiles;
        }

        public bool canMoveTo () {

            return curWorker == null && (pipe == null || pipe.getHeight() < 4);
        }
    }
}