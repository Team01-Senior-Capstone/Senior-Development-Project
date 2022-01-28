using System.IO;
using System.Collections;

namespace Gamecore {

    class Worker {

        private Player workerOwner;

        public Worker (Player owner) {
            this.workerOwner = owner;
        }

        public bool isCorrectOwner (Player player) {
            return this.workerOwner == player;
        }
    }
}