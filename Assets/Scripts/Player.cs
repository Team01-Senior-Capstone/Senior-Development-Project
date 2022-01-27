using System.IO;
using System.Collections;

namespace Gamecore {

    class Player {

        private bool playerOne;
        private Worker workerOne, workerTwo;

        public Player () {

        }
        
        public Player (bool PlayerOne) {

            this.playerOne = PlayerOne;
        }

        public bool getIsPlayerOne() {
            return this.playerOne;
        }

        public void setIsPlayerOne (bool playerOne) {
            this.playerOne = playerOne;
        }
    }
}