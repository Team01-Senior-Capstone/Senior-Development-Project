using System;

namespace Gamecore {

    [Serializable]
    public class Worker {

        private Player workerOwner;
        public bool workerOne;

        public Worker (Player owner, bool worker1) {
            this.workerOwner = owner;
            workerOne = worker1;
        }

        public bool isCorrectOwner (Player player) {
            return this.workerOwner.getTypeOfPlayer() == player.getTypeOfPlayer();
        }

        public Player getOwner () {
            return this.workerOwner;
        }
    }
}