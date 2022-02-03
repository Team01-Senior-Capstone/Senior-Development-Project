
namespace Gamecore {

    class Winner {

        // I am considering making this into a sort of entity that carries information about the 
        // winner of the game. Each time a move is perfomed, the game will check if the current state
        // of the game is a winning state (i.e. someone has won or lost the game) and return back 
        // information to the game manager.  So if there is not a winner it will just return back
        // this object where the winner is null. If there is a winner it will return back the player
        // who won and maybe a reason to why they won (i.e. they got to the third level or the 
        // other player simply has no more moves to make)

        private bool gameHasWinner;
        private Player winner, loser;
        private Worker resultMaker;
        private CauseOfWin causeOfWin;

        public Winner (bool gameHasWinner) {

            this.gameHasWinner = gameHasWinner;
        }

        public Winner (bool gameHasWinner, Player winner, Player loser, Worker resultMaker, CauseOfWin causeOfWin) {
            
            this.gameHasWinner = gameHasWinner;
            this.winner = winner;
            this.loser = loser;
            this.resultMaker = resultMaker;
            this.causeOfWin = causeOfWin;
        }

        public Player getWinner () {
            return this.winner;
        }

        public Player getLoser () {
            return this.loser;
        }

        public Worker getWorkerThatCausedResult () {
            return this.resultMaker;
        }

        public bool getGameHasWinner () {
            return this.gameHasWinner;
        }

        public CauseOfWin GetCauseOfWin () {
            return this.causeOfWin;
        }
    }
}