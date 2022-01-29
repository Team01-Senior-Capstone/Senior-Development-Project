
namespace Gamecore {

    class WorkerMoveInfo : StateInfo {
        
        private bool moveSuccessful;
        private Tile tileMovedFrom, tileMovedTo;
        Worker worker;
        Player player;

        public WorkerMoveInfo (bool moveS, Tile tileMovedFrom, Tile tileMovedTo, Worker worker, Player player) {

            this.moveSuccessful = moveS;
            this.tileMovedFrom = tileMovedFrom;
            this.tileMovedTo = tileMovedTo;
            this.worker = worker;
            this.player = player;
        }

        public WorkerMoveInfo (bool moveSuccessful) {
            this.moveSuccessful = moveSuccessful;
        }

        public bool wasMoveSuccessful () {
            return this.moveSuccessful;
        }

        public Tile getTileMovedFrom () {
            return this.tileMovedFrom;
        }

        public Tile getTileMovedTo () {
            return this.tileMovedTo;
        }

        public Worker getWorkerMoved () {
            return this.worker;
        }

        public Player getPlayerWhoMoved () {
            return this.player;
        }
    }
}