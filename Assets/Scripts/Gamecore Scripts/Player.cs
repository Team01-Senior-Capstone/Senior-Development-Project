using System;

namespace Gamecore {

    [Serializable]
    class Player {

        private bool playerOne;
        private Worker workerOne, workerTwo;
        
        public Player (bool PlayerOne) {

            this.playerOne = PlayerOne;
            this.workerOne = new Worker(this);
            this.workerTwo = new Worker(this);
        }

        public bool getIsPlayerOne() {
            return this.playerOne;
        }
    }
}