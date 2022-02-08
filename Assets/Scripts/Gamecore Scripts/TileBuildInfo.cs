
namespace Gamecore {

    public class TileBuildInfo : StateInfo {

        private Tile tileBuiltOn, tileOrigCopy;
        private Player player;
        private bool buildSuccessful;

        public TileBuildInfo (bool buildS, Tile origCopy, Player player) {

            this.buildSuccessful = buildS;
            this.tileOrigCopy = origCopy;
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

        public Player getPlayer () {
            return this.player;
        }
    }
}