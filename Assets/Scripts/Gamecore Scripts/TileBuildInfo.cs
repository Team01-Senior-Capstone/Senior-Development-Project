using System;

namespace Gamecore {

    [Serializable]
    public class TileBuildInfo : StateInfo {

        private Tile builtFrom, tileOrigCopy;
        private int buildRow, buildCol;
        private Player player;
        private bool buildSuccessful;

        public TileBuildInfo (bool buildS, Tile builtFrom, int buildRow, int buildCol, Player player) {

            this.buildSuccessful = buildS;
            //this.tileOrigCopy = origCopy;
            this.buildRow = buildRow;
            this.buildCol = buildCol;
            this.builtFrom = builtFrom;
            this.player = player;
        }

        public TileBuildInfo (bool buildS) {

            this.buildSuccessful = buildS;
        }

        public bool wasBuildSuccessful () {
            return this.buildSuccessful;
        }

        //public Tile getTileOrigCopy () {
        //    return this.tileOrigCopy;
        //}

        public Tile getBuiltFrom()
        {
            return builtFrom;
        }

        public Player getPlayer () {
            return this.player;
        }

        public int getBuildRow()
        {
            return buildRow;
        }

        public int getBuildCol()
        {
            return buildCol;
        }
    }
}