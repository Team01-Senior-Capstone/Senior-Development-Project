using System;

namespace Gamecore {

    [Serializable]
    public class TileBuildInfo : StateInfo {

        private Tile builtFrom, tileOrigCopy;
        private Player player;
        private bool buildSuccessful;

        public TileBuildInfo (bool buildS, Tile builtFrom, Tile origCopy, Player player) {

            this.buildSuccessful = buildS;
            this.tileOrigCopy = origCopy;
            this.builtFrom = builtFrom;
            this.player = player;
        }

        public TileBuildInfo (bool buildS) {

            this.buildSuccessful = buildS;
        }

        public bool wasBuildSuccessful () {
            return this.buildSuccessful;
        }

        public Tile getTileOrigCopy () {
            return this.tileOrigCopy;
        }

        public Tile getBuiltFrom()
        {
            return builtFrom;
        }

        public Player getPlayer () {
            return this.player;
        }
    }
}