
namespace Gamecore {

    class TileBuildInfo : StateInfo {

        private Tile tileBuiltOn, tileBuiltFrom;
        private Player player;
        private Worker worker;
        private bool buildSuccessful;

        public TileBuildInfo (bool buildS, Tile tileBuiltFrom, Tile tileBuiltOn, Worker worker, Player player) {

            this.buildSuccessful = buildS;
            this.tileBuiltFrom = tileBuiltFrom;
            this.tileBuiltOn = tileBuiltOn;
            this.worker = worker;
            this.player = player;
        }

        public TileBuildInfo (bool buildS) {

            this.buildSuccessful = buildS;
        }

        public bool wasBuildSuccessful () {
            return this.buildSuccessful;
        }

        public Tile getTileBuiltFrom () {
            return this.tileBuiltFrom;
        }

        public Tile getTileBuiltOn () {
            return this.tileBuiltOn;
        }

        public Worker getWorker () {
            return this.worker;
        }

        public Player getPlayer () {
            return this.player;
        }
    }
}