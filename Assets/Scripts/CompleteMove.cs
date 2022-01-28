using System.IO;
using System.Collections;

namespace Gamecore {

    class CompleteMove {

        private Tile movedFrom, movedTo, builtOn;
        private Player playerWhoMoved;
        private Worker workerThatMoved;

        public CompleteMove (Tile movedFrom, Tile movedTo, Tile builtOn, Player playerWhoMoved, Worker workerThatMoved) {

            this.movedFrom = movedFrom;
            this.movedTo = movedTo;
            this.builtOn = builtOn;
            this.playerWhoMoved = playerWhoMoved;
            this.workerThatMoved = workerThatMoved;
        }
    }
}