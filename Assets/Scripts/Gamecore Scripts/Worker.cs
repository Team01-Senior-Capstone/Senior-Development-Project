using System;

namespace Gamecore {

    [Serializable]
    public class Worker {

        private Player workerOwner;

        public Worker (Player owner) {
            this.workerOwner = owner;
        }

        public bool isCorrectOwner (Player player) {
            return this.workerOwner == player;
        }

        public Player getOwner () {
            return this.workerOwner;
        }
    }
}