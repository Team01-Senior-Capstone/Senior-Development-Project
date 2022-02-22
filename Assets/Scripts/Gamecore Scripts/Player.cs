using System;

namespace Gamecore {

    [Serializable]
    public class Player {

        private bool playerOne;
        private Worker workerOne, workerTwo;
        private Identification typeOfPlayer;
        int cantMove = 0;
        
        public Player (bool PlayerOne, Identification _typeOfPlayer) {

            this.playerOne = PlayerOne;
            this.workerOne = new Worker(this, true);
            this.workerTwo = new Worker(this, false);
            this.typeOfPlayer = _typeOfPlayer;
        }

        public bool getIsPlayerOne() {
            return this.playerOne;
        }

        public Identification getTypeOfPlayer () {
            return this.typeOfPlayer;
        }

        public void increaseCantMove () {
            cantMove++;
        }

        public int getCantMove () {
            return cantMove;
        }

        public void resetCantMove () {
            cantMove = 0;
        }
    }
}