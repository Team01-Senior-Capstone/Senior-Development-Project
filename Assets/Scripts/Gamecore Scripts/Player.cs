using System;

namespace Gamecore {

    [Serializable]
    public class Player {

        private bool playerOne;
        private Worker workerOne, workerTwo;
        private Identification typeOfPlayer;
        
        public Player (bool PlayerOne, Identification typeOfPlayer) {

            this.playerOne = PlayerOne;
            this.workerOne = new Worker(this);
            this.workerTwo = new Worker(this);
        }

        public bool getIsPlayerOne() {
            return this.playerOne;
        }

        public Identification getTypeOfPlayer () {
            return this.typeOfPlayer;
        }
    }
}