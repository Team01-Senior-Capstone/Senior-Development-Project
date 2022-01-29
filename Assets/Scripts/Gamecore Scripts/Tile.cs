using System.Collections.Generic;

namespace Gamecore {

    class Tile {

        private int row, col;
        private Worker curWorker;
        private PipeEntity pipe;
        private List<Tile> adjacentTiles;
        
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

            List<Tile> tiles = new List<Tile>();

            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    if (dx != 0 || dy != 0)
                        tiles.Add(gameboard[row + dx, col + dy]);

            this.adjacentTiles = tiles;
        }

        public List<Tile> getAdjacentTiles() {
            return this.adjacentTiles;
        }

        public bool canMoveTo (int heightOfCurTile) {

            return (curWorker == null) && 
            (pipe == null || (!pipe.isCompleted() && 
            (pipe.getHeight() - heightOfCurTile == 1 || heightOfCurTile - pipe.getHeight() > 0)));
        }

        public void build () {

            if (pipe == null)
                pipe = new PipeEntity();
            else {
                pipe.increaseHeight();
            }
        }

        public bool canBuildOn () {

            return curWorker == null && (pipe == null || !pipe.isCompleted());
        }

        public int getHeight() {
            
            return (pipe == null ? 0 : pipe.getHeight());
        }
    }
}