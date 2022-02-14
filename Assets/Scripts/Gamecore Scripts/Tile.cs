using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gamecore {

    [Serializable]
    public class Tile {

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
                    if ((dx != 0 || dy != 0) && (row + dx >= 0 && row + dx < 5) && (col + dy >= 0 && col + dy < 5))
                        tiles.Add(gameboard[row + dx, col + dy]);

            this.adjacentTiles = tiles;
        }

        public List<Tile> getAdjacentTiles() {
            return this.adjacentTiles;
        }

        public bool canMoveTo (int heightOfCurTile) {

            return (curWorker == null) && 
            (pipe == null || (!pipe.isCompleted() && 
            (pipe.getHeight() - heightOfCurTile == 1 || heightOfCurTile - pipe.getHeight() > 0 || pipe.getHeight() == heightOfCurTile)));
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
            
            return pipe == null ? 0 : pipe.getHeight();
        }

        public int getRow () {
            return this.row;
        }

        public int getCol () {
            return this.col;
        }

        public bool isWinner () {

            return curWorker != null && pipe != null && pipe.getHeight() == 3;
        }

        public Tile Clone () {
            
            using (MemoryStream stream = new MemoryStream())
            {
                if (this.GetType().IsSerializable)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Position = 0;
                    return (Tile)formatter.Deserialize(stream);
                }
                return null;
            }
        }
    }
}